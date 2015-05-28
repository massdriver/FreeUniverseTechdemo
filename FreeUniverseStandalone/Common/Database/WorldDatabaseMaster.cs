using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using FreeUniverse.Server;
using FreeUniverse.Arch;
using UnityEngine;
using FreeUniverse.Core;

namespace FreeUniverse.Common.Database
{
    public class WorldDatabaseMaster : WorldDatabase
    {
        private MySqlConnection sqlConnection { get; set; }
        private MySqlAccountDatabase accountDatabase { get; set; } // wrapper here since accounts have more specific info
        private MySqlCharacterDatabase characterDatabase { get; set; }

        public ClientDesc[] clients { get; set; }
        private float checkTimer { get; set; }

        public const float CHECK_INTERVAL = 1.0f;
        public int maxClients { get; private set; }

        public WorldDatabaseMaster()
        {
            checkTimer = 0.0f;
            
            maxClients = ConstData.MAX_CLIENTS;
            clients = new ClientDesc[maxClients];

            for (int i = 0; i < maxClients; i++)
            {
                clients[i] = new ClientDesc();
                clients[i].Reset();
            }

            string connectString = "Database=" + ConstData.DATABASE_NAME + ";Data Source=127.0.0.1;User Id=" + ConstData.DATABASE_USER + ";Password=" + ConstData.DATABASE_PASSWORD;
            sqlConnection = new MySqlConnection(connectString);
            sqlConnection.Open();

            accountDatabase = new MySqlAccountDatabase(sqlConnection);
            characterDatabase = new MySqlCharacterDatabase(sqlConnection);
        }

        public void Shutdown()
        {
            sqlConnection.Close();
            sqlConnection = null;
        }

        public Account AccountLogin(string email, string pass, int clid)
        {
            ulong accid = accountDatabase.GetAccountID(email);

            if (accid == 0) return null;

            if (accounts.ContainsKey(accid))
                return null;

            ClientDesc desc = clients[clid];

            if (desc.client != -1 )
                return null;

            Account account = accountDatabase.Login(email, pass);

            if (account == null)
                return null;

            accounts[account.id] = account;

            desc.account = account;
            desc.client = clid;
            desc.status = PlayerClientStatus.LoginServer;
            desc.dataLocation = PlayerClientDataLocation.LoginServer;

            characterDatabase.LoadCharacters(account);

            return account;
        }

        public void AccountLogout(int clid)
        {
            ClientDesc desc = clients[clid];

            if (desc == null)
                return;

            accounts.Remove(desc.account.id);
            desc.Reset();
            clients[clid] = null;
        }

        public void OnPlayerClientDisconnect(int clid)
        {
            ClientDesc desc = clients[clid];

            if (desc == null)
                return;

            if (desc.status == PlayerClientStatus.LoginServer)
                return;

            desc.status = PlayerClientStatus.AwaitLogout;
        }

        public bool AccountCreate(string email, string password)
        {
            return accountDatabase.CreateNewAccount(email, password) == DatabaseResult.Ok;
        }

        public CharacterPersonality CharacterCreate(Account account, CharacterCreateDesc desc)
        {
            return characterDatabase.CreateCharacter(account, desc);
        }

        public void Update(float time)
        {
            checkTimer += time;

            if (checkTimer < CHECK_INTERVAL)
                return;

            checkTimer = 0.0f;

            // Logout everyone
            foreach (ClientDesc desc in clients)
            {
                if (desc == null)
                    continue;

                if (desc.status == PlayerClientStatus.AwaitLogout && desc.dataLocation == PlayerClientDataLocation.LoginServer)
                {
                    AccountLogout(desc.client);
                }
            }
        }
    }
}
