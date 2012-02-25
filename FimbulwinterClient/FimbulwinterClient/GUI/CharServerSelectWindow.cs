using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI.System;
using FimbulwinterClient.Network.Packets;
using Nuclex.Input;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Network.Packets.Login;

namespace FimbulwinterClient.GUI
{
    public class CharServerSelectWindow : Window
    {
        ROConfig cfg;

        public CharServerSelectWindow(ROConfig cfg, CharServerInfo[] charServerInfo)
        {
            this.cfg = cfg;

            InitializeComponent();

            foreach (CharServerInfo csi in charServerInfo)
                lstServices.Items.Add(csi);
            lstServices.SelectedIndex = 0;

            lstServices.Focus();
        }

        private void InitializeComponent()
        {
            this.Size = new Vector2(280, 200);
            this.Position = new Vector2(cfg.ScreenWidth / 2 - 140, cfg.ScreenHeight - 140 - 200);
            this.Text = "Server Select";

            lstServices = new Listbox();
            lstServices.Size = new Vector2(256, 143);
            lstServices.Position = new Vector2(12, 21);
            lstServices.OnActivate += new Action(lstServices_OnActivate);

            btnOK = new Button();
            btnOK.Text = "enter";
            btnOK.Position = new Vector2(189, 176);
            btnOK.Size = new Vector2(42, 20);
            btnOK.Clicked += new Action<MouseButtons, float, float>(btnOK_Clicked);

            btnCancel = new Button();
            btnCancel.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(btnCancel_Clicked);
            btnCancel.Text = "cancel";
            btnCancel.Position = new Vector2(234, 176);
            btnCancel.Size = new Vector2(42, 20);

            this.Controls.Add(lstServices);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }

        void lstServices_OnActivate()
        {
            btnOK_Clicked(MouseButtons.Left, 0, 0);
        }

        void btnOK_Clicked(MouseButtons arg1, float arg2, float arg3)
        {
            if (arg1 == MouseButtons.Left)
            {
                TingSound.Play();

                if (ServerSelected != null)
                    ServerSelected((CharServerInfo)lstServices.Items[lstServices.SelectedIndex]);

                this.Close();
            }
        }

        void btnCancel_Clicked(MouseButtons arg1, float arg2, float arg3)
        {
            if (arg1 == MouseButtons.Left)
            {
                TingSound.Play();

                MessageBox.ShowYesNo("You really want to exit?", msgBoxResult);
            }
        }

        void msgBoxResult(int result)
        {
            TingSound.Play();

            if (result == 1)
                GuiManager.Singleton.Client.Exit();
        }

        public override void OnKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                if (lstServices.SelectedIndex >= 0 && lstServices.SelectedIndex < lstServices.Items.Count)
                {
                    TingSound.Play();

                    if (ServerSelected != null)
                        ServerSelected((CharServerInfo)lstServices.Items[lstServices.SelectedIndex]);

                    this.Close();
                }
            }

            base.OnKeyDown(key);
        }

        Listbox lstServices;
        Button btnOK;
        Button btnCancel;

        public event Action<CharServerInfo> ServerSelected;
    }
}
