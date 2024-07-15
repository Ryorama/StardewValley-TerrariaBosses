using TerrariaBosses.NPCs;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using GenericModConfigMenu;
using StardewValley.Locations;

namespace TerrariaBosses
{
    public class ModEntry : Mod
    {
        public static Config config;
        public static IMonitor monitor;
        private string[] demonEyeVariants =
        {
            "DemonEye",
            "CataractEye",
            "SleepyEye",
            "DilatedEye",
            "GreenEye",
            "PurpleEye"
        };
        private string[] fallDemonEyeVariants =
        {
            "DemonEye",
            "CataractEye",
            "SleepyEye",
            "DilatedEye",
            "GreenEye",
            "PurpleEye",
            "Owl DemonEye",
            "SpaceshipDemon Eye"
        };
        public override void Entry(IModHelper helper)
        {
            AssetHelper assetHelper = new AssetHelper(helper);
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
            helper.Events.GameLoop.OneSecondUpdateTicking += this.OnOneSecondUpdateTicking;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Content.AssetRequested += assetHelper.OnAssetRequested;
            assetHelper.AddCue("Hit1", "hit.wav");
            assetHelper.AddCue("Killed1", "killed.wav");
            assetHelper.AddCue("Roar", "roar.wav");
            assetHelper.AddCue("Music", "music.wav", "Music", true);
            monitor = this.Monitor;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Tools.Slingshot), nameof(StardewValley.Tools.Slingshot.canThisBeAttached), new Type[] { typeof(StardewValley.Object), typeof(int) }),
               prefix: new HarmonyMethod(typeof(Patches.Slingshot), nameof(Patches.Slingshot.canThisBeAttached_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Tools.Slingshot), nameof(StardewValley.Tools.Slingshot.GetAmmoDamage)),
               prefix: new HarmonyMethod(typeof(Patches.Slingshot), nameof(Patches.Slingshot.GetAmmoDamage_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Projectiles.BasicProjectile), "explosionAnimation"),
               prefix: new HarmonyMethod(typeof(Patches.BasicProjectile), nameof(Patches.BasicProjectile.explosionAnimation_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Debris), nameof(StardewValley.Debris.updateChunks)),
               prefix: new HarmonyMethod(typeof(Patches.Debris), nameof(Patches.Debris.updateChunks_Prefix))
            );

            config = this.Helper.ReadConfig<Config>();
        }
        private Vector2 GetSpawnLocation()
        {
            GameLocation location = Game1.player.currentLocation;
            Vector2[] corners = new Vector2[]
            {
                new Vector2(Game1.viewport.Width / 2, Game1.viewport.Height / 2),
                new Vector2(location.Map.DisplayWidth - Game1.viewport.Width / 2, Game1.viewport.Height / 2),
                new Vector2(Game1.viewport.Width / 2, location.Map.DisplayHeight - Game1.viewport.Height / 2),
                new Vector2(location.Map.DisplayWidth - Game1.viewport.Width / 2, location.Map.DisplayHeight - Game1.viewport.Height / 2)
            };
            Func<Vector2, Vector2, float> distance = (v1, v2) => Vector2.Distance(v1, v2);
            float[] distances = new float[corners.Length];
            for (int i = 0; i < corners.Length; i++)
            {
                distances[i] = distance(Game1.player.position.Get(), corners[i]);
            }
            int idx = Array.IndexOf(distances, Math.Max(distances[0], Math.Max(distances[1], Math.Max(distances[2], distances[3]))));
            return corners[idx];
        }
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.Button.IsUseToolButton())
            {
                var item = Game1.player.ActiveItem;
                if (item != null && item.ItemId == "GlitchedDeveloper.TerrariaBosses_SuspiciousLookingEye")
                {
                    GameLocation location = Game1.player.currentLocation;
                    if (
                        !CanSpawnAt(location, config.EyeOfCthulhu.SpawnLocation) ||
                        (Game1.timeOfDay < config.EyeOfCthulhu.SpawnAfter) ||
                        (GetEoC() != null && config.EyeOfCthulhu.OnlyOne) ||
                        !Game1.player.CanMove || Game1.activeClickableMenu != null
                    ) return;
                    Vector2 spawnLocation = GetSpawnLocation();
                    EyeOfCthulhu EoC = new EyeOfCthulhu(spawnLocation);
                    Game1.currentLocation.addCharacter(EoC);
                    item.Stack--;
                    if (item.Stack <= 0)
                    {
                        Game1.player.removeItemFromInventory(item);
                    }
                    Game1.playSound("GlitchedDeveloper-TerrariaBosses-Roar");
                }
            }
        }
        public EyeOfCthulhu? GetEoC()
        {
            foreach (NPC npc in Game1.player.currentLocation.characters)
            {
                if (npc is EyeOfCthulhu && (npc as EyeOfCthulhu).Health > 0)
                {
                    return npc as EyeOfCthulhu;
                }
            }
            return null;
        }

        public bool CanSpawnAt(GameLocation location, SpawnLocationConfig config)
        {
            if (config.Anywhere)
                return true;
            if (location.IsOutdoors && config.AnywhereOutdoors)
                return true;
            if (location.IsFarm && config.Farm)
                return true;
            if (Game1.CurrentMineLevel > 0 && Game1.CurrentMineLevel <= 120 && config.Mines)
                return true;
            if (Game1.CurrentMineLevel > 121 && Game1.CurrentMineLevel != 77377 && config.SkullCavern)
                return true;
            if (Game1.CurrentMineLevel == 77377 && config.QuarryMine)
                return true;
            if (location is BugLand && config.MutantBugLair)
                return true;
            if (location is VolcanoDungeon && config.VolcanoDungeon)
                return true;
            if (location is Woods && config.SecretWoods)
                return true;
            if (location is IslandLocation && config.GingerIsland)
                return true;
            if (location is Desert && config.Desert)
                return true;
            return false;
        }

        string previousSong;
        private void OnOneSecondUpdateTicking(object? sender, OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            GameLocation location = Game1.player.currentLocation;
            EyeOfCthulhu EoC = GetEoC();
            if (
                CanSpawnAt(location, config.DemonEyeSpawning.SpawnLocation) &&
                (Game1.timeOfDay >= config.DemonEyeSpawning.SpawnAfter) &&
                (Game1.player.mailReceived.Contains("hasActivatedForestPylon") || !config.DemonEyeSpawning.OnlyAfterTerrariaEasterEgg) &&
                Game1.player.CanMove && Game1.activeClickableMenu == null &&
                (EoC == null || !config.DemonEyeSpawning.BlockWhenEOCAlive) &&
                Game1.random.NextDouble() < (double)config.DemonEyeSpawning.SpawnChance / 100)
            {
                Monitor.Log("Spawn Demon Eye");
                Vector2 position;
                switch (Game1.random.Next(4))
                {
                    case 0:
                        position = new Vector2(Game1.random.Next(-10, location.Map.DisplayWidth + 10), -100);
                        break;
                    case 1:
                        position = new Vector2(Game1.random.Next(-10, location.Map.DisplayWidth + 10), 100);
                        break;
                    case 2:
                        position = new Vector2(-100, Game1.random.Next(-10, location.Map.DisplayWidth + 10));
                        break;
                    default:
                        position = new Vector2(100, Game1.random.Next(-10, location.Map.DisplayWidth + 10));
                        break;
                }
                DemonEye DemonEye;
                if (Game1.currentSeason == "fall" && config.DemonEyeSpawning.SpawnHalloweenVariants == "During Fall")
                    DemonEye = new DemonEye(position, fallDemonEyeVariants[Game1.random.Next(fallDemonEyeVariants.Length)]);
                else if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 27 && config.DemonEyeSpawning.SpawnHalloweenVariants == "During Spirit's Eve")
                    DemonEye = new DemonEye(position, fallDemonEyeVariants[Game1.random.Next(fallDemonEyeVariants.Length)]);
                else
                    DemonEye = new DemonEye(position, demonEyeVariants[Game1.random.Next(demonEyeVariants.Length)]);
                location.addCharacter(DemonEye);
            }
            if (Game1.currentSong != null)
            {
                if (EoC != null && Game1.currentSong.Name != "GlitchedDeveloper-TerrariaBosses-Music" && Game1.requestedMusicTrack != "GlitchedDeveloper-TerrariaBosses-Music")
                {
                    Monitor.Log("Change Music To Boss");
                    previousSong = Game1.currentSong.Name;
                    Game1.changeMusicTrack("GlitchedDeveloper-TerrariaBosses-Music");
                }
                else if (EoC == null && Game1.currentSong.Name == "GlitchedDeveloper-TerrariaBosses-Music" && Game1.requestedMusicTrack == "GlitchedDeveloper-TerrariaBosses-Music")
                {
                    Monitor.Log("Change Music To Vanilla");
                    Game1.changeMusicTrack(previousSong);
                    previousSong = null;
                }
            }
            else
            {
                if (EoC != null && Game1.requestedMusicTrack != "GlitchedDeveloper-TerrariaBosses-Music")
                {
                    Monitor.Log("Change Music To Boss");
                    previousSong = "none";
                    Game1.changeMusicTrack("GlitchedDeveloper-TerrariaBosses-Music");
                }
            }
        }
        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            EyeOfCthulhu EoC = GetEoC();
            if (EoC != null)
            {
                Texture2D icon;
                if (EoC.Health > EoC.MaxHealth * 0.75)
                    icon = Game1.content.Load<Texture2D>("Mods\\GlitchedDeveloper.TerrariaBosses\\UI\\NPC_Head_Boss_0");
                else
                    icon = Game1.content.Load<Texture2D>("Mods\\GlitchedDeveloper.TerrariaBosses\\UI\\NPC_Head_Boss_1");
                BossBar.DrawFancyBar(Game1.spriteBatch, EoC.Health, EoC.MaxHealth, icon);
            }
        }
        private void OnSaving(object? sender, SavingEventArgs e)
        {
            Monitor.Log("Trying to remove npcs");
            foreach(GameLocation location in  Game1.locations)
            {
                List<NPC> npcsToRemove = new List<NPC>();
                foreach (NPC npc in location.characters)
                {
                    if (npc is EyeOfCthulhu || npc is DemonEye)
                    {
                        Monitor.Log("Removing NPC");
                        npcsToRemove.Add(npc);
                    }
                }
                foreach (NPC npc in npcsToRemove)
                {
                    location.characters.Remove(npc);
                }
            }
        }
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => config = new Config(),
                save: () => this.Helper.WriteConfig(config)
            );
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "General",
                text: () => "General Settings"
            );
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "DemonEye",
                text: () => "Demon Eye Settings"
            );
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "EyeOfCthulhu",
                text: () => "Eye of Cthulhu Settings"
            );

            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "General"
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "General Settings"
             );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Spawn Gore",
                tooltip: () => "Whether to spawn gore when a custom NPC dies",
                getValue: () => config.SpawnGore,
                setValue: value => config.SpawnGore = value
            );

            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "DemonEye"
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Demon Eye Settings"
             );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Spawn Chance",
                tooltip: () => "How likely a demon eye is to spawn each second",
                min: 0,
                max: 100,
                formatValue: value => $"{value}%",
                getValue: () => config.DemonEyeSpawning.SpawnChance,
                setValue: value => config.DemonEyeSpawning.SpawnChance = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Spawn After",
                tooltip: () => "Restricts spawning to after a certain time",
                min: 600,
                max: 2600,
                interval: 100,
                formatValue: value =>
                {
                    int t = value / 100 % 24;
                    if (t == 0)
                        return "12:00am";
                    else if (t < 12)
                        return $"{t}:00am";
                    else if (t == 12)
                        return "12:00pm";
                    else
                        return $"{t % 12}:00pm";
                },
                getValue: () => config.DemonEyeSpawning.SpawnAfter,
                setValue: value => config.DemonEyeSpawning.SpawnAfter = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Only After Terraria Easter Egg",
                tooltip: () => "Restricts spawning to after obtaining Meowmere",
                getValue: () => config.DemonEyeSpawning.OnlyAfterTerrariaEasterEgg,
                setValue: value => config.DemonEyeSpawning.OnlyAfterTerrariaEasterEgg = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Block When EoC Alive",
                tooltip: () => "Prevents spawning when Eye of Cthulhu is alive",
                getValue: () => config.DemonEyeSpawning.BlockWhenEOCAlive,
                setValue: value => config.DemonEyeSpawning.BlockWhenEOCAlive = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Spawn Halloween Variants",
                tooltip: () => "When to allow Halloween variants to spawn",
                getValue: () => config.DemonEyeSpawning.SpawnHalloweenVariants,
                setValue: value => config.DemonEyeSpawning.SpawnHalloweenVariants = value,
                allowedValues: new string[] { "Never", "During Fall", "During Spirit's Eve" }
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Spawn Location"
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Anywhere",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.Anywhere,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.Anywhere = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Anywhere Outdoors",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.AnywhereOutdoors,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.AnywhereOutdoors = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Farm",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.Farm,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.Farm = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mines",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.Mines,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.Mines = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Skull Cavern",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.SkullCavern,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.SkullCavern = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Quarry Mine",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.QuarryMine,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.QuarryMine = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mutant Bug Lair",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.MutantBugLair,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.MutantBugLair = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Volcano Dungeon",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.VolcanoDungeon,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.VolcanoDungeon = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Secret Woods",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.SecretWoods,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.SecretWoods = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Ginger Island",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.GingerIsland,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.GingerIsland = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Desert",
                getValue: () => config.DemonEyeSpawning.SpawnLocation.Desert,
                setValue: value => config.DemonEyeSpawning.SpawnLocation.Desert = value
            );

            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "EyeOfCthulhu"
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Eye of Cthulhu Settings"
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Play Cutscene",
                tooltip: () => "Show spawning cutscene",
                getValue: () => config.EyeOfCthulhu.PlayCutscene,
                setValue: value => config.EyeOfCthulhu.PlayCutscene = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Spawn After",
                tooltip: () => "Restricts spawning to after a certain time",
                min: 600,
                max: 2600,
                interval: 100,
                formatValue: value =>
                {
                    int t = value / 100 % 24;
                    if (t == 0)
                        return "12:00am";
                    else if (t < 12)
                        return $"{t}:00am";
                    else if (t == 12)
                        return "12:00pm";
                    else
                        return $"{t % 12}:00pm";
                },
                getValue: () => config.EyeOfCthulhu.SpawnAfter,
                setValue: value => config.EyeOfCthulhu.SpawnAfter = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Only One",
                tooltip: () => "Restricts max spawned Eye of Cthulhu at a time to 1",
                getValue: () => config.EyeOfCthulhu.OnlyOne,
                setValue: value => config.EyeOfCthulhu.OnlyOne = value
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Spawn Location"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Anywhere",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.Anywhere,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.Anywhere = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Anywhere Outdoors",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.AnywhereOutdoors,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.AnywhereOutdoors = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Farm",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.Farm,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.Farm = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mines",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.Mines,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.Mines = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Skull Cavern",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.SkullCavern,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.SkullCavern = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Quarry Mine",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.QuarryMine,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.QuarryMine = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mutant Bug Lair",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.MutantBugLair,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.MutantBugLair = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Volcano Dungeon",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.VolcanoDungeon,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.VolcanoDungeon = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Secret Woods",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.SecretWoods,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.SecretWoods = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Ginger Island",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.GingerIsland,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.GingerIsland = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Desert",
                getValue: () => config.EyeOfCthulhu.SpawnLocation.Desert,
                setValue: value => config.EyeOfCthulhu.SpawnLocation.Desert = value
            );
        }
    }
}
