using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Net
{
    public class NetworkMessageHandler
    {
        protected delegate void NetMsgHandler(NetworkMessage msg);
        protected Dictionary<NetworkMessageType, NetMsgHandler> msgHandlers { get; set; }

        public virtual void ProcessNetworkMessage(NetworkMessage msg)
        {
            try
            {
                msgHandlers[msg.type].Invoke(msg);
            }
            catch (Exception e)
            {
                Debug.Log("NetworkMessageHandler: error on net message processing msg=" + msg.type.ToString());
                Debug.Log("NetworkMessageHandler: exception=" + e.Message + " type=" + e.GetType().FullName);
            }
        }

        protected virtual void InitNetMessageHandlers()
        {
            msgHandlers = new Dictionary<NetworkMessageType, NetMsgHandler>();
        }
    }
}
