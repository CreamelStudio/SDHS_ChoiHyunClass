using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct PlayerAttackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<GamePrefabs>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var player = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerT = SystemAPI.GetComponent<LocalTransform>(player);
        float2 playerPos = new float2(playerT.Position.x, playerT.Position.y);

        var stats = SystemAPI.GetComponent<PlayerCombatStats>(player);
        var prefabs = SystemAPI.GetSingleton<GamePrefabs>();

        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                           .CreateCommandBuffer(state.WorldUnmanaged);

        // 가장 가까운 적 찾기
        Entity nearest = Entity.Null;
        float nearestD2 = float.MaxValue;
        float2 nearestPos = default;

        foreach (var (eTrans, et) in
                 SystemAPI.Query<RefRO<LocalTransform>>()
                          .WithAll<EnemyTag>()
                          .WithEntityAccess())
        {
            float2 ep = new float2(eTrans.ValueRO.Position.x, eTrans.ValueRO.Position.y);
            float2 d = ep - playerPos;
            float d2 = math.lengthsq(d);

            if (d2 < nearestD2)
            {
                nearestD2 = d2;
                nearest = et;
                nearestPos = ep;
            }
        }

        if (nearest == Entity.Null) return;

        var weapons = state.EntityManager.GetBuffer<EquippedWeapon>(player);

        for (int i = 0; i < weapons.Length; i++)
        {
            var w = weapons[i];
            w.CooldownLeft -= dt;

            if (w.CooldownLeft > 0f)
            {
                weapons[i] = w;
                continue;
            }

            float baseCd = GetBaseCooldown(w.Type);
            w.CooldownLeft = baseCd / math.max(0.01f, stats.AttackSpeedMul);

            switch (w.Type)
            {
                case WeaponType.Pistol:
                    FireProjectile(ecb, prefabs.ProjectilePrefab, playerPos, nearestPos, player,
                        baseDamage: 10f, speed: 14f, pierce: 0, stats, spreadDeg: 0f);
                    break;

                case WeaponType.Shotgun:
                {
                    int pellets = 6 + (w.Level - 1);
                    float spread = 18f;
                    for (int p = 0; p < pellets; p++)
                    {
                        float t = (pellets == 1) ? 0f : (p / (float)(pellets - 1));
                        float angle = math.lerp(-spread, spread, t);
                        FireProjectile(ecb, prefabs.ProjectilePrefab, playerPos, nearestPos, player,
                            baseDamage: 5f, speed: 12f, pierce: 0, stats, spreadDeg: angle);
                    }
                    break;
                }

                case WeaponType.Railgun:
                    FireProjectile(ecb, prefabs.ProjectilePrefab, playerPos, nearestPos, player,
                        baseDamage: 25f, speed: 20f, pierce: 2, stats, spreadDeg: 0f);
                    break;

                case WeaponType.Knife:
                    // ✅ SGSG0002 해결: Query 쓰는 함수는 ref SystemState를 받아야 함
                    KnifeHitInUpdate(ref state, playerPos, stats);
                    break;
            }

            weapons[i] = w;
        }
    }

    // ✅ Query 접근 함수는 ref SystemState가 필요 (SGSG0002 방지)
    void KnifeHitInUpdate(ref SystemState state, float2 playerPos, PlayerCombatStats stats)
    {
        float range = 1.8f + stats.RangeAdd;
        int dmg = (int)math.max(1, math.round(12f * stats.DamageMul));

        foreach (var (enemyT, enemyR, enemyHp) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRO<HitRadius>, RefRW<Health>>()
                          .WithAll<EnemyTag>())
        {
            float2 ep = new float2(enemyT.ValueRO.Position.x, enemyT.ValueRO.Position.y);
            float rr = range + enemyR.ValueRO.Value;

            float2 d = ep - playerPos;
            if (math.lengthsq(d) <= rr * rr)
                enemyHp.ValueRW.Current = math.max(0, enemyHp.ValueRO.Current - dmg);
        }
    }

    static float GetBaseCooldown(WeaponType t) => t switch
    {
        WeaponType.Pistol => 0.35f,
        WeaponType.Shotgun => 0.9f,
        WeaponType.Railgun => 1.2f,
        WeaponType.Knife => 0.45f,
        _ => 0.5f
    };

    // ✅ static OK (Query 안 씀)
    static void FireProjectile(
        EntityCommandBuffer ecb,
        Entity projectilePrefab,
        float2 from,
        float2 target,
        Entity owner,
        float baseDamage,
        float speed,
        int pierce,
        PlayerCombatStats stats,
        float spreadDeg)
    {
        var e = ecb.Instantiate(projectilePrefab);

        float2 dir = target - from;
        float d2 = math.lengthsq(dir);
        dir = (d2 < 0.0001f) ? new float2(1, 0) : dir * math.rsqrt(d2);

        // spread 적용
        float rad = math.radians(spreadDeg);
        float cs = math.cos(rad);
        float sn = math.sin(rad);
        dir = new float2(dir.x * cs - dir.y * sn, dir.x * sn + dir.y * cs);

        ecb.SetComponent(e, LocalTransform.FromPosition(new float3(from.x, from.y, 0f)));
        ecb.SetComponent(e, new ProjectileVelocity { Value = dir * speed * stats.ProjectileSpeedMul });
        ecb.SetComponent(e, new ProjectileOwner { Value = owner });

        int finalDamage = (int)math.max(1, math.round(baseDamage * stats.DamageMul));
        ecb.SetComponent(e, new Damage { Value = finalDamage });

        int finalPierce = pierce + stats.PierceAdd;
        ecb.SetComponent(e, new ProjectilePierce { Remaining = finalPierce });
    }
}
