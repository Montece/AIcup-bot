using Aicup2020.Model;
using System.Collections.Generic;

namespace AICUP.Mark2
{
    public class DefenceModule
    {
        EntityType[] Targets;

        public DefenceModule(Dictionary<EntityType, List<Entity>> Entities, PlayerView playerView)
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

            UnitsCenter.Main.Defence[EntityType.MeleeUnit].AddRange(Entities[EntityType.MeleeUnit]);
            UnitsCenter.Main.Defence[EntityType.RangedUnit].AddRange(Entities[EntityType.RangedUnit]);
        }

        public void DoAction(Dictionary<EntityType, List<Entity>> Entities, Dictionary<int, EntityAction> Actions, int money, PlayerView playerView)
        {
            System.Random rand = new System.Random();

            foreach (Entity unit in UnitsCenter.Main.Defence[EntityType.MeleeUnit])
            {
                EntityAction def = new EntityAction()
                {
                    AttackAction = new AttackAction()
                    {
                        AutoAttack = new AutoAttack()
                        {
                            PathfindRange = 40,
                            ValidTargets = Targets
                        }
                    },
                    MoveAction = new MoveAction()
                    {
                        BreakThrough = false,
                        FindClosestPosition = true,
                        Target = new Vec2Int(playerView.MapSize / 4 + rand.Next(-10, 10), playerView.MapSize / 3 + rand.Next(-10, 10))
                    }
                };

                if (Actions.ContainsKey(unit.Id)) Actions[unit.Id] = def;
                else Actions.Add(unit.Id, def);
            }

            foreach (Entity unit in UnitsCenter.Main.Defence[EntityType.RangedUnit])
            {
                EntityAction def = new EntityAction()
                {
                    AttackAction = new AttackAction()
                    {
                        AutoAttack = new AutoAttack()
                        {
                            PathfindRange = 40,
                            ValidTargets = Targets
                        }
                    },
                    MoveAction = new MoveAction()
                    {
                        BreakThrough = false,
                        FindClosestPosition = true,
                        Target = new Vec2Int(playerView.MapSize / 4 + rand.Next(-5, 5), playerView.MapSize / 4 + rand.Next(-5, 5))
                    }
                };

                if (Actions.ContainsKey(unit.Id))
                    Actions[unit.Id] = def;
                else Actions.Add(unit.Id, def);
            }
        }
    }
}
