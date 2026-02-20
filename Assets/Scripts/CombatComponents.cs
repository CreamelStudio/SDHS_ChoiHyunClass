using Unity.Entities;
using Unity.Mathematics;

public enum WeaponType : byte
{
    Pistol,
    Shotgun,
    Railgun,
    Knife
}

public struct PlayerProgress : IComponentData
{
    public int Level;
    public float CurrentXp;
    public float NextXp;
}

public struct PlayerCombatStats : IComponentData
{
    public float DamageMul;
    public float AttackSpeedMul;
    public float RangeAdd;
    public float ProjectileSpeedMul;
    public int PierceAdd;
}

public struct EquippedWeapon : IBufferElementData
{
    public WeaponType Type;
    public int Level;
    public float CooldownLeft;
}

public struct ProjectileTag : IComponentData { }

public struct ProjectileVelocity : IComponentData
{
    public float2 Value;
}

public struct ProjectileLifetime : IComponentData
{
    public float TimeLeft;
}

public struct ProjectilePierce : IComponentData
{
    public int Remaining;
}

public struct ProjectileOwner : IComponentData
{
    public Entity Value;
}

public struct EnemyXpValue : IComponentData
{
    public int Value;
}

public struct XpOrbTag : IComponentData { }

public struct XpValue : IComponentData
{
    public int Value;
}
