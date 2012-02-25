using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.GUI.System
{
    public class MessageBox : Window
    {
        public static MessageBox ShowYesNo(string text, Action<int> result)
        {
            MessageBox msg = new MessageBox(2, text, result);
            GuiManager.Singleton.Controls.Add(msg);
            msg.Focus();
            return msg;
        }

        public static MessageBox ShowOk(string text, Action<int> result)
        {
            MessageBox msg = new MessageBox(1, text, result);
            GuiManager.Singleton.Controls.Add(msg);
            msg.Focus();
            return msg;
        }

        public static MessageBox ShowMessage(string text)
        {
            MessageBox msg = new MessageBox(0, text, null);
            GuiManager.Singleton.Controls.Add(msg);
            msg.Focus();
            return msg;
        }

        private MessageBox(int type, string text, Action<int> callback)
        {
            InitializeComponent(type);

            lblText.Text = text;
            this.callback = callback;
        }

        Action<int> callback;

        private void InitializeComponent(int type)
        {
            this.Size = new Vector2(280, 120);
            this.Position = new Vector2(GuiManager.Singleton.Client.Config.ScreenWidth / 2 - 140, GuiManager.Singleton.Client.Config.ScreenHeight / 2 - 60);
            this.Text = "message";

            lblText = new Label();
            lblText.Position = new Vector2(13, 29);

            if (type == 1)
            {
                btnOkYes = new Button();
                btnOkYes.Size = new Vector2(42, 20);
                btnOkYes.Position = new Vector2(189, 96);
                btnOkYes.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(btnOkYes_Clicked);
                btnOkYes.Text = "OK";
                this.Controls.Add(btnOkYes);
            }
            
            if (type == 2)
            {
                btnNo = new Button();
                btnNo.Size = new Vector2(42, 20);
                btnNo.Position = new Vector2(234, 96);
                btnNo.Text = "cancel";
                btnNo.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(btnNo_Clicked);
                this.Controls.Add(btnNo);
            }

            this.Controls.Add(lblText);
        }

        void btnNo_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            if (arg1 == Nuclex.Input.MouseButtons.Left)
            {
                if (callback != null)
                    callback(0);
                
                this.Close();
            }
        }

        void btnOkYes_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            if (arg1 == Nuclex.Input.MouseButtons.Left)
            {
                if (callback != null)
                    callback(1);

                this.Close();
            }
        }

        Button btnOkYes;
        Button btnNo;
        Label lblText;
    }
}
