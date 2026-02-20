using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct XpPickupAndLevelUpSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged);

        var player = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerT = SystemAPI.GetComponent<LocalTransform>(player);
        float2 playerPos = new float2(playerT.Position.x, playerT.Position.y);

        float pickupR = SystemAPI.GetComponent<HitRadius>(player).Value;
        var prog = SystemAPI.GetComponentRW<PlayerProgress>(player);

        int gained = 0;

        // ✅ Entity를 Query 제네릭으로 넣지 말고 WithEntityAccess로 받기
        foreach (var (orbT, orbR, xp, orbEntity) in
                 SystemAPI.Query<LocalTransform, HitRadius, XpValue>()
                          .WithAll<XpOrbTag>()
                          .WithEntityAccess())
        {
            float2 op = new float2(orbT.Position.x, orbT.Position.y);
            float rr = pickupR + orbR.Value;

            float2 d = op - playerPos;
            if (math.lengthsq(d) <= rr * rr)
            {
                gained += xp.Value;
                ecb.DestroyEntity(orbEntity);
            }
        }

        if (gained > 0)
            prog.ValueRW.CurrentXp += gained;

        while (prog.ValueRO.CurrentXp >= prog.ValueRO.NextXp)
        {
            prog.ValueRW.CurrentXp -= prog.ValueRO.NextXp;
            prog.ValueRW.Level += 1;
            prog.ValueRW.NextXp = math.floor(prog.ValueRO.NextXp * 1.25f + 2f);

            ApplyRandomUpgrade(ref state, player);
            Debug.Log($"LEVEL UP -> {prog.ValueRO.Level}");
        }
    }

    static void ApplyRandomUpgrade(ref SystemState state, Entity player)
    {
        var em = state.EntityManager;
        int r = UnityEngine.Random.Range(0, 5);

        var stats = em.GetComponentData<PlayerCombatStats>(player);
        switch (r)
        {
            case 0: stats.DamageMul *= 1.10f; break;
            case 1: stats.AttackSpeedMul *= 1.12f; break;
            case 2: stats.RangeAdd += 0.35f; break;
            case 3: stats.ProjectileSpeedMul *= 1.15f; break;
            case 4: stats.PierceAdd += 1; break;
        }
        em.SetComponentData(player, stats);
    }
}
