using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.ViewControllers
{
    public class ViewControllerClientLogin : ViewControllerBase
    {
        const string CLIENT_LOGIN_WINDOW_CAPTION = "Free Universe Login";

        const float VIEW_WIDTH = 200.0f;
        const float VIEW_HEIGHT = 185.0f;
        const float QUIT_BUTTON_HEIGHT = 50.0f;
        const float VIEW_HEIGHT_INDENT = 10.0f;
        const float VIEW_WIDTH_INDENT = 10.0f;

        private Rect wndRect { get { return Helpers.MakeRect(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f), VIEW_WIDTH, VIEW_HEIGHT); } }
        private Rect waitViewRect { get { return Helpers.MakeRect(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f), 300.0f, 100.0f); } }

        public IViewControllerClientLoginDelegate controllerDelegate { get; set; }

        public ViewControllerClientLogin()
        {
            visible = false;
        }

        enum LoginViewState
        {
            Login,
            ConnectionWait,
            ConnectionRefused
        }

        LoginViewState _state = LoginViewState.Login;

        public void ShowWaitView()
        {
            _state = LoginViewState.ConnectionWait;
        }

        public void ShowLoginForm()
        {
            _state = LoginViewState.Login;
        }

        public void ShowFailedConnectionView()
        {
            _state = LoginViewState.ConnectionRefused;
        }

        override public void OnGUI(Event e)
        {
            base.OnGUI(e);

            GUI.skin = GUIExtensions.SKIN_DEFAULT;

            if( !visible )
                return;

            switch(_state)
            {
                case LoginViewState.Login:
                    {
                        GUI.Window(GUIStaticData.WND_CLIENTLOGINVIEW_LOGIN, wndRect, WindowFunctionLogin, CLIENT_LOGIN_WINDOW_CAPTION);

                        // Register button
                        //const float offset = -15.0f;
                        //if (GUI.Button(Rect.MinMaxRect(Screen.width - 100.0f + offset, Screen.height - 25.0f + offset, Screen.width + offset, Screen.height + offset), "Create Account") && controllerDelegate != null)
                        //    controllerDelegate.OnClientLoginViewControllerActionCreateAccount(this);

                    }
                    break;
                case LoginViewState.ConnectionWait:
                    GUI.Window(GUIStaticData.WND_CLIENTLOGINVIEW_WAIT, waitViewRect, WindowFunctionConnectionWait, "Message");
                    break;
                case LoginViewState.ConnectionRefused:
                    GUI.Window(GUIStaticData.WND_CLIENTLOGINVIEW_WAIT, waitViewRect, WindowFunctionConnectionRefused, "Message");
                    break;
            }

            // check esc key
            if ((controllerDelegate != null) && (e.type == EventType.KeyUp) && (e.keyCode == KeyCode.Escape))
            {
                if (_state == LoginViewState.ConnectionWait)
                    controllerDelegate.OnClientLoginViewControllerActionConnectionAttemptAborted(this);
                else
                if (_state == LoginViewState.ConnectionRefused)
                    _state = LoginViewState.Login;
                
            }
        }

        string _user = "";
        string _password = "";

        void WindowFunctionConnectionRefused(int id)
        {
            GUILayout.BeginArea(Rect.MinMaxRect(10.0f, 10.0f, 250.0f, 50.0f));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Connection refused. Press Escape to continue.");
            GUILayout.EndArea();
        }

        void WindowFunctionConnectionWait(int id)
        {
            GUILayout.BeginArea(Rect.MinMaxRect(10.0f, 10.0f, 250.0f, 50.0f));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Connecting ... (hit escape to abort)");
            GUILayout.EndArea();
        }

        void WindowFunctionLogin(int id)
        {
            GUILayout.BeginArea(Rect.MinMaxRect(VIEW_WIDTH_INDENT, 5.0f + VIEW_HEIGHT_INDENT, VIEW_WIDTH - VIEW_WIDTH_INDENT, VIEW_HEIGHT - VIEW_HEIGHT_INDENT));
            
            GUILayout.Label("Email");
            _user = GUILayout.TextField(_user, 32);
            
            GUILayout.Label("Password");
            _password = GUILayout.PasswordField(_password,'*', 32);

            if (GUILayout.Button("Login") && (controllerDelegate != null))
            {
                controllerDelegate.OnClientLoginViewControllerActionLogin(this, _user, _password);
                _password = "";
            }
            
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Quit") && (controllerDelegate != null))
                controllerDelegate.OnClientLoginViewControllerActionQuit(this);

            GUILayout.EndArea();
            GUI.DragWindow();
        }

    }
}
