using Aicup2020.Model;

namespace AICUP.Mark2
{
    public class EntityBuilder
    {
        public Entity Entity = default(Entity);
        public BuilderTask Task = BuilderTask.Idle;
        public Vec2Int MoveCoords = default(Vec2Int);
        public Vec2Int BuildCoords = default(Vec2Int);
        public EntityType Base = default(EntityType);
        public int ID = -1;
    }

    public enum BuilderTask
    {
        Idle,
        Moving,
        Repairing
    }
}
