using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct ProjectileMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (t, v, life) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<ProjectileVelocity>, RefRW<ProjectileLifetime>>().WithAll<ProjectileTag>())
        {
            var p = t.ValueRO.Position;
            p.x += v.ValueRO.Value.x * dt;
            p.y += v.ValueRO.Value.y * dt;
            t.ValueRW.Position = p;

            life.ValueRW.TimeLeft -= dt;
        }
    }
}
