using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float moveSpeedMin = 1f;
    public float moveSpeedMax = 4f;
    public int maxHp = 10;
    public float hitRadius = 0.35f;
    public int contactDamage = 5;

    class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<EnemyTag>(e);
            AddComponent<TargetPlayer>(e);

            AddComponent(e, new MoveSpeed { Value = Random.Range(authoring.moveSpeedMin, authoring.moveSpeedMax) });
            AddComponent(e, new Health { Current = authoring.maxHp, Max = authoring.maxHp });
            AddComponent(e, new HitRadius { Value = authoring.hitRadius });
            AddComponent(e, new Damage { Value = authoring.contactDamage });
        }
    }
}