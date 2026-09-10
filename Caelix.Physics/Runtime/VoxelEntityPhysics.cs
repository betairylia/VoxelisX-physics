using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Caelix.Simulation
{
    public class VoxelEntityPhysics
    {
        public struct RegionMassMomentInput
        {
            public int3 RegionPos;
            public VoxelEntityData Entity;
        }

        public struct MassMoments
        {
            public float Mass;
            public float3 FirstMoment;
            public float3 InertiaOrigin;

            public static MassMoments operator +(MassMoments a, MassMoments b)
            {
                return new MassMoments
                {
                    Mass = a.Mass + b.Mass,
                    FirstMoment = a.FirstMoment + b.FirstMoment,
                    InertiaOrigin = a.InertiaOrigin + b.InertiaOrigin
                };
            }

            public static MassMoments operator -(MassMoments a, MassMoments b)
            {
                return new MassMoments
                {
                    Mass = a.Mass - b.Mass,
                    FirstMoment = a.FirstMoment - b.FirstMoment,
                    InertiaOrigin = a.InertiaOrigin - b.InertiaOrigin
                };
            }
        }

        public struct RegionMassMomentResult
        {
            public int3 RegionPos;
            public MassMoments Moments;
        }

        [BurstCompile]
        public struct ComputeRegionMassMomentsJob : IJobParallelFor
        {
            public PhysicsSettings settings;
            [ReadOnly] public NativeArray<RegionMassMomentInput> inputs;
            [WriteOnly] public NativeArray<RegionMassMomentResult> results;

            public void Execute(int index)
            {
                RegionMassMomentInput input = inputs[index];
                results[index] = new RegionMassMomentResult
                {
                    RegionPos = input.RegionPos,
                    Moments = ComputeRegionMassMoments(input.Entity, input.RegionPos, settings)
                };
            }
        }

        /// <remarks>
        /// The Block slot occupancy mask must have been refreshed after the region's latest voxel
        /// writes; the bitmask enumerator deliberately has no scan fallback.
        ///
        /// TODO: LIMITATION: only the diagonal of the inertia tensor is accumulated
        /// (Ixx, Iyy, Izz). The products of inertia (Ixy, Ixz, Iyz) are not computed,
        /// and the downstream rigid body forces its motion (principal-axis) frame to be
        /// axis-aligned with the body (BodyFromMotion rotation = identity). This is exact
        /// only when the mass distribution's principal axes coincide with the voxel-grid
        /// axes (e.g. symmetric bodies); an asymmetric voxel body (L-shape, diagonally
        /// weighted) will rotate without the correct inertial coupling. Computing the full
        /// symmetric tensor + eigendecomposition for BodyFromMotion is future work.
        /// </remarks>
        [BurstCompile]
        public static unsafe MassMoments ComputeRegionMassMoments(
            in VoxelEntityData entity,
            int3 regionPos,
            PhysicsSettings settings)
        {
            MassMoments result = default;

            // One local copy: the facade has pointer semantics on its storage, and binding through
            // an `in` parameter would take a defensive copy of the struct on every call.
            VoxelEntityData data = entity;
            var cursor = default(BrickCursor);

            foreach (int3 key in data.EnumerateBricks(
                VoxelRegion.FirstKeyOf(regionPos), VoxelRegion.LastKeyOf(regionPos)))
            {
                if (!data.TryBindBrick<Block>(SectorSlotId.Block, key, ref cursor, out Block* blocks) ||
                    !data.TryBindBrickAux(SectorSlotId.Block, key, ref cursor, out void* occupancy))
                {
                    continue;
                }

                result += ComputeBrickMassMoments(
                    blocks, (ulong*)occupancy, BrickKey.ToBlockOrigin(key), settings);
            }

            return result;
        }

        /// <summary>
        /// Accumulates one brick's mass moments, in entity-local coordinates. Positions are voxel
        /// centres, i.e. <c>blockOrigin + local + 0.5</c>.
        /// </summary>
        public static unsafe MassMoments ComputeBrickMassMoments(
            Block* blocks,
            ulong* occupancy,
            int3 blockOrigin,
            PhysicsSettings settings)
        {
            MassMoments result = default;

            foreach (BrickSlotEntry<Block> blockIter in
                new BrickBitmaskSlotEnumerator<Block>(blocks, occupancy, blockOrigin))
            {
                float mass = settings.GetBlockMass(blockIter.value);
                if (mass <= 0f)
                {
                    continue;
                }

                float3 position = new float3(blockIter.position) + 0.5f;
                result.Mass += mass;
                result.FirstMoment += mass * position;
                result.InertiaOrigin += mass * new float3(
                    position.y * position.y + position.z * position.z,
                    position.x * position.x + position.z * position.z,
                    position.x * position.x + position.y * position.y);
            }

            return result;
        }

        public static float3 InertiaAroundCenterOfMass(MassMoments moments, float3 centerOfMass)
        {
            if (moments.Mass <= 0f)
            {
                return float3.zero;
            }

            return moments.InertiaOrigin - moments.Mass * new float3(
                centerOfMass.y * centerOfMass.y + centerOfMass.z * centerOfMass.z,
                centerOfMass.x * centerOfMass.x + centerOfMass.z * centerOfMass.z,
                centerOfMass.x * centerOfMass.x + centerOfMass.y * centerOfMass.y);
        }
    }
}
