using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Core.Assets;
using FimbulwinterClient.Core;

namespace FimbulwinterClient.Gui.System
{
    class Character : Control
    {
        private int _job;
        public int Job
        {
            get { return _job; }
            set { _job = value; _init = false; }
        }

        private int _head = 1;
        public int Head
        {
            get { return _head; }
            set { _head = value; _init = false; }
        }

        private int _gender;
        public int Gender
        {
            get { return _gender; }
            set { _gender = value; _init = false; }
        }

        private int _headPalette;
        public int HeadPalette
        {
            get { return _headPalette; }
            set { _headPalette = value; _init = false; }
        }

        private int _bodyPalette;
        public int BodyPalette
        {
            get { return _bodyPalette; }
            set { _bodyPalette = value; _init = false; }
        }

        private int _direction;
        public int Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                _se = SpriteEffects.None;
                if (_direction == 8)
                    _direction = 0;
                if (_direction < 0)
                    _direction = 7;
                if (_direction > 4)
                    _se = SpriteEffects.FlipHorizontally;
                _init = false;
            }
        }

        private SpriteEffects _se;

        private bool _init = false;
        private SpriteAction _headSprite;
        private SpriteAction _bodySprite;

        private void Init()
        {
            _bodySprite = SharedInformation.ContentManager.Load<SpriteAction>(string.Format("data\\sprite\\{0}\\{1}\\{2}\\{3}_{2}.act", Statics.Humans, Statics.Body, Statics.Sex[_gender], Statics.ClassSprites[_job]));
            _headSprite = SharedInformation.ContentManager.Load<SpriteAction>(string.Format("data\\sprite\\{0}\\{1}\\{2}\\{3}_{2}.act", Statics.Humans, Statics.Head, Statics.Sex[_gender], _head));

            if (_bodyPalette != 0)
            {
                Palette pal = SharedInformation.ContentManager.Load<Palette>(string.Format("data\\palette\\{0}\\{1}_{2}_{3}.pal", Statics.Palette_Body, Statics.ClassSprites[_job], Statics.Sex[_gender], _bodyPalette));
                _bodySprite.SetPalette(pal);
            }

            if (_headPalette != 0)
            {
                Palette pal = SharedInformation.ContentManager.Load<Palette>(string.Format("data\\palette\\{0}\\{0}{1}_{2}_{3}.pal", Statics.Palette_Head, _head, Statics.Sex[_gender], _headPalette));
                _headSprite.SetPalette(pal);
            }
            _init = true;
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            if (!_init) Init();

            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            /*_bodySprite.Action = _direction;
            _headSprite.Action = _direction;
            _bodySprite.Draw(sb, new Point(absX + 43, absY + 80), null, false, _se);
            _headSprite.Draw(sb, new Point(absX + 43, absY + 80), _bodySprite, true, _se);*/
        }
    }
}
