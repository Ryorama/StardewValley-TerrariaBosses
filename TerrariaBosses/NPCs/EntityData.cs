using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaBosses.NPCs
{
    public class EntityData
    {
        public List<ITerrariaEntity> daySpawnList = new List<ITerrariaEntity>();
        public List<ITerrariaEntity> nightSpawnList = new List<ITerrariaEntity>();
        public List<ITerrariaEntity> fallDaySpawnList = new List<ITerrariaEntity>();
        public List<ITerrariaEntity> fallNightSpawnList = new List<ITerrariaEntity>();

        public EntityData() 
        {
        }

        public void clearLists()
        {
            daySpawnList.Clear();
            nightSpawnList.Clear();
            fallDaySpawnList.Clear();
            fallNightSpawnList.Clear();
        }

        public void setupLists()
        {
            addDayEntities();
            addNightEntities();
        }

        public void addDayEntities()
        {
            daySpawnList.Add(new Slime("Green Slime"));
            daySpawnList.Add((new Slime("Blue Slime")));

            fallDaySpawnList.Add(new Slime("Green Slime"));
            fallDaySpawnList.Add((new Slime("Blue Slime")));
        }

        public void addNightEntities()
        {
            nightSpawnList.Add(new DemonEye("Demon Eye"));
            nightSpawnList.Add(new DemonEye("Cataract Eye"));
            nightSpawnList.Add(new DemonEye("Sleepy Eye"));
            nightSpawnList.Add(new DemonEye("Dilated Eye"));
            nightSpawnList.Add(new DemonEye("Green Eye"));
            nightSpawnList.Add(new DemonEye("Purple Eye"));

            fallNightSpawnList.Add(new DemonEye("Demon Eye"));
            fallNightSpawnList.Add(new DemonEye("Cataract Eye"));
            fallNightSpawnList.Add(new DemonEye("Sleepy Eye"));
            fallNightSpawnList.Add(new DemonEye("Dilated Eye"));
            fallNightSpawnList.Add(new DemonEye("Green Eye"));
            fallNightSpawnList.Add(new DemonEye("Purple Eye"));
            fallNightSpawnList.Add(new DemonEye("Owl Demon Eye"));
            fallNightSpawnList.Add(new DemonEye("Spaceship Demon Eye"));
        }
    }
}
