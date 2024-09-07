using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;

namespace TerrariaBosses.NPCs;

public class Slime : ITerrariaEntity
{
    public const float rotationIncrement = MathF.PI / 64f;

    public string spritePath;

    public int spriteFrameCount;

    public string hitSoundID;

    public string killSoundID;

    public Texture2D texture;

    public int spriteWidth;

    public int spriteHeight;

    [XmlElement("slimeColor")]
    public Color slimeColor = new Color(0, 0, 0);

    public Slime()
    {
        InitializeAttributes();
    }

    public Slime(string name) : base(name)
    {
        InitializeAttributes();
    }

    public Slime(Vector2 position) : base("Slime", position)
    {
        InitializeAttributes();
    }

    public Slime(Vector2 position, string name) : base(name, position)
    {
        InitializeAttributes();
    }

    public virtual void InitializeAttributes()
    {
        Slipperiness = 24 + Game1.random.Next(10);
        Halt();
        IsWalkingTowardPlayer = false;
        HideShadow = true;
        spriteFrameCount = 2;
        spritePath = $"Mods/GlitchedDeveloper.TerrariaBosses/Monsters/Slime";
        texture = Game1.content.Load<Texture2D>(spritePath);
        spriteWidth = texture.Width / 2;
        spriteHeight = texture.Height;
        frame.Width = spriteWidth;
        frame.Height = spriteHeight;
        hitSoundID = "Hit 1";
        killSoundID = "Killed 1";
        switch (name.Value)
        {
            case "Slime":
                base.maxHealth.Value = 14;
                damageToFarmer.Value = GetAttackDamage_ScaledByStrength(damageToFarmer);
                base.resilience.Value = 0;
                Scale = 1f;
                break;
            case "Green Slime":
                base.maxHealth.Value = 14;
                damageToFarmer.Value = GetAttackDamage_ScaledByStrength(6);
                base.resilience.Value = 0;
                Scale = 1f;
                slimeColor = new Color(80, 255, 100);
                break;
            case "Blue Slime":
                base.maxHealth.Value = 25;
                damageToFarmer.Value = GetAttackDamage_ScaledByStrength(7);
                base.resilience.Value = 2;
                Scale = 1.5f;
                slimeColor = new Color(80, 197, 255);
                break;
        }
        reloadSprite();
    }
    public int GetAttackDamage_ScaledByStrength(float normalDamage)
    {
        float damageMultiplier = 1f;
        switch (ModEntry.config.Difficulty)
        {
            case "Expert":
                damageMultiplier = 2f;
                break;
            case "Master":
                damageMultiplier = 3f;
                break;
            case "Legendary":
                damageMultiplier = 3f;
                break;
        }
        return (int)(normalDamage * damageMultiplier);
    }

    public override void reloadSprite(bool onlyAppearance = false)
    {
        Sprite = new AnimatedSprite(spritePath);
        Sprite.SpriteWidth = spriteWidth;
        Sprite.SpriteHeight = spriteHeight;
        Sprite.SourceRect = new Rectangle(0, 0, spriteWidth, spriteHeight);
        base.HideShadow = true;
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
        //if (ModEntry.config.SpawnGore)
        //{
        //    foreach (string id in goreIDs[name])
        //    {
        //        Rectangle hitbox = GetBoundingBox();
        //        base.currentLocation.debris.Add(new Gore($"Mods/GlitchedDeveloper.TerrariaBosses/Gore/{id}", new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y + hitbox.Height / 2)));
        //    }
        //}
        currentLocation.localSound($"GlitchedDeveloper.TerrariaBosses_{killSoundID}");
    }
    public override List<Item> getExtraDropItems()
    {
        List<Item> list = new List<Item>()
        {
            ItemRegistry.Create("GlitchedDeveloper.TerrariaBosses_Gel"),
            ItemRegistry.Create("GlitchedDeveloper.TerrariaBosses_CopperCoin", 25)
        };
        return list;
    }

    public override void draw(SpriteBatch b)
    {
        if (Sprite.SpriteWidth != spriteWidth || Sprite.SpriteHeight != spriteHeight)
        {
            reloadSprite();
            Sprite.UpdateSourceRect();
        }
        int y = StandingPixel.Y;
        Vector2 vector2 = Position;

        if (Utility.isOnScreen(vector2, 128))
        {
            flip = dirX < 0;
            Vector2 vector4 = Game1.GlobalToLocal(Game1.viewport, vector2) + drawOffset + new Vector2(0f, yJumpOffset);
            int height = GetBoundingBox().Height;
            b.Draw(texture, vector4 + new Vector2(64f, height / 2), Sprite.sourceRect, slimeColor, 0f, new Vector2(spriteWidth / 2, spriteHeight / 2), Math.Max(0.2f, scale.Value) * 4f, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((float)(y + 8) / 10000f)));
        }
    }

    public override Rectangle GetBoundingBox()
    {
        Vector2 vector = Position;
        return new Rectangle((int)vector.X + 39, (int)(vector.Y - jumpPos) + 3, 50, 35);
    }

    protected override void updateAnimation(GameTime time)
    {
        base.updateAnimation(time);
        if (wasHitCounter >= 0)
        {
            wasHitCounter -= time.ElapsedGameTime.Milliseconds;
        }

        int num = spriteWidth;
        int num2 = 0;
        if (aiAction == 0)
        {
            num2 = jumpVelocity < 0f ? 2 : jumpVelocity > 0f ? 3 : velocity.X != 0f || velocity.Y != 0f ? 1 : 0;
        }
        else if (aiAction == 1)
        {
            num2 = 4;
        }
        frameCounter += 1.0;
        if (num2 > 0)
        {
            frameCounter += 1.0;
        }
        if (num2 == 4)
        {
            frameCounter += 1.0;
        }
        if (frameCounter >= 8.0)
        {
            frame.X += num;
            frameCounter = 0.0;
        }
        if (frame.X >= num * spriteFrameCount)
        {
            frame.X = 0;
        }
    }

    public void TargetClosest()
    {
        target = Game1.player;
        float distX = position.X + spriteWidth / 2 - target.position.X;
        float distY = position.Y + spriteHeight / 2 - target.position.Y;
        float dir = (float)Math.Atan2(distY, distX) + 4.71f;
        dirX = (float)Math.Sin(dir);
        dirY = -(float)Math.Cos(dir);
    }

    public float currentAttack = 0f;
    public float ticks = 0f;
    public int aiAction = 0;
    public Rectangle frame = new Rectangle();
    public double frameCounter;
    public Farmer target;
    public float dirX = 1f;
    public float dirY = 1f;
    public float previousX = 0f;
    public float previousY = 0f;
    public float jumpPos = 0f;
    public float jumpVelocity = 0f;
    public float oldJumpVelocity = 0f;
    public override void behaviorAtGameTick(GameTime time)
    {
        if (currentAttack == -999f)
        {
            frame.Y = 0;
            frameCounter = 0.0;
            rotation = 0f;
            return;
        }
        bool isAggressive = true;
        if (Game1.timeOfDay > 1800 || Health != MaxHealth)
        {
            isAggressive = true;
        }
        if (ticks > 1f)
        {
            ticks -= 1f;
        }
        aiAction = 0;
        if (ticks == 0f)
        {
            currentAttack = -100f;
            ticks = 1f;
            TargetClosest();
        }
        if (jumpVelocity == 0f)
        {
            if (previousX == position.X)
            {
                dirX *= -1;
                ticks = 200f;
            }
            previousX = 0f;
            if (previousY == position.Y)
            {
                dirY *= -1;
                ticks = 200f;
            }
            previousY = 0f;
            if (jumpPos == 0f && oldJumpVelocity != 0f && jumpVelocity == 0f)
            {
                position.X -= velocity.X + dirX;
                position.Y -= velocity.Y + dirY;
                oldJumpVelocity = jumpVelocity;
            }
            velocity.X *= 0.8f;
            if (velocity.X > -0.1 && velocity.X < 0.1)
            {
                velocity.X = 0f;
            }
            velocity.Y *= 0.8f;
            if (velocity.Y > -0.1 && velocity.Y < 0.1)
            {
                velocity.Y = 0f;
            }
            if (isAggressive)
            {
                currentAttack += 1f;
            }
            currentAttack += 1f;
            float num33 = -1000f;
            int num34 = 0;
            if (currentAttack >= 0f)
            {
                num34 = 1;
            }
            if (currentAttack >= num33 && currentAttack <= num33 * 0.5f)
            {
                num34 = 2;
            }
            if (currentAttack >= num33 * 2f && currentAttack <= num33 * 1.5f)
            {
                num34 = 3;
            }
            if (num34 > 0)
            {
                if (isAggressive && ticks == 1f)
                {
                    TargetClosest();
                }
                if (num34 == 3)
                {
                    jumpVelocity = 8f;
                    velocity.X += 3 * dirX;
                    velocity.Y += 3 * dirY;
                    currentAttack = -200f;
                    previousX = position.X;
                    previousY = position.Y;
                }
                else
                {
                    jumpVelocity = 6f;
                    velocity.X += 2 * dirX;
                    velocity.Y += 2 * dirY;
                    currentAttack = -120f;
                    if (num34 == 1)
                    {
                        currentAttack += num33;
                    }
                    else
                    {
                        currentAttack += num33 * 2f;
                    }
                }
            }
            else if (currentAttack >= -30f)
            {
                aiAction = 1;
            }
        }
        else
        {
            if (jumpPos == 0f && oldJumpVelocity != 0f && jumpVelocity == 0f)
            {
                position.X -= velocity.X + dirX;
                position.Y -= velocity.Y + dirY;
                oldJumpVelocity = jumpVelocity;
            }
            if (target != null && (dirX > 0 && velocity.X < 3f || dirX < 0 && velocity.X > -3f))
            {
                if (dirX < 0 && velocity.X < 0.01 || dirX > 0 && velocity.X > -0.01)
                {
                    velocity.X += 0.2f * dirX;
                }
                else
                {
                    velocity.X *= 0.93f;
                }
            }
            if (target != null && (dirY > 0 && velocity.Y < 3f || dirY < 0 && velocity.Y > -3f))
            {
                if (dirY < 0 && velocity.Y < 0.01 || dirY > 0 && velocity.Y > -0.01)
                {
                    velocity.Y += 0.2f * dirY;
                }
                else
                {
                    velocity.Y *= 0.93f;
                }
            }
        }
        jumpPos += jumpVelocity;
        if (jumpPos > 0f)
        {
            jumpVelocity -= 0.2f;
            oldJumpVelocity = jumpVelocity;
        }
        else
        {
            jumpPos = 0f;
            jumpVelocity = 0f;
        }
        var rect = GetBoundingBox();
        if (willCollide(velocity.X, 0f))
            velocity.X = 0f;
        if (willCollide(0f, velocity.Y))
            velocity.Y = 0f;
        if (willCollide(velocity.X, velocity.Y))
            velocity.X = velocity.Y = 0f;
        position.Set(new(position.X + velocity.X, position.Y + velocity.Y));
    }
    public bool willCollide(float sx, float sy)
    {
        var rect = GetBoundingBox();
        Rectangle value = new Rectangle((int)Math.Floor(position.X) + (int)Math.Floor(sx), (int)Math.Floor(position.Y) + (int)Math.Floor(sy), rect.Width, rect.Height);
        Rectangle nextPositionCeil = new Rectangle((int)Math.Floor(position.X) + (int)Math.Ceiling(sx), (int)Math.Floor(position.Y) + (int)Math.Ceiling(sy), rect.Width, rect.Height);
        Rectangle nextPosition = Rectangle.Union(value, nextPositionCeil);
        return currentLocation.isCollidingPosition(nextPosition, Game1.viewport, this);
    }
    public override void updateMovement(GameLocation location, GameTime time)
    {

    }
}