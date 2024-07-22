using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using xTile.Tiles;

namespace TerrariaBosses
{
    internal class BossBar
    {
        private static Rectangle Frame(Texture2D tex, int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0, int sizeOffsetX = 0, int sizeOffsetY = 0)
        {
            int num = tex.Width / horizontalFrames;
            int num2 = tex.Height / verticalFrames;
            return new Rectangle(num * frameX, num2 * frameY, num + sizeOffsetX, num2 + sizeOffsetY);
        }
        private static Rectangle CenteredRectangle(Vector2 center, Vector2 size)
        {
            return new Rectangle((int)(center.X - size.X / 2f), (int)(center.Y - size.Y / 2f), (int)size.X, (int)size.Y);
        }
        private static Vector2 Size(Rectangle r)
        {
            return new Vector2(r.Width, r.Height);
        }
        public static Vector2 TopLeft(Rectangle r)
        {
            return new Vector2(r.X, r.Y);
        }
        public static void DrawFancyBar(SpriteBatch spriteBatch, float lifeAmount, float lifeMax, Texture2D barIconTexture)
        {
            DrawFancyBar(spriteBatch, lifeAmount, lifeMax, barIconTexture, Frame(barIconTexture));
        }
        public static Toolbar findToolbar()
        {
            for (int i = 0; i < Game1.onScreenMenus.Count; i++)
            {
                if (Game1.onScreenMenus[i] is Toolbar toolbar)
                {
                    return toolbar;
                }
            }
            return null;
        }
        public static void DrawFancyBar(SpriteBatch spriteBatch, float lifeAmount, float lifeMax, Texture2D barIconTexture, Rectangle barIconFrame)
        {
            Texture2D value = Game1.content.Load<Texture2D>("Mods/GlitchedDeveloper.TerrariaBosses/UI/UI_BossBar");
            Point p = new Point(456, 22);
            Point p2 = new Point(32, 24);
            int verticalFrames = 6;
            Rectangle value2 = Frame(value, 1, verticalFrames, 0, 3);
            Color color = Color.White * 0.2f;
            float num = lifeAmount / lifeMax;
            int num2 = (int)((float)p.X * num);
            num2 -= num2 % 2;
            Rectangle value3 = Frame(value, 1, verticalFrames, 0, 2);
            value3.X += p2.X;
            value3.Y += p2.Y;
            value3.Width = 2;
            value3.Height = p.Y;
            Rectangle value4 = Frame(value, 1, verticalFrames, 0, 1);
            value4.X += p2.X;
            value4.Y += p2.Y;
            value4.Width = 2;
            value4.Height = p.Y;
            Toolbar toolbar = findToolbar();
            int bottomY = Game1.uiViewport.Height;
            if (toolbar != null && toolbar.yPositionOnScreen - toolbar.toolbarTextSource.Height > Game1.uiViewport.Height - 100)
                bottomY = toolbar.yPositionOnScreen - toolbar.toolbarTextSource.Height - 20;
            Rectangle rectangle = CenteredRectangle(new Vector2(Game1.uiViewport.Width, bottomY) * new Vector2(0.5f, 1f) + new Vector2(0f, -50f), p.ToVector2());
            Vector2 vector = TopLeft(rectangle) - p2.ToVector2();
            spriteBatch.Draw(value, vector, value2, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(value, TopLeft(rectangle), value3, Color.White, 0f, Vector2.Zero, new Vector2(num2 / value3.Width, 1f), SpriteEffects.None, 0f);
            spriteBatch.Draw(value, TopLeft(rectangle) + new Vector2(num2 - 2, 0f), value4, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Rectangle value5 = Frame(value, 1, verticalFrames);
            spriteBatch.Draw(value, vector, value5, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Vector2 vector2 = new Vector2(4f, 20f) + new Vector2(26f, 28f) / 2f;
            spriteBatch.Draw(barIconTexture, vector + vector2, barIconFrame, Color.White, 0f, Size(barIconFrame) / 2f, 1f, SpriteEffects.None, 0f);
        }
    }
}
