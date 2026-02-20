using Unity.Entities;
using UnityEngine;

public class PlayerEntityAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int maxHp = 100;
    public float hitRadius = 0.5f;

    class Baker : Baker<PlayerEntityAuthoring>
    {
        public override void Bake(PlayerEntityAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PlayerTag>(e);
            AddComponent(e, new MoveSpeed { Value = authoring.moveSpeed });
            AddComponent(e, new Health { Current = authoring.maxHp, Max = authoring.maxHp });
            AddComponent(e, new HitRadius { Value = authoring.hitRadius });
            
            AddComponent(e, new PlayerCombatStats
            {
                DamageMul = 1f,
                AttackSpeedMul = 1f,
                RangeAdd = 0f,
                ProjectileSpeedMul = 1f,
                PierceAdd = 0
            });

            AddComponent(e, new PlayerProgress
            {
                Level = 1,
                CurrentXp = 0f,
                NextXp = 5f
            });

            AddBuffer<EquippedWeapon>(e);
        }
    }
}