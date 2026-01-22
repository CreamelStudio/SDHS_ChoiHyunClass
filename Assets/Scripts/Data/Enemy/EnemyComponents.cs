using Unity.Entities;

public struct EnemyTag : IComponentData { }

public struct TargetPlayer : IComponentData { }

public struct SpawnCooldown : IComponentData
{
    public float TimeLeft;
}

public struct SpawnRate : IComponentData
{
    public float Interval;
}

public struct SpawnCountPerWave : IComponentData
{
    public int Value;
}

public struct EnemyPrefab : IComponentData
{
    public Entity Value;
}