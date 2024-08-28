using StardewValley.Monsters;

namespace TerrariaBosses
{
    public class Config
    {
        public string Difficulty { get; set; } = "Classic";
        public bool SpawnGore { get; set; } = true;
        public DemonEyeSpawningConfig DemonEyeSpawning { get; set; } = new DemonEyeSpawningConfig();
        public EyeOfCthulhuConfig EyeOfCthulhu { get; set; } = new EyeOfCthulhuConfig();
    }
    public class DemonEyeSpawningConfig
    {
        public int SpawnChance { get; set; } = 5;
        public int SpawnAfter { get; set; } = 1800;
        public bool OnlyAfterTerrariaEasterEgg { get; set; } = true;
        public bool BlockWhenEOCAlive { get; set; } = true;
        public string SpawnHalloweenVariants { get; set; } = "During Fall";
        public SpawnLocationConfig SpawnLocation { get; set; } = new SpawnLocationConfig();
    }
    public class EyeOfCthulhuConfig
    {
        public bool PlayCutscene { get; set; } = true;
        public int SpawnAfter { get; set; } = 1800;
        public bool OnlyOne { get; set; } = true;
        public SpawnLocationConfig SpawnLocation { get; set; } = new SpawnLocationConfig();
    }
    public class SpawnLocationConfig
    {
        public bool Anywhere { get; set; } = false;
        public bool AnywhereOutdoors { get; set; } = false;
        public bool Farm { get; set; } = true;
        public bool Mines { get; set; } = false;
        public bool SkullCavern { get; set; } = false;
        public bool QuarryMine { get; set; } = false;
        public bool MutantBugLair { get; set; } = false;
        public bool VolcanoDungeon { get; set; } = false;
        public bool SecretWoods { get; set; } = false;
        public bool GingerIsland { get; set; } = false;
        public bool Desert { get; set; } = false;
    }
}
