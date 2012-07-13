using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI;
using FimbulwinterClient.Network.Packets.Login;
using FimbulwinterClient.GUI.System;

namespace FimbulwinterClient.Screens
{
    public class LoginScreen : BaseLoginScreen
    {
        private LoginWindow window;

        public LoginScreen()
        {
            window = new LoginWindow();
            window.GoBack += new Action(m_window_GoBack);
            window.DoLogin += new Action<string, string>(m_window_DoLogin);

            ROClient.Singleton.GuiManager.Controls.Add(window);
        }

        void m_window_DoLogin(string user, string pass)
        {
            ShowWait();

            if (ROClient.Singleton.CurrentConnection != null && ROClient.Singleton.CurrentConnection.Client.Connected)
            {
                ROClient.Singleton.CurrentConnection.Disconnect();
            }

            ROClient.Singleton.CurrentConnection = new Network.Connection();
            ROClient.Singleton.CurrentConnection.PacketSerializer.PacketHooks[0x69] = new Action<ushort, int, AC_Accept_Login>(packetLoginAccepted);
            ROClient.Singleton.CurrentConnection.PacketSerializer.PacketHooks[0x6A] = new Action<ushort, int, AC_Refuse_Login>(packetLoginRejected);

            try
            {
                ROClient.Singleton.CurrentConnection.Connect(ROClient.Singleton.NetworkState.SelectedServer.Address, ROClient.Singleton.NetworkState.SelectedServer.Port);
            }
            catch
            {
                CloseWait();
                MessageBox.ShowOk("Could not connect to server.", ReenterScreen);
            }

            ROClient.Singleton.CurrentConnection.Start();

            new CA_Login(user, pass, ROClient.Singleton.NetworkState.SelectedServer.Version, 1).Write(ROClient.Singleton.CurrentConnection.BinaryWriter);
        }

        void packetLoginAccepted(ushort cmd, int size, AC_Accept_Login pkt)
        {
            CloseWait();
            ROClient.Singleton.NetworkState.LoginAccept = pkt;
            ROClient.Singleton.ChangeScreen(new CharServerSelectScreen());
        }

        void packetLoginRejected(ushort cmd, int size, AC_Refuse_Login pkt)
        {
            CloseWait();
            MessageBox.ShowOk(pkt.Text, ReenterScreen);
        }

        void ReenterScreen(int dummy)
        {
            ROClient.Singleton.ChangeScreen(new LoginScreen());
        }

        void m_window_GoBack()
        {
            ROClient.Singleton.ChangeScreen(new ServiceSelectScreen());
        }

        public override void Dispose()
        {
            base.Dispose();

            window.Close();
        }
    }
}
