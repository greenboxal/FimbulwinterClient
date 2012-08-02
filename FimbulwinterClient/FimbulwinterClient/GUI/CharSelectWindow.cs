using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI.System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Core;

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
            this.FullImage = SharedInformation.ContentManager.Load<Texture2D>("data\\texture\\유저인터페이스\\login_interface\\win_select2.bmp");
            this.Size = new Vector2(576, 358);
            this.Position = new Vector2(GuiManager.Singleton.Client.Config.ScreenWidth / 2 - (576 / 2), GuiManager.Singleton.Client.Config.ScreenHeight / 2 - (358 / 2));

            cbChars = new CharBrowser(ROClient.Singleton.NetworkState.CharAccept.Chars, ROClient.Singleton.NetworkState.CharAccept.MaxSlots, ROClient.Singleton.NetworkState.CharAccept.PremiumSlots, ROClient.Singleton.NetworkState.CharAccept.AvailableSlots);
            cbChars.Position = new Vector2(60, 43);
            cbChars.Size = new Vector2(457, 138);
            cbChars.SelectedIndexChanged += new Action(cbChars_SelectedIndexChanged);
            cbChars.PageChanged += new Action<int, int>(cbChars_PageChanged);
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
            lblName.Position = new Vector2(70, 206);
            this.Controls.Add(lblName);

            lblJob = new Label();
            lblJob.Position = new Vector2(70, 222);
            this.Controls.Add(lblJob);

            lblLv = new Label();
            lblLv.Position = new Vector2(70, 238);
            this.Controls.Add(lblLv);

            lblEXP = new Label();
            lblEXP.Position = new Vector2(70, 254);
            this.Controls.Add(lblEXP);

            lblHP = new Label();
            lblHP.Position = new Vector2(70, 270);
            this.Controls.Add(lblHP);

            lblSP = new Label();
            lblSP.Position = new Vector2(70, 286);
            this.Controls.Add(lblSP);

            lblMap = new Label();
            lblMap.Position = new Vector2(70, 302);
            this.Controls.Add(lblMap);

            lblStr = new Label();
            lblStr.Position = new Vector2(214, 206);
            this.Controls.Add(lblStr);

            lblAgi = new Label();
            lblAgi.Position = new Vector2(214, 222);
            this.Controls.Add(lblAgi);

            lblVit = new Label();
            lblVit.Position = new Vector2(214, 238);
            this.Controls.Add(lblVit);

            lblInt = new Label();
            lblInt.Position = new Vector2(214, 254);
            this.Controls.Add(lblInt);

            lblDex = new Label();
            lblDex.Position = new Vector2(214, 270);
            this.Controls.Add(lblDex);

            lblLuk = new Label();
            lblLuk.Position = new Vector2(214, 286);
            this.Controls.Add(lblLuk);

            Texture2D scrollleft = SharedInformation.ContentManager.Load<Texture2D>("data\\texture\\유저인터페이스\\scroll1left.bmp"); ;
            ibScrollLeft = new ImageButton(scrollleft, scrollleft, scrollleft);
            ibScrollLeft.Size = new Vector2(13, 13);
            ibScrollLeft.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(ibScrollLeft_Clicked);
            ibScrollLeft.Position = new Vector2(45, 110);
            this.Controls.Add(ibScrollLeft);

            Texture2D scrollright = SharedInformation.ContentManager.Load<Texture2D>("data\\texture\\유저인터페이스\\scroll1right.bmp"); ;
            ibScrollRight = new ImageButton(scrollright, scrollright, scrollright);
            ibScrollRight.Size = new Vector2(13, 13);
            ibScrollRight.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(ibScrollRight_Clicked);
            ibScrollRight.Position = new Vector2(519, 110);
            this.Controls.Add(ibScrollRight);

            lblPage = new Label();
            lblPage.Position = new Vector2(276, 187);
            lblPage.Text = "1";
            lblPage.ForeColor = new Color(255, 58, 123, 255);
            lblPage.Font = Gulim8B;
            this.Controls.Add(lblPage);

            lblPage2 = new Label();
            lblPage2.Position = new Vector2(282, 187);
            lblPage2.Text =  " / " + (int)(ROClient.Singleton.NetworkState.CharAccept.MaxSlots / 3);
            lblPage2.Font = Gulim8B;
            lblPage2.ForeColor = new Color(99, 99, 99, 255);
            this.Controls.Add(lblPage2);

            borCharacters = new Border();
            borCharacters.Position = new Vector2(422, 195);
            borCharacters.Size = new Vector2(144, 20);
            this.Controls.Add(borCharacters);

            lblCharacters = new Label();
            lblCharacters.Position = new Vector2(439, 200);
            lblCharacters.Text = ROClient.Singleton.NetworkState.CharAccept.Chars.Count() + " / " + (int)(ROClient.Singleton.NetworkState.CharAccept.AvailableSlots);
            lblCharacters.Font = Gulim8B;
            this.Controls.Add(lblCharacters);

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

        void ibScrollLeft_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            cbChars.GoLeftOnce();
        }

        void ibScrollRight_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            cbChars.GoRightOnce();
        }

        void cbChars_PageChanged(int page, int maxpages) {
            lblPage.Text = string.Format("{0}", page);
        }

        void cbChars_SelectedIndexChanged()
        {
            if (cbChars.SelectedIndex == -1)
            {
                btnAction.Text = "create";

                lblName.Text = string.Empty;
                lblJob.Text = string.Empty;
                lblLv.Text = string.Empty;
                lblEXP.Text = string.Empty;
                lblHP.Text = string.Empty;
                lblSP.Text = string.Empty;
                lblMap.Text = string.Empty;

                lblStr.Text = string.Empty;
                lblAgi.Text = string.Empty;
                lblVit.Text = string.Empty;
                lblInt.Text = string.Empty;
                lblDex.Text = string.Empty;
                lblLuk.Text = string.Empty;
            }
            else
            {
                lblName.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Name;
                lblJob.Text = Statics.ClassNames[ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Job];
                lblLv.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].BaseLevel.ToString();
                lblEXP.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Exp.ToString();
                lblHP.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].HP.ToString();
                lblSP.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].SP.ToString();
                lblMap.Text = Statics.MapNames[ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].MapName];

                lblStr.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Str.ToString();
                lblAgi.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Agi.ToString();
                lblVit.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Vit.ToString();
                lblInt.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Int.ToString();
                lblDex.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Dex.ToString();
                lblLuk.Text = ROClient.Singleton.NetworkState.CharAccept.Chars[cbChars.SelectedIndex].Luk.ToString();

                btnAction.Text = "enter";
            }
        }

        public override void OnKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            cbChars.OnKeyDown(key);

            base.OnKeyDown(key);
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

        Label lblPage;
        Label lblPage2;

        Border borCharacters;
        Label lblCharacters;

        ImageButton ibScrollLeft;
        ImageButton ibScrollRight;
    }
}
