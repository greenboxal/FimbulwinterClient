using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenTK.Graphics;

namespace FimbulvetrEngine
{
    public class Dispatcher
    {
        public static Dispatcher Instance { get; private set; }

        public IGraphicsContext Context { get; set; }
        public Queue<Tuple<WaitCallback, object>> TaskQueue { get; private set; }

        public Dispatcher()
        {
            if (Instance != null)
                throw new Exception("Only one instance of Dispatcher is allowed, use the Instance property.");

            TaskQueue = new Queue<Tuple<WaitCallback, object>>();

            Instance = this;
        }

        // Common tasks can run in any thread, so just put then into the ThreadPool
        // I've set background argument so we can reduce the code size when using lambda expressions
        public void DispatchTask(WaitCallback task, bool background = true, object state = null)
        {
            if (background)
            {
                ThreadPool.QueueUserWorkItem(task, state);
            }
            else
            {
                task(state);
            }
        }

        // Core tasks run in the application main thread, useful for tasks that are bound to some thread specific context(eg: OpenGL)
        public void DispatchCoreTask(WaitCallback task, object state = null)
        {
            if (Context != null && Context.IsCurrent)
            {
                task(state);
            }
            else
            {
                lock (TaskQueue)
                {
                    TaskQueue.Enqueue(new Tuple<WaitCallback, object>(task, state));
                }
            }
        }

        // Poll the task queue and executes the avaiable tasks. This should be called only from the main thread(thread which owns the Context)
        public void PollTasks(int max = 0)
        {
            int count = 0;

            while (true)
            {
                Tuple<WaitCallback, object> callback = null;

                lock (TaskQueue)
                {
                    if (TaskQueue.Count > 0)
                        callback = TaskQueue.Dequeue();
                }

                if (callback == null)
                    break;

                callback.Item1(callback.Item2);

                count++;
                if (max != 0 && count >= max)
                    break;
            }
        }
    }
}
