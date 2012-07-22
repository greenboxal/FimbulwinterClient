using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI;
using FimbulwinterClient.Network.Packets.Account;
using FimbulwinterClient.GUI.System;
using FimbulwinterClient.Network.Packets.Character;

namespace FimbulwinterClient.Screens
{
    public class CharServerSelectScreen : BaseLoginScreen
    {
        private CharServerSelectWindow window;

        public CharServerSelectScreen()
        {
            window = new CharServerSelectWindow();
            window.ServerSelected += new Action<CharServerInfo>(window_ServerSelected);

            ROClient.Singleton.GuiManager.Controls.Add(window);
        }

        void window_ServerSelected(CharServerInfo obj)
        {
            ROClient.Singleton.NetworkState.SelectedCharServer = obj;

            if (ROClient.Singleton.CurrentConnection != null && ROClient.Singleton.CurrentConnection.Client.Connected)
            {
                ROClient.Singleton.CurrentConnection.Disconnect();
            }

            ROClient.Singleton.CurrentConnection = new Network.Connection();
            ROClient.Singleton.CurrentConnection.PacketSerializer.PacketHooks[0x6B] = new Action<ushort, int, HC_Accept_Enter>(packetLoginAccepted);
            ROClient.Singleton.CurrentConnection.PacketSerializer.PacketHooks[0x6C] = new Action<ushort, int, HC_Refuse_Enter>(packetLoginRejected);

            try
            {
                ROClient.Singleton.CurrentConnection.Connect(obj.IP.ToString(), obj.Port);
            }
            catch
            {
                CloseWait();
                MessageBox.ShowOk("Could not connect to server.", ReenterScreen);
            }

            ROClient.Singleton.CurrentConnection.PacketSerializer.BytesToSkip = 4; // Skip AID
            ROClient.Singleton.CurrentConnection.Start();

            new CH_Enter(
                ROClient.Singleton.NetworkState.LoginAccept.AccountID,
                ROClient.Singleton.NetworkState.LoginAccept.LoginID1,
                ROClient.Singleton.NetworkState.LoginAccept.LoginID2,
                ROClient.Singleton.NetworkState.LoginAccept.Sex).Write(ROClient.Singleton.CurrentConnection.BinaryWriter);
        }

        void packetLoginAccepted(ushort cmd, int size, HC_Accept_Enter pkt)
        {
            CloseWait();
            ROClient.Singleton.NetworkState.CharAccept = pkt;
            ROClient.Singleton.ChangeScreen(new CharSelectScreen());
        }

        void packetLoginRejected(ushort cmd, int size, HC_Refuse_Enter pkt)
        {
            CloseWait();
            MessageBox.ShowOk("Connection rejected.", ReenterScreen);
        }

        void ReenterScreen(int dummy)
        {
            ROClient.Singleton.ChangeScreen(new LoginScreen());
        }

        public override void Dispose()
        {
            base.Dispose();

            window.Close();
        }
    }
}
