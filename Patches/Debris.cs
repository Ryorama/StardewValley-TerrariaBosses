using Microsoft.Xna.Framework;
using StardewValley;

namespace TerrariaBosses.Patches
{
    internal class Debris
    {
        public static bool updateChunks_Prefix(StardewValley.Debris __instance, GameTime time, GameLocation location, ref bool __result)
        {
            if (__instance is SlingshotDebris)
            {
                __result = (__instance as SlingshotDebris).updateChunks(time, location);
                return false;
            }
            return true;
        }
    }
}
