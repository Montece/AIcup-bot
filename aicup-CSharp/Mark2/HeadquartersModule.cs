using Aicup2020.Model;
using AICUP.Mark2;
using System.Collections.Generic;
using System.Linq;

namespace AICUP.Mark2
{
    public class HeadquartersModule
    {
        private RearModule rearModule = null;
        private DefenceModule defenceModule = null;
        private AttackModule attackModule = null;
        private ExploringModule exploringModule = null;
        private UnitsCenter unitsCenter = null;

        private Vec2Int enemyBase = new Vec2Int(0, 0);
        private long actionID = 0;

        public HeadquartersModule(PlayerView playerView)
        {
            Out.Print("Инициализация штаба.");
    
            //Заполнение переменной координат нижнего угла
            enemyBase = new Vec2Int(playerView.MapSize - 1, playerView.MapSize - 1);

            //Обнуление ACTION ID
            actionID = 1;
        }

        public Action DoAction(PlayerView playerView)
        {
            //Out.Print("--- Новое действие #" + actionID + " ---");
            System.DateTime start = System.DateTime.Now;
            //Показать статистику по юнитам из модулей с clear

            //Новое действие игрока
            Action action = new Action();
            //Новые действие сущностей
            Dictionary<int, EntityAction> actions = new Dictionary<int, EntityAction>();

            //----------------------------

            //Текущий игрок
            Player player = playerView.Players.Where(p => p.Id == playerView.MyId).FirstOrDefault();
            //Кол-во ресурсов игрока
            int money = player.Resource;

            //Сущности игрока
            List<Entity> Entities = new List<Entity>();
            //Сортированные сущности игрока
            Dictionary<EntityType, List<Entity>> EntitiesSort = new Dictionary<EntityType, List<Entity>>();

            //Инициализация модуля разведки
            if (exploringModule == null) exploringModule = new ExploringModule(playerView.MapSize);
            //Инициализация модуля юнитов
            if (unitsCenter == null) unitsCenter = new UnitsCenter(exploringModule);

            //Инициализатор (чтобы не было рандомных null)
            for (int i = 0; i < UnitsCenter.Main.EntityTypes.Length; i++) EntitiesSort[UnitsCenter.Main.EntityTypes[i]] = new List<Entity>();

            //Заполняем массив сущностей
            for (int i = 0; i < playerView.Entities.Length; i++)
                if (playerView.Entities[i].PlayerId == playerView.MyId) Entities.Add(playerView.Entities[i]);

            //Заполняем массив сортированных сущностей
            for (int i = 0; i < Entities.Count; i++)
                EntitiesSort[Entities[i].EntityType].Add(Entities[i]);

            //Сортируем юниты по отделам
            unitsCenter.SortUnits(Entities, actions);

            //Обновление карты
            exploringModule.UpdateMap(playerView.Entities, playerView);

            //Инициализация модулей  
            if (rearModule == null) rearModule = new RearModule(EntitiesSort, playerView);
            if (defenceModule == null) defenceModule = new DefenceModule(EntitiesSort, playerView);
            if (attackModule == null) attackModule = new AttackModule(EntitiesSort, playerView);
            rearModule.DoAction(EntitiesSort, actions, money, playerView);
            defenceModule.DoAction(EntitiesSort, actions, money, playerView);
            attackModule.DoAction(EntitiesSort, actions, money, playerView);

            //Обрабатываем каждый тип сущности
            /*foreach (EntityType type in entityTypes)
            {
                //Обрабатываем каждую сущность
                foreach (Entity entity in Entities[type])
                {
                    if (!entity.Active) continue;

                    EntityAction action = new EntityAction();

                    MoveAction action_move = new MoveAction();
                    BuildAction action_build = new BuildAction();
                    AttackAction action_attack = new AttackAction();
                    RepairAction action_repair = new RepairAction();

                    switch (entity.EntityType)
                    {
                        case EntityType.Wall:
                            break;
                        case EntityType.House:
                            break;
                        case EntityType.BuilderBase:
                            if (Entities[EntityType.BuilderUnit].Count < 5)
                            {
                                if (resources >= playerView.EntityProperties[EntityType.BuilderUnit].Cost)
                                {
                                    action_build.EntityType = EntityType.BuilderUnit;
                                    action_build.Position = new Vec2Int(entity.Position.X + playerView.EntityProperties[EntityType.BuilderBase].Size, entity.Position.Y + playerView.EntityProperties[EntityType.BuilderBase].Size - 1);
                                    Out.Print("Builder Base -> Строю!");
                                    action.BuildAction = action_build;
                                }
                            }
                            break;
                        case EntityType.BuilderUnit:
                            action_attack.AutoAttack = new AutoAttack
                            {
                                PathfindRange = 30,
                                ValidTargets = new EntityType[] { EntityType.Resource }
                            };

                            action.AttackAction = action_attack;
                            //Out.Print("Builder Unit -> Добываю!");
                            /*if (current_units + 1) *player_view.entity_properties[&entity_type].population_use
                         <= properties.population_provide
                            action_build.EntityType
                            action_move.FindClosestPosition = true;
                            action_move.BreakThrough = true;
                            action_move.Target = new Vec2Int(playerView.MapSize - 1, playerView.MapSize - 1);
                            action.MoveAction = action_move;
                            break;
                        case EntityType.MeleeBase:
                            if (resources >= playerView.EntityProperties[EntityType.MeleeUnit].Cost)
                            {
                                action_build.EntityType = EntityType.MeleeUnit;
                                action_build.Position = new Vec2Int(entity.Position.X + playerView.EntityProperties[EntityType.MeleeBase].Size, entity.Position.Y + playerView.EntityProperties[EntityType.MeleeBase].Size - 1);
                                action.BuildAction = action_build;
                            }
                            break;
                        case EntityType.MeleeUnit:
                            //Отряд для ликвидации вражеской пехоты
                            action_attack.AutoAttack = new AutoAttack()
                            {
                                PathfindRange = 30,
                                ValidTargets = new EntityType[] { EntityType.MeleeUnit, EntityType.RangedUnit, EntityType.Turret }
                            };
                            action_move.FindClosestPosition = true;
                            action_move.BreakThrough = true;
                            action_move.Target = enemyBase;

                            action.AttackAction = action_attack;
                            action.MoveAction = action_move;
                            //Out.Print("Melee Unit -> Атакую!");
                            break;
                        case EntityType.RangedBase:
                            if (resources >= playerView.EntityProperties[EntityType.RangedUnit].Cost)
                            {
                                action_build.EntityType = EntityType.RangedUnit;
                                action_build.Position = new Vec2Int(entity.Position.X + playerView.EntityProperties[EntityType.RangedBase].Size, entity.Position.Y + playerView.EntityProperties[EntityType.RangedBase].Size - 1);
                                action.BuildAction = action_build;
                            }
                            break;
                        case EntityType.RangedUnit:
                            //Отряд для ликвидации вражеской пехоты
                            action_attack.AutoAttack = new AutoAttack()
                            {
                                PathfindRange = 30,
                                ValidTargets = new EntityType[] { EntityType.MeleeUnit, EntityType.RangedUnit, EntityType.Turret }
                            };
                            action_move.FindClosestPosition = true;
                            action_move.BreakThrough = true;
                            action_move.Target = enemyBase;

                            action.AttackAction = action_attack;
                            action.MoveAction = action_move;
                            break;
                        case EntityType.Resource:
                            break;
                        case EntityType.Turret:
                            break;
                        default:
                            break;
                    }

                    players_acts.Add(entity.Id, action);
                }
            }*/

            action.EntityActions = actions;
            actionID++;
            System.DateTime stop = System.DateTime.Now;
            //Out.Print("Время: " + (stop - start).Milliseconds + " мс");

            //System.Console.Clear();
            //System.Console.WriteLine(UnitsCenter.Main.GetInfo());

            return action;
        }
    }
}
