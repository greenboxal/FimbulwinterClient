using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI.System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.GUI
{
    public class CharSelectWindow : Window
    {
        public CharSelectWindow()
        {
            InitializeComponent();

            cbChars.RefreshIndex();
            cbChars.Focus();
        }

        private void InitializeComponent()
        {
            this.FullImage = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data/texture/À¯ÀúÀÎÅÍÆäÀÌ½º/login_interface/win_select2.bmp");
            this.Size = new Vector2(576, 358);
            this.Position = new Vector2(GuiManager.Singleton.Client.Config.ScreenWidth / 2 - (576 / 2), GuiManager.Singleton.Client.Config.ScreenHeight / 2 - (358 / 2));

            cbChars = new CharBrowser(ROClient.Singleton.NetworkState.CharAccept.Chars);
            cbChars.Position = new Vector2(60, 43);
            cbChars.Size = new Vector2(421, 120);
            cbChars.SelectedIndexChanged += new Action(cbChars_SelectedIndexChanged);
            cbChars.OnSelect += new Action(cbChars_OnSelect);

            btnAction = new Button();
            btnAction.Size = new Vector2(42, 20);
            btnAction.Position = new Vector2(477, 334);
            btnAction.Text = "create";
            btnAction.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(btnAction_Clicked);

            btnCancel = new Button();
            btnCancel.Size = new Vector2(42, 20);
            btnCancel.Position = new Vector2(524, 334);
            btnCancel.Text = "cancel";
            btnCancel.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(btnCancel_Clicked);

            lblName = new Label();
            lblName.Position = new Vector2(66, 204);
            this.Controls.Add(lblName);

            lblJob = new Label();
            lblJob.Position = new Vector2(66, 220);
            this.Controls.Add(lblJob);

            lblLv = new Label();
            lblLv.Position = new Vector2(66, 236);
            this.Controls.Add(lblLv);

            lblEXP = new Label();
            lblEXP.Position = new Vector2(66, 252);
            this.Controls.Add(lblEXP);

            lblHP = new Label();
            lblHP.Position = new Vector2(66, 268);
            this.Controls.Add(lblHP);

            lblSP = new Label();
            lblSP.Position = new Vector2(66, 284);
            this.Controls.Add(lblSP);

            lblMap = new Label();
            lblMap.Position = new Vector2(66, 300);
            this.Controls.Add(lblMap);

            lblStr = new Label();
            lblStr.Position = new Vector2(210, 204);
            this.Controls.Add(lblStr);

            lblAgi = new Label();
            lblAgi.Position = new Vector2(210, 220);
            this.Controls.Add(lblAgi);

            lblVit = new Label();
            lblVit.Position = new Vector2(210, 236);
            this.Controls.Add(lblVit);

            lblInt = new Label();
            lblInt.Position = new Vector2(210, 252);
            this.Controls.Add(lblInt);

            lblDex = new Label();
            lblDex.Position = new Vector2(210, 268);
            this.Controls.Add(lblDex);

            lblLuk = new Label();
            lblLuk.Position = new Vector2(210, 284);
            this.Controls.Add(lblLuk);

            this.Controls.Add(cbChars);
            this.Controls.Add(btnAction);
            this.Controls.Add(btnCancel);
        }

        public event Action<int> OnCreateChar;
        public event Action<int> OnSelectChar;

        void btnAction_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            if (arg1 == Nuclex.Input.MouseButtons.Left)
            {
                if (cbChars.SelectedIndex == -1)
                {
                    if (OnCreateChar != null)
                        OnCreateChar(cbChars.SelectedSlot);
                }
                else
                {
                    if (OnSelectChar != null)
                        OnSelectChar(cbChars.SelectedSlot);
                }
            }
        }

        void cbChars_SelectedIndexChanged()
        {
            if (cbChars.SelectedIndex == -1)
            {
                btnAction.Text = "create";
            }
            else
            {
                lblName.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Name;
                lblJob.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Class.ToString();
                lblLv.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].BaseLevel.ToString();
                lblEXP.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].BaseExp.ToString();
                lblHP.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].HP.ToString();
                lblSP.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].SP.ToString();
                lblMap.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].MapName;

                lblStr.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Str.ToString();
                lblAgi.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Agi.ToString();
                lblVit.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Vit.ToString();
                lblInt.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Int.ToString();
                lblDex.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Dex.ToString();
                lblLuk.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Luk.ToString();

                btnAction.Text = "enter";
            }
        }

        void cbChars_OnSelect()
        {
            btnAction_Clicked(Nuclex.Input.MouseButtons.Left, 0, 0);
        }

        void btnCancel_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            
        }

        CharBrowser cbChars;
        Button btnAction;
        Button btnCancel;

        Label lblName;
        Label lblJob;
        Label lblLv;
        Label lblEXP;
        Label lblHP;
        Label lblSP;
        Label lblMap;

        Label lblStr;
        Label lblAgi;
        Label lblVit;
        Label lblInt;
        Label lblDex;
        Label lblLuk;
    }
}
