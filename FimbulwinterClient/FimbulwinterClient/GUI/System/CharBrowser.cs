using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Network.Packets.Char;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FimbulwinterClient.Content;
using FimbulwinterClient.Config;

namespace FimbulwinterClient.GUI.System
{
    public class CharBrowser : Control
    {
        private static readonly Rectangle Slot1 = new Rectangle(0, 0, 131, 138);
        private static readonly Rectangle Slot2 = new Rectangle(163, 0, 131, 138);
        private static readonly Rectangle Slot3 = new Rectangle(326, 0, 131, 138);

        private CSCharData[] chars;
        public CSCharData[] Chars
        {
            get { return chars; }
            set { chars = value; RefreshIndex(); ReloadSprites(); }
        }

        private SpriteAction[] heads;
        private SpriteAction[] bodies;

        private void ReloadSprites()
        {
            heads = new SpriteAction[chars.Length];
            bodies = new SpriteAction[chars.Length];

            for (int i = 0; i < chars.Length; i++)
            {
                bodies[i] = ROClient.Singleton.ContentManager.LoadContent<SpriteAction>(string.Format("data/sprite/{0}/{1}/{2}/{3}_{4}.act", ROConst.Humans, ROConst.Body, ROConst.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], ROConst.ClassNames[chars[i].Class], ROConst.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex]));
                heads[i] = ROClient.Singleton.ContentManager.LoadContent<SpriteAction>(string.Format("data/sprite/{0}/{1}/{2}/{3}_{4}.act", ROConst.Humans, ROConst.Head, ROConst.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], chars[i].Hair, ROConst.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex]));
            }
        }

        public CharBrowser(CSCharData[] chars)
        {
            Chars = chars;
        }

        public int SelectedIndex { get; set; }
        public int SelectedSlot { get { return selectionBase + selectionOffset; } }

        public void GoLeft()
        {
            selectionBase -= 3;

            if (selectionBase < 0)
                selectionBase = Configuration.MaxCharacters - 3;

            RefreshIndex();
        }

        public void GoRight()
        {
            selectionBase += 3;

            if (selectionBase >= Configuration.MaxCharacters)
                selectionBase = 0;

            RefreshIndex();
        }

        private int selectionBase;
        private int selectionOffset;

        static int[] DrawSprX = new int[] { 64, 227, 390 };
        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            if (selectionBase < chars.Length)
            {
                for (int i = 0; i < 3; i++)
                {
                    int idx = selectionBase + i;

                    for (int n = 0; n < chars.Length; n++)
                    {
                        if (chars[n].Slot == idx)
                        {
                            bodies[n].Draw(sb, new Point(absX + DrawSprX[i], absY + 115), null, false);
                            heads[n].Draw(sb, new Point(absX + DrawSprX[i], absY + 115), bodies[n], true);
                            break;
                        }
                    }
                }
            }

            Rectangle rect = Slot1;

            if (selectionOffset == 1)
            {
                rect = Slot2;
            }
            else if (selectionOffset == 2)
            {
                rect = Slot3;
            }

            rect.X += absX;
            rect.Y += absY;

            sb.Draw(FormSkin, rect, new Rectangle(109, 0, 139, 160), Color.White);
        }

        public override void OnClick(Nuclex.Input.MouseButtons buttons, float x, float y)
        {
            if (buttons == Nuclex.Input.MouseButtons.Left)
            {
                if (Slot1.Contains((int)x, (int)y))
                    selectionOffset = 0;
                else if (Slot2.Contains((int)x, (int)y))
                    selectionOffset = 1;
                else if (Slot3.Contains((int)x, (int)y))
                    selectionOffset = 2;

                RefreshIndex();
            }
        }

        public override void OnKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    {
                        selectionOffset--;

                        if (selectionOffset < 0)
                        {
                            selectionOffset = 2;
                            GoLeft();
                        }
                        else
                        {
                            RefreshIndex();
                        }
                    }
                    break;
                case Keys.Right:
                    {
                        selectionOffset++;

                        if (selectionOffset > 2)
                        {
                            selectionOffset = 0;
                            GoRight();
                        }
                        else
                        {
                            RefreshIndex();
                        }
                    }
                    break;
                case Keys.Enter:
                    {
                        if (OnSelect != null)
                            OnSelect();
                    }
                    break;
            }

            base.OnKeyDown(key);
        }

        public void RefreshIndex()
        {
            SelectedIndex = -1;

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i].Slot == SelectedSlot)
                {
                    SelectedIndex = i;
                    break;
                }
            }

            if (SelectedIndexChanged != null)
                SelectedIndexChanged();
        }

        public event Action SelectedIndexChanged;
        public event Action OnSelect;
    }
}
