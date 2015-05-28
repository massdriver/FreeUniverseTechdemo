using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.ViewControllers
{
    public interface IViewControllerClientLoginDelegate
    {
        void OnClientLoginViewControllerActionLogin(ViewControllerClientLogin controller, string email, string password);
        void OnClientLoginViewControllerActionQuit(ViewControllerClientLogin controller);
        void OnClientLoginViewControllerActionConnectionAttemptAborted(ViewControllerClientLogin controller);
        void OnClientLoginViewControllerActionCreateAccount(ViewControllerClientLogin controller);
    }
}
