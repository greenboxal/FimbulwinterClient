using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI;
using FimbulwinterClient.GUI.System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Network.Packets;
using FimbulwinterClient.Network.Packets.Character;
using FimbulwinterClient.Network.Packets.Zone;

namespace FimbulwinterClient.Screens
{
    public class CharSelectScreen : BaseLoginScreen
    {
        private CharSelectWindow window;
        private string _mapname;
        public CharSelectScreen()
        {
            window = new CharSelectWindow();
            window.OnCreateChar += new Action<int>(window_OnCreateChar);
            window.OnSelectChar += new Action<int>(window_OnSelectChar);

            ROClient.Singleton.GuiManager.Controls.Add(window);
        }

        void window_OnSelectChar(int obj)
        {
            ROClient.Singleton.NetworkState.SelectedChar = ROClient.Singleton.NetworkState.CharAccept.Chars[obj];
            ROClient.Singleton.CurrentConnection.PacketSerializer.PacketHooks[(int)PacketHeader.HEADER_HC_NOTIFY_ZONESVR] = new Action<ushort, int, HC_Notify_Zonesvr>(packet_notify_zonesrv);
            
            new CH_Select_Char(obj).Write(ROClient.Singleton.CurrentConnection.BinaryWriter);
        }

        private void packet_notify_zonesrv(ushort cmd, int size, HC_Notify_Zonesvr pkt)
        {
            _mapname = pkt.Mapname.Replace(".gat", ".gnd");
            if (ROClient.Singleton.CurrentConnection != null && ROClient.Singleton.CurrentConnection.Client.Connected)
            {
                ROClient.Singleton.CurrentConnection.Disconnect();
            }
            
            ROClient.Singleton.CurrentConnection = new Network.Connection();
            ROClient.Singleton.CurrentConnection.PacketSerializer.PacketHooks[(int)PacketHeader.HEADER_ZC_ACCEPT_ENTER2] = new Action<ushort, int, ZC_Accept_Enter2>(packetLoginAccepted);

            try
            {
                ROClient.Singleton.CurrentConnection.Connect(pkt.IP.ToString(), pkt.Port);
            }
            catch
            {
                CloseWait();
                MessageBox.ShowOk("Could not connect to server.", GotoLoginScreen);
            }

            ROClient.Singleton.CurrentConnection.Start();
            
            new CZ_Enter(
                ROClient.Singleton.NetworkState.LoginAccept.AccountID,
                ROClient.Singleton.NetworkState.LoginAccept.LoginID1,
                ROClient.Singleton.NetworkState.LoginAccept.LoginID2,
                ROClient.Singleton.NetworkState.LoginAccept.Sex).Write(ROClient.Singleton.CurrentConnection.BinaryWriter);
        }

        void GotoLoginScreen(int dummy)
        {
            ROClient.Singleton.ChangeScreen(new LoginScreen());
        }

        void packetLoginAccepted(ushort cmd, int size, ZC_Accept_Enter2 pkt)
        {
            CloseWait();
            ROClient.Singleton.ChangeScreen(new TestMap(_mapname));
        }

        NewCharWindow newCharWindow;
        void window_OnCreateChar(int obj)
        {
            if (ROClient.Singleton.GuiManager.Controls.Contains(newCharWindow))
            {
                // bring to front
                return;
            }
            newCharWindow = new NewCharWindow();
            ROClient.Singleton.GuiManager.Controls.Add(newCharWindow);
        }

        public override void Update(SpriteBatch sb, GameTime gameTime)
        {
            base.Update(sb, gameTime);

            if (gameTime.TotalGameTime.TotalSeconds % 12 < 1.0F)
            {
                new Ping((int)gameTime.TotalGameTime.TotalMilliseconds).Write(ROClient.Singleton.CurrentConnection.BinaryWriter);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            window.Close();
        }
    }
}
