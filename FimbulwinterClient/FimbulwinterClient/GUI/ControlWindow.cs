using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Gui.System;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Core;

namespace FimbulwinterClient.Gui
{
    public class ControlWindow : Window
    {
        public ControlWindow()
        {
            InitializeComponent();
            AddDebugText("Initialized");
        }

        private const int width = 640;
        private const int height = 480;

        private void InitializeComponent()
        {
            this.Size = new Vector2(width, height);
            this.Position = new Vector2(SharedInformation.Config.ScreenWidth / 2 - width / 2, SharedInformation.Config.ScreenHeight / 2 - height / 2);
            this.Text = "Controltest";

            lblTextbox = new Label();
            lblTextbox.Text = "Textbox";
            lblTextbox.Position = new Vector2(15, 30);
            lblTextbox.Font = Gulim8B;

            txtTextbox = new TextBox();
            txtTextbox.Position = new Vector2(15, 45);
            txtTextbox.Size = new Vector2(140, 18);
            txtTextbox.BackColor = Color.FromNonPremultiplied(242, 242, 242, 255);
            
            lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Position = new Vector2(15, 80);
            lblPassword.Font = Gulim8B;

            txtPassword = new TextBox();
            txtPassword.Position = new Vector2(15, 95);
            txtPassword.Size = new Vector2(140, 18);
            txtPassword.TextMask = "*";
            txtPassword.BackColor = Color.FromNonPremultiplied(242, 242, 242, 255);

            lblListbox = new Label();
            lblListbox.Text = "Listbox";
            lblListbox.Position = new Vector2(15, 130);
            lblListbox.Font = Gulim8B;

            lstListbox = new Listbox();
            lstListbox.Position = new Vector2(15, 145);
            lstListbox.Size = new Vector2(140, 80);
            lstListbox.Items.Add("Listboxentry 1");
            lstListbox.Items.Add("Listboxentry 2");
            lstListbox.Items.Add("Listboxentry 3");
            lstListbox.Items.Add("Listboxentry 4");

            lblCheckbox = new Label();
            lblCheckbox.Text = "Checkbox";
            lblCheckbox.Position = new Vector2(15, 240);
            lblCheckbox.Font = Gulim8B;

            chkCheckbox = new CheckBox();
            chkCheckbox.Position = new Vector2(15, 255);
            chkCheckbox.Text = "Check me";

            lblButton = new Label();
            lblButton.Text = "Button";
            lblButton.Position = new Vector2(15, 290);
            lblButton.Font = Gulim8B;

            btnButton = new Button();
            btnButton.Text = "Button";
            btnButton.Position = new Vector2(15, 305);
            btnButton.Size = new Vector2(60, 20);
            btnButton.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(btnButton_Clicked);

            lblArrowSelector = new Label();
            lblArrowSelector.Text = "Arrow Selector";
            lblArrowSelector.Position = new Vector2(180, 30);
            lblArrowSelector.Font = Gulim8B;

            asArrowSelector = new ArrowSelector();
            asArrowSelector.Position = new Vector2(180, 45);
            asArrowSelector.Size = new Vector2(124, 18);
            asArrowSelector.Maximum = 10;
            asArrowSelector.ValueChanged += new Action(asArrowSelector_ValueChanged);

            asArrowSelector2 = new ArrowSelector();
            asArrowSelector2.Position = new Vector2(180, 60);
            asArrowSelector2.Size = new Vector2(124, 18);
            asArrowSelector2.Maximum = 5;
            asArrowSelector2.ValueChanged += new Action(asArrowSelector_ValueChanged);

            lstDebug = new Listbox();
            lstDebug.Position = new Vector2(15, height - 130);
            lstDebug.Size = new Vector2(width - 30, 130 - 30);

            chrCharacter = new Character();
            chrCharacter.Position = new Vector2(180, 110);

            this.Controls.Add(this.txtTextbox);
            this.Controls.Add(this.lblTextbox);

            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);

            this.Controls.Add(this.lstListbox);
            this.Controls.Add(this.lblListbox);

            this.Controls.Add(this.lblCheckbox);
            this.Controls.Add(this.chkCheckbox);

            this.Controls.Add(this.lblButton);
            this.Controls.Add(this.btnButton);

            this.Controls.Add(this.lblArrowSelector);
            this.Controls.Add(this.asArrowSelector);
            this.Controls.Add(this.asArrowSelector2);
            this.Controls.Add(chrCharacter);

            this.Controls.Add(this.lstDebug);
        }

        NewCharWindow newCharWindow;
        private void btnButton_Clicked(Nuclex.Input.MouseButtons buttons, float arg1, float arg2)
        {
            AddDebugText("Button clicked");
            if (ROClient.Singleton.GuiManager.Controls.Contains(newCharWindow))
            {
                // bring to front
                return;
            }
            newCharWindow = new NewCharWindow();
            ROClient.Singleton.GuiManager.Controls.Add(newCharWindow);
        }
        
        private void asArrowSelector_ValueChanged()
        {
            AddDebugText("asArrowSelector_ValueChanged, new value = " + asArrowSelector.Value);
        }

        private void AddDebugText(string message)
        {
            lstDebug.Items.Insert(0, string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}", DateTime.Now, message));
        }

        Label lblTextbox;
        TextBox txtTextbox;
        Label lblPassword;
        TextBox txtPassword;
        Label lblListbox;
        Listbox lstListbox;
        Label lblCheckbox;
        CheckBox chkCheckbox;
        Label lblButton;
        Button btnButton;
        Label lblArrowSelector;
        ArrowSelector asArrowSelector;
        ArrowSelector asArrowSelector2;

        Character chrCharacter;

        Listbox lstDebug;
    }
}
