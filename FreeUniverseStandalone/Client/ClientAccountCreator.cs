using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net;

namespace FreeUniverse.Client
{
    public enum ClientAccountCreatorResult
    {
        AccountCreated,
        AccountExists,
        InvalidCredentials
    }

    public interface IClientAccountCreatorDelegate
    {
        void OnClientAccountCreatorResult(ClientAccountCreator controller, ClientAccountCreatorResult result);
    }

    public class ClientAccountCreator
    {
        public IClientAccountCreatorDelegate controllerDelegate { get; set; }
        private NetworkClient client { get; set; }

        public ClientAccountCreator()
        {

        }

        public void CreateAccount(string email, string password)
        {

        }
    }
}
