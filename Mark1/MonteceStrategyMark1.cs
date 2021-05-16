using Aicup2020.Model;
using System.Collections.Generic;
using System.Linq;

namespace Aicup2020
{
    //public class MyStrategy
    //{
        /*private bool IsInited = false;
        private EntityType[] EntityTypes;
        private Vec2Int OpposeCorner;

        public MyStrategy()
        {
            Print("������ �����");
        }

        private void Init(PlayerView playerView)
        {
            IsInited = true;
            Print("���������������");

            //��������
            string[] names = System.Enum.GetNames(typeof(EntityType));
            EntityTypes = new EntityType[names.Length];
            for (int i = 0; i < EntityTypes.Length; i++) EntityTypes[i] = (EntityType)System.Enum.Parse(typeof(EntityType), names[i]);

            OpposeCorner = new Vec2Int(playerView.MapSize - 1, playerView.MapSize - 1);
        }

        public Action GetAction(PlayerView playerView, DebugInterface debugInterface)
        {
            if (!IsInited) Init(playerView);
            //Print("--- ����� �������� ---");

            //����� �������� ������
            Action player_act = new Action();
            //����� �������� ���������
            Dictionary<int, EntityAction> players_acts = new Dictionary<int, EntityAction>();

            //----------------------------

            Player player = playerView.Players.Where(p => p.Id == playerView.MyId).FirstOrDefault();
            int resources = player.Resource;

            //�������� ������
            Dictionary<EntityType, List<Entity>> Entities = new Dictionary<EntityType, List<Entity>>();

            //������������� (����� �� ���� ��������� null)
            for (int i = 0; i < EntityTypes.Length; i++) Entities[EntityTypes[i]] = new List<Entity>();

            //��������� ������ ���������
            for (int i = 0; i < playerView.Entities.Length; i++)
                if (playerView.Entities[i].PlayerId == playerView.MyId) Entities[playerView.Entities[i].EntityType].Add(playerView.Entities[i]);

            //������������ ������ ��������
            foreach (EntityType type in EntityTypes)
            {
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
                                    Print("Builder Base -> �����!");
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
                            //Print("Builder Unit -> �������!");
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
                            //����� ��� ���������� ��������� ������
                            action_attack.AutoAttack = new AutoAttack()
                            {
                                PathfindRange = 30,
                                ValidTargets = new EntityType[] { EntityType.MeleeUnit, EntityType.RangedUnit, EntityType.Turret }
                            };
                            action_move.FindClosestPosition = true;
                            action_move.BreakThrough = true;
                            action_move.Target = OpposeCorner;

                            action.AttackAction = action_attack;
                            action.MoveAction = action_move;
                            //Print("Melee Unit -> ������!");
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
                            //����� ��� ���������� ��������� ������
                            action_attack.AutoAttack = new AutoAttack()
                            {
                                PathfindRange = 30,
                                ValidTargets = new EntityType[] { EntityType.MeleeUnit, EntityType.RangedUnit, EntityType.Turret }
                            };
                            action_move.FindClosestPosition = true;
                            action_move.BreakThrough = true;
                            action_move.Target = OpposeCorner;

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
            }

            player_act.EntityActions = players_acts;         

            //Print("--- ����� �������� ---");
            return player_act;
        }

        public void DebugUpdate(PlayerView playerView, DebugInterface debugInterface) 
        {
            debugInterface.Send(new DebugCommand.Clear());
            debugInterface.GetState();
        }

        private void Print(string text)
        {
            System.Console.WriteLine("[" + System.DateTime.Now.ToShortTimeString() + "]: " + text);
        }*/
    //}
}