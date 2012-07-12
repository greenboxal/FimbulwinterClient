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
using FimbulwinterClient.Lua;
using Nuclex.Input;
using FimbulwinterClient.GUI;
using FimbulwinterClient.Content;
using FimbulwinterClient.GUI.System;
using System.IO;
using FimbulwinterClient.Network;
using FimbulwinterClient.Network.Packets;
using FimbulwinterClient.Network.Packets.Char;
using FimbulwinterClient.Network.Packets.Login;
using FimbulwinterClient.Screens;
using FimbulwinterClient.Config;

namespace FimbulwinterClient
{
    public class ROClient : Microsoft.Xna.Framework.Game
    {
        public static ROClient Singleton { get; set; }

        GraphicsDeviceManager graphics;
        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        Configuration cfg;
        public Configuration Config
        {
            get { return cfg; }
            set { cfg = value; }
        }
        
        public ROContentManager ContentManager
        {
            get { return (ROContentManager)Content; }
        }

        BGMManager bgmManager;
        public BGMManager BgmManager
        {
            get { return bgmManager; }
            set { bgmManager = value; }
        }

        EffectManager effectManager;
        public EffectManager EffectManager
        {
            get { return effectManager; }
            set { effectManager = value; }
        }

        LuaManager luaManager;
        public LuaManager LuaManager
        {
            get { return luaManager; }
            set { luaManager = value; }
        }

        InputManager inputManager;
        public InputManager InputManager
        {
            get { return inputManager; }
            set { inputManager = value; }
        }

        GuiManager guiManager;
        public GuiManager GuiManager
        {
            get { return guiManager; }
            set { guiManager = value; }
        }

        IGameScreen screen;
        public IGameScreen Screen
        {
            get { return screen; }
            set { screen = value; }
        }

        NetworkState netState;
        public NetworkState NetworkState
        {
            get { return netState; }
            set { netState = value; }
        }

        Connection currentConnection;
        public Connection CurrentConnection
        {
            get { return currentConnection; }
            set { currentConnection = value; }
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
                Stream s = ContentManager.LoadContent<Stream>("data\\fb\\config\\config.xml");
                cfg = Configuration.FromStream(s);
                cfg.Client = this;
                s.Close();
            }
            catch
            {
                cfg = new Configuration(this);
            }

            cfg.ReadConfig();

            bgmManager = new BGMManager();
            effectManager = new EffectManager();
            luaManager = new LuaManager();

            inputManager = new Nuclex.Input.InputManager(Services, Window.Handle);

            guiManager = new GuiManager(this);
            guiManager.DrawOrder = 1000;

            Components.Add(inputManager);
            Components.Add(guiManager);

            Services.AddService(typeof(InputManager), inputManager);
            Services.AddService(typeof(GuiManager), guiManager);
            Services.AddService(typeof(EffectManager), effectManager);
            Services.AddService(typeof(BGMManager), bgmManager);
            Services.AddService(typeof(LuaManager), luaManager);

            graphics.PreferredBackBufferWidth = cfg.ScreenWidth;
            graphics.PreferredBackBufferHeight = cfg.ScreenHeight;
            graphics.ApplyChanges();

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            netState = new NetworkState();
        }

        protected override void Initialize()
        {
            base.Initialize();

            GUI.Utils.Init(GraphicsDevice);

            ChangeScreen(new ServiceSelectScreen());
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (screen != null)
                screen.Update(spriteBatch, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (screen != null)
                screen.Draw(spriteBatch, gameTime);

            base.Draw(gameTime);
        }

        public void ChangeScreen(IGameScreen screen)
        {
            if (this.screen != null)
                this.screen.Dispose();

            this.screen = screen;
        }

        /*public void ChangeLoginState(ROLoginState s)
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
                        acceptedLogin = (LSAcceptLogin)arg3;

                        CloseWaitDlg();

                        ChangeLoginState(ROLoginState.CharServerSelect);
                    }
                    else if (arg1 == 0x6a)
                    {
                        LSRejectLogin rl = (LSRejectLogin)arg3;

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

            currentConnection.SendPacket(new LSPlainTextLogin(arg1, arg2, selectedSInfo.Version, 14));
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

            CSLoginPacket cslp = new CSLoginPacket(acceptedLogin.AccountID, acceptedLogin.LoginID1, acceptedLogin.LoginID2, acceptedLogin.Sex);
            currentConnection.PacketSerializer.BytesToSkip = 4; // Skip AID
            currentConnection.SendPacket(cslp);
        }*/
    }
}
