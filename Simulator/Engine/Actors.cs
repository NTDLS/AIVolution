using System.Collections.Generic;

namespace Simulator.Engine
{
    public class Actors
    {
        private Core _core;

        public List<BaseGraphicObject> Collection = new List<BaseGraphicObject>();

        public Actors(Core core)
        {
            _core = core;

        }

        public void Add(BaseGraphicObject actor)
        {
            Collection.Add(actor);
        }

        public void RemoveDeletedActors()
        {
            Collection.RemoveAll(o => o.IsDeleted);
        }
    }
}
