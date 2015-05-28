using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Arch
{
    public enum HullHitboxType
    {
        Unknown,
        Sphere,
        Box,
        StaticMesh
    }

    public struct HullHitboxParameters : ICloneable
    {
        public float a;
        public float b;
        public float c;
        public HullHitboxType type;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public HullHitboxParameters(HullHitboxParameters src)
        {
            a = src.a;
            b = src.b;
            c = src.c;
            type = src.type;
            position = Helpers.CopyVector3(src.position);
            rotation = Helpers.CopyVector3(src.rotation);
            scale = Helpers.CopyVector3(src.scale);
        }

        HullHitboxType FromString(string str)
        {
            if (str.CompareTo("sphere") == 0)
                return HullHitboxType.Sphere;

            if (str.CompareTo("box") == 0)
                return HullHitboxType.Box;

            if (str.CompareTo("static_mesh") == 0)
                return HullHitboxType.StaticMesh;

            return HullHitboxType.Unknown;
        }

        public void Read(INIReaderParameter p)
        {
            type = FromString(p.GetString(0));
            a = p.GetFloat(1);
            b = p.GetFloat(2);
            c = p.GetFloat(3);
            position = p.GetVector3(4);
            rotation = p.GetVector3(4 + 3);
            scale = p.GetVector3(4 + 3 + 3);
        }

        #region ICloneable Members

        public object Clone()
        {
            return new HullHitboxParameters(this);
        }

        #endregion
    }

    public struct CameraViewProperties
    {
        public Vector2 offset;

        public CameraViewProperties(CameraViewProperties src)
        {
            offset = new Vector2(src.offset.x, src.offset.y);
        }

        public void Read(INIReaderParameter p)
        {
            offset = p.GetVector2(0);
        }
    }

    public class ArchWorldObjectComponentPropertyHull : ArchWorldObjectComponentProperty
    {
        public ArchWorldObjectComponentPropertyHull()
        {

        }

        public ArchWorldObjectComponentPropertyHull(ArchWorldObjectComponentPropertyHull src)
        {
            this._id = src.id;
            this._hitboxShape = src.hitboxShape;
            this._invincible = src.invincible;
            this._mass = src.mass;
            this._mobility = src.mobility;
            this._model = src.model;
            this._nickname = src.nickname;
            this._resistanceDarkMatter = src.resistanceDarkMatter;
            this._resistanceEnergy = src.resistanceEnergy;
            this._resistanceExplosive = src.resistanceExplosive;
            this._resistanceProjectile = src.resistanceProjectile;
            this._staticHitbox = src.staticHitbox;
            this._strDesc = src.strDesc;
            this._strInfo = src.strInfo;
            this._strName = src.strName;
            this._structurePoints = src.structurePoints;
            this._type = src.type;
            this._cameraProperties = new CameraViewProperties(src.cameraProperties);
            this._hullHitboxes = Helpers.DeepCopyCloneableList<HullHitboxParameters>(src._hullHitboxes);
        }

        List<HullHitboxParameters> _hullHitboxes = new List<HullHitboxParameters>();
        public List<HullHitboxParameters> hullHitboxes { get{ return _hullHitboxes; } }

        CameraViewProperties _cameraProperties = new CameraViewProperties();
        public CameraViewProperties cameraProperties { get { return _cameraProperties; } set { _cameraProperties = value; } }

        float _mass = 1.0f;
        public float mass { get { return _mass; } set { _mass = value; } }

        float _mobility = 1.0f;
        public float mobility { get { return _mobility; } set { _mobility = value; } }

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

        bool _invincible = false;
        public bool invincible { get { return _invincible; } set { _invincible = value; } }

        bool _staticHitbox = false;
        public bool staticHitbox { get { return _staticHitbox; } }

        string _hitboxShape;
        public string hitboxShape { get { return _hitboxShape; } }

        public struct DeathEffect
        {
            public string prefab;
            public float time;
        }

        List<DeathEffect> _deathEffects = new List<DeathEffect>();
        public List<DeathEffect> deathEffects { get { return _deathEffects; } }

        public override void ReadHeader(INIReaderHeader header)
        {
            foreach (INIReaderParameter p in header.parameters)
            {
                if (p.Check("death_effect"))
                {
                    DeathEffect e = new DeathEffect();
                    e.prefab = p.GetString(0);
                    e.time = p.GetFloat(1);
                    _deathEffects.Add(e);
                    continue;
                }

                if (p.Check("model"))
                {
                    _model = p.GetString(0);
                    continue;
                }

                if (p.Check("static_hitbox"))
                {
                    _hitboxShape = p.GetString(0);
                    continue;
                }

                if (p.Check("hitbox_shape"))
                {
                    _hitboxShape = p.GetString(0);
                    continue;
                }

                if (p.Check("mass"))
                {
                    _mass = p.GetFloat(0);
                    continue;
                }

                if (p.Check("structure_points"))
                {
                    _structurePoints = p.GetFloat(0);

                    if (_structurePoints <= 0.0f)
                        _invincible = true;

                    continue;
                }

                if (p.Check("mobility"))
                {
                    _mobility = p.GetFloat(0);
                    continue;
                }

                if (p.Check("resistance_explosion"))
                {
                    _resistanceExplosive = p.GetFloat(0);
                    continue;
                }

                if (p.Check("resistance_projectile"))
                {
                    _resistanceProjectile = p.GetFloat(0);
                    continue;
                }

                if (p.Check("resistance_energy"))
                {
                    _resistanceEnergy = p.GetFloat(0);
                    continue;
                }

                if (p.Check("resistance_darkmatter"))
                {
                    _resistanceDarkMatter = p.GetFloat(0);
                    continue;
                }

                if (p.Check("camera_properties"))
                {
                    _cameraProperties.Read(p);
                    continue;
                }

                if (p.Check("define_hitbox"))
                {
                    HullHitboxParameters hitbox = new HullHitboxParameters();
                    hitbox.Read(p);
                    _hullHitboxes.Add(hitbox);
                    continue;
                }
            }
        }
    }
}
