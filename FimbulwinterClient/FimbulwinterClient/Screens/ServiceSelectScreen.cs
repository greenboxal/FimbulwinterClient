using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI;
using FimbulwinterClient.Config;

namespace FimbulwinterClient.Screens
{
    public class ServiceSelectScreen : BaseLoginScreen
    {
        private ServiceSelectWindow window;

        public ServiceSelectScreen()
        {
            window = new ServiceSelectWindow();
            window.ServerSelected += new Action<Config.ServerInfo>(m_window_ServerSelected);

            ROClient.Singleton.GuiManager.Controls.Add(window);
        }

        void m_window_ServerSelected(ServerInfo obj)
        {
            ROClient.Singleton.NetworkState.SelectedServer = obj;
            ROClient.Singleton.ChangeScreen(new LoginScreen());
        }

        public override void Dispose()
        {
            base.Dispose();

            window.Close();
        }
    }
}
