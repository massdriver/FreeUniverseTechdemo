using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Core.Serialization;
using FreeUniverse.Common.Database;

namespace FreeUniverse.Common
{
    [FastSerializable]
    public class Account : DatabaseElement<Account>
    {
        public bool logged { get; set; }

        [FastSerializable]
        public string email { get; set; }

        [FastSerializable]
        public DateTime lastLoginDate { get; set; }

        [FastSerializable]
        public DateTime creationDate { get; set; }

        [FastSerializable]
        public List<CharacterPersonality> accountCharacters { get; set; }

        public Account()
        {
            logged = false;
            accountCharacters = new List<CharacterPersonality>();
        }

        public Account(string email, string password) : this()
        {
            id = HashUtils.StringToUINT64(email);
            creationDate = DateTime.Now;
        }
    }
}
