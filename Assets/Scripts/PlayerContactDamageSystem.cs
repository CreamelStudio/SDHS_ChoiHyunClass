using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerContactDamageSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerT = SystemAPI.GetComponent<LocalTransform>(playerEntity);
        float2 playerPos = new float2(playerT.Position.x, playerT.Position.z);
        float playerR = SystemAPI.GetComponent<HitRadius>(playerEntity).Value;

        var playerHp = SystemAPI.GetComponentRW<Health>(playerEntity);

        foreach (var (enemyT, enemyR, enemyDmg) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<HitRadius>, RefRO<Damage>>().WithAll<EnemyTag>())
        {
            float2 enemyPos = new float2(enemyT.ValueRO.Position.x, enemyT.ValueRO.Position.z);
            float r = playerR + enemyR.ValueRO.Value;

            float2 d = enemyPos - playerPos;
            if (math.lengthsq(d) <= r * r)
            {
                int next = playerHp.ValueRO.Current - enemyDmg.ValueRO.Value;
                playerHp.ValueRW.Current = math.max(0, next);
            }
        }
    }
}