using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FimbulwinterClient.Network.Packets.Character;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FimbulwinterClient.Core.Assets;
using FimbulwinterClient.Config;
using FimbulwinterClient.Core;

namespace FimbulwinterClient.GUI.System
{
    public class CharBrowser : Control
    {
        private static readonly Rectangle Slot1 = new Rectangle(0, 0, 131, 138);
        private static readonly Rectangle Slot2 = new Rectangle(163, 0, 131, 138);
        private static readonly Rectangle Slot3 = new Rectangle(326, 0, 131, 138);

        private CSCharData[] _chars;
        public CSCharData[] Chars
        {
            get { return _chars; }
            set { _chars = value; RefreshIndex(); ReloadSprites(); }
        }

        private int _maxSlots;
        private int _availSlots;
        private int _premiumSlots;

        private SpriteAction[] _heads;
        private SpriteAction[] _bodies;
        private SpriteAction[][] _accessories;

        private void ReloadSprites()
        {
            _heads = new SpriteAction[_chars.Length];
            _bodies = new SpriteAction[_chars.Length];
            _accessories = new SpriteAction[_chars.Length][];
            Console.WriteLine(Statics.Folder_Accessories);
            for (int i = 0; i < _chars.Length; i++)
            {
                _bodies[i] = SharedInformation.ContentManager.Load<SpriteAction>(string.Format("data\\sprite\\{0}\\{1}\\{2}\\{3}_{2}.act", Statics.Humans, Statics.Body, Statics.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], Statics.ClassSprites[_chars[i].Job]));
                _heads[i] = SharedInformation.ContentManager.Load<SpriteAction>(string.Format("data\\sprite\\{0}\\{1}\\{2}\\{3}_{2}.act", Statics.Humans, Statics.Head, Statics.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], _chars[i].Hair));

                if (_chars[i].ClothesColor != 0)
                {
                    Palette pal = SharedInformation.ContentManager.Load<Palette>(string.Format("data\\palette\\{0}\\{1}_{2}_{3}.pal", Statics.Palette_Body, Statics.ClassSprites[_chars[i].Job], Statics.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], _chars[i].ClothesColor));
                    _bodies[i].SetPalette(pal);
                }

                if (_chars[i].HairColor != 0)
                {
                    Palette pal = SharedInformation.ContentManager.Load<Palette>(string.Format("data\\palette\\{0}\\{0}{1}_{2}_{3}.pal", Statics.Palette_Head, _chars[i].Hair, Statics.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], _chars[i].HairColor));
                    _heads[i].SetPalette(pal);
                }

                // headgears
                _accessories[i] = new SpriteAction[4];
                if (_chars[i].Accessory > 0)
                    _accessories[i][0] = SharedInformation.ContentManager.Load<SpriteAction>(string.Format("data\\sprite\\{0}\\{1}\\{1}{2}.act", Statics.Folder_Accessories, Statics.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], Statics.Accessories[_chars[i].Accessory3].Item2));
                if (_chars[i].Accessory2 > 0)
                    _accessories[i][1] = SharedInformation.ContentManager.Load<SpriteAction>(string.Format("data\\sprite\\{0}\\{1}\\{1}{2}.act", Statics.Folder_Accessories, Statics.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], Statics.Accessories[_chars[i].Accessory2].Item2));
                if (_chars[i].Accessory3 > 0)
                    _accessories[i][2] = SharedInformation.ContentManager.Load<SpriteAction>(string.Format("data\\sprite\\{0}\\{1}\\{1}{2}.act", Statics.Folder_Accessories, Statics.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], Statics.Accessories[_chars[i].Accessory].Item2));

                if (_chars[i].Robe > 0)
                    _accessories[i][3] = SharedInformation.ContentManager.Load<SpriteAction>(string.Format("data\\sprite\\{0}\\{1}\\{2}\\{3}_{2}.act", Statics.Folder_Robes, Statics.Robes[_chars[i].Robe].Item2, Statics.Sex[ROClient.Singleton.NetworkState.LoginAccept.Sex], Statics.ClassSprites[_chars[i].Job]));
            }
        }

        public CharBrowser(CSCharData[] chars, int maxSlots, int premiumSlots, int availSlots)
        {
            Chars = chars;
            _maxSlots = maxSlots;
            _premiumSlots = premiumSlots;
            _availSlots = availSlots;
        }

        public int SelectedIndex { get; set; }
        public int SelectedSlot { get { return selectionBase + selectionOffset; } }

        public void GoLeft()
        {
            selectionBase -= 3;

            if (selectionBase < 0)
                selectionBase = _maxSlots - 3;

            PageChanged((selectionBase + 3) / 3, (_maxSlots) / 3);

            RefreshIndex();
        }

        public void GoRight()
        {
            selectionBase += 3;

            if (selectionBase >= _maxSlots)
                selectionBase = 0;

            PageChanged((selectionBase + 3) / 3, (_maxSlots) / 3);

            RefreshIndex();
        }

        public void GoLeftOnce()
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

        public void GoRightOnce()
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

        private int selectionBase;
        private int selectionOffset;

        static int[] DrawSprX = new int[] { 64, 227, 390 };
        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            if (selectionBase < _chars.Length)
            {
                for (int i = 0; i < 3; i++)
                {
                    int idx = selectionBase + i;

                    for (int n = 0; n < _chars.Length; n++)
                    {
                        if (_chars[n].Slot == idx)
                        {
                            // Important: accessories and robe needs to be positioned by body, not head!
                            if (_accessories[n][3] != null)
                                _accessories[n][3].Draw(sb, new Point(absX + DrawSprX[i], absY + 115), _bodies[n], true);

                            _bodies[n].Draw(sb, new Point(absX + DrawSprX[i], absY + 115), null, false);
                            _heads[n].Draw(sb, new Point(absX + DrawSprX[i], absY + 115), _bodies[n], true);

                            // add headgears
                            for (int x = 0; x < 3; x++ )
                                if (_accessories[n][x] != null)
                                    _accessories[n][x].Draw(sb, new Point(absX + DrawSprX[i], absY + 115), _bodies[n], true);

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

            var selection = SharedInformation.ContentManager.Load<Texture2D>("data\\texture\\유저인터페이스\\client_select_cs.bmp");
            sb.Draw(selection, rect, Color.White);
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
                        GoLeftOnce();
                    }
                    break;
                case Keys.Right:
                    {
                        GoRightOnce();
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

            for (int i = 0; i < _chars.Length; i++)
            {
                if (_chars[i].Slot == SelectedSlot)
                {
                    SelectedIndex = i;
                    break;
                }
            }

            if (SelectedIndexChanged != null)
                SelectedIndexChanged();
        }

        public event Action SelectedIndexChanged;
        public event Action<int, int> PageChanged;
        public event Action OnSelect;
    }
}
