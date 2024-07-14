using StardewValley;
using StardewValley.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerrariaBosses
{
    internal class Gore : Debris
    {
        public Gore(string sprite, Vector2 debrisOrigin)
            : base()
        {
            InitializeChunks(1, debrisOrigin, Game1.player.Position, 0.6f);
            Texture2D texture = Game1.content.Load<Texture2D>(sprite);
            this.sizeOfSourceRectSquares.Value = Math.Max(texture.Width, texture.Height);
            debrisType.Value = DebrisType.SPRITECHUNKS;
            spriteChunkSheetName.Value = sprite;
            for (int i = 0; i < Chunks.Count; i++)
            {
                Chunk chunk = Chunks[i];
                chunk.xSpriteSheet.Value = 0;
                chunk.ySpriteSheet.Value = 0;
                chunk.rotationVelocity = (Game1.random.NextBool() ? ((float)(Math.PI / (double)Game1.random.Next(-32, -16))) : ((float)(Math.PI / (double)Game1.random.Next(16, 32))));
                chunk.xVelocity.Value *= 1.2f;
                chunk.yVelocity.Value *= 1.2f;
                chunk.scale = 4f;
            }
        }
    }
}
