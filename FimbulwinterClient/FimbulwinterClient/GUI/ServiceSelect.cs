using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework.Input;
using Nuclex.UserInterface.Controls;

namespace FimbulwinterClient.GUI
{
    public class ServiceSelect : WindowControl
    {
        private ROClient m_client;
        public ROClient Client
        {
            get { return m_client; }
            set { m_client = value; }
        }

        public ServiceSelect(ROClient roc)
        {
            m_client = roc;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            serverList = new ListControl();
            serverList.Name = "serverList";
            serverList.Bounds = new UniRectangle(11, 21, 257, 145);

            foreach (ServerInfo si in m_client.Config.Servers)
            {
                serverList.Items.Add(si.Display);
            }

            serverList.SelectionMode = ListSelectionMode.Single;
            serverList.SelectedItems.Add(0);

            this.Title = "Service Select";
            this.Children.Add(serverList);
        }

        private Nuclex.UserInterface.Controls.Desktop.ListControl serverList;
    }
}
