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
        }
    }
}