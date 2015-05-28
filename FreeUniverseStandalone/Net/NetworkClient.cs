using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using UnityEngine;

namespace FreeUniverse.Net
{
    public class NetworkClient
    {
        INetworkClientDelegate _delegate;
        Lidgren.Network.NetClient _client;
        Lidgren.Network.NetPeerConfiguration _config;
        INetworkMessageFactory _msgFactory;

        public INetworkMessageFactory messageFactory
        {
            get { return _msgFactory; }
            set { _msgFactory = value; }
        }

        public NetworkClient()
        {
            _msgFactory = new DefaultNetworkMessageFactory();

            _config = new NetPeerConfiguration("FreeUniverse");
            //_config.Port = ConstData.CLIENT_PORT;
            _client = new NetClient(_config);
        }

        public void Start()
        {
            _client.Start();
        }

        public void Shutdown()
        {
            _client.Shutdown("bye");
        }

        public int Connect(string ip, int port)
        {
            _client.Connect(ip, port);
            return 0;
        }

        public void Disconnect()
        {
            _client.Disconnect("bye");
        }

        public bool IsConnected()
        {
            return _client.ConnectionStatus == NetConnectionStatus.Connected;
        }

        public void Update()
        {
            if (_client == null)
                return;

            NetIncomingMessage msg;

            while ((msg = _client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();

                            if (_delegate != null)
                            {
                                if (status == NetConnectionStatus.Connected)
                                {
                                    _delegate.OnNetworkClientConnect(this, 1);
                                }

                                if (status == NetConnectionStatus.Disconnected)
                                {
                                    _delegate.OnNetworkClientDisconnect(this, 1);
                                }
                            }

                        }
                        break;

                    case NetIncomingMessageType.Data:
                        {
                            NetworkMessage flmsg = _msgFactory.Create(msg);

                            if ((_delegate != null) && (flmsg != null))
                                _delegate.OnNetworkClientMessage(this, flmsg);
                        }
                        break;

                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    default:
                        break;
                }

                _client.Recycle(msg);
            }
        }

        public void Send(NetworkMessage message, NetDeliveryMethod deliveryType)
        {
            NetOutgoingMessage msg = _client.CreateMessage();
            message.Write(msg);
            _client.SendMessage(msg, deliveryType);
        }

        public INetworkClientDelegate netClientDelegate
        {
            get { return _delegate; }
            set { _delegate = value; }
        }
    }

}
