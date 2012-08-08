#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: DefaultForm.cs 3344 2012-07-20 16:51:24Z romeoxbm $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Drawing;
using System.Windows.Forms;
using Axiom.Core;
using Axiom.Graphics;
using IO = System.IO;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    public sealed class DefaultForm : Form
    {
        private readonly WindowClassStyle _classStyle;
        private readonly WindowsExtendedStyle _dwStyleEx;

        #region RenderWindow

        private RenderWindow _renderWindow;

        ///<summary>
        ///  Get/Set the RenderWindow associated with this form.
        ///</summary>
        public RenderWindow RenderWindow
        {
            get { return this._renderWindow; }
            set { this._renderWindow = value; }
        }

        #endregion RenderWindow

        #region WindowStyles

        private WindowStyles _windowStyle;

        /// <summary>
        ///   Get/Set window styles
        /// </summary>
        public WindowStyles WindowStyles
        {
            get { return this._windowStyle; }

            set { this._windowStyle = value; }
        }

        #endregion WindowStyles

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style = (int) this._windowStyle;
                cp.ExStyle = (int) this._dwStyleEx;
                cp.ClassStyle = (int) this._classStyle;
                return cp;
            }
        }

        public DefaultForm(WindowClassStyle classStyle, WindowsExtendedStyle dwStyleEx, string title,
                           WindowStyles windowStyle, int left, int top, int winWidth, int winHeight, Control parentHWnd)
        {
            this._classStyle = classStyle;
            this._dwStyleEx = dwStyleEx;
            this._windowStyle = windowStyle;
            this.Text = title;

            SuspendLayout();

            BackColor = Color.Black;
            Name = title;
            Left = left;
            Top = top;
            Width = winWidth;
            Height = winHeight;
            if (parentHWnd != null)
            {
                Parent = parentHWnd;
            }

            Load += _defaultFormLoad;
            Deactivate += _defaultFormDeactivate;
            Activated += _defaultFormActivated;
            Closing += _defaultFormClose;
            Resize += _defaultFormResize;
            Cursor.Hide();

            ResumeLayout(false);
        }

        protected override void WndProc(ref Message m)
        {
            if (!Win32MessageHandling.WndProc(this._renderWindow, ref m))
            {
                base.WndProc(ref m);
            }
        }

        public void _defaultFormDeactivate(object source, EventArgs e)
        {
            if (this._renderWindow != null)
            {
                this._renderWindow.IsActive = false;
            }
        }

        public void _defaultFormActivated(object source, EventArgs e)
        {
            if (this._renderWindow != null)
            {
                this._renderWindow.IsActive = true;
            }
        }

        public void _defaultFormClose(object source, System.ComponentModel.CancelEventArgs e)
        {
            // set the window to inactive
            if (this._renderWindow != null)
            {
                this._renderWindow.IsActive = false;
            }
        }

        private void _defaultFormLoad(object sender, EventArgs e)
        {
            try
            {
                IO.Stream strm = ResourceGroupManager.Instance.OpenResource("AxiomIcon.ico",
                                                                            ResourceGroupManager.
                                                                                BootstrapResourceGroupName);
                if (strm != null)
                {
                    Icon = new Icon(strm);
                }
            }
            catch (IO.FileNotFoundException)
            {
            }
        }

        private void _defaultFormResize(object sender, EventArgs e)
        {
            Root.Instance.SuspendRendering = WindowState == FormWindowState.Minimized;
        }
    };
}