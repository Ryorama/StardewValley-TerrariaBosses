using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;

namespace TerrariaBosses.Patches
{
    internal class BasicProjectile
    {
        private static void createRadialDebris(GameLocation location, string texture, Rectangle sourcerectangle, int xTile, int yTile, int numberOfChunks)
        {
            Vector2 debrisOrigin = new Vector2(xTile * 64 + 64, yTile * 64 + 64);
            location.debris.Add(new SlingshotDebris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(-64f, 0f)));
            numberOfChunks++;
            location.debris.Add(new SlingshotDebris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(64f, 0f)));
            numberOfChunks++;
            location.debris.Add(new SlingshotDebris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0f, -64f)));
            numberOfChunks++;
            location.debris.Add(new SlingshotDebris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0f, 64f)));
        }
        public static bool explosionAnimation_Prefix(StardewValley.Projectiles.BasicProjectile __instance, GameLocation location)
        {
            if (__instance.itemId.Value == "(O)GlitchedDeveloper.TerrariaBosses_CrimtaneOre")
            {
                createRadialDebris(location, "Mods\\GlitchedDeveloper.TerrariaBosses\\Debris\\Crimtane", new Rectangle(0, 0, 32, 16), (int)(__instance.position.X + 32f) / 64, (int)(__instance.position.Y + 32f) / 64, 6);
                if (!string.IsNullOrEmpty(__instance.collisionSound.Value))
                {
                    location.playSound(__instance.collisionSound);
                }
                if ((bool)__instance.explode)
                {
                    Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, __instance.position.Value, flicker: false, Game1.random.NextBool()));
                }
                __instance.collisionBehavior?.Invoke(location, __instance.getBoundingBox().Center.X, __instance.getBoundingBox().Center.Y, __instance.GetPlayerWhoFiredMe(location));
                __instance.destroyMe = true;
                return false;
            }
            else if (__instance.itemId.Value == "(O)GlitchedDeveloper.TerrariaBosses_DemoniteOre")
            {
                createRadialDebris(location, "Mods\\GlitchedDeveloper.TerrariaBosses\\Debris\\Demonite", new Rectangle(0, 0, 32, 16), (int)(__instance.position.X + 32f) / 64, (int)(__instance.position.Y + 32f) / 64, 6);
                if (!string.IsNullOrEmpty(__instance.collisionSound.Value))
                {
                    location.playSound(__instance.collisionSound);
                }
                if ((bool)__instance.explode)
                {
                    Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(362, Game1.random.Next(30, 90), 6, 1, __instance.position.Value, flicker: false, Game1.random.NextBool()));
                }
                __instance.collisionBehavior?.Invoke(location, __instance.getBoundingBox().Center.X, __instance.getBoundingBox().Center.Y, __instance.GetPlayerWhoFiredMe(location));
                __instance.destroyMe = true;
                return false;
            }
            return true;
        }
    }
}
