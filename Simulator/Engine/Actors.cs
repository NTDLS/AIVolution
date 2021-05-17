using System.Collections.Generic;

namespace Simulator.Engine
{
    public class Actors
    {
        private Core _core;

        public List<ActorBase> Collection = new List<ActorBase>();

        public Actors(Core core)
        {
            _core = core;

        }

        public void Add(ActorBase actor)
        {
            Collection.Add(actor);
        }

        public void RemoveDeletedActors()
        {
            Collection.RemoveAll(o => o.IsDeleted);
        }
    }
}
