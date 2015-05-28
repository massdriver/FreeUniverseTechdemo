using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.ViewControllers
{
    public interface IViewControllerCreateAccountDelegate
    {
        void OnCreateAccountViewControllerSubmit(ViewControllerCreateAccount controller, string email, string password);
        void OnCreateAccountViewControllerBack(ViewControllerCreateAccount controller);
    }

    public class ViewControllerCreateAccount : ViewControllerBase
    {
        public IViewControllerCreateAccountDelegate controllerDelegate { get; set; }
        private Rect area { get; set; }
        private string email { get; set; }
        private string password { get; set; }

        public ViewControllerCreateAccount()
        {
            ResetFields();
            area = Helpers.MakeRectCentered(WindowWidth, WindowHeight);
        }

        public const float WindowWidth = 200.0f;
        public const float WindowHeight = 185.0f;

        public override void OnGUI(Event e)
        {
            if( !visible ) return;

            base.OnGUI(e);

            GUI.Window((int)GUIWindowTypes.CreateAccount, area, WndProc, "Create Account");
        }

        void ResetFields()
        {
            email = "";
            password = "";
        }

        public override bool visible
        {
            get
            {
                return base.visible;
            }
            set
            {
                ResetFields();
                base.visible = value;
            }
        }

        bool CheckFields()
        {
            if (!email.Contains('@'))
                return false;

            if (!email.Contains('.'))
                return false;

            if (email.Length < 5)
                return false;

            if (password.Length < 8)
                return false;

            return true;
        }

        void WndProc(int id)
        {
            GUI.skin = GUIExtensions.SKIN_DEFAULT;

            GUILayout.BeginArea(Rect.MinMaxRect(
                GUIStaticData.WindowHorizontalOffset,
                GUIStaticData.WindowVerticalOffset,
                WindowWidth - GUIStaticData.WindowHorizontalOffset,
                WindowHeight - GUIStaticData.WindowVerticalOffset / 2.0f));

            GUILayout.Label("Email");
            email = GUILayout.TextField(email);

            GUILayout.Label("Password (8 chars min)");
            password = GUILayout.PasswordField(password, '*');

            if (GUILayout.Button("Create") )
            {
                if (CheckFields())
                {
                    if( controllerDelegate != null )
                        controllerDelegate.OnCreateAccountViewControllerSubmit(this, email, password);

                    PopupMessageBox.Show("Success", "Account activation email was sent to: " + email);
                    ResetFields();
                }
                else
                {
                    PopupMessageBox.Show("Error", "Some fields are invalid");
                }
            }

            if (GUILayout.Button("Back") && controllerDelegate != null)
                controllerDelegate.OnCreateAccountViewControllerBack(this);

            GUILayout.EndArea();
        }
    }
}
