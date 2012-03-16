using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI.System;
using Microsoft.Xna.Framework;
using Nuclex.Input;
using FimbulwinterClient.Config;

namespace FimbulwinterClient.GUI
{
    public class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            if (GuiManager.Singleton.Client.Config.SaveLast)
            {
                txtLogin.Text = GuiManager.Singleton.Client.Config.LastLogin;
                ckSave.Checked = true;
                txtPassword.Focus();
            }
            else
            {
                txtLogin.Text = "";
                ckSave.Checked = false;
                txtLogin.Focus();
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Vector2(280, 120);
            this.Position = new Vector2(GuiManager.Singleton.Client.Config.ScreenWidth / 2 - 140, GuiManager.Singleton.Client.Config.ScreenHeight - 140 - 120);
            this.Text = "Login";

            btnOK = new Button();
            btnOK.Text = "enter";
            btnOK.Position = new Vector2(189, 96);
            btnOK.Size = new Vector2(42, 20);
            btnOK.Clicked += new Action<MouseButtons, float, float>(btnOK_Clicked);

            btnCancel = new Button();
            btnCancel.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(btnCancel_Clicked);
            btnCancel.Text = "cancel";
            btnCancel.Position = new Vector2(234, 96);
            btnCancel.Size = new Vector2(42, 20);

            txtLogin = new TextBox();
            txtLogin.Position = new Vector2(91, 29);
            txtLogin.Size = new Vector2(127, 18);
            txtLogin.BackColor = Color.FromNonPremultiplied(242, 242, 242, 255);

            txtPassword = new TextBox();
            txtPassword.Position = new Vector2(91, 61);
            txtPassword.Size = new Vector2(127, 18);
            txtPassword.TextMask = "*";
            txtPassword.BackColor = Color.FromNonPremultiplied(242, 242, 242, 255);
            txtPassword.OnEnter += new Action(txtPassword_OnEnter);

            lblLogin = new Label();
            lblLogin.Text = "Username";
            lblLogin.Position = new Vector2(25, 31);
            lblLogin.ForeColor = Color.FromNonPremultiplied(90, 111, 153, 255);
            lblLogin.Font = Control.Arial10B;

            lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Position = new Vector2(28, 62);
            lblPassword.ForeColor = Color.FromNonPremultiplied(90, 111, 153, 255);
            lblPassword.Font = Control.Arial10B;

            ckSave = new CheckBox();
            ckSave.Position = new Vector2(232, 33);
            ckSave.Text = "Save";

            this.Controls.Add(lblPassword);
            this.Controls.Add(lblLogin);
            this.Controls.Add(txtLogin);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            this.Controls.Add(ckSave);
        }

        void txtPassword_OnEnter()
        {
            btnOK_Clicked(MouseButtons.Left, 0, 0);
        }

        public event Action<string, string> DoLogin;
        public event Action GoBack;

        void btnOK_Clicked(MouseButtons arg1, float arg2, float arg3)
        {
            if (arg1 == MouseButtons.Left)
            {
                TingSound.Play();

                if (ckSave.Checked)
                {
                    GuiManager.Singleton.Client.Config.SaveLast = true;
                    GuiManager.Singleton.Client.Config.LastLogin = txtLogin.Text;
                }

                if (DoLogin != null)
                    DoLogin(txtLogin.Text, txtPassword.Text);

                this.Close();
            }
        }

        void btnCancel_Clicked(MouseButtons arg1, float arg2, float arg3)
        {
            if (arg1 == MouseButtons.Left)
            {
                TingSound.Play();

                if (GoBack != null)
                    GoBack();

                this.Close();
            }
        }

        public override void OnKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                TingSound.Play();

                if (DoLogin != null)
                    DoLogin(txtLogin.Text, txtPassword.Text);

                this.Close();
            }

            base.OnKeyDown(key);
        }

        Button btnOK;
        Button btnCancel;
        TextBox txtLogin;
        TextBox txtPassword;
        Label lblLogin;
        Label lblPassword;
        CheckBox ckSave;
    }
}
