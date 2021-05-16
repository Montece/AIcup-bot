using Aicup2020.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AICUP.Mark2
{
    public class ExploringModule
    {
        public Entity?[,] Map;

        private int MapSize;

        public ExploringModule(int size)
        {
            MapSize = size;
            Map = new Entity?[size, size];
            ClearMap();
        }

        private void ClearMap()
        {
            for (int i = 0; i < MapSize; i++)
                for (int j = 0; j < MapSize; j++)
                    Map[i, j] = null;
        }

        public void UpdateMap(Entity[] entities, PlayerView playerView)
        {
            ClearMap();

            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i].EntityType != EntityType.BuilderUnit || entities[i].EntityType != EntityType.MeleeUnit || entities[i].EntityType != EntityType.RangedUnit)
                {
                    int size = playerView.EntityProperties[entities[i].EntityType].Size;
                    for (int j = 0; j < size; j++)
                    {
                        for (int c = 0; c < size; c++)
                        {
                            Map[entities[i].Position.X + j, entities[i].Position.Y + c] = entities[i];
                        }
                    }
                }
            }

            string a = ShowMap();
        }

        public List<Vec2Int> GetSpacesForBase(int base_size)
        {
            //+1 для прохода юнитов
            int unit_size = 1;
            base_size += unit_size;
            List<Vec2Int> vects = new List<Vec2Int>();

            //Ищем ближайшую клеточку свободную
            for (int i = unit_size; i < (MapSize - base_size) / 2; i++)
            {
                for (int j = unit_size; j < (MapSize - base_size) / 2; j++)
                {
                    if (!Map[i, j].HasValue)
                    {
                        bool good_cell = true;
                        int ii = 0;

                        while (ii < base_size + unit_size && good_cell)
                        {
                            int jj = 0;

                            while (jj < base_size + unit_size && good_cell)
                            {
                                good_cell = !Map[i + ii, j + jj].HasValue;
                                jj++;
                            }

                            ii++;
                        }

                        if (good_cell) vects.Add(new Vec2Int(i, j));
                    }
                }
            }

            if (vects.Count > 0) return vects;

            //Ищем дальнюю клеточку свободную
            for (int i = (MapSize - base_size) / 2 + 1; i < MapSize - base_size; i++)
            {
                for (int j = (MapSize - base_size) / 2 + 1; j < MapSize - base_size; j++)
                {
                    if (Map[i, j].HasValue)
                    {
                        bool good_cell = true;
                        int ii = 0;

                        while (ii < base_size && good_cell)
                        {
                            int jj = 0;

                            while (jj < base_size && good_cell)
                            {
                                good_cell = !Map[i + ii, j + jj].HasValue;
                                jj++;
                            }
                        }

                        if (good_cell) vects.Add(new Vec2Int(i, j));
                    }
                }
            }

            return vects;
        }

        public void CorrectMap(int base_size, List<EntityBuilder> units)
        {
            foreach (EntityBuilder unit in units)
            {
                if (unit.Task == BuilderTask.Moving)
                {
                    for (int i = unit.BuildCoords.X; i < base_size; i++)
                    {
                        for (int j = unit.BuildCoords.Y; j < base_size; j++)
                        {
                            Map[unit.BuildCoords.X + i, unit.BuildCoords.Y + j] = new Entity();
                        }
                    }
                }
            }
        }

        public Vec2Int? GetPlaceForBuild(int size, List<EntityBuilder> units)
        {
            List<Vec2Int> vecs = GetSpacesForBase(size);
            if (vecs.Count == 0) return null;

            for (int i = 0; i < units.Count; i++)
            {
                if (vecs.Contains(units[i].MoveCoords)) vecs.Remove(units[i].MoveCoords);
            }

            Vec2Int choosen = vecs.FirstOrDefault();

            return choosen;
        }

        public string ShowMap()
        {
            string map = "";
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                    map += Map[i, j].HasValue ? '☺' : ' ';
                map += Environment.NewLine;
            }

            //System.IO.File.WriteAllText("map.txt", map);

            return map;
        }

        public List<Entity> SearchBasesToRepair(PlayerView playerView)
        {
            List<Entity> ToRepair = new List<Entity>();

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (Map[i, j].HasValue)
                        if (Map[i, j].Value.EntityType == EntityType.BuilderBase || Map[i, j].Value.EntityType == EntityType.MeleeBase || Map[i, j].Value.EntityType == EntityType.RangedBase || Map[i, j].Value.EntityType == EntityType.House)
                            if (Map[i, j].Value.Health < playerView.EntityProperties[Map[i, j].Value.EntityType].MaxHealth)
                                ToRepair.Add(Map[i, j].Value);
                }
            }

            return ToRepair;
        }
    }
}
