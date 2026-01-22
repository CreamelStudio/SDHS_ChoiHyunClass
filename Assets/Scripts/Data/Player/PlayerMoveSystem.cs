using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<PlayerInput>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var input = SystemAPI.GetSingleton<PlayerInput>().Move;
        var dt = SystemAPI.Time.DeltaTime;

        foreach (var (transform, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>>().WithAll<PlayerTag>())
        {
            float2 move = input;
            if (math.lengthsq(move) > 1f) move = math.normalize(move);

            var p = transform.ValueRO.Position;
            p.x += move.x * speed.ValueRO.Value * dt;
            p.y += move.y * speed.ValueRO.Value * dt;
            transform.ValueRW.Position = p;
        }
    }
}