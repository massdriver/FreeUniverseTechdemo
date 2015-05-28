using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using UnityEngine;

namespace FreeUniverse.Net
{
    public class NetworkMessage
    {
        public int clientID { get; set; } // this field is not serialized, it is filled only when server receives message from client

        public NetworkMessageType type { get; protected set; }

        public NetworkMessage()
        {
            clientID = -1;
            type = NetworkMessageType.Null;
        }

        public static NetworkMessageType PeekType(NetIncomingMessage msg)
        {
            return (NetworkMessageType)msg.PeekInt16();
        }

        public virtual NetworkMessage Read(NetIncomingMessage msgIn)
        {
            type = (NetworkMessageType)msgIn.ReadInt16();
            return this;
        }

        public virtual NetworkMessage Write(NetOutgoingMessage msgOut)
        {
            msgOut.Write((short)type);
            return this;
        }

        protected void WriteVector3(Vector3 vec, NetOutgoingMessage msgOut)
        {
            msgOut.Write(vec.x);
            msgOut.Write(vec.y);
            msgOut.Write(vec.z);
        }

        protected Vector3 ReadVector3(NetIncomingMessage msgIn)
        {
            return new Vector3(msgIn.ReadFloat(), msgIn.ReadFloat(), msgIn.ReadFloat());
        }

        protected void WriteQuaternion(Quaternion vec, NetOutgoingMessage msgOut)
        {
            msgOut.Write(vec.x);
            msgOut.Write(vec.y);
            msgOut.Write(vec.z);
            msgOut.Write(vec.w);
        }

        protected Quaternion ReadQuaternion(NetIncomingMessage msgIn)
        {
            return new Quaternion(msgIn.ReadFloat(), msgIn.ReadFloat(), msgIn.ReadFloat(), msgIn.ReadFloat());
        }

        protected void WriteBytes(byte[] data, NetOutgoingMessage msgOut)
        {
            msgOut.Write(data.Length);
            msgOut.Write(data);
        }

        protected byte[] ReadBytes(NetIncomingMessage msgIn)
        {
            return msgIn.ReadBytes(msgIn.ReadInt32());
        }
    }
}
