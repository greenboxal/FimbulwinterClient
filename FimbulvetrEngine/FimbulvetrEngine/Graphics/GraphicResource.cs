using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;

namespace FimbulvetrEngine.Graphics
{
    public class GraphicResource : ThreadBoundDisposable
    {
        public IGraphicsContext Context { get; private set; }

        protected GraphicResource()
        {
            Context = GraphicsContext.CurrentContext;
        }

        protected override bool CanDispose()
        {
            return Context.IsCurrent;
        }
    }
}
