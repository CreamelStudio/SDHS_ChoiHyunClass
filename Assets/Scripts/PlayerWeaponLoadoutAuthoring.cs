using Unity.Entities;
using UnityEngine;

public class PlayerWeaponLoadoutAuthoring : MonoBehaviour
{
    public bool startPistol = true;
    public bool startShotgun = false;
    public bool startRailgun = false;
    public bool startKnife = true;

    class Baker : Baker<PlayerWeaponLoadoutAuthoring>
    {
        public override void Bake(PlayerWeaponLoadoutAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.Dynamic);

            var buf = AddBuffer<EquippedWeapon>(e);

            void Add(WeaponType t)
            {
                buf.Add(new EquippedWeapon { Type = t, Level = 1, CooldownLeft = 0f });
            }

            if (authoring.startPistol) Add(WeaponType.Pistol);
            if (authoring.startShotgun) Add(WeaponType.Shotgun);
            if (authoring.startRailgun) Add(WeaponType.Railgun);
            if (authoring.startKnife) Add(WeaponType.Knife);
        }
    }
}
