using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World;
using FreeUniverse.ViewControllers;
using FreeUniverse.Core;
using UnityEngine;
using FreeUniverse.World.Database;
using FreeUniverse.Server;
using FreeUniverse.Common;

namespace FreeUniverse.Client
{
    public class GameClient : 
        ILoginClientDelegate,
        IViewControllerClientLoginDelegate,
        INotificationCenterDelegate,
        ITestLoginViewControllerDelegate,
        IViewControllerCreateAccountDelegate,
        IViewControllerCharacterSelectionDelegate
    {
        private ClientAccountCreator accountCreator { get; set; }
        private ChatClient chatClient { get; set; }
        private LoginClient loginClient { get; set; }
        private ZoneClient zoneClient { get; set; }
        private ViewControllerCreateAccount createAccountViewController { get; set; }
        private ViewControllerCharacterSelection characterSelectionViewController { get; set; }
        private ViewControllerClientLogin loginViewController { get; set; }
        private WorldControllerClient clientWorld { get; set; }
        private ViewControllerTestLogin testLoginViewController { get; set; }

        public GameClient()
        {
            chatClient = new ChatClient();
            loginClient = new LoginClient();
            zoneClient = new ZoneClient();
            clientWorld = new WorldControllerClient(ConstData.LAYER_CLIENT_WORLD, ConstData.LAYER_STATIC_BACKGROUND);
            loginViewController = new ViewControllerClientLogin();
            testLoginViewController = new ViewControllerTestLogin();
            characterSelectionViewController = new ViewControllerCharacterSelection();
            createAccountViewController = new ViewControllerCreateAccount();
            zoneClient.worldControllerClient = clientWorld;
            clientWorld.zoneClient = zoneClient;
        }

        void TestInit()
        {
            clientWorld.LoadSystem("system_demo");

            WorldObject obj2 = clientWorld.CreateWorldObject("rha_vhf_hull");
            clientWorld.ControlWorldObjectByHuman(obj2.id);
            obj2.SetRootHullPosition(new Vector3(0.0f, 0.0f, -10000.0f));
            clientWorld.AddZoneObserver(obj2);
            clientWorld.FollowThirdPersonCamera(obj2.id);
        }

        public void Start()
        {
            testLoginViewController.controllerDelegate = this;
            testLoginViewController.visible = false;

            loginViewController.controllerDelegate = this;
            loginViewController.visible = false;

            characterSelectionViewController.controllerDelegate = this;
            characterSelectionViewController.visible = false;

            createAccountViewController.controllerDelegate = this;
            createAccountViewController.visible = false;

            loginClient.loginClientDelegate = this;
            loginClient.Start();

            clientWorld.Init();

            TestInit();

        }

        public void Shutdown()
        {
            if (zoneClient != null) zoneClient.DisconnectFromZoneServer();
        }

        public void Update(float time)
        {
            loginClient.Update();
            zoneClient.Update(time);
            clientWorld.Update(time);
        }

        public void OnGUI()
        {
            if( clientWorld != null )
                clientWorld.OnGUI();
        }

        #region IClientLoginViewControllerDelegate implementation

        public void OnClientLoginViewControllerActionCreateAccount(ViewControllerClientLogin controller)
        {
            loginViewController.visible = false;
            createAccountViewController.visible = true;
        }

        public void OnClientLoginViewControllerActionLogin(ViewControllerClientLogin controller, string email, string password)
        {
            if (email.Length < 4 || password.Length < 4)
                return;

            loginClient.ConnectAndLogin(email, password);
            loginViewController.ShowWaitView();
        }

        public void OnClientLoginViewControllerActionQuit(ViewControllerClientLogin controller)
        {
            Application.Quit();
        }

        public void OnClientLoginViewControllerActionConnectionAttemptAborted(ViewControllerClientLogin controller)
        {
            loginClient.AbortConnection();
            loginViewController.ShowLoginForm();
        }

        #endregion

        #region ILoginClientDelegate implementation

        public void OnLoginClientConnectionAccept(LoginClient client, Account account)
        {
            Debug.Log("GameClient: account data received email=" + account.email);

            loginClient.RequestAccountData();
            loginViewController.Hide();
            characterSelectionViewController.Show();
        }

        public void OnLoginClientConnectionLost(LoginClient client)
        {
            NotificationCenter.Post(NotificationID.LoginServerConnectionLost, null);
        }

        public void OnLoginClientConnectionRefused(LoginClient client)
        {
            Debug.Log("GameClient: failed to authorize at login server");
            loginViewController.ShowFailedConnectionView();
            loginClient.AbortConnection();
        }


        #endregion

        // notification center
        public void OnNotificationCenterEvent(NotificationID id, object data)
        {

        }

        #region ITestLoginViewControllerDelegate Members

        public void OnTestLoginViewControllerActionLogin(string player, string ip)
        {
            zoneClient.playerNickname = player;
            zoneClient.startTemplate = testLoginViewController.startingTemplate;
            zoneClient.ConnectToZoneServer(ip, ServerZone.TEST_PORT);

            testLoginViewController.visible = false;
        }

        #endregion

        #region ICreateAccountViewControllerDelegate Members

        public void OnCreateAccountViewControllerSubmit(ViewControllerCreateAccount controller, string email, string password)
        {
            
        }

        public void OnCreateAccountViewControllerBack(ViewControllerCreateAccount controller)
        {
            createAccountViewController.visible = false;
            loginViewController.visible = true;
        }

        #endregion

        #region ICharacterSelectionViewControllerDelegate Members

        public void OnCharacterSelectionViewControllerCreateCharacter(ViewControllerCharacterSelection controller)
        {
            
        }

        public void OnCharacterSelectionViewControllerDeleteCharacter(ViewControllerCharacterSelection controller, int id)
        {
            
        }

        public void OnCharacterSelectionViewControllerLogCharacter(ViewControllerCharacterSelection controller, int id)
        {
            
        }

        public void OnCharacterSelectionViewControllerBack(ViewControllerCharacterSelection controller)
        {
            
        }

        #endregion
    }
}
