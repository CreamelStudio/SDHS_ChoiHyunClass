using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float interval = 0.25f;
    public int countPerWave = 5;

    class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.None);

            AddComponent(e, new SpawnCooldown { TimeLeft = 0f });
            AddComponent(e, new SpawnRate { Interval = authoring.interval });
            AddComponent(e, new SpawnCountPerWave { Value = authoring.countPerWave });


            var prefabEntity = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic);
            AddComponent(e, new EnemyPrefab { Value = prefabEntity });
        }
    }
}