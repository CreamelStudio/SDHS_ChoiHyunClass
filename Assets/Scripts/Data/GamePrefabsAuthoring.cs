using Unity.Entities;
using UnityEngine;

public struct GamePrefabs : IComponentData
{
    public Entity ProjectilePrefab;
    public Entity XpOrbPrefab;
}

public class GamePrefabsAuthoring : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject xpOrbPrefab;

    class Baker : Baker<GamePrefabsAuthoring>
    {
        public override void Bake(GamePrefabsAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.None);

            var proj = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic);
            var xp = GetEntity(authoring.xpOrbPrefab, TransformUsageFlags.Dynamic);

            AddComponent(e, new GamePrefabs
            {
                ProjectilePrefab = proj,
                XpOrbPrefab = xp
            });
        }
    }
}
