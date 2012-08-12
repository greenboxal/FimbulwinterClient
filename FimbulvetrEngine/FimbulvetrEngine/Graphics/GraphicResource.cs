using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;

namespace FimbulvetrEngine.Graphics
{
    public class GraphicResource : ThreadBoundDisposable
    {
        protected override bool CanDispose()
        {
            return Dispatcher.Instance.Context.IsCurrent;
        }
    }
}
