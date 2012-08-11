using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenTK.Graphics;

namespace FimbulvetrEngine
{
    public class ThreadBoundDisposable : IDisposable
    {
        public bool Disposed { get; protected set; }
        public IGraphicsContext Context { get; private set; }

        protected ThreadBoundDisposable()
        {
            Context = GraphicsContext.CurrentContext;
        }

        ~ThreadBoundDisposable()
        {
            if (!Disposed)
            {
                Dispose(false);
            }
            else
            {
                GCManagedFinalize();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (!Context.IsCurrent || !disposing)
                {
                    GC.KeepAlive(this);
                    ThreadBoundGC.RegisterForDestruction(this, () => Context.IsCurrent);
                }
                else
                {
                    GCUnmanagedFinalize();
                    Disposed = true;
                    GC.ReRegisterForFinalize(this);
                }
            }
        }

        protected virtual void GCUnmanagedFinalize()
        {
            
        }
        
        protected virtual void GCManagedFinalize()
        {
            
        }
    }
}
