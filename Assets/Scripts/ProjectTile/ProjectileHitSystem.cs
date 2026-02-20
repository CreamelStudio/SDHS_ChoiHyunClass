using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;


[BurstCompile]
public partial struct ProjectileHitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileTag>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged);

        // 적 데이터 스냅샷 (간단/안전 버전)
        var enemyQuery = SystemAPI.QueryBuilder().WithAll<EnemyTag, LocalTransform, HitRadius, Health>().Build();
        var enemies = enemyQuery.ToEntityArray(state.WorldUpdateAllocator);
        var enemyT = enemyQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
        var enemyR = enemyQuery.ToComponentDataArray<HitRadius>(state.WorldUpdateAllocator);
        var enemyH = enemyQuery.ToComponentDataArray<Health>(state.WorldUpdateAllocator);

        new ProjectileHitJob
        {
            Enemies = enemies,
            EnemyTransforms = enemyT,
            EnemyRadii = enemyR,
            EnemyHealths = enemyH,
            Ecb = ecb
        }.Run();

        // 변경된 HP를 다시 반영
        for (int i = 0; i < enemies.Length; i++)
        {
            state.EntityManager.SetComponentData(enemies[i], enemyH[i]);
        }
    }

    [BurstCompile]
    public partial struct ProjectileHitJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> Enemies;
        [ReadOnly] public NativeArray<LocalTransform> EnemyTransforms;
        [ReadOnly] public NativeArray<HitRadius> EnemyRadii;
        public NativeArray<Health> EnemyHealths;

        public EntityCommandBuffer Ecb;

        void Execute(Entity projEntity, in LocalTransform projT, in HitRadius projR, in Damage dmg, ref ProjectilePierce pierce, in ProjectileTag tag)
        {
            float2 pp = new float2(projT.Position.x, projT.Position.y);

            for (int i = 0; i < Enemies.Length; i++)
            {
                float2 ep = new float2(EnemyTransforms[i].Position.x, EnemyTransforms[i].Position.y);
                float rr = projR.Value + EnemyRadii[i].Value;

                float2 d = ep - pp;
                if (math.lengthsq(d) <= rr * rr)
                {
                    var h = EnemyHealths[i];
                    h.Current = math.max(0, h.Current - dmg.Value);
                    EnemyHealths[i] = h;

                    pierce.Remaining -= 1;
                    if (pierce.Remaining < 0)
                    {
                        Ecb.DestroyEntity(projEntity);
                        return;
                    }
                }
            }
        }
    }
}
