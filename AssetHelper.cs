using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Tools;

namespace TerrariaBosses
{
    internal class AssetHelper
    {
        private IModHelper Helper;
        public AssetHelper(IModHelper helper) {
            Helper = helper;
        }
        private Dictionary<string, string> assets = new Dictionary<string, string>();
        public void AddAssets(string category)
        {
            foreach (string path in Directory.EnumerateFiles(Path.Combine(this.Helper.DirectoryPath, $"assets\\{category}"), "*.png"))
            {
                assets[$"Mods/GlitchedDeveloper.TerrariaBosses/{category}/{Path.GetFileNameWithoutExtension(path)}"] = path;
            }
        }
        public void AddSound(string name, string path)
        {
            CueDefinition cue = new CueDefinition();
            cue.name = $"GlitchedDeveloper-TerrariaBosses-{name}";
            cue.instanceLimit = 1;
            cue.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;
            SoundEffect audio;
            string filePathCombined = Path.Combine(this.Helper.DirectoryPath, $"assets\\Sounds\\{path}");
            using (var stream = new FileStream(filePathCombined, FileMode.Open))
            {
                audio = SoundEffect.FromStream(stream);
            }
            cue.SetSound(audio, Game1.audioEngine.GetCategoryIndex("Sound"), false);
            Game1.soundBank.AddCue(cue);
        }
        public void AddMusic(string name, string path)
        {
            CueDefinition cue = new CueDefinition();
            cue.name = $"GlitchedDeveloper-TerrariaBosses-{name}";
            cue.instanceLimit = 1;
            cue.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;
            SoundEffect audio;
            string filePathCombined = Path.Combine(this.Helper.DirectoryPath, $"assets\\Sounds\\{path}");
            using (var stream = new FileStream(filePathCombined, FileMode.Open))
            {
                audio = SoundEffect.FromStream(stream);
            }
            cue.SetSound(audio, Game1.audioEngine.GetCategoryIndex("Music"), true);
            Game1.soundBank.AddCue(cue);
        }
        private MachineOutputRule GenerateFurnaceRecipe(string ruleID, string inputID, string outputID)
        {
            MachineOutputRule rule = new MachineOutputRule();
            rule.Id = ruleID;
            rule.UseFirstValidOutput = false;
            rule.MinutesUntilReady = 30;
            rule.DaysUntilReady = -1;
            rule.RecalculateOnCollect = false;

            MachineOutputTriggerRule trigger = new MachineOutputTriggerRule();
            trigger.Id = "ItemPlacedInMachine";
            trigger.Trigger = MachineOutputTrigger.ItemPlacedInMachine;
            trigger.RequiredItemId = inputID;
            trigger.RequiredCount = 5;
            rule.Triggers = new List<MachineOutputTriggerRule>() { trigger };

            MachineItemOutput output = new MachineItemOutput();
            output.CopyColor = false;
            output.CopyPrice = false;
            output.CopyQuality = false;
            output.IncrementMachineParentSheetIndex = 0;
            output.Id = outputID;
            output.ItemId = outputID;
            output.PriceModifierMode = QuantityModifier.QuantityModifierMode.Stack;
            rule.OutputItem = new List<MachineItemOutput>() { output };

            return rule;
        }
        public void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, MachineData>().Data;
                    data["(BC)13"].OutputRules.Add(GenerateFurnaceRecipe("Default_CrimtaneOre", "(O)GlitchedDeveloper.TerrariaBosses_CrimtaneOre", "(O)GlitchedDeveloper.TerrariaBosses_CrimtaneBar"));
                    data["(BC)13"].OutputRules.Add(GenerateFurnaceRecipe("Default_DemoniteOre", "(O)GlitchedDeveloper.TerrariaBosses_DemoniteOre", "(O)GlitchedDeveloper.TerrariaBosses_DemoniteBar"));
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Monsters"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["Demon Eye"] = "60/18/0/0/true/1000//2/.01/99999/2/.0/false/20/DemonEye";
                    data["Cataract Eye"] = "65/18/0/0/true/1000//4/.01/99999/2/.0/false/20/DemonEye";
                    data["Sleepy Eye"] = "60/16/0/0/true/1000//2/.01/99999/2/.0/false/20/DemonEye";
                    data["Dilated Eye"] = "50/18/0/0/true/1000//2/.01/99999/2/.0/false/20/DemonEye";
                    data["Green Eye"] = "60/20/0/0/true/1000//0/.01/99999/2/.0/false/20/DemonEye";
                    data["Purple Eye"] = "60/14/0/0/true/1000//4/.01/99999/2/.0/false/20/DemonEye";
                    data["Owl Demon Eye"] = "75/16/0/0/true/1000//6/.01/99999/2/.0/false/20/DemonEye";
                    data["Spaceship Demon Eye"] = "60/20/0/0/true/1000//4/.01/99999/2/.0/false/20/DemonEye";
                    data["Eye of Cthulhu"] = "2800/15/0/0/true/1000//12/.01/99999/2/.0/false/20/EyeOfCthulhu";
                    data["Servant of Cthulhu"] = "8/12/0/0/true/1000//0/.01/99999/2/.0/false/20/DemonEye";
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    data["GlitchedDeveloper.TerrariaBosses_Lens"] = new ObjectData()
                    {
                        Name = "GlitchedDeveloper.TerrariaBosses_Lens",
                        DisplayName = "Lens",
                        Description = "Material",
                        Type = "Basic",
                        Category = -28,
                        Price = 100,
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Objects",
                        SpriteIndex = 0
                    };

                    data["GlitchedDeveloper.TerrariaBosses_SuspiciousLookingEye"] = new ObjectData()
                    {
                        Name = "GlitchedDeveloper.TerrariaBosses_SuspiciousLookingEye",
                        DisplayName = "Suspicious Looking Eye",
                        Description = "Summons the Eye of Cthulhu",
                        Type = "Crafting",
                        Category = 0,
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Objects",
                        SpriteIndex = 1,
                        ContextTags = new List<string> { "not_placeable" }
                    };

                    data["GlitchedDeveloper.TerrariaBosses_CrimtaneOre"] = new ObjectData()
                    {
                        Name = "GlitchedDeveloper.TerrariaBosses_CrimtaneOre",
                        DisplayName = "Crimtane Ore",
                        Description = "Can be smelted into bars.",
                        Type = "Basic",
                        Category = -15,
                        Price = 1300,
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Objects",
                        SpriteIndex = 2,
                        ContextTags = new List<string> { "ore_item" }
                    };

                    data["GlitchedDeveloper.TerrariaBosses_CrimtaneBar"] = new ObjectData()
                    {
                        Name = "GlitchedDeveloper.TerrariaBosses_CrimtaneBar",
                        DisplayName = "Crimtane Bar",
                        Description = "A bar of pure crimtane.",
                        Type = "Basic",
                        Category = -15,
                        Price = 3900,
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Objects",
                        SpriteIndex = 3,
                        ContextTags = new List<string> { "furnace_item" }
                    };

                    data["GlitchedDeveloper.TerrariaBosses_DemoniteOre"] = new ObjectData()
                    {
                        Name = "GlitchedDeveloper.TerrariaBosses_DemoniteOre",
                        DisplayName = "Demonite Ore",
                        Description = "Can be smelted into bars.",
                        Type = "Basic",
                        Category = -15,
                        Price = 1000,
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Objects",
                        SpriteIndex = 4,
                        ContextTags = new List<string> { "ore_item" }
                    };

                    data["GlitchedDeveloper.TerrariaBosses_DemoniteBar"] = new ObjectData()
                    {
                        Name = "GlitchedDeveloper.TerrariaBosses_DemoniteBar",
                        DisplayName = "Demonite Bar",
                        Description = "A bar of pure demonite.",
                        Type = "Basic",
                        Category = -15,
                        Price = 3000,
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Objects",
                        SpriteIndex = 5,
                        ContextTags = new List<string> { "furnace_item" }
                    };
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["Suspicious Looking Eye"] = "GlitchedDeveloper.TerrariaBosses_Lens 6/Home/GlitchedDeveloper.TerrariaBosses_SuspiciousLookingEye/false/default/";
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Tools"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ToolData>().Data;
                    data["GlitchedDeveloper.TerrariaBosses_CrimtaneHoe"] = new ToolData()
                    {
                        ClassName = "Hoe",
                        Name = "GlitchedDeveloper.TerrariaBosses_CrimtaneHoe",
                        SalePrice = 30000,
                        DisplayName = "Crimtane Hoe",
                        Description = "[LocalizedText Strings\\StringsFromCSFiles:Hoe.cs.14102]",
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Crimtane Tools",
                        SpriteIndex = 0,
                        MenuSpriteIndex = 17,
                        UpgradeLevel = 5,
                        ApplyUpgradeLevelToDisplayName = true,
                        UpgradeFrom = new List<ToolUpgradeData>
                        {
                            new ToolUpgradeData()
                            {
                                RequireToolId = "(T)IridiumHoe",
                                TradeItemId = "(O)GlitchedDeveloper.TerrariaBosses_CrimtaneBar",
                                TradeItemAmount = 5
                            }
                        },
                        CanBeLostOnDeath = false
                    };
                    data["GlitchedDeveloper.TerrariaBosses_CrimtanePickaxe"] = new ToolData()
                    {
                        ClassName = "Pickaxe",
                        Name = "GlitchedDeveloper.TerrariaBosses_CrimtanePickaxe",
                        SalePrice = 30000,
                        DisplayName = "Crimtane Pickaxe",
                        Description = "[LocalizedText Strings\\StringsFromCSFiles:Pickaxe.cs.14185]",
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Crimtane Tools",
                        SpriteIndex = 6,
                        MenuSpriteIndex = 23,
                        UpgradeLevel = 5,
                        ApplyUpgradeLevelToDisplayName = true,
                        UpgradeFrom = new List<ToolUpgradeData>
                        {
                            new ToolUpgradeData()
                            {
                                RequireToolId = "(T)IridiumPickaxe",
                                TradeItemId = "(O)GlitchedDeveloper.TerrariaBosses_CrimtaneBar",
                                TradeItemAmount = 5
                            }
                        },
                        CanBeLostOnDeath = false
                    };
                    data["GlitchedDeveloper.TerrariaBosses_CrimtaneAxe"] = new ToolData()
                    {
                        ClassName = "Axe",
                        Name = "GlitchedDeveloper.TerrariaBosses_CrimtaneAxe",
                        SalePrice = 30000,
                        DisplayName = "Crimtane Axe",
                        Description = "[LocalizedText Strings\\StringsFromCSFiles:Axe.cs.14019]",
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Crimtane Tools",
                        SpriteIndex = 24,
                        MenuSpriteIndex = 41,
                        UpgradeLevel = 5,
                        ApplyUpgradeLevelToDisplayName = true,
                        UpgradeFrom = new List<ToolUpgradeData>
                        {
                            new ToolUpgradeData()
                            {
                                RequireToolId = "(T)IridiumAxe",
                                TradeItemId = "(O)GlitchedDeveloper.TerrariaBosses_CrimtaneBar",
                                TradeItemAmount = 5
                            }
                        },
                        CanBeLostOnDeath = false
                    };
                    data["GlitchedDeveloper.TerrariaBosses_CrimtaneWateringCan"] = new ToolData()
                    {
                        ClassName = "WateringCan",
                        Name = "GlitchedDeveloper.TerrariaBosses_CrimtaneWateringCan",
                        SalePrice = 30000,
                        DisplayName = "Crimtane Watering Can",
                        Description = "[LocalizedText Strings\\StringsFromCSFiles:WateringCan.cs.14325]",
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Crimtane Tools",
                        SpriteIndex = 30,
                        MenuSpriteIndex = 44,
                        UpgradeLevel = 5,
                        ApplyUpgradeLevelToDisplayName = true,
                        UpgradeFrom = new List<ToolUpgradeData>
                        {
                            new ToolUpgradeData()
                            {
                                RequireToolId = "(T)IridiumWateringCan",
                                TradeItemId = "(O)GlitchedDeveloper.TerrariaBosses_CrimtaneBar",
                                TradeItemAmount = 5
                            }
                        },
                        CanBeLostOnDeath = false
                    };
                    data["GlitchedDeveloper.TerrariaBosses_DemoniteHoe"] = new ToolData()
                    {
                        ClassName = "Hoe",
                        Name = "GlitchedDeveloper.TerrariaBosses_DemoniteHoe",
                        SalePrice = 30000,
                        DisplayName = "Demonite Hoe",
                        Description = "[LocalizedText Strings\\StringsFromCSFiles:Hoe.cs.14102]",
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Demonite Tools",
                        SpriteIndex = 0,
                        MenuSpriteIndex = 17,
                        UpgradeLevel = 5,
                        ApplyUpgradeLevelToDisplayName = true,
                        UpgradeFrom = new List<ToolUpgradeData>
                        {
                            new ToolUpgradeData()
                            {
                                RequireToolId = "(T)IridiumHoe",
                                TradeItemId = "(O)GlitchedDeveloper.TerrariaBosses_DemoniteBar",
                                TradeItemAmount = 5
                            }
                        },
                        CanBeLostOnDeath = false
                    };
                    data["GlitchedDeveloper.TerrariaBosses_DemonitePickaxe"] = new ToolData()
                    {
                        ClassName = "Pickaxe",
                        Name = "GlitchedDeveloper.TerrariaBosses_DemonitePickaxe",
                        SalePrice = 30000,
                        DisplayName = "Demonite Pickaxe",
                        Description = "[LocalizedText Strings\\StringsFromCSFiles:Pickaxe.cs.14185]",
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Demonite Tools",
                        SpriteIndex = 6,
                        MenuSpriteIndex = 23,
                        UpgradeLevel = 5,
                        ApplyUpgradeLevelToDisplayName = true,
                        UpgradeFrom = new List<ToolUpgradeData>
                        {
                            new ToolUpgradeData()
                            {
                                RequireToolId = "(T)IridiumPickaxe",
                                TradeItemId = "(O)GlitchedDeveloper.TerrariaBosses_DemoniteBar",
                                TradeItemAmount = 5
                            }
                        },
                        CanBeLostOnDeath = false
                    };
                    data["GlitchedDeveloper.TerrariaBosses_DemoniteAxe"] = new ToolData()
                    {
                        ClassName = "Axe",
                        Name = "GlitchedDeveloper.TerrariaBosses_DemoniteAxe",
                        SalePrice = 30000,
                        DisplayName = "Demonite Axe",
                        Description = "[LocalizedText Strings\\StringsFromCSFiles:Axe.cs.14019]",
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Demonite Tools",
                        SpriteIndex = 24,
                        MenuSpriteIndex = 41,
                        UpgradeLevel = 5,
                        ApplyUpgradeLevelToDisplayName = true,
                        UpgradeFrom = new List<ToolUpgradeData>
                        {
                            new ToolUpgradeData()
                            {
                                RequireToolId = "(T)IridiumAxe",
                                TradeItemId = "(O)GlitchedDeveloper.TerrariaBosses_DemoniteBar",
                                TradeItemAmount = 5
                            }
                        },
                        CanBeLostOnDeath = false
                    };
                    data["GlitchedDeveloper.TerrariaBosses_DemoniteWateringCan"] = new ToolData()
                    {
                        ClassName = "WateringCan",
                        Name = "GlitchedDeveloper.TerrariaBosses_DemoniteWateringCan",
                        SalePrice = 30000,
                        DisplayName = "Demonite Watering Can",
                        Description = "[LocalizedText Strings\\StringsFromCSFiles:WateringCan.cs.14325]",
                        Texture = "Mods/GlitchedDeveloper.TerrariaBosses/Items/Demonite Tools",
                        SpriteIndex = 30,
                        MenuSpriteIndex = 44,
                        UpgradeLevel = 5,
                        ApplyUpgradeLevelToDisplayName = true,
                        UpgradeFrom = new List<ToolUpgradeData>
                        {
                            new ToolUpgradeData()
                            {
                                RequireToolId = "(T)IridiumWateringCan",
                                TradeItemId = "(O)GlitchedDeveloper.TerrariaBosses_DemoniteBar",
                                TradeItemAmount = 5
                            }
                        },
                        CanBeLostOnDeath = false
                    };
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Furniture"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["GlitchedDeveloper.TerrariaBosses_EyeOfCthulhuTrophy"] = "Eye of Cthulhu Trophy/painting/2 2/2 2/1/10000/-1/Eye of Cthulhu Trophy/0/Mods\\GlitchedDeveloper.TerrariaBosses\\Items\\Trophies/true";
                });
            }
            else
            {
                foreach (var key in assets.Keys)
                {
                    if (e.Name.IsEquivalentTo(key))
                    {
                        e.LoadFromModFile<Texture2D>(assets[key], AssetLoadPriority.Medium);
                        break;
                    }
                }
            }
        }
    }
}
