using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net;

namespace FreeUniverse.World.Database
{
    public class UniverseDatabaseServer : UniverseDatabase, INetworkServerDelegate
    {
        IDatabaseStorage<DatabaseStorageElementAccount> _accountStorage = DatabaseStorageFactory<DatabaseStorageElementAccount>.Create(TABLE_ACCOUNTS);
        IDatabaseStorage<DatabaseStorageElementCharacter> _characterStorage = DatabaseStorageFactory<DatabaseStorageElementCharacter>.Create(TABLE_CHARACTERS);
        IDatabaseStorage<DatabaseStorageElementSolar> _solarStorage = DatabaseStorageFactory<DatabaseStorageElementSolar>.Create(TABLE_SOLARS);

        public static string TABLE_ACCOUNTS = "accounts";
        public static string TABLE_CHARACTERS = "characters";
        public static string TABLE_SOLARS = "solars";
        public static int UNIVERSE_DATABASE_SERVER_PORT = 32128;
        public static int UNIVERSE_DATABASE_MAX_CLIENTS = 32;

        NetworkServer _server = new NetworkServer(UNIVERSE_DATABASE_MAX_CLIENTS, UNIVERSE_DATABASE_SERVER_PORT);

        // zone server clients, not players
        UniverseDatabaseClientDescription[] _clients = new UniverseDatabaseClientDescription[UNIVERSE_DATABASE_MAX_CLIENTS];

        public UniverseDatabaseServer()
        {

        }

        public bool AccountCreate(string user, string password)
        {
            DatabaseStorageElementAccount account = new DatabaseStorageElementAccount();
            account.id = HashUtils.StringToUINT64(user);
            account.user = user;
            account.password = password;

            if (_accountStorage.Insert(account) == 1)
                return true;

            return false;
        }

        public void LoadAccountData(ulong account)
        {

        }

        public DatabaseStorageElementAccount AccountLogin(string user, string password)
        {
            DatabaseStorageElementAccount account;
            _accountStorage.Get(HashUtils.StringToUINT64(user), out account);

            if (account == null)
                return null;

            if (account.password.CompareTo(password) != 0)
                return null;

            _accounts[account.id] = account;

            // read character and solar data
            LoadAccountData(account.id);

            return account;
        }

        void AccountLogout(ulong id)
        {
            DatabaseStorageElementAccount account = null;
            _accounts.TryGetValue(id, out account);

            if (account == null)
                return;

            StoreAccountDataFull(account);
            _accounts.Remove(account.id);
        }

        void StoreAccountDataFull(DatabaseStorageElementAccount account)
        {
            _accountStorage.Update(account);

            // store characters
            // ***

            // store solars
            // ***
        }

        public void Start()
        {
            _server.messageFactory = new DefaultNetworkMessageFactory();
            _server.netServerDelegate = this;
            _server.Start();
        }

        public void Shutdown()
        {
            _server.Shutdown();
        }

        public void OnNetworkServerClientConnected(NetworkServer server, int client)
        {
            _clients[client] = new UniverseDatabaseClientDescription();
        }

        public void OnNetworkServerClientDisconnected(NetworkServer server, int client)
        {
            _clients[client] = null;
        }

        public void OnNetworkServerClientMessage(NetworkServer server, int client, NetworkMessage msg)
        {
            switch (msg.type)
            {
                default:
                    break;
            }
        }

        private class UniverseDatabaseClientDescription
        {
            int _id = -1;
            public int id { get { return _id; } set { _id = value; } }
        }
    }
}
