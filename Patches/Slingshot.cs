namespace TerrariaBosses.Patches
{
    internal class Slingshot
    {
        public static bool canThisBeAttached_Prefix(StardewValley.Tools.Slingshot __instance, StardewValley.Object o, int slot, ref bool __result)
        {
            if (o.QualifiedItemId == "(O)GlitchedDeveloper.TerrariaBosses_CrimtaneOre" || o.QualifiedItemId == "(O)GlitchedDeveloper.TerrariaBosses_DemoniteOre")
            {
                __result = true;
                return false;
            }
            return true;
        }
        public static bool GetAmmoDamage_Prefix(StardewValley.Tools.Slingshot __instance, StardewValley.Object ammunition, ref int __result)
        {
            if (ammunition.QualifiedItemId == "(O)GlitchedDeveloper.TerrariaBosses_CrimtaneOre" || ammunition.QualifiedItemId == "(O)GlitchedDeveloper.TerrariaBosses_DemoniteOre")
            {
                __result = 55;
                return false;
            }
            return true;
        }
    }
}
