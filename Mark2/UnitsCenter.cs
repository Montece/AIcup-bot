using Aicup2020.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AICUP.Mark2
{
    public class UnitsCenter
    {
        public static UnitsCenter Main;

        public Dictionary<EntityType, List<Entity>> Rear = new Dictionary<EntityType, List<Entity>>();
        public Dictionary<EntityType, List<Entity>> Defence = new Dictionary<EntityType, List<Entity>>();
        public Dictionary<EntityType, List<Entity>> Attack = new Dictionary<EntityType, List<Entity>>();

        public List<EntityType> RearToCreate = new List<EntityType>();
        public List<EntityType> DefenceToCreate = new List<EntityType>();
        public List<EntityType> AttackToCreate = new List<EntityType>();

        public EntityType[] EntityTypes = null;

        private readonly Vec2Int BuildPosition = new Vec2Int(5, 4);

        private ExploringModule exploringModuile;

        public UnitsCenter(ExploringModule exploringModuile_)
        {
            //Singleton
            Main = this;

            exploringModuile = exploringModuile_;

            //Заполение массива типов сущностей
            string[] names = System.Enum.GetNames(typeof(EntityType));
            EntityTypes = new EntityType[names.Length];
            for (int i = 0; i < EntityTypes.Length; i++) EntityTypes[i] = (EntityType)System.Enum.Parse(typeof(EntityType), names[i]);

            //Инициализатор (чтобы не было рандомных null)
            foreach (EntityType type in EntityTypes)
            {
                Rear.Add(type, new List<Entity>());
                Defence.Add(type, new List<Entity>());
                Attack.Add(type, new List<Entity>());
            }
        }

        public void FirstFill(Dictionary<EntityType, List<Entity>> entitiesSort)
        {
            //Начальное заполнение
            Rear[EntityType.BuilderBase].AddRange(entitiesSort[EntityType.BuilderBase]);
            Rear[EntityType.BuilderUnit].AddRange(entitiesSort[EntityType.BuilderUnit]);
            //Rear[EntityType.House].AddRange(entitiesSort[EntityType.House]);
            Rear[EntityType.MeleeBase].AddRange(entitiesSort[EntityType.MeleeBase]);
            Rear[EntityType.RangedBase].AddRange(entitiesSort[EntityType.RangedBase]);

            Defence[EntityType.MeleeUnit].AddRange(entitiesSort[EntityType.MeleeUnit]);
            Defence[EntityType.RangedUnit].AddRange(entitiesSort[EntityType.RangedUnit]);
            Defence[EntityType.Turret].AddRange(entitiesSort[EntityType.Turret]);
            //Defence[EntityType.Resource].AddRange(entitiesSort[EntityType.Resource]);
            //Defence[EntityType.Wall].AddRange(entitiesSort[EntityType.Wall]);
        }

        public void SortUnits(List<Entity> allEntities, Dictionary<int, EntityAction> Actions)
        {
            Entity[] allEntities_ = new Entity[allEntities.Count];
            allEntities.CopyTo(allEntities_);
            List<Entity> AllEntities = allEntities_.ToList();

            //Удаляем все существующие в модулях юниты
            foreach (EntityType type in EntityTypes)
            {
                for (int i = 0; i < AllEntities.Count; i++)
                {
                    if (Rear[type].Where(e => e.Id == AllEntities[i].Id).Count() > 0)
                    {
                        AllEntities.RemoveAt(i);
                        i--;
                    }
                    else if (Defence[type].Where(e => e.Id == AllEntities[i].Id).Count() > 0)
                    {
                        AllEntities.RemoveAt(i);
                        i--;
                    }
                    else if (Attack[type].Where(e => e.Id == AllEntities[i].Id).Count() > 0)
                    {
                        AllEntities.RemoveAt(i);
                        i--;
                    }
                }
            }

            //Остались только неизвестные юниты
            for (int i = 0; i < AllEntities.Count; i++)
            {
                //Ищем, делал ли Тыл юниты
                List<EntityType> entities = RearToCreate.Where(e => e == AllEntities[i].EntityType).ToList();
                if (entities.Count() > 0)
                {
                    Rear[AllEntities[i].EntityType].Add(AllEntities[i]);
                    RearToCreate.Remove(AllEntities[i].EntityType);
                    AllEntities.RemoveAt(i);
                    i--;
                }
                else
                {
                    List<EntityType> entities_a = AttackToCreate.Where(e => e == AllEntities[i].EntityType).ToList();
                    if (entities_a.Count() > 0)
                    {
                        Attack[AllEntities[i].EntityType].Add(AllEntities[i]);
                        AttackToCreate.Remove(AllEntities[i].EntityType);
                        AllEntities.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        List<EntityType> entities_d = DefenceToCreate.Where(e => e == AllEntities[i].EntityType).ToList();
                        if (entities_d.Count() > 0)
                        {
                            Defence[AllEntities[i].EntityType].Add(AllEntities[i]);
                            DefenceToCreate.Remove(AllEntities[i].EntityType);
                            AllEntities.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            if (AllEntities[i].EntityType == EntityType.BuilderBase) Rear[AllEntities[i].EntityType].Add(AllEntities[i]);
                            else
                            if (AllEntities[i].EntityType == EntityType.MeleeBase) Rear[AllEntities[i].EntityType].Add(AllEntities[i]);
                            else
                            if (AllEntities[i].EntityType == EntityType.RangedBase) Rear[AllEntities[i].EntityType].Add(AllEntities[i]);
                        }
                    }
                }

                /*//Ищем, делала ли Защита юниты
                entities = DefenceToCreate.Where(e => e == AllEntities[i].EntityType);
                if (entities.Count() > 0)
                {
                    Defence[AllEntities[i].EntityType].Add(AllEntities[i]);
                    DefenceToCreate.Remove(AllEntities[i].EntityType);
                    AllEntities.RemoveAt(i);
                }

                //Ищем, делала ли Атака юниты
                entities = AttackToCreate.Where(e => e == AllEntities[i].EntityType);
                if (entities.Count() > 0)
                {
                    Attack[AllEntities[i].EntityType].Add(AllEntities[i]);
                    AttackToCreate.Remove(AllEntities[i].EntityType);
                    AllEntities.RemoveAt(i);
                }*/
            }

            //Проверяем, живы ли остальные юниты
            CheckAlive(allEntities, Rear);
            CheckAlive(allEntities, Defence);
            CheckAlive(allEntities, Attack);

            //Отменяем постройку во всех домах
            StopAllBases(Actions);
        }

        private void CheckAlive(List<Entity> All, Dictionary<EntityType, List<Entity>> Units)
        {
            foreach (EntityType type in EntityTypes)
            {
                for (int i = 0; i < Units[type].Count; i++)
                {
                    if (All.Where(e => e.Id == Units[type][i].Id).Count() == 0)
                    {
                        Units[type].RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void StopAllBases(Dictionary<int, EntityAction> Actions)
        {
            EntityAction action = new EntityAction();

            for (int i = 0; i < Rear[EntityType.BuilderBase].Count; i++)
            {
                if (Actions.ContainsKey(Rear[EntityType.BuilderBase][i].Id)) Actions[Rear[EntityType.BuilderBase][i].Id] = action;
                else Actions.Add(Rear[EntityType.BuilderBase][i].Id, action);
            }

            for (int i = 0; i < Rear[EntityType.MeleeBase].Count; i++)
            {
                if (Actions.ContainsKey(Rear[EntityType.MeleeBase][i].Id)) Actions[Rear[EntityType.MeleeBase][i].Id] = action;
                Actions.Add(Rear[EntityType.MeleeBase][i].Id, action);
            }

            for (int i = 0; i < Rear[EntityType.RangedBase].Count; i++)
            {
                if (Actions.ContainsKey(Rear[EntityType.RangedBase][i].Id)) Actions[Rear[EntityType.RangedBase][i].Id] = action;
                Actions.Add(Rear[EntityType.RangedBase][i].Id, action);
            }
        }

        public void CreateUnit(Dictionary<int, EntityAction> actions, List<Entity> bases, int units_count, int money, int unit_cost, UnitModule module, EntityType unit_type, PlayerView playerView)
        {
            units_count = bases.Count < units_count ? bases.Count : units_count;

            for (int i = 0; i < units_count; i++)
            {
                if (!actions.ContainsKey(bases[i].Id) || (actions.ContainsKey(bases[i].Id) && actions[bases[i].Id].BuildAction == null))
                {
                    if (unit_cost <= money)
                    {
                        EntityAction action = new EntityAction();
                        BuildAction buildAction = new BuildAction
                        {
                            EntityType = unit_type,
                            //Position = new Vec2Int(bases[i].Position.X + BuildPosition.X, bases[i].Position.Y + BuildPosition.Y)
                            Position = new Vec2Int(bases[i].Position.X + playerView.EntityProperties[bases[i].EntityType].Size, bases[i].Position.Y + playerView.EntityProperties[bases[i].EntityType].Size - 1)
                        };
                        action.BuildAction = buildAction;
                        actions[bases[i].Id] = action;
                        switch (module)
                        {
                            case UnitModule.Rear:
                                RearToCreate.Add(unit_type);
                                break;
                            case UnitModule.Defence:
                                DefenceToCreate.Add(unit_type);
                                break;
                            case UnitModule.Attack:
                                AttackToCreate.Add(unit_type);
                                break;
                            default:
                                break;
                        }

                        money -= unit_cost;
                    }
                }
            }
        }

        //Только для строителей тыла
        public void CreateBase(EntityBuilder unit, List<EntityBuilder> units, Dictionary<int, EntityAction> actions, EntityType base_type, PlayerView playerView, int money)
        {
            //На месте ли наш рабочий
            if (unit.Task == BuilderTask.Moving)
            {
                if (unit.Entity.Position.X == unit.MoveCoords.X && unit.Entity.Position.Y == unit.MoveCoords.Y)
                {
                    //Если есть деньги - строим
                    if (playerView.EntityProperties[base_type].Cost <= money)
                    {
                        //Строим базу
                        BuildAction build_action = new BuildAction()
                        {
                            EntityType = base_type,
                            Position = unit.BuildCoords       
                        };

                        if (actions.ContainsKey(unit.ID)) actions[unit.ID] = new EntityAction(null, build_action, null, null);
                        else actions.Add(unit.ID, new EntityAction(null, build_action, null, null));

                        unit.Task = BuilderTask.Idle;
                        Out.Print("Рабочий " + unit.ID + " строит в " + unit.BuildCoords.X + "|" + unit.BuildCoords.Y);
                    }
                }
                else
                {
                    //Если нет, двигаемся к месту
                    MoveAction move_action = new MoveAction()
                    {
                        BreakThrough = false,
                        FindClosestPosition = true,
                        Target = unit.MoveCoords
                    };

                    if (actions.ContainsKey(unit.ID)) actions[unit.ID] = new EntityAction(move_action, null, null, null);
                    else actions.Add(unit.ID, new EntityAction(move_action, null, null, null));

                    unit.Task = BuilderTask.Moving;
                    Out.Print("Рабочий " + unit.ID + " движется к " + unit.MoveCoords.X + "|" + unit.MoveCoords.Y);
                }
            }
            else
            {
                exploringModuile.CorrectMap(playerView.EntityProperties[base_type].Size, units);
                var a = exploringModuile.ShowMap();
                Vec2Int? target = exploringModuile.GetPlaceForBuild(playerView.EntityProperties[base_type].Size, units);
                if (!target.HasValue) return;
                unit.BuildCoords = new Vec2Int(target.Value.X + 1, target.Value.Y);
                unit.MoveCoords = target.Value;
                unit.Base = base_type;

                //Если нет, двигаемся к месту
                MoveAction move_action = new MoveAction()
                {
                    BreakThrough = false,
                    FindClosestPosition = true,
                    Target = unit.MoveCoords
                };

                if (actions.ContainsKey(unit.ID)) actions[unit.ID] = new EntityAction(move_action, null, null, null);
                else actions.Add(unit.ID, new EntityAction(move_action, null, null, null));

                unit.Task = BuilderTask.Moving;
            }
        }

        public List<Entity> GetBasesToRepair(PlayerView playerView)
        {
            return exploringModuile.SearchBasesToRepair(playerView);
        }

        public void Repair(Dictionary<int, EntityAction> actions, EntityBuilder unit, Entity baseToRepair)
        {
            RepairAction action = new RepairAction()
            {
                Target = baseToRepair.Id
            };
            if (actions.ContainsKey(unit.ID)) actions[unit.ID] = new EntityAction(null, null, null, action);
        }

        public string GetInfo()
        {
            string info = "Юниты" + System.Environment.NewLine + System.Environment.NewLine;
            info += System.Environment.NewLine + "Тыл: " + System.Environment.NewLine + System.Environment.NewLine;
            foreach (EntityType type in EntityTypes)
                if (Rear.Count > 0)
                    info += type.ToString() + ": " + Rear[type].Count + System.Environment.NewLine;
            info += System.Environment.NewLine + "Защита: " + System.Environment.NewLine + System.Environment.NewLine;
            foreach (EntityType type in EntityTypes)
                if (Defence.Count > 0)
                    info += type.ToString() + ": " + Defence[type].Count + System.Environment.NewLine;
            info += System.Environment.NewLine + "Атака: " + System.Environment.NewLine + System.Environment.NewLine;
            foreach (EntityType type in EntityTypes)
                if (Attack.Count > 0)
                    info += type.ToString() + ": " + Attack[type].Count + System.Environment.NewLine;

            return info;
        }
    }

    public enum UnitModule
    {
        Rear,
        Defence,
        Attack
    }
}