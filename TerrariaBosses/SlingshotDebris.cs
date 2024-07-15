using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;

namespace TerrariaBosses.Patches
{
    public class SlingshotDebris : StardewValley.Debris
    {
        public SlingshotDebris(string spriteSheet, Rectangle sourceRect, int numberOfChunks, Vector2 debrisOrigin, Vector2 playerPosition)
            : base()
        {
            chunkType.Value = 10;
            InitializeChunks(numberOfChunks, debrisOrigin, playerPosition);
            chunkType.Value = -1;
            this.sizeOfSourceRectSquares.Value = 16;
            debrisType.Value = DebrisType.SPRITECHUNKS;
            spriteChunkSheetName.Value = spriteSheet;
            for (int i = 0; i < Chunks.Count; i++)
            {
                Chunk chunk = Chunks[i];
                chunk.xSpriteSheet.Value = Game1.random.Next(2) * 16 + sourceRect.X;
                chunk.ySpriteSheet.Value = sourceRect.Y;
                chunk.scale = 4f;
            }
        }
        public bool updateChunks(GameTime time, GameLocation location)
        {
            if (Chunks.Count == 0)
            {
                return true;
            }
            timeSinceDoneBouncing += time.ElapsedGameTime.Milliseconds;
            if (timeSinceDoneBouncing >= 600f)
            {
                return true;
            }
            if (!location.farmers.Any() && !location.IsTemporary)
            {
                return false;
            }
            Farmer farmer = player.Value;
            bool anyCouldMove = false;
            for (int i = Chunks.Count - 1; i >= 0; i--)
            {
                Chunk chunk = Chunks[i];
                chunk.position.UpdateExtrapolation(chunk.getSpeed());
                if (chunk.position.X < -128f || chunk.position.Y < -64f || chunk.position.X >= (float)(location.map.DisplayWidth + 64) || chunk.position.Y >= (float)(location.map.DisplayHeight + 64))
                {
                    Chunks.RemoveAt(i);
                }
                else
                {
                    bool canMoveTowardPlayer = farmer != null;
                    if (canMoveTowardPlayer)
                    {
                        canMoveTowardPlayer = true;
                        anyCouldMove = anyCouldMove || canMoveTowardPlayer;
                        if (canMoveTowardPlayer && shouldControlThis(location))
                        {
                            player.Value = farmer;
                        }
                    }
                    if (isFishable && canMoveTowardPlayer && player.Value != null)
                    {
                        if (player.Value.IsLocalPlayer)
                        {
                            if (chunk.position.X < player.Value.Position.X - 12f)
                            {
                                chunk.xVelocity.Value = Math.Min(chunk.xVelocity.Value + 0.8f, 8f);
                            }
                            else if (chunk.position.X > player.Value.Position.X + 12f)
                            {
                                chunk.xVelocity.Value = Math.Max(chunk.xVelocity.Value - 0.8f, -8f);
                            }
                            int playerStandingY = player.Value.StandingPixel.Y;
                            if (chunk.position.Y + 32f < (float)(playerStandingY - 12))
                            {
                                chunk.yVelocity.Value = Math.Max(chunk.yVelocity.Value - 0.8f, -8f);
                            }
                            else if (chunk.position.Y + 32f > (float)(playerStandingY + 12))
                            {
                                chunk.yVelocity.Value = Math.Min(chunk.yVelocity.Value + 0.8f, 8f);
                            }
                            chunk.position.X += chunk.xVelocity.Value;
                            chunk.position.Y -= chunk.yVelocity.Value;
                            Point playerPixel = player.Value.StandingPixel;
                            if (Math.Abs(chunk.position.X + 32f - (float)playerPixel.X) <= 64f && Math.Abs(chunk.position.Y + 32f - (float)playerPixel.Y) <= 64f)
                            {
                                Item old = item;
                                if (collect(player.Value, chunk))
                                {
                                    if (Game1.debrisSoundInterval <= 0f)
                                    {
                                        Game1.debrisSoundInterval = 10f;
                                        if ((old == null || old.QualifiedItemId != "(O)73") && itemId != "(O)73")
                                        {
                                            location.localSound("coin");
                                        }
                                    }
                                    Chunks.RemoveAt(i);
                                }
                            }
                        }
                    }
                    else
                    {
                        chunk.position.X += chunk.xVelocity.Value;
                        chunk.position.Y -= chunk.yVelocity.Value;
                        if (movingFinalYLevel)
                        {
                            chunkFinalYLevel -= (int)Math.Ceiling(chunk.yVelocity.Value / 2f);
                            if (chunkFinalYLevel <= chunkFinalYTarget)
                            {
                                chunkFinalYLevel = chunkFinalYTarget;
                                movingFinalYLevel = false;
                            }
                        }
                        if (chunk.bounces <= 2)
                        {
                            chunk.yVelocity.Value -= 0.4f;
                        }
                        bool destroyThisChunk = false;
                        if (chunk.position.Y >= (float)chunkFinalYLevel && (bool)chunk.hasPassedRestingLineOnce && chunk.bounces <= 2)
                        {
                            Point tile_point = new Point((int)chunk.position.X / 64, chunkFinalYLevel / 64);
                            if (Game1.currentLocation is IslandNorth && Game1.currentLocation.isTileOnMap(tile_point.X, tile_point.Y) && Game1.currentLocation.getTileIndexAt(tile_point, "Back") == -1)
                            {
                                chunkFinalYLevel += 48;
                            }
                            if (shouldControlThis(location))
                            {
                                location.playSound("shiny4");
                            }
                            chunk.bounces++;
                            chunk.yVelocity.Value = Math.Abs(chunk.yVelocity.Value * 2f / 3f);
                            chunk.rotationVelocity = (Game1.random.NextBool() ? (chunk.rotationVelocity / 2f) : ((0f - chunk.rotationVelocity) * 2f / 3f));
                            chunk.xVelocity.Value -= chunk.xVelocity.Value / 2f;
                            Vector2 chunkTile = new Vector2((int)((chunk.position.X + 32f) / 64f), (int)((chunk.position.Y + 32f) / 64f));
                            if (location.doesTileSinkDebris((int)chunkTile.X, (int)chunkTile.Y, debrisType.Value))
                            {
                                destroyThisChunk = location.sinkDebris(this, chunkTile, chunk.position.Value);
                            }
                        }
                        int tile_x = (int)((chunk.position.X + 32f) / 64f);
                        int tile_y = (int)((chunk.position.Y + 32f) / 64f);
                        if ((!chunk.hitWall && location.Map.RequireLayer("Buildings").Tiles[tile_x, tile_y] != null && location.doesTileHaveProperty(tile_x, tile_y, "Passable", "Buildings") == null) || location.Map.RequireLayer("Back").Tiles[tile_x, tile_y] == null)
                        {
                            chunk.xVelocity.Value = 0f - chunk.xVelocity.Value;
                            chunk.hitWall = true;
                        }
                        if (chunk.position.Y < (float)chunkFinalYLevel)
                        {
                            chunk.hasPassedRestingLineOnce.Value = true;
                        }
                        if (chunk.bounces > 2)
                        {
                            chunk.yVelocity.Value = 0f;
                            chunk.xVelocity.Value = 0f;
                            chunk.rotationVelocity = 0f;
                        }
                        chunk.rotation += chunk.rotationVelocity;
                        if (destroyThisChunk)
                        {
                            Chunks.RemoveAt(i);
                        }
                    }
                }
            }
            if (!anyCouldMove && shouldControlThis(location))
            {
                player.Value = null;
            }
            if (Chunks.Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}