using Aicup2020.Model;
using System.Collections.Generic;

namespace AICUP.Mark2
{
    public class AttackModule
    {
        EntityType[] Targets;

        public AttackModule(Dictionary<EntityType, List<Entity>> Entities, PlayerView playerView)
        {
            Targets = new EntityType[]
            {
                EntityType.BuilderBase,
                EntityType.BuilderUnit,
                EntityType.House,
                EntityType.MeleeBase,
                EntityType.MeleeUnit,
                EntityType.RangedBase,
                EntityType.RangedUnit,
                EntityType.Turret,
                EntityType.Wall
            };

            //UnitsCenter.Main.Attack[EntityType.MeleeUnit].AddRange(Entities[EntityType.MeleeUnit]);
            //UnitsCenter.Main.Attack[EntityType.RangedUnit].AddRange(Entities[EntityType.RangedUnit]);
        }

        public void DoAction(Dictionary<EntityType, List<Entity>> Entities, Dictionary<int, EntityAction> Actions, int money, PlayerView playerView)
        {
            foreach (Entity unit in UnitsCenter.Main.Attack[EntityType.MeleeUnit])
            {
                EntityAction def = new EntityAction()
                {
                    AttackAction = new AttackAction()
                    {
                        AutoAttack = new AutoAttack()
                        {
                            PathfindRange = 1000,
                            ValidTargets = Targets
                        }
                    },
                    MoveAction = new MoveAction()
                    {
                        BreakThrough = false,
                        FindClosestPosition = true,
                        Target = new Vec2Int(playerView.MapSize - 1, playerView.MapSize - 1)
                    }
                };

                if (Actions.ContainsKey(unit.Id)) Actions[unit.Id] = def;
                else Actions.Add(unit.Id, def);
            }

            foreach (Entity unit in UnitsCenter.Main.Attack[EntityType.RangedUnit])
            {
                EntityAction def = new EntityAction()
                {
                    AttackAction = new AttackAction()
                    {
                        AutoAttack = new AutoAttack()
                        {
                            PathfindRange = 1000,
                            ValidTargets = Targets
                        }
                    },
                    MoveAction = new MoveAction()
                    {
                        BreakThrough = false,
                        FindClosestPosition = true,
                        Target = new Vec2Int(playerView.MapSize - 1, playerView.MapSize - 1)
                    }
                };

                if (Actions.ContainsKey(unit.Id)) Actions[unit.Id] = def;
                else Actions.Add(unit.Id, def);
            }
        }
    }
}
