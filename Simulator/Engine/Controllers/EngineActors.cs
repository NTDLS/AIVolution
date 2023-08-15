namespace Simulator.Engine.Controllers
{
    public class EngineActors
    {
        private EngineCore _core;

        public List<Actors.ActorBase> Collection { get; set; } = new List<Actors.ActorBase>();

        public EngineActors(EngineCore core)
        {
            _core = core;
        }

        public void Add(Actors.ActorBase actor)
        {
            Collection.Add(actor);
        }

        public void RemoveDeletedActors()
        {
            Collection.RemoveAll(o => o.IsDeleted);
        }
    }
}
