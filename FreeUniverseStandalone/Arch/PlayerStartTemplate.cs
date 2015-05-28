using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Arch
{
    public class PlayerStartTemplate
    {
        public ulong id;
        public string nickname;
        public ulong hull;
        public Vector3 position;

        public PlayerStartTemplate()
        {

        }

        public PlayerStartTemplate(string nickname, string hull, Vector3 pos)
        {
            this.nickname = nickname;
            this.id = HashUtils.StringToUINT64(this.nickname);
            this.hull = HashUtils.StringToUINT64(hull);
            this.position = pos;
        }

        public static Dictionary<ulong, PlayerStartTemplate> templates { get; set; }
    }
}
