using Unity.Entities;

public struct PlayerTag : IComponentData { }

public struct PlayerInput : IComponentData
{
    public Unity.Mathematics.float2 Move;
}

public struct MoveSpeed : IComponentData
{
    public float Value;
}

public struct Health : IComponentData
{
    public int Current;
    public int Max;
}

public struct Damage : IComponentData
{
    public int Value;
}

public struct HitRadius : IComponentData
{
    public float Value;
}
