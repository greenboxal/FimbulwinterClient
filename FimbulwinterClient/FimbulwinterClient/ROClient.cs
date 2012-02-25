using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FimbulwinterClient.IO;
using FimbulwinterClient.Audio;
using Nuclex.Input;
using FimbulwinterClient.GUI;
using FimbulwinterClient.Content;
using FimbulwinterClient.GUI.System;
using System.IO;
using FimbulwinterClient.Network;
using FimbulwinterClient.Network.Packets;
using FimbulwinterClient.Network.Packets.Char;
using FimbulwinterClient.Network.Packets.Login;

namespace FimbulwinterClient
{
    public enum ROClientState
    {
        None,
        Test,
        Login,
        Loading,
        InGame,
    }

    public enum ROLoginState
    {
        ServiceSelect,
        Login,
        CharServerSelect,
        CharSelect,
        CreateChar,
    }

    public class ROClient : Microsoft.Xna.Framework.Game
    {
        public static ROClient Singleton { get; set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ROConfig cfg;
        BGMManager bgmManager;
        EffectManager effectManager;

        ROClientState state;
        ROLoginState loginState;

        Texture2D loginScreenBG;

        InputManager inputManager;
        GuiManager guiManager;
        ServerInfo selectedSInfo;
        MessageBox currentWait;

        AcceptLogin acceptedLogin;
        public AcceptLogin AcceptedLogin
        {
            get { return acceptedLogin; }
            set { acceptedLogin = value; }
        }

        CSAcceptLogin charAccept;
        public CSAcceptLogin CharAccept
        {
            get { return charAccept; }
            set { charAccept = value; }
        }

        Connection currentConnection;
        public Connection CurrentConnection
        {
            get { return currentConnection; }
            set { currentConnection = value; }
        }

        public GuiManager GuiManager
        {
            get { return guiManager; }
            set { guiManager = value; }
        }

        public InputManager InputManager
        {
            get { return inputManager; }
            set { inputManager = value; }
        }

        public ROConfig Config
        {
            get { return cfg; }
        }

        public ROContentManager ContentManager
        {
            get { return (ROContentManager)Content; }
        }

        public BGMManager BgmManager
        {
            get { return bgmManager; }
        }

        public EffectManager EffectManager
        {
            get { return effectManager; }
        }

        public ROClient()
        {
            Singleton = this;
            graphics = new GraphicsDeviceManager(this);

            Window.Title = "Ragnarök - Fimbulwinter Client";

            Content = (ContentManager)new ROContentManager(Services, this);
            Content.RootDirectory = "data";

            try
            {
                Stream s = ContentManager.LoadContent<Stream>("data/fb/config.xml");
                cfg = ROConfig.FromStream(s);
                cfg.Client = this;
                s.Close();
            }
            catch
            {
                cfg = new ROConfig(this);
            }
            cfg.ReadConfig();

            bgmManager = new BGMManager(this, cfg);
            effectManager = new EffectManager(this, cfg);

            inputManager = new Nuclex.Input.InputManager(Services, Window.Handle);

            guiManager = new GuiManager(this);
            guiManager.DrawOrder = 1000;

            Components.Add(inputManager);
            Components.Add(guiManager);

            Services.AddService(typeof(InputManager), inputManager);

            graphics.PreferredBackBufferWidth = cfg.ScreenWidth;
            graphics.PreferredBackBufferHeight = cfg.ScreenHeight;
            graphics.ApplyChanges();

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        protected override void Initialize()
        {
            base.Initialize();

            ChangeState(ROClientState.Login);
            ChangeLoginState(ROLoginState.ServiceSelect);

            GUI.Utils.Init(GraphicsDevice);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            loginScreenBG = ContentManager.LoadContent<Texture2D>("data/texture/À¯ÀúÀÎÅÍÆäÀÌ½º/bgi_temp.bmp");
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (currentConnection != null && state == ROClientState.Login && loginState == ROLoginState.CharSelect && (int)gameTime.TotalGameTime.TotalSeconds % 12 == 0)
            {
                Ping p = new Ping(Environment.TickCount);
                currentConnection.SendPacket(p);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (state == ROClientState.Login || state == ROClientState.Test)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(loginScreenBG, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);                
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void ChangeLoginState(ROLoginState s)
        {
            if (state != ROClientState.Login)
                ChangeState(ROClientState.Login);

            if (s == ROLoginState.ServiceSelect)
            {
                ServiceSelectWindow w = new ServiceSelectWindow(cfg);
                w.ServerSelected += new Action<ServerInfo>(w_ServerSelected);
                guiManager.Controls.Add(w);
            }
            else if (s == ROLoginState.Login)
            {
                LoginWindow l = new LoginWindow(cfg);
                l.GoBack += new Action(l_GoBack);
                l.DoLogin += new Action<string, string>(l_DoLogin);
                guiManager.Controls.Add(l);
            }
            else if (s == ROLoginState.CharServerSelect)
            {
                CharServerSelectWindow c = new CharServerSelectWindow(cfg, acceptedLogin.Servers);
                c.ServerSelected += new Action<CharServerInfo>(c_ServerSelected);
                guiManager.Controls.Add(c);
            }
            else if (s == ROLoginState.CharSelect)
            {
                CharSelectWindow c = new CharSelectWindow();
                guiManager.Controls.Add(c);
            }
            
            loginState = s;
        }

        public void ChangeState(ROClientState s)
        {
            if (s == ROClientState.Login)
            {
                bgmManager.PlayBGM("01");
            }

            state = s;
        }

        public void OpenServerWaitDialog()
        {
            currentWait = MessageBox.ShowMessage("Please wait...");
            currentWait.Position = new Vector2(cfg.ScreenWidth / 2 - 140, cfg.ScreenHeight - 140 - 120);
        }

        public void CloseWaitDlg()
        {
            if (currentWait != null)
            {
                currentWait.Close();
                currentWait = null;
            }
        }

        void PacketSerializer_InvalidPacket()
        {
            MessageBox.ShowOk("Invalid packet received.", backToLogin);
        }

        void PacketSerializer_PacketReceived(ushort arg1, int arg2, InPacket arg3)
        {
            if (state == ROClientState.Login)
            {
                if (loginState == ROLoginState.Login)
                {
                    if (arg1 == 0x69)
                    {
                        acceptedLogin = (AcceptLogin)arg3;

                        CloseWaitDlg();

                        ChangeLoginState(ROLoginState.CharServerSelect);
                    }
                    else if (arg1 == 0x6a)
                    {
                        RejectLogin rl = (RejectLogin)arg3;

                        MessageBox.ShowOk(rl.Text, backToLogin);
                    }
                }
                else if (loginState == ROLoginState.CharServerSelect)
                {
                    if (arg1 == 0x6c)
                    {
                        CloseWaitDlg();
                        MessageBox.ShowOk("Rejected from server.", backToLogin);
                    }
                    else if (arg1 == 0x6b)
                    {
                        charAccept = (CSAcceptLogin)arg3;

                        CloseWaitDlg();

                        ChangeLoginState(ROLoginState.CharSelect);
                    }
                }
            }
        }

        void currentConnection_Disconnected()
        {
            currentConnection = null;
            MessageBox.ShowOk("Disconnected from server.", backToLogin);
        }

        void backToLogin(int res)
        {
            CloseWaitDlg();

            ChangeLoginState(ROLoginState.Login);
        }

        void w_ServerSelected(ServerInfo obj)
        {
            selectedSInfo = obj;
            ChangeLoginState(ROLoginState.Login);
        }

        void l_GoBack()
        {
            ChangeLoginState(ROLoginState.ServiceSelect);
        }

        void l_DoLogin(string arg1, string arg2)
        {
            OpenServerWaitDialog();

            currentConnection = new Connection();
            currentConnection.Disconnected += new Action(currentConnection_Disconnected);
            currentConnection.PacketSerializer.PacketReceived += new Action<ushort, int, InPacket>(PacketSerializer_PacketReceived);
            currentConnection.PacketSerializer.InvalidPacket += new Action(PacketSerializer_InvalidPacket);
            try
            {
                currentConnection.Connect(selectedSInfo.Address, selectedSInfo.Port);
            }
            catch
            {
                MessageBox.ShowOk("Could not connect to server.", backToLogin);
                return;
            }

            currentConnection.SendPacket(new PlainTextLogin(arg1, arg2, selectedSInfo.Version, 14));
        }

        void c_ServerSelected(CharServerInfo csi)
        {
            currentConnection.Disconnect(); 
            try
            {
                currentConnection.Connect(csi.IP.ToString(), csi.Port);
            }
            catch
            {
                MessageBox.ShowOk("Could not connect to server.", backToLogin);
                return;
            }

            CharServerLoginPacket cslp = new CharServerLoginPacket(acceptedLogin.AccountID, acceptedLogin.LoginID1, acceptedLogin.LoginID2, acceptedLogin.Sex);
            currentConnection.PacketSerializer.BytesToSkip = 4; // Skip AID
            currentConnection.SendPacket(cslp);
        }
    }
}
