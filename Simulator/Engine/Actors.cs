using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
