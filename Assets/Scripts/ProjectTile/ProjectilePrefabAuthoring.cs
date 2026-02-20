using Unity.Entities;
using UnityEngine;

public class ProjectilePrefabAuthoring : MonoBehaviour
{
    public float hitRadius = 0.15f;
    public float lifetime = 2.0f;

    class Baker : Baker<ProjectilePrefabAuthoring>
    {
        public override void Bake(ProjectilePrefabAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<ProjectileTag>(e);
            AddComponent(e, new HitRadius { Value = authoring.hitRadius });
            AddComponent(e, new ProjectileLifetime { TimeLeft = authoring.lifetime });
        }
    }
}
