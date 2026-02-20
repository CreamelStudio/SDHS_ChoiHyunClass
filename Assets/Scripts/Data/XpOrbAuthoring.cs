using Unity.Entities;
using UnityEngine;

public class XpOrbAuthoring : MonoBehaviour
{
    public int xpValue = 1;
    public float pickupRadius = 0.35f;

    class Baker : Baker<XpOrbAuthoring>
    {
        public override void Bake(XpOrbAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<XpOrbTag>(e);
            AddComponent(e, new XpValue { Value = authoring.xpValue });
            AddComponent(e, new HitRadius { Value = authoring.pickupRadius });
        }
    }
}
