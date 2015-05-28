using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Arch
{
    public enum ProjectileType
    {
        Spherical
    }

    public enum ProjectileAimType
    {
        None,
        FOV,
        Heat
    }

    public class ProjectileDesc
    {
        public ProjectileType projectile_type;
        public string projectile_prefab;
        public float projectile_radius;
        public float projectile_launch_speed;
        public float projectile_lifetime;
        public float projectile_damage_energy;
        public float projectile_damage_explosion;
        public float projectile_damage_projectile;
        public float projectile_damage_darkmatter;
        public float projectile_random_damage;
        public ProjectileAimType projectile_aim_type;
        public float projectile_aim_fov;
        public float projectile_turn_rate;
        public ulong projectile_effect;
        public float projectile_detonation_radius;
        public float projectile_detonation_damage_radius;
        public ulong projectile_detonation_effect;
        public ulong projectile_shield_hit_effect;
        public string projectile_hull_hit_effect;
        public float projectile_hull_hit_effect_duration = 5.0f;

        ProjectileType ProjectileTypeFromString(string s)
        {
            return ProjectileType.Spherical;
        }

        ProjectileAimType AimTypeFromString(string s)
        {
            return ProjectileAimType.FOV;
        }

        public void Read(INIReaderParameter p)
        {
            if (p.Check("projectile_radius")) projectile_radius = p.GetFloat(0);
            else if (p.Check("projectile_type")) projectile_type = ProjectileTypeFromString(p.GetString(0));
            else if (p.Check("projectile_aim_type")) projectile_aim_type = AimTypeFromString(p.GetString(0));
            else if (p.Check("projectile_launch_speed")) projectile_launch_speed = p.GetFloat(0);
            else if (p.Check("projectile_lifetime")) projectile_lifetime = p.GetFloat(0);
            else if (p.Check("projectile_damage_energy")) projectile_damage_energy = p.GetFloat(0);
            else if (p.Check("projectile_damage_explosion")) projectile_damage_explosion = p.GetFloat(0);
            else if (p.Check("projectile_damage_projectile")) projectile_damage_projectile = p.GetFloat(0);
            else if (p.Check("projectile_damage_darkmatter")) projectile_damage_darkmatter = p.GetFloat(0);
            else if (p.Check("projectile_aim_fov")) projectile_aim_fov = p.GetFloat(0);
            else if (p.Check("projectile_turn_rate")) projectile_turn_rate = p.GetFloat(0);
            else if (p.Check("projectile_effect")) projectile_effect = p.GetStrkey64(0);
            else if (p.Check("projectile_detonation_radius")) projectile_detonation_radius = p.GetFloat(0);
            else if (p.Check("projectile_detonation_damage_radius")) projectile_detonation_damage_radius = p.GetFloat(0);
            else if (p.Check("projectile_detonation_effect")) projectile_detonation_effect = p.GetStrkey64(0);
            else if (p.Check("projectile_shield_hit_effect")) projectile_shield_hit_effect = p.GetStrkey64(0);
            else if (p.Check("projectile_hull_hit_effect"))
            {
                projectile_hull_hit_effect = p.GetString(0);
                projectile_hull_hit_effect_duration = p.GetFloat(1);
            }
            else if (p.Check("projectile_prefab"))
            {
                projectile_prefab = p.GetString(0);
            }

        }
    }

    public enum WeaponType
    {
        Energy,
        Projectile,
        DarkMatter,
        Explosive
    }

    public class ArchWorldObjectComponentPropertyWeapon : ArchWorldObjectComponentProperty
    {
        ProjectileDesc _projectileDesc = new ProjectileDesc();
        public ProjectileDesc projectileDesc { get { return _projectileDesc; } }

        WeaponType _weaponType = WeaponType.Energy;
        float _powerPerShot = 1000.0f;
        int _ammoPerShot = 0;
        int _maxAmmo = 0;
        ulong _requiredCargoAmmo = 0;
        string _weaponModel;
        Vector3 _relativeLaunchPoint;
        Vector3 _relativeLaunchOrientation;
        bool _disableCloakOnFire = true;
        bool _canBeFiredWhenCloaked = true;
        bool _fixedDirection = false;
        float _maxFireAngleCone = 35.0f;
        float _cannonTurnRate = 1.0f;
        int _modSlots = 3;
        ulong _modSlotType = 0;
        string _launchSound;
        float _refireRate = 0.5f;

        public float refireRate { get { return _refireRate; } }
        public string launchSound { get { return _launchSound; } }
        public WeaponType weaponType { get { return _weaponType; } }
        public float powerPerShot { get { return _powerPerShot; } }
        public int ammoPerShot { get { return _ammoPerShot; } }
        public int maxAmmo { get { return _maxAmmo; } }
        public ulong requiredCargoAmmo { get { return _requiredCargoAmmo; } }
        public string weaponModel { get { return _weaponModel; } }
        public Vector3 relativeLaunchPoint { get { return _relativeLaunchPoint; } }
        public Vector3 relativeLaunchOrientation { get { return _relativeLaunchOrientation; } }
        public bool disableCloakOnFire { get { return _disableCloakOnFire; } }
        public bool canBeFiredWhenCloaked { get { return _canBeFiredWhenCloaked; } }
        public bool fixedDirection { get { return _fixedDirection; } }
        public float maxFireAngleCone { get { return _maxFireAngleCone; } }
        public float cannonTurnRate { get { return _cannonTurnRate; } }
        public int modSlots { get { return _modSlots; } }
        public ulong modSlotType { get { return _modSlotType; } }

        WeaponType WeaponTypeFromString(string s)
        {
            return WeaponType.Energy;
        }

        public override void ReadHeader(INIReaderHeader header)
        {
            foreach (INIReaderParameter p in header.parameters)
            {
                if (p.Check("weapon_type"))
                {
                    _weaponType = WeaponTypeFromString(p.GetString(0));
                }
                else if (p.Check("refire_rate"))
                {
                    _refireRate = p.GetFloat(0);
                }
                else if (p.Check("mod_slot_type"))
                {
                    _modSlotType = p.GetStrkey64(0);
                }
                else if (p.Check("mod_slots"))
                {
                    _modSlots = p.GetInt(0);
                }
                else if (p.Check("cannon_turn_rate"))
                {
                    _cannonTurnRate = p.GetFloat(0);
                }
                else if (p.Check("max_fire_angle_cone"))
                {
                    _maxFireAngleCone = p.GetFloat(0);
                }
                else if (p.Check("fixed_direction"))
                {
                    _fixedDirection = p.GetBool(0);
                }
                else if (p.Check("can_be_fired_when_cloaked"))
                {
                    _canBeFiredWhenCloaked = p.GetBool(0);
                }
                else if (p.Check("disable_cloak_on_fire"))
                {
                    _disableCloakOnFire = p.GetBool(0);
                }
                else if (p.Check("relative_launch_orientation"))
                {
                    _relativeLaunchOrientation = p.GetVector3(0);
                }
                else if (p.Check("relative_launch_point"))
                {
                    _relativeLaunchPoint = p.GetVector3(0);
                }
                else if (p.Check("weapon_model"))
                {
                    _weaponModel = p.GetString(0);
                }
                else if (p.Check("require_cargo_ammo"))
                {
                    _requiredCargoAmmo = p.GetStrkey64(0);
                }
                else if (p.Check("max_ammo"))
                {
                    _maxAmmo = p.GetInt(0);
                }
                else if (p.Check("ammo_per_shot"))
                {
                    _ammoPerShot = p.GetInt(0);
                }
                else if (p.Check("launch_sound"))
                {
                    _launchSound = p.GetString(0);
                }
                else if (p.Check("power_per_shot"))
                {
                    _powerPerShot = p.GetFloat(0);
                }
                else
                    _projectileDesc.Read(p);
            }
        }
    }
}
