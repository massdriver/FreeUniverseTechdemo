using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Core.Serialization;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using FreeUniverse.Core;
using UnityEngine;

namespace FreeUniverse.Common.Database
{
    public class MySqlAccountDatabase : MySqlDatabase<Account>
    {
        public MySqlAccountDatabase(MySqlConnection sql) : base(sql)
        {

        }

        bool Validate(string email, string password)
        {
            if (email.Length < 5)
                return false;

            if (!Helpers.IsValidAlphaNumeric(email))
                return false;

            if (password.Length < 6)
                return false;

            return false;
        }

        private const string PASSWORD_SALT = "some_random_salt_you_know_bro";

        public ulong GetAccountID(string email)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = sqlConnection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "select id from free_universe_database.accounts where user_email=@email";
                    cmd.Parameters.AddWithValue("@email", email);
                    return (ulong)cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Debug.Log("AccDB: error on GetAccountID");
            }

            return 0;
        }

        public ulong GetPremiumCurrency(ulong accid)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select premium_currency from free_universe_database.accounts where id=@accid";
                cmd.Parameters.AddWithValue("@accid", accid);
                return (ulong)cmd.ExecuteScalar();
            }
        }

        public Account Login(string email, string password)
        {

            string passwordHash = HashUtils.SHA256(password + PASSWORD_SALT);

            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select free_universe_database.AccountLogin(@email, @password)";
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", passwordHash);

                try
                {
                    string result = cmd.ExecuteScalar() as string;

                    if (result == null)
                        return null;

                    Account account = serializer.Deserialize(result) as Account;

                    account.lastLoginDate = DateTime.Now;

                    // A fresh account created by php environment on registration
                    if (result.Length == 0)
                    {
                        account.id = GetAccountID(email);
                        account.email = email;
                        account.creationDate = DateTime.Now;

                        Store(account);
                    }
                    return account;
                }
                catch (Exception e)
                {
                    Debug.Log("AccDB: account data deserialization failed exc=" + e.GetType().FullName);
                }
            }
            
            return null;
        }

        public bool Store(Account account)
        {
            if (account.id == 0)
                return false;

            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = sqlConnection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "update free_universe_database.accounts set data=@val where id=@accid";
                    cmd.Parameters.AddWithValue("@val", serializer.Serialize(account));
                    cmd.Parameters.AddWithValue("@accid", account.id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.Log("AccDB: failed to store account");
            }

            return false;
        }

        public DatabaseResult CreateNewAccount(string email, string password)
        {
            if (!Validate(email, password))
                return DatabaseResult.InvalidCredentials;

            Account account = new Account(email, password);
            string activationKey = MakeUniqueActivationCode();
            string passwordHash = HashUtils.CalculateMD5Hash(password+PASSWORD_SALT);
            
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "select free_universe_database.AccountCreate(@email, @password, @code, @data)";
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", passwordHash);
                cmd.Parameters.AddWithValue("@code", activationKey);
                cmd.Parameters.AddWithValue("@data", serializer.Serialize(account));

                try
                {
                    if ((int)cmd.ExecuteScalar() == 0)
                        return DatabaseResult.Ok;
                }
                catch (MySqlException e)
                {
                    return DatabaseResult.Error;
                }
            }

            return DatabaseResult.Error;
        }

        string MakeUniqueActivationCode()
        {
            string key = null;

            while (true)
            {
                key = HashUtils.RandomSHA256();
                if (CheckActivationCode(key))
                    break;
            }

            return key;
        }

        bool CheckActivationCode(string key)
        {
            return true;
        }
    }
}
