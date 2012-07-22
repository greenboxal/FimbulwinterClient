using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI;
using FimbulwinterClient.GUI.System;

namespace FimbulwinterClient.Screens
{
    class ControlTestScreen : BaseLoginScreen
    {
        private ControlWindow window;

        public ControlTestScreen()
        {
            window = new ControlWindow();

            ROClient.Singleton.GuiManager.Controls.Add(window);
        }
    }
}
