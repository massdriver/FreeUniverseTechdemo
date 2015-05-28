using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;

namespace FreeUniverse.Common
{
    public enum DamageType
    {
        Energy, // laser plasma etc
        Explosive, // HE ammo, flaks
        Projectile, // ballistic guns armor piercing
        DarkMatter // nomad guns
    }

    public class HullStructureData
    {
        public HullStructureData(ArchWorldObjectComponentPropertyHull arch)
        {
            _arch = arch;
            _structurePoints = arch.structurePoints;
            _resistanceExplosive = arch.resistanceExplosive;
            _resistanceEnergy = arch.resistanceEnergy;
            _resistanceProjectile = arch.resistanceProjectile;
            _resistanceDarkMatter = arch.resistanceDarkMatter;
            _invincible = arch.invincible;
        }

        ArchWorldObjectComponentPropertyHull _arch;
        public ArchWorldObjectComponentPropertyHull arch { get { return _arch; } }

        public static float MAX_RESISTANCE = 0.90f;

        bool _invincible = false;
        public bool invincible { get { return _invincible; } set { _invincible = value; } }

        float _structurePoints = 1.0f;
        public float structurePoints { get { return _structurePoints; } set { _structurePoints = value; } }

        float _resistanceExplosive = 0.0f;
        public float resistanceExplosive { get { return _resistanceExplosive; } set { _resistanceExplosive = value; } }

        float _resistanceEnergy = 0.0f;
        public float resistanceEnergy { get { return _resistanceEnergy; } set { _resistanceEnergy = value; } }

        float _resistanceProjectile = 0.0f;
        public float resistanceProjectile { get { return _resistanceProjectile; } set { _resistanceProjectile = value; } }

        float _resistanceDarkMatter = 0.0f;
        public float resistanceDarkMatter { get { return _resistanceDarkMatter; } set { _resistanceDarkMatter = value; } }

        public void Heal(float value)
        {
            if (value < 0.0f)
                return;

            _structurePoints += value;
        }

        public bool InflictDamage(DamageType type, float value)
        {
            if( value <= 0.0f )
                return false;

            switch (type)
            {
                case DamageType.Energy:
                    _structurePoints -= value * (1.0f - _resistanceEnergy);
                    break;
                case DamageType.Explosive:
                    _structurePoints -= value * (1.0f - _resistanceExplosive);
                    break;
                case DamageType.Projectile:
                    _structurePoints -= value * (1.0f - _resistanceProjectile);
                    break;
                case DamageType.DarkMatter:
                    _structurePoints -= value * (1.0f - _resistanceDarkMatter);
                    break;
            }

            if( _structurePoints > 0.0f )
                return false;

            _structurePoints = 0.0f;
            return true;
        }

    }
}
