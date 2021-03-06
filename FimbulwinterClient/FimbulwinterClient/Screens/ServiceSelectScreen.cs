﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Gui;
using FimbulwinterClient.Core.Config;

namespace FimbulwinterClient.Screens
{
    public class ServiceSelectScreen : BaseLoginScreen
    {
        private ServiceSelectWindow window;

        public ServiceSelectScreen()
        {
            window = new ServiceSelectWindow();
            window.ServerSelected += new Action<ServerInfo>(window_ServerSelected);

            ROClient.Singleton.GuiManager.Controls.Add(window);
        }

        void window_ServerSelected(ServerInfo obj)
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
