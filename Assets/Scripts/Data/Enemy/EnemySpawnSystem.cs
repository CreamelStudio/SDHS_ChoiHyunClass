using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyPrefab>();
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerPos3 = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        float2 playerPos = new float2(playerPos3.x, playerPos3.y);

        foreach (var (cooldown, rate, count, prefab) in SystemAPI.Query<RefRW<SpawnCooldown>, RefRO<SpawnRate>, RefRO<SpawnCountPerWave>, RefRO<EnemyPrefab>>())
        {
            cooldown.ValueRW.TimeLeft -= dt;
            if (cooldown.ValueRO.TimeLeft > 0f) continue;

            cooldown.ValueRW.TimeLeft = rate.ValueRO.Interval;

            for (int i = 0; i < count.ValueRO.Value; i++)
            {
                var e = state.EntityManager.Instantiate(prefab.ValueRO.Value);
                

                float angle = (i * 0.6180339f) * 6.2831853f;
                float dist = 10f + i * 0.15f;
                float2 spawnPos = playerPos + new float2(math.cos(angle), math.sin(angle)) * dist;

                var t = LocalTransform.FromPosition(new float3(spawnPos.x, spawnPos.y, 0f));
                state.EntityManager.SetComponentData(e, t);
            }
        }
    }
}