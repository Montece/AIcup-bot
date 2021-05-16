using Aicup2020.Model;
using System.Collections.Generic;

namespace AICUP.Mark2
{
    public interface IModule
    {
        void DoAction(Dictionary<EntityType, List<Entity>> Entities, Dictionary<int, EntityAction> Actions, PlayerView playerView, int money);
    }
}
