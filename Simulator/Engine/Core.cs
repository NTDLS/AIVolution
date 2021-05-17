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

        public bool IsRendering { get; private set; } = false;
        public object DrawingSemaphore { get; private set; } = new object();
        public Actors Actors { get; private set; }
        public EngineThread Thread { get; private set; }
        public EngineDisplay Display { get; private set; }
        public EngineInput Input { get; set; }

        public Control DrawingSurface;
        private Dictionary<string, Bitmap> _Bitmaps { get; set; } = new Dictionary<string, Bitmap>();

        public Core(Control drawingSurface, Size visibleSize)
        {
            DrawingSurface = drawingSurface;
            Actors = new Actors(this);
            Thread = new EngineThread(this);
            Input = new EngineInput(this);
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

            lock (_Bitmaps)
            {
                if (_Bitmaps.ContainsKey(path))
                {
                    result = _Bitmaps[path].Clone() as Bitmap;
                }
                else
                {
                    using (var image = Image.FromFile(path))
                    using (var newbitmap = new Bitmap(image))
                    {
                        result = (Bitmap)newbitmap.Clone();
                        _Bitmaps.Add(path, result);
                    }
                }
            }

            return result;
        }
    }
}
