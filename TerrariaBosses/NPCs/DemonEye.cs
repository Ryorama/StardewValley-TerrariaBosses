using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Extensions;
using StardewValley.Network;
using StardewModdingAPI;
using System.IO;
using StardewValley.Menus;
using System.Runtime.InteropServices;

namespace TerrariaBosses.NPCs;

public class DemonEye : Monster
{
    public const float rotationIncrement = MathF.PI / 64f;

    private int wasHitCounter;

    private float targetRotation;

    private bool turningRight;

    private readonly NetFarmerRef killer = new NetFarmerRef().Delayed(interpolationWait: false);

    public List<Vector3> segments = new List<Vector3>();

    public NetInt segmentCount = new NetInt(0);

    public string spritePath;

    public int eyeWidth;

    public int eyeHeight;

    public int spriteFrameCount;

    public string hitSoundID;

    public string killSoundID;

    private Dictionary<string, string[]> goreIDs = new Dictionary<string, string[]>()
    {
        { "DemonEye", new string[] { "1", "2" } },
        { "CataractEye", new string[] { "249", "2" } },
        { "SleepyEye", new string[] { "248", "2" } },
        { "DilatedEye", new string[] { "247", "2" } },
        { "GreenEye", new string[] { "252", "253" } },
        { "PurpleEye", new string[] { "250", "251" } },
        { "OwlDemonEye", new string[] { "447", "448" } },
        { "SpaceshipDemonEye", new string[] { "449", "450" } },
        { "ServantOfCthulhu", new string[] { "6", "7" } }
    };

    public DemonEye()
    {
    }

    public DemonEye(Vector2 position)
        : base("DemonEye", position)
    {
        InitializeAttributes();
    }

    public DemonEye(Vector2 position, string name)
        : base(name, position)
    {
        InitializeAttributes();
    }

    public virtual void InitializeAttributes()
    {
        base.Slipperiness = 24 + Game1.random.Next(10);
        Halt();
        base.IsWalkingTowardPlayer = false;
        base.HideShadow = true;
        base.Scale = 1f;
        spriteFrameCount = 2;
        spritePath = $"Mods\\GlitchedDeveloper.TerrariaBosses\\Monsters\\{name}";
        Texture2D texture = Game1.content.Load<Texture2D>(spritePath);
        eyeWidth = texture.Width / spriteFrameCount;
        eyeHeight = texture.Height;
        hitSoundID = "Hit1";
        killSoundID = "Killed1";
        reloadSprite();
    }

    public override void reloadSprite(bool onlyAppearance = false)
    {
        Sprite = new AnimatedSprite(spritePath);
        Sprite.SpriteWidth = eyeWidth;
        Sprite.SpriteHeight = eyeHeight;
        Sprite.SourceRect = new Rectangle(0, 0, eyeWidth, eyeHeight);
        base.HideShadow = true;
    }

    public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
    {
        int num = (int)(damage - (resilience * 0.5));
        base.Health -= num;
        velocity.X = xTrajectory / 3;
        velocity.Y = yTrajectory / 3;
        wasHitCounter = 500;
        base.currentLocation.playSound($"GlitchedDeveloper-TerrariaBosses-Hit1");
        if (base.Health <= 0)
        {
            killer.Value = who;
            deathAnimation();
        }
        return num;
    }

    protected override void sharedDeathAnimation()
    {
    }

    protected override void localDeathAnimation()
    {
        if (killer.Value == null)
        {
            return;
        }
        if (ModEntry.config.SpawnGore)
        {
            foreach (string id in goreIDs[name])
            {
                Rectangle hitbox = GetBoundingBox();
                base.currentLocation.debris.Add(new Gore($"Mods\\GlitchedDeveloper.TerrariaBosses\\Gore\\{id}", new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y + hitbox.Height / 2)));
            }
        }
        base.currentLocation.localSound($"GlitchedDeveloper-TerrariaBosses-{killSoundID}");
    }
    public override List<Item> getExtraDropItems()
    {
        List<Item> list = new List<Item>();
        if (name != "ServantOfCthulhu" && Game1.random.Next(3) == 0)
        {
            list.Add(ItemRegistry.Create("GlitchedDeveloper.TerrariaBosses_Lens"));
        }
        return list;
    }

    public override void drawAboveAllLayers(SpriteBatch b)
    {
        int y = base.StandingPixel.Y;
        Rectangle value = new Rectangle(0, 0, eyeWidth, eyeHeight);
        Vector2 vector2 = base.Position;

        if (Utility.isOnScreen(vector2, 128))
        {
            double r = rotation % (Math.PI * 2);
            if (r < 0) r += Math.PI * 2;
            flip = r > Math.PI;

            Vector2 vector4 = Game1.GlobalToLocal(Game1.viewport, vector2) + drawOffset + new Vector2(0f, yJumpOffset);
            int height = GetBoundingBox().Height;
            b.Draw(Sprite.Texture, vector4 + new Vector2(64f, height / 2), value, Color.White, rotation, new Vector2(eyeWidth / 2, eyeHeight / 2), Math.Max(0.2f, scale.Value) * 4f, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((float)(y + 8) / 10000f)));
            if (isGlowing)
            {
                b.Draw(Sprite.Texture, vector4 + new Vector2(64f, height / 2), value, glowingColor * glowingTransparency, rotation, new Vector2(eyeWidth / 2, eyeHeight / 2), Math.Max(0.2f, scale.Value) * 4f, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((float)(y + 8) / 10000f + 0.0001f)));
            }
            //var rectangle = GetBoundingBox();
            //var graphicsDevice = Game1.game1.GraphicsDevice;
            //Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
            //pixel.SetData(new[] { Color.Red });
            //b.Draw(pixel, Game1.GlobalToLocal(Game1.viewport, new Vector2(rectangle.X, rectangle.Y)), rectangle, Color.Red);
        }
    }

    public override Rectangle GetBoundingBox()
    {
        Vector2 vector = base.Position;
        return new Rectangle((int)vector.X + 45 - (int)(Math.Cos(rotation + 1.57f) * 12), (int)vector.Y - (int)(Math.Sin(rotation + 1.57f) * 12), 40, 40);
    }

    protected override void updateAnimation(GameTime time)
    {
        base.updateAnimation(time);
        if (wasHitCounter >= 0)
        {
            wasHitCounter -= time.ElapsedGameTime.Milliseconds;
        }

        Sprite.Animate(time, 0, spriteFrameCount, 200f);
    }

    public Vector2 velocity;
    public override void behaviorAtGameTick(GameTime time)
    {
        Farmer target = Player;
        int direction;
        int directionY;
        if (target.position.X > position.X)
            direction = 1;
        else
            direction = -1;
        if (target.position.Y > position.Y)
            directionY = 1;
        else
            directionY = -1;

        float maxSpeed = 4f * 2;
        float maxSpeedY = 1.5f * 2;
        if (direction == -1 && velocity.X > 0f - maxSpeed)
        {
            velocity.X -= 0.1f;
            if (velocity.X > maxSpeed)
            {
                velocity.X -= 0.1f;
            }
            else if (velocity.X > 0f)
            {
                velocity.X += 0.05f;
            }
            if (velocity.X < 0f - maxSpeed)
            {
                velocity.X = 0f - maxSpeed;
            }
        }
        else if (direction == 1 && velocity.X < maxSpeed)
        {
            velocity.X += 0.1f;
            if (velocity.X < 0f - maxSpeed)
            {
                velocity.X += 0.1f;
            }
            else if (velocity.X < 0f)
            {
                velocity.X -= 0.05f;
            }
            if (velocity.X > maxSpeed)
            {
                velocity.X = maxSpeed;
            }
        }
        if (directionY == -1 && velocity.Y > 0f - maxSpeedY)
        {
            velocity.Y -= 0.04f;
            if (velocity.Y > maxSpeedY)
            {
                velocity.Y -= 0.05f;
            }
            else if (velocity.Y > 0f)
            {
                velocity.Y += 0.03f;
            }
            if (velocity.Y < 0f - maxSpeedY)
            {
                velocity.Y = 0f - maxSpeedY;
            }
        }
        else if (directionY == 1 && velocity.Y < maxSpeedY)
        {
            velocity.Y += 0.04f;
            if (velocity.Y < 0f - maxSpeedY)
            {
                velocity.Y += 0.05f;
            }
            else if (velocity.Y < 0f)
            {
                velocity.Y -= 0.03f;
            }
            if (velocity.Y > maxSpeedY)
            {
                velocity.Y = maxSpeedY;
            }
        }
        rotation = (float)Math.Atan2(velocity.Y, velocity.X) + 1.57f;
        position.Set(new(position.X + velocity.X, position.Y + velocity.Y));
    }
    public override void updateMovement(GameLocation location, GameTime time)
    {

    }
}