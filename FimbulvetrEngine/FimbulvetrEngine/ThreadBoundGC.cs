using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;

namespace FimbulvetrEngine
{
    public class ThreadBoundGC
    {
        private class GCEntry
        {
            public Func<bool> CanDispose { get; set; }
            public IDisposable Value { get; set; }
        }

        private readonly static List<GCEntry> DestructionQueue;

        static ThreadBoundGC()
        {
            DestructionQueue = new List<GCEntry>();
        }

        public static void RegisterForDestruction(IDisposable threadBoundDisposable, Func<bool> canDispose)
        {
            // Check if we can dispose the object now as we should do it ASAP
            // FIXME: This is crashing some times when Disposing the main context
#if FALSE
            if (canDispose())
            {
                threadBoundDisposable.Dispose();
            }
            else
#endif
            {
                GCEntry entry = new GCEntry();

                entry.CanDispose = canDispose;
                entry.Value = threadBoundDisposable;

                lock (DestructionQueue)
                {
                    DestructionQueue.Add(entry);
                }
            }
        }

        public static void Collect()
        {
            lock (DestructionQueue)
            {
                foreach (GCEntry entry in DestructionQueue.Where(entry => entry.CanDispose()).ToArray())
                {
                    entry.Value.Dispose();
                    DestructionQueue.Remove(entry);
                }
            }
        }
    }
}
