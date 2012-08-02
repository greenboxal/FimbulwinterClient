using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FimbulwinterClient.Gui.System
{
    public class TextBox : Control
    {
        public string TextMask { get; set; }

        string rtext;
        bool drawCaret;

        int caretPosition;
        int startDraw;

        private string _text;
        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                if (caretPosition > _text.Length)
                    caretPosition = _text.Length;

                CheckPositions();
            }
        }

        public TextBox()
        {
            _text = "";
            rtext = "";
            TabStop = true;
        }

        public override void OnKeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Back:
                    {
                        if (caretPosition > 0)
                        {
                            caretPosition--;
                            _text = Text.Remove(caretPosition, 1);

                            CheckPositions();
                        }
                    }
                    break;

                case Keys.Delete:
                    {
                        if (caretPosition < Text.Length)
                        {
                            _text = Text.Remove(caretPosition, 1);

                            CheckPositions();
                        }
                    }
                    break;

                case Keys.Right:
                    {
                        if (caretPosition < Text.Length)
                        {
                            caretPosition++;

                            CheckPositions();
                        }
                    }
                    break;

                case Keys.Left:
                    {
                        if (caretPosition > 0)
                        {
                            caretPosition--;

                            CheckPositions();
                        }
                    }
                    break;

                case Keys.Home:
                    {
                        caretPosition = 0;
                        CheckPositions();
                    }
                    break;

                case Keys.End:
                    {
                        caretPosition = Text.Length;
                        CheckPositions();
                    }
                    break;

                case Keys.Enter:
                    {
                        if (OnEnter != null)
                            OnEnter();
                    }
                    break;
            }
        }

        public override void OnClick(Nuclex.Input.MouseButtons buttons, float x, float y)
        {
            caretPosition = Text.Length;
        }

        public event Action<char> OnCharacterEntered;

        public override void OnKeyPress(char c)
        {
            if (OnCharacterEntered != null)
                OnCharacterEntered(c);

            if (char.IsControl(c))
                return;

            _text = _text.Insert(caretPosition, c.ToString());
            caretPosition++;

            CheckPositions();
        }

        private void CheckPositions()
        {
            int drawSize = 0;

            if (caretPosition < startDraw)
                startDraw = caretPosition;

            if (!string.IsNullOrEmpty(TextMask))
            {
                rtext = "";

                for (int i = 0; i < Text.Length; i++)
                    rtext += TextMask;
            }
            else
            {
                rtext = Text;
            }

            int totalW = 0;
            for (int i = startDraw; i < rtext.Length; i++)
            {
                totalW += (int)Font.MeasureString(rtext[i].ToString()).X;

                if (totalW > Size.X - 2)
                    break;

                drawSize++;
            }

            if (caretPosition > startDraw + drawSize)
            {
                startDraw++;

                if (startDraw + drawSize > rtext.Length)
                    drawSize = rtext.Length - startDraw;
            }

            rtext = rtext.Substring(startDraw, drawSize);
        }

        public override void Update(GameTime gt)
        {
            if ((int)gt.TotalGameTime.TotalMilliseconds % 500 == 0)
                drawCaret = !drawCaret;
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            Utils.DrawBackground(sb, Color.FromNonPremultiplied(192, 192, 192, 255), absX, absY, (int)Size.X, (int)Size.Y);
            Utils.DrawBackground(sb, BackColor, absX + 1, absY + 1, (int)Size.X - 2, (int)Size.Y - 2);

            Vector2 textPosition = new Vector2(absX + 3, absY + 4);
            sb.DrawString(
                  Font,
                  rtext,
                  textPosition,
                  ForeColor
                );

            if (GuiManager.Singleton.ActiveControl == this && drawCaret)
            {
                Vector2 caretPosition;

                caretPosition = Font.MeasureString(rtext.Substring(0, this.caretPosition - startDraw));
                caretPosition.X -= 1;
                caretPosition.Y = 0.0f;

                sb.DrawString(
                  Font,
                  "|",
                  textPosition + caretPosition,
                  ForeColor
                );
            }
        }

        public event Action OnEnter;
    }
}
