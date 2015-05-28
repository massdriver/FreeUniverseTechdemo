using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using FreeUniverse.Arch;
using UnityEngine;

namespace FreeUniverse.Common.Database
{
    public class MySqlCharacterDatabase : MySqlDatabase<CharacterPersonality>
    {
        public MySqlCharacterDatabase(MySqlConnection connection)
            : base(connection)
        {

        }

        public CharacterPersonality CreateCharacter(Account account, CharacterCreateDesc desc)
        {
            if (CharacterExists(desc.name))
                return null;

            CharacterPersonality character = new CharacterPersonality(desc);

            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = sqlConnection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "select free_universe_database.CreateCharacter(@charid, @parentacc, @dataText)";
                    cmd.Parameters.AddWithValue("@charid", HashUtils.StringToUINT64(desc.name));
                    cmd.Parameters.AddWithValue("@parentacc", account.id);
                    cmd.Parameters.AddWithValue("@dataText", serializer.Serialize(character));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.Log("CharDB: error on create character");
                return null;
            }

            return character;
        }

        public ulong GetCharacterID(string name)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = sqlConnection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "select id from free_universe_database.characters where id=@charid";
                    cmd.Parameters.AddWithValue("@charid", HashUtils.StringToUINT64(name));
                    return (ulong)cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Debug.Log("CharDB: error trying to get char id msg=" + e.Message);
                return 0;
            }
        }

        public void LoadCharacters(Account account)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = sqlConnection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    //cmd.CommandText = "select dataText from free_universe_database.characters where parentacc=@charid";
                    //cmd.Parameters.AddWithValue("@charid", HashUtils.StringToUINT64(name));
                    //cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Debug.Log("CharDB: error on character load msg="+e.Message);
            }
        }

        public bool CharacterExists(string name)
        {
            return GetCharacterID(name) != 0;
        }
    }
}
