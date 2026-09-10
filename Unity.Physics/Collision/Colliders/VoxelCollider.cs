using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Caelix;

namespace Unity.Physics
{
    public unsafe struct VoxelCollider : ICollider
    {
        // Header
        private ColliderHeader m_Header;

        /// <summary>   The material. </summary>
        public Material Material;

        // Mass properties (can be set externally via SetMassProperties)
        private MassProperties m_MassProperties;

        /// <summary>
        /// The voxel storage this collider reads, as the brick-key facade. The owning entity owns
        /// the storage; this is a view with pointer semantics, so it needs no per-tick copy.
        /// </summary>
        public VoxelEntityData Entity;

        /// <summary>
        /// Calculate the axis-aligned bounding box of this voxel collider in local space.
        /// Returns an AABB that encompasses the entity's whole storage box.
        /// </summary>
        /// <returns>The local-space AABB covering the entity's storage, or an empty AABB if it holds none.</returns>
        public Aabb CalculateAabb()
        {
            return CalculateAabb(RigidTransform.identity);
        }

        /// <summary>
        /// Calculate the axis-aligned bounding box of this voxel collider transformed by the given rigid transform.
        /// This properly handles rotation by transforming the corners of the local AABB.
        /// </summary>
        /// <param name="transform">The rigid transform to apply (rotation and translation).</param>
        /// <param name="uniformScale">Uniform scale factor to apply (default: 1.0).</param>
        /// <returns>The transformed AABB covering the entity's storage.</returns>
        public Aabb CalculateAabb(RigidTransform transform, float uniformScale = 1.0f)
        {
            // If the entity holds no storage at all, return an empty AABB.
            if (!Entity.TryGetStorageBounds(out int3 minBlock, out int3 maxBlockExclusive))
            {
                return Aabb.Empty;
            }

            // Residency-sized bound on purpose: a region contributes its whole box even when only
            // one brick in it is allocated. Sector.blockAABB is a grow-only upper bound that is
            // never shrunk, not persisted, and not maintained by every writer (see its field doc in
            // Caelix-Core Sector.cs), so a per-region content bound needs a RecomputeBlockAABB pass
            // that does not exist yet.
            Aabb localAabb = Aabb.Empty;
            localAabb.Include((float3)minBlock);
            localAabb.Include((float3)maxBlockExclusive);

            // Transform the local AABB by the given transform and scale
            // This handles rotation properly by transforming the corner points
            return Math.TransformAabb(localAabb, transform, uniformScale);
        }

        #region Query Methods

        public bool CastRay(RaycastInput input)
        {
            throw new System.NotImplementedException();
        }

        public bool CastRay(RaycastInput input, out RaycastHit closestHit)
        {
            throw new System.NotImplementedException();
        }

        public bool CastRay(RaycastInput input, ref NativeList<RaycastHit> allHits)
        {
            throw new System.NotImplementedException();
        }

        public bool CastRay<T>(RaycastInput input, ref T collector) where T : struct, ICollector<RaycastHit>
        {
            throw new System.NotImplementedException();
        }

        public bool CastCollider(ColliderCastInput input)
        {
            throw new System.NotImplementedException();
        }

        public bool CastCollider(ColliderCastInput input, out ColliderCastHit closestHit)
        {
            throw new System.NotImplementedException();
        }

        public bool CastCollider(ColliderCastInput input, ref NativeList<ColliderCastHit> allHits)
        {
            throw new System.NotImplementedException();
        }

        public bool CastCollider<T>(ColliderCastInput input, ref T collector) where T : struct, ICollector<ColliderCastHit>
        {
            throw new System.NotImplementedException();
        }

        public bool CalculateDistance(PointDistanceInput input)
        {
            throw new System.NotImplementedException();
        }

        public bool CalculateDistance(PointDistanceInput input, out DistanceHit closestHit)
        {
            throw new System.NotImplementedException();
        }

        public bool CalculateDistance(PointDistanceInput input, ref NativeList<DistanceHit> allHits)
        {
            throw new System.NotImplementedException();
        }

        public bool CalculateDistance<T>(PointDistanceInput input, ref T collector) where T : struct, ICollector<DistanceHit>
        {
            throw new System.NotImplementedException();
        }

        public bool CalculateDistance(ColliderDistanceInput input)
        {
            throw new System.NotImplementedException();
        }

        public bool CalculateDistance(ColliderDistanceInput input, out DistanceHit closestHit)
        {
            throw new System.NotImplementedException();
        }

        public bool CalculateDistance(ColliderDistanceInput input, ref NativeList<DistanceHit> allHits)
        {
            throw new System.NotImplementedException();
        }

        public bool CalculateDistance<T>(ColliderDistanceInput input, ref T collector) where T : struct, ICollector<DistanceHit>
        {
            throw new System.NotImplementedException();
        }

        public bool CheckSphere(float3 position, float radius, CollisionFilter filter,
            QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool OverlapSphere(float3 position, float radius, ref NativeList<DistanceHit> outHits, CollisionFilter filter,
            QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool OverlapSphereCustom<T>(float3 position, float radius, ref T collector, CollisionFilter filter,
            QueryInteraction queryInteraction = QueryInteraction.Default) where T : struct, ICollector<DistanceHit>
        {
            throw new System.NotImplementedException();
        }

        public bool CheckCapsule(float3 point1, float3 point2, float radius, CollisionFilter filter,
            QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool OverlapCapsule(float3 point1, float3 point2, float radius, ref NativeList<DistanceHit> outHits, CollisionFilter filter,
            QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool OverlapCapsuleCustom<T>(float3 point1, float3 point2, float radius, ref T collector, CollisionFilter filter,
            QueryInteraction queryInteraction = QueryInteraction.Default) where T : struct, ICollector<DistanceHit>
        {
            throw new System.NotImplementedException();
        }

        public bool CheckBox(float3 center, quaternion orientation, float3 halfExtents, CollisionFilter filter,
            QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool OverlapBox(float3 center, quaternion orientation, float3 halfExtents, ref NativeList<DistanceHit> outHits,
            CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool OverlapBoxCustom<T>(float3 center, quaternion orientation, float3 halfExtents, ref T collector,
            CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default) where T : struct, ICollector<DistanceHit>
        {
            throw new System.NotImplementedException();
        }

        public bool SphereCast(float3 origin, float radius, float3 direction, float maxDistance, CollisionFilter filter,
            QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool SphereCast(float3 origin, float radius, float3 direction, float maxDistance, out ColliderCastHit hitInfo,
            CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool SphereCastAll(float3 origin, float radius, float3 direction, float maxDistance, ref NativeList<ColliderCastHit> outHits,
            CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool SphereCastCustom<T>(float3 origin, float radius, float3 direction, float maxDistance, ref T collector,
            CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default) where T : struct, ICollector<ColliderCastHit>
        {
            throw new System.NotImplementedException();
        }

        public bool BoxCast(float3 center, quaternion orientation, float3 halfExtents, float3 direction, float maxDistance,
            CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool BoxCast(float3 center, quaternion orientation, float3 halfExtents, float3 direction, float maxDistance,
            out ColliderCastHit hitInfo, CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool BoxCastAll(float3 center, quaternion orientation, float3 halfExtents, float3 direction, float maxDistance,
            ref NativeList<ColliderCastHit> outHits, CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool BoxCastCustom<T>(float3 center, quaternion orientation, float3 halfExtents, float3 direction, float maxDistance,
            ref T collector, CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default) where T : struct, ICollector<ColliderCastHit>
        {
            throw new System.NotImplementedException();
        }

        public bool CapsuleCast(float3 point1, float3 point2, float radius, float3 direction, float maxDistance,
            CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool CapsuleCast(float3 point1, float3 point2, float radius, float3 direction, float maxDistance,
            out ColliderCastHit hitInfo, CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool CapsuleCastAll(float3 point1, float3 point2, float radius, float3 direction, float maxDistance,
            ref NativeList<ColliderCastHit> outHits, CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default)
        {
            throw new System.NotImplementedException();
        }

        public bool CapsuleCastCustom<T>(float3 point1, float3 point2, float radius, float3 direction, float maxDistance,
            ref T collector, CollisionFilter filter, QueryInteraction queryInteraction = QueryInteraction.Default) where T : struct, ICollector<ColliderCastHit>
        {
            throw new System.NotImplementedException();
        }
        
        #endregion
        
        public ColliderType Type => m_Header.Type;
        public CollisionType CollisionType => m_Header.CollisionType;

        /// <summary>
        /// Gets the mass properties. This should be set using SetMassProperties()
        /// after computing via VoxelBody.UpdateBody() or similar methods.
        /// </summary>
        public MassProperties MassProperties => m_MassProperties;

        /// <summary>
        /// Sets the mass properties for this voxel collider.
        /// Use this to apply computed mass properties from VoxelBody.UpdateBody() or other sources.
        /// </summary>
        /// <param name="massProperties">The computed mass properties to apply.</param>
        public void SetMassProperties(MassProperties massProperties)
        {
            m_MassProperties = massProperties;
            m_Header.Version++;
        }

        public int MemorySize => UnsafeUtility.SizeOf<VoxelCollider>();
        
        public CollisionFilter GetCollisionFilter()
        {
            return m_Header.Filter;
        }

        public void SetCollisionFilter(CollisionFilter filter)
        {
            if (!m_Header.Filter.Equals(filter))
            {
                m_Header.Version++;
                m_Header.Filter = filter;
            }
        }

        public void BakeTransform(AffineTransform transform)
        {
            SafetyChecks.LogWarning($"Not implemented for collider type {Type}.");
        }

        /// <summary>
        /// Indicates whether collider should collide normally with others,
        /// or skip collision, but still move and intercept queries.
        /// </summary>
        internal bool RespondsToCollision => Material.CollisionResponse != CollisionResponsePolicy.None;

        /// <summary>
        /// Sets a material field for this voxel collider.
        /// </summary>
        internal void SetMaterialField(Material material, Material.MaterialField option)
        {
            if (option == Material.MaterialField.Friction || option == Material.MaterialField.FrictionCombinePolicy)
                Material.Friction = material.Friction;
            if (option == Material.MaterialField.Restitution || option == Material.MaterialField.RestitutionCombinePolicy)
                Material.Restitution = material.Restitution;
            if (option == Material.MaterialField.CollisionResponsePolicy)
                Material.CollisionResponse = material.CollisionResponse;
            // Increment version when material changes
            m_Header.Version++;
        }

        /// <summary>
        /// Creates a VoxelCollider over one entity's voxel storage.
        /// Note: Mass properties are initialized with default values (mass=1, inertia tensor identity).
        /// Call SetMassProperties() on the created collider to apply computed mass properties.
        /// </summary>
        public static BlobAssetReference<Collider> Create(
            in VoxelEntityData entity,
            CollisionFilter filter, Material material)
        {
            unsafe
            {
                var collider = default(VoxelCollider);

                // Initialize header
                collider.m_Header.Type = ColliderType.Voxel;
                collider.m_Header.CollisionType = CollisionType.Voxel;
                collider.m_Header.Version = 0;
                collider.m_Header.Magic = 0xff;
                collider.m_Header.ForceUniqueBlobID = ~ColliderConstants.k_SharedBlobID;
                collider.m_Header.Filter = filter;

                // Initialize material
                collider.Material = material;

                // Initialize with default mass properties
                // These should be updated using SetMassProperties() after computing actual values
                collider.m_MassProperties = new MassProperties
                {
                    MassDistribution = new MassDistribution
                    {
                        Transform = new RigidTransform(quaternion.identity, float3.zero),
                        InertiaTensor = new float3(1.0f, 1.0f, 1.0f) // Default identity-like inertia
                    },
                    Volume = 1.0f,
                    AngularExpansionFactor = 0.0f
                };

                // Bind the entity's storage. A default entity is a valid empty one.
                collider.Entity = entity;

                return BlobAssetReference<Collider>.Create(&collider, sizeof(VoxelCollider));
            }
        }

        /// <summary>
        /// Rebinds this collider to an entity's voxel storage. The facade has pointer semantics, so
        /// this is an assignment rather than a copy of the storage index.
        /// </summary>
        public void SetEntity(in VoxelEntityData entity)
        {
            Entity = entity;
            m_Header.Version++;
        }

        /// <summary>
        /// No-op: the entity owns its voxel storage and this collider only holds a view of it.
        /// Kept so <c>VoxelBodyData.Dispose</c> and the test fixtures still compile and so a future
        /// collider-owned resource has a place to be released.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
