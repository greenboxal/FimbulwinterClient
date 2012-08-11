using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FimbulvetrEngine
{
    public class ThreadBoundDisposable : IDisposable
    {
        public bool Disposed { get; protected set; }

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
                if (!CanDispose() || !disposing)
                {
                    GC.KeepAlive(this);
                    ThreadBoundGC.RegisterForDestruction(this, CanDispose);
                }
                else
                {
                    GCUnmanagedFinalize();
                    Disposed = true;
                    GC.ReRegisterForFinalize(this);
                }
            }
        }

        protected virtual bool CanDispose()
        {
            return true;
        }

        protected virtual void GCUnmanagedFinalize()
        {
            
        }
        
        protected virtual void GCManagedFinalize()
        {
            
        }
    }
}
