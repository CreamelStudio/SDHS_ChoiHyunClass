using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct CleanupDeadAndSpawnXpSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GamePrefabs>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var prefabs = SystemAPI.GetSingleton<GamePrefabs>();
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged);

        // 적 죽음 -> XP 오브 -> 적 삭제
        new SpawnXpOnEnemyDeathJob
        {
            Prefabs = prefabs,
            Ecb = ecb
        }.Run();

        // 투사체 수명 끝 -> 삭제
        new CleanupProjectileJob
        {
            Ecb = ecb
        }.Run();
    }

    [BurstCompile]
    public partial struct SpawnXpOnEnemyDeathJob : IJobEntity
    {
        public GamePrefabs Prefabs;
        public EntityCommandBuffer Ecb;

        void Execute(Entity enemyEntity, in Health hp, in LocalTransform t, in EnemyXpValue xp)
        {
            if (hp.Current > 0) return;

            var orb = Ecb.Instantiate(Prefabs.XpOrbPrefab);
            Ecb.SetComponent(orb, LocalTransform.FromPosition(t.Position));
            Ecb.SetComponent(orb, new XpValue { Value = xp.Value });

            Ecb.DestroyEntity(enemyEntity);
        }
    }

    [BurstCompile]
    public partial struct CleanupProjectileJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;

        void Execute(Entity projEntity, in ProjectileLifetime life, in ProjectileTag tag)
        {
            if (life.TimeLeft <= 0f)
                Ecb.DestroyEntity(projEntity);
        }
    }
}
