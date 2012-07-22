using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI.System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nuclex.Input;

namespace FimbulwinterClient.GUI
{
    class NewCharWindow : Window
    {
        public NewCharWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Vector2(150, 286);
            this.Position = new Vector2(GuiManager.Singleton.Client.Config.ScreenWidth / 2 - 75, GuiManager.Singleton.Client.Config.ScreenHeight / 2 - 143);
            this.Text = "New Character";

            lblName = new Label();
            lblName.Text = "Name";
            lblName.Position = new Vector2(6, 147);
            lblName.Font = Gulim8B;
            lblName.ForeColor = Color.FromNonPremultiplied(90, 107, 156, 255);

            lblHairStyle = new Label();
            lblHairStyle.Text = "Hair Style";
            lblHairStyle.Position = new Vector2(6, 171);
            lblHairStyle.Font = Gulim8B;
            lblHairStyle.ForeColor = Color.FromNonPremultiplied(90, 107, 156, 255);

            lblHairColor = new Label();
            lblHairColor.Text = "Hair Color";
            lblHairColor.Position = new Vector2(6, 212);
            lblHairColor.Font = Gulim8B;
            lblHairColor.ForeColor = Color.FromNonPremultiplied(90, 107, 156, 255);

            btnOK = new Button();
            btnOK.Text = "Ok";
            btnOK.Position = new Vector2(104, 261);
            btnOK.Size = new Vector2(42, 20);
            btnOK.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(btnOK_Clicked);

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Position = new Vector2(57, 261);
            btnCancel.Size = new Vector2(42, 20);
            btnCancel.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(btnCancel_Clicked);

            Texture2D scrollleft = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\유저인터페이스\\scroll1left.bmp"); ;
            ibScrollLeft = new ImageButton(scrollleft, scrollleft, scrollleft);
            ibScrollLeft.Size = new Vector2(13, 13);
            ibScrollLeft.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(ibScrollLeft_Clicked);
            ibScrollLeft.Position = new Vector2(22, 80);
            this.Controls.Add(ibScrollLeft);

            Texture2D scrollright = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\유저인터페이스\\scroll1right.bmp"); ;
            ibScrollRight = new ImageButton(scrollright, scrollright, scrollright);
            ibScrollRight.Size = new Vector2(13, 13);
            ibScrollRight.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(ibScrollRight_Clicked);
            ibScrollRight.Position = new Vector2(111, 80);
            this.Controls.Add(ibScrollRight);

            txtName = new TextBox();
            txtName.Position = new Vector2(40, 143);
            txtName.Size = new Vector2(101, 18);
            txtName.BackColor = Color.FromNonPremultiplied(255, 255, 255, 255);

            asHead = new ArrowSelector();
            asHead.Position = new Vector2(13, 190);
            asHead.Size = new Vector2(124, 13);
            asHead.ValueChanged += new Action(asHead_ValueChanged);
            asHead.Maximum = 23;
            asHead.Minimum = 1;
            asHead.Value = 1;

            asHeadPalette = new ArrowSelector();
            asHeadPalette.Position = new Vector2(13, 230);
            asHeadPalette.Size = new Vector2(124, 13);
            asHeadPalette.ValueChanged += new Action(asHeadPalette_ValueChanged);
            asHeadPalette.Maximum = 8;
            asHeadPalette.Minimum = 1;            
            asHeadPalette.Value = 1;

            chrCharacter = new Character();
            chrCharacter.Position = new Vector2(32, 41);

            this.Controls.Add(lblName);
            this.Controls.Add(lblHairStyle);
            this.Controls.Add(lblHairColor);
            this.Controls.Add(ibScrollLeft);
            this.Controls.Add(ibScrollRight);
            this.Controls.Add(txtName);
            this.Controls.Add(asHead);
            this.Controls.Add(asHeadPalette);
            this.Controls.Add(chrCharacter);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }

        private void asHeadPalette_ValueChanged()
        {
            chrCharacter.HeadPalette = asHeadPalette.Value;
        }

        private void asHead_ValueChanged()
        {
            chrCharacter.Head = asHead.Value;
        }

        void ibScrollLeft_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            chrCharacter.Direction++;
        }

        void ibScrollRight_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            chrCharacter.Direction--;
        }

        void btnOK_Clicked(MouseButtons arg1, float arg2, float arg3)
        {
            if (arg1 == MouseButtons.Left)
            {
                this.Close();
            }
        }

        void btnCancel_Clicked(MouseButtons arg1, float arg2, float arg3)
        {
            if (arg1 == MouseButtons.Left)
            {
                this.Close();
            }
        }

        Button btnOK;
        Button btnCancel;
        Label lblName;
        Label lblHairStyle;
        Label lblHairColor;
        TextBox txtName;
        ArrowSelector asHead;
        ArrowSelector asHeadPalette;
        ImageButton ibScrollLeft;
        ImageButton ibScrollRight;
        Character chrCharacter;
    }
}
