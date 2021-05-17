using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace Simulator.Engine
{
    public class Core
    {
        private static dynamic DynamicCast(dynamic source, Type dest) => Convert.ChangeType(source, dest);
        private Dictionary<string, Bitmap> _bitmapCache { get; set; } = new Dictionary<string, Bitmap>();

        public bool IsRendering { get; private set; } = false;
        public object DrawingSemaphore { get; private set; } = new object();
        public Actors Actors { get; private set; }
        public EngineThread Thread { get; private set; }
        public EngineDisplay Display { get; private set; }
        public EngineInput Input { get; private set; }
        public EngineWorld World { get; private set; }

        public Core(Control drawingSurface, Size visibleSize)
        {
            Actors = new Actors(this);
            Thread = new EngineThread(this);
            Input = new EngineInput(this);
            World = new EngineWorld(this);
            Display = new EngineDisplay(this, drawingSurface, visibleSize);
        }

        public void Start()
        {
            Thread.Start();
        }

        public void Stop()
        {
            Thread.Stop();
        }

        public void Render(Graphics dc)
        {
            IsRendering = true;

            var timeout = TimeSpan.FromMilliseconds(2);
            bool lockTaken = false;

            try
            {
                Monitor.TryEnter(this.DrawingSemaphore, timeout, ref lockTaken);

                if (lockTaken)
                {
                    lock (Actors)
                    {
                        foreach (var actor in Actors.Collection)
                        {
                            DynamicCast(actor, actor.GetType()).Render(dc);
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    Monitor.Exit(DrawingSemaphore);
                }
            }

            IsRendering = false;
        }

        public Bitmap GetBitmapCached(string path)
        {
            Bitmap result = null;

            path = path.ToLower();

            lock (_bitmapCache)
            {
                if (_bitmapCache.ContainsKey(path))
                {
                    result = _bitmapCache[path].Clone() as Bitmap;
                }
                else
                {
                    using (var image = Image.FromFile(path))
                    using (var newbitmap = new Bitmap(image))
                    {
                        result = (Bitmap)newbitmap.Clone();
                        _bitmapCache.Add(path, result);
                    }
                }
            }

            return result;
        }
    }
}
