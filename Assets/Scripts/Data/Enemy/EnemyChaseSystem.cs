using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyChaseSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerPos3 = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;
        float2 playerPos = new float2(playerPos3.x, playerPos3.y);

        foreach (var (transform, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>>().WithAll<EnemyTag, TargetPlayer>())
        {
            var p3 = transform.ValueRO.Position;
            float2 p = new float2(p3.x, p3.y);

            float2 to = playerPos - p;
            float d2 = math.lengthsq(to);
            if (d2 < 0.0001f) continue;

            float2 dir = to * math.rsqrt(d2);

            p += dir * speed.ValueRO.Value * dt;
            transform.ValueRW.Position = new float3(p.x, p.y, p3.z);
        }
    }
}