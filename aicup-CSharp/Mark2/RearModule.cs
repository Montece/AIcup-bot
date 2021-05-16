using Aicup2020.Model;
using System.Collections.Generic;
using System.Linq;

namespace AICUP.Mark2
{
    public class RearModule
    {
        private int buildersCount = 30;
        private int builderBasesCount = 1;

        private int defenceMeleeCount = 5;
        private int defenceRangedCount = 7;
        private float attackKoeff = 1 / 3;

        private int buildersCost;
        private int meleeCost;
        private int rangedCost;

        List<EntityBuilder> BaseBuilders = new List<EntityBuilder>();

        public RearModule(Dictionary<EntityType, List<Entity>> Entities, PlayerView playerView)
        {
            UnitsCenter.Main.Rear[EntityType.BuilderUnit].AddRange(Entities[EntityType.BuilderUnit]);

            buildersCost = playerView.EntityProperties[EntityType.BuilderUnit].Cost;
            meleeCost = playerView.EntityProperties[EntityType.MeleeUnit].Cost;
            rangedCost = playerView.EntityProperties[EntityType.RangedUnit].Cost;
        }

        public void DoAction(Dictionary<EntityType, List<Entity>> Entities, Dictionary<int, EntityAction> Actions, int money, PlayerView playerView)
        {
            //Создание юнитов
            if (UnitsCenter.Main.Rear[EntityType.BuilderUnit].Count < buildersCount)
            {
                UnitsCenter.Main.CreateUnit(Actions, UnitsCenter.Main.Rear[EntityType.BuilderBase], buildersCount - UnitsCenter.Main.Rear[EntityType.BuilderUnit].Count, money, buildersCost, UnitModule.Rear, EntityType.BuilderUnit, playerView);
            }
            //else if (Entities[EntityType.BuilderBase].Count < builderBasesCount) CreateBuilderBase();

            //Управление юнитами
            for (int i = 0; i < UnitsCenter.Main.Rear[EntityType.BuilderUnit].Count; i++)
            {
                Actions.Add(UnitsCenter.Main.Rear[EntityType.BuilderUnit][i].Id, GetDefaultWorkerAction());
            }

            //Если рабочих достаточно, чтобы начать делать строителей зданий
            if (UnitsCenter.Main.Rear[EntityType.BuilderUnit].Count > 5)
            { 
                //Удаляем мертвых строителей зданий
                for (int i = 0; i < BaseBuilders.Count; i++)
                {
                    if (UnitsCenter.Main.Rear[EntityType.BuilderUnit].Where(e => e.Id == BaseBuilders[i].ID).Count() == 0)
                    {
                        BaseBuilders.RemoveAt(i);
                        i--;
                    }
                }

                //Если не хватает строителей зданий
                if (BaseBuilders.Count < builderBasesCount)
                {
                    //Смотрим, можем ли мы их призвать или не хватает вообще строителей
                    if (builderBasesCount - BaseBuilders.Count <= buildersCount)
                    {
                        int i = 0;
                        int last = builderBasesCount - BaseBuilders.Count;
                        while (i < UnitsCenter.Main.Rear[EntityType.BuilderUnit].Count/* - BaseBuilders.Count*/ && last != 0)
                        {
                            if (BaseBuilders.Where(e => (e.ID == UnitsCenter.Main.Rear[EntityType.BuilderUnit][i].Id)).Count() == 0)
                            {
                                BaseBuilders.Add(new EntityBuilder()
                                {
                                    Entity = UnitsCenter.Main.Rear[EntityType.BuilderUnit][i],
                                    ID = UnitsCenter.Main.Rear[EntityType.BuilderUnit][i].Id
                                });

                                last--;
                            }
                            i++;
                        }
                    }
                }
            }

            List<Entity> ToRepair = UnitsCenter.Main.GetBasesToRepair(playerView);

            //Даем задачи строителям
            for (int i = 0; i < BaseBuilders.Count; i++)
            {
                if (i < ToRepair.Count)
                {
                    Entity toRepair = ToRepair[i];
                    UnitsCenter.Main.Repair(Actions, BaseBuilders[i], toRepair);
                    continue;
                }

                BaseBuilders[i].Entity = Entities[EntityType.BuilderUnit].Where(e => e.Id == BaseBuilders[i].ID).FirstOrDefault();

                if (UnitsCenter.Main.Rear[EntityType.BuilderBase].Count < 1) UnitsCenter.Main.CreateBase(BaseBuilders[i], BaseBuilders, Actions, EntityType.BuilderBase, playerView, money);
                else
                if (UnitsCenter.Main.Rear[EntityType.MeleeBase].Count < 1) UnitsCenter.Main.CreateBase(BaseBuilders[i], BaseBuilders, Actions, EntityType.MeleeBase, playerView, money);
                else
                if (UnitsCenter.Main.Rear[EntityType.RangedBase].Count < 1) UnitsCenter.Main.CreateBase(BaseBuilders[i], BaseBuilders, Actions, EntityType.RangedBase, playerView, money);
                else
                UnitsCenter.Main.CreateBase(BaseBuilders[i], BaseBuilders, Actions, EntityType.House, playerView, money);
            }

            int d_m = UnitsCenter.Main.Defence[EntityType.MeleeUnit].Count;
            int d_r = UnitsCenter.Main.Defence[EntityType.RangedUnit].Count;
            int a_m = UnitsCenter.Main.Attack[EntityType.MeleeUnit].Count;
            int a_r = UnitsCenter.Main.Attack[EntityType.RangedUnit].Count;

            /*if (d_m < defenceMeleeCount) UnitsCenter.Main.CreateUnit(Actions, UnitsCenter.Main.Rear[EntityType.MeleeBase], defenceMeleeCount - UnitsCenter.Main.Defence[EntityType.MeleeUnit].Count, money, meleeCost, UnitModule.Defence, EntityType.MeleeUnit, playerView);
            else 
            if (d_r < defenceRangedCount) UnitsCenter.Main.CreateUnit(Actions, UnitsCenter.Main.Rear[EntityType.RangedBase], defenceRangedCount - UnitsCenter.Main.Defence[EntityType.RangedUnit].Count, money, rangedCost, UnitModule.Defence, EntityType.RangedUnit, playerView);
            else
            {
                if (a_r != 0 && a_m / a_r < attackKoeff) UnitsCenter.Main.CreateUnit(Actions, UnitsCenter.Main.Attack[EntityType.MeleeBase], 2, money, meleeCost, UnitModule.Attack, EntityType.MeleeUnit, playerView);
                else
                if (a_r == 0 || (a_r != 0 && a_m / a_r > attackKoeff)) UnitsCenter.Main.CreateUnit(Actions, UnitsCenter.Main.Attack[EntityType.RangedBase], 2, money, rangedCost, UnitModule.Attack, EntityType.RangedUnit, playerView);
                else
                {*/
                    UnitsCenter.Main.CreateUnit(Actions, UnitsCenter.Main.Rear[EntityType.MeleeBase], 2, money, meleeCost, UnitModule.Attack, EntityType.MeleeUnit, playerView);
                    UnitsCenter.Main.CreateUnit(Actions, UnitsCenter.Main.Rear[EntityType.RangedBase], 2, money, rangedCost, UnitModule.Attack, EntityType.RangedUnit, playerView);
                //}
            //}

            //Создать защитников 10 / 10
            //Создать атакующих
        }

        public EntityAction GetDefaultWorkerAction()
        {
            return new EntityAction()
            {
                AttackAction = new AttackAction
                {
                    AutoAttack = new AutoAttack()
                    {
                        PathfindRange = 400,
                        ValidTargets = new EntityType[] { EntityType.Resource }
                    }
                }
            };
        }

        /*
         * Приоритеты (с самого приоритетного до менее):
         * 
         * Рабочий юнит
         * Фабрика рабочих
         * Юниты защиты
         * Фабрика юнитов защиты
         * Фабрика юнитов атаки
         * Юниты атаки
         * 
         */
    }
}
