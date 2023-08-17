using Simulator.Engine.Actors;
using Simulator.Engine.Types;

namespace Simulator.Engine.Controllers
{
    public class EngineActors
    {
        private readonly EngineCore _core;

        public List<ActorBase> Collection { get; private set; } = new();

        public EngineActors(EngineCore core)
        {
            _core = core;
        }

        public void Add(Actors.ActorBase actor)
        {
            lock (this)
            {
                Collection.Add(actor);
            }
        }

        public void RemoveDeletedActors()
        {
            lock (this)
            {
                Collection.RemoveAll(o => o.IsDeleted);
            }
        }

        public List<ActorBase> Intersections(Point<double> location, Point<double> size)
        {
            lock (this)
            {
                var objs = new List<ActorBase>();

                foreach (var obj in Collection.Where(o => o.Visible == true))
                {
                    if (obj.Intersects(location, size))
                    {
                        objs.Add(obj);
                    }
                }
                return objs;
            }
        }
    }
}
