using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Arch
{
    public class ArchObjectHardpoint : ArchObject
    {
        public static uint HP_TYPE_ANY = HashUtils.StringToUINT32("hp_type_any");

        uint _hptype = HP_TYPE_ANY;
        public uint hptype { get { return _hptype; } }

        Vector3 _position;
        public Vector3 position { get { return _position; } }

        Vector3 _rotation;
        public Vector3 rotation { get { return _rotation; } }

        Vector2 _viewAngles;
        public Vector2 viewAngles { get { return _viewAngles; } }

        public override void ReadHeader(INIReaderHeader header)
        {
            
        }

        public override void ReadParameter(INIReaderParameter parameter)
        {
            _id = parameter.GetStrkey64(0);
            _nickname = parameter.GetString(0);

            _hptype = parameter.GetStrkey(1);

            _position = new Vector3(parameter.GetFloat(2), parameter.GetFloat(3), parameter.GetFloat(4));
            _rotation = new Vector3(parameter.GetFloat(5), parameter.GetFloat(6), parameter.GetFloat(7));
            _viewAngles = new Vector2(parameter.GetFloat(8), parameter.GetFloat(9));
        }

    }
}
