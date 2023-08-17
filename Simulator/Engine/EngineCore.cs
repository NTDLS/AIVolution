using Simulator.Engine.Controllers;

namespace Simulator.Engine
{
    /// <summary>
    /// This is the engine core, all other components branch off from here.
    /// </summary>
    public class EngineCore
    {
        private static dynamic DynamicCast(dynamic source, Type dest) => Convert.ChangeType(source, dest);
        private readonly Dictionary<string, Bitmap> _bitmapCache = new();

        public string InitialBrainFile { get; set; } = string.Empty;
        public bool IsRendering { get; private set; } = false;
        public object DrawingSemaphore { get; private set; } = new object();
        public EngineActors Actors { get; private set; }
        public EngineThread Thread { get; private set; }
        public EngineDisplay Display { get; private set; }
        public EngineInput Input { get; private set; }
        public EngineWorld World { get; private set; }
        public bool IsPaused { get; private set; }

        public EngineCore(Control drawingSurface, Size visibleSize)
        {
            Actors = new EngineActors(this);
            Thread = new EngineThread(this);
            Input = new EngineInput(this);
            World = new EngineWorld(this);
            Display = new EngineDisplay(this, drawingSurface, visibleSize);
        }

        public void Start(string initialBrainFile)
        {
            InitialBrainFile = initialBrainFile;
            Thread.Start();
        }

        public void Start()
        {
            Thread.Start();
        }


        public void TogglePause()
        {
            IsPaused = !IsPaused;
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
                Monitor.TryEnter(DrawingSemaphore, timeout, ref lockTaken);

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

        public Bitmap? GetBitmapCached(string path)
        {
            Bitmap? result = null;

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
