using Unity.Entities;
using Unity.Mathematics;

public struct ProjectileTag : IComponentData { }

public struct Velocity2D : IComponentData
{
    public float2 Value;
}

public struct Lifetime : IComponentData
{
    public float TimeLeft;
}