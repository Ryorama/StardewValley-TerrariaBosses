using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Monsters;
using StardewValley.Network;

namespace TerrariaBosses.NPCs;

public class EyeOfCthulhu : Monster
{
    public const float rotationIncrement = MathF.PI / 64f;

    private int wasHitCounter;

    private readonly NetFarmerRef killer = new NetFarmerRef().Delayed(interpolationWait: false);

    public List<Vector3> segments = new List<Vector3>();

    public NetInt segmentCount = new NetInt(0);

    int width = 55;
    int height = 83;
    bool cutscene = true;
    bool startedFading = false;

    public EyeOfCthulhu()
    {
    }

    public EyeOfCthulhu(Vector2 position)
        : base("EyeOfCthulhu", position)
    {
        InitializeAttributes();
    }

    public EyeOfCthulhu(Vector2 position, string name)
        : base(name, position)
    {
        InitializeAttributes();
    }

    public virtual void InitializeAttributes()
    {
        Halt();
        base.IsWalkingTowardPlayer = false;
        velocity = new(0, 0);
        forceOneTileWide.Value = false;
        reloadSprite();
    }

    public override void reloadSprite(bool onlyAppearance = false)
    {
        Sprite = new AnimatedSprite("Mods\\GlitchedDeveloper.TerrariaBosses\\Monsters\\EyeOfCthulhu");
        Sprite.SpriteWidth = 55;
        Sprite.SpriteHeight = 83;
        Sprite.SourceRect = new Rectangle(0, 0, 55, 83);
        base.HideShadow = true;
    }

    public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
    {
        int num = (int)(damage - (resilience * 0.5));
        base.Health -= num;
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
        Rectangle hitbox = GetBoundingBox();
        if (ModEntry.config.SpawnGore)
        {
            base.currentLocation.debris.Add(new Gore("Mods\\GlitchedDeveloper.TerrariaBosses\\Gore\\9", new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y + hitbox.Height / 2)));
            base.currentLocation.debris.Add(new Gore("Mods\\GlitchedDeveloper.TerrariaBosses\\Gore\\9", new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y + hitbox.Height / 2)));
            base.currentLocation.debris.Add(new Gore("Mods\\GlitchedDeveloper.TerrariaBosses\\Gore\\10", new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y + hitbox.Height / 2)));
            base.currentLocation.debris.Add(new Gore("Mods\\GlitchedDeveloper.TerrariaBosses\\Gore\\10", new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y + hitbox.Height / 2)));
        }
        base.currentLocation.localSound("GlitchedDeveloper-TerrariaBosses-Killed1");
    }

    public override List<Item> getExtraDropItems()
    {
        List<Item> list = new List<Item>
        {
            ItemRegistry.Create("GlitchedDeveloper.TerrariaBosses_DemoniteOre", Game1.random.Next(30, 90)),
            ItemRegistry.Create("GlitchedDeveloper.TerrariaBosses_CrimtaneOre", Game1.random.Next(30, 90))
        };
        if (Game1.random.Next(10) == 0)
        {
            list.Add(ItemRegistry.Create("GlitchedDeveloper.TerrariaBosses_EyeOfCthulhuTrophy"));
        }
        return list;
    }

    public override void drawAboveAllLayers(SpriteBatch b)
    {
        int y = base.StandingPixel.Y;
        Rectangle value = new Rectangle(0, 0, 55, 83);
        Vector2 vector2 = base.Position;

        if (Utility.isOnScreen(vector2, 128))
        {
            Vector2 vector4 = Game1.GlobalToLocal(Game1.viewport, vector2) + drawOffset + new Vector2(0f, yJumpOffset);
            int height = GetBoundingBox().Height;
            b.Draw(Sprite.Texture, vector4 + new Vector2(64f, height / 2), value, Color.White, rotation, new Vector2(55 / 2, 83 / 2), Math.Max(0.2f, scale.Value) * 4f, SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((float)(y + 8) / 10000f)));
            if (isGlowing)
            {
                b.Draw(Sprite.Texture, vector4 + new Vector2(64f, height / 2), value, glowingColor * glowingTransparency, rotation, new Vector2(55 / 2, 83 / 2), Math.Max(0.2f, scale.Value) * 4f, SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((float)(y + 8) / 10000f + 0.0001f)));
            }
        }
    }

    public override Rectangle GetBoundingBox()
    {
        Vector2 vector = base.Position;
        return new Rectangle((int)vector.X - 55 / 3 - (int)(Math.Cos(rotation + 1.57f) * 50), (int)vector.Y - (int)(Math.Sin(rotation + 1.57f) * 50), 55 * 3, 55 * 3);
    }

    protected override void updateAnimation(GameTime time)
    {
        base.updateAnimation(time);
        if (currentPhase > 1f)
            Sprite.Animate(time, 3, 3, 0);
        else
            Sprite.Animate(time, 0, 3, 0);

        ModEntry.monitor.LogOnce("Anim frame: " + Sprite.currentFrame.ToString());
    }

    private float currentPhase;
    private float currentAttackMode;
    private float attackModeTicks;
    private float attackPartTicks;
    private Vector2 velocity;

    public override void behaviorAtGameTick(GameTime time)
    {

        float num4 = 20f;
        Farmer target = Player;
        float distX = position.X + (float)(width / 2) - target.position.X;
        float distY = position.Y + (float)(height / 2) - 59f - target.position.Y;
        if (cutscene)
        {
            if (!ModEntry.config.EyeOfCthulhu.PlayCutscene)
                cutscene = false;
            else
            {
                if (!startedFading && Math.Sqrt(distX * distX + distY * distY) < 1000)
                {
                    startedFading = true;
                    Game1.globalFadeToBlack(delegate
                    {
                        cutscene = false;
                        Game1.viewportTarget = new Vector2(-2.14748365E+09f, -2.14748365E+09f);
                        Game1.viewportHold = 0;
                        Game1.viewportFreeze = false;
                        Game1.viewportCenter = Game1.player.StandingPixel;
                        Game1.globalFadeToClear(null, 0.05f);
                    }, 0.05f);
                }
                Game1.moveViewportTo(new Vector2(position.X, position.Y), 20f, 0);
            }
        }

        float targetAngle = (float)Math.Atan2(distY, distX) + 4.71f;
        if (targetAngle < 0f)
        {
            targetAngle += 6.283f;
        }
        else if ((double)targetAngle > 6.283)
        {
            targetAngle -= 6.283f;
        }

        float turnSpeed = 0f;
        if (currentPhase == 0f && currentAttackMode == 0f)
        {
            turnSpeed = 0.02f;
        }
        if (currentPhase == 0f && currentAttackMode == 2f && attackModeTicks > 40f)
        {
            turnSpeed = 0.05f;
        }
        if (currentPhase == 3f && currentAttackMode == 0f)
        {
            turnSpeed = 0.05f;
        }
        if (currentPhase == 3f && currentAttackMode == 2f && attackModeTicks > 40f)
        {
            turnSpeed = 0.08f;
        }
        if (currentPhase == 3f && currentAttackMode == 4f && attackModeTicks > num4)
        {
            turnSpeed = 0.15f;
        }
        if (currentPhase == 3f && currentAttackMode == 5f)
        {
            turnSpeed = 0.05f;
        }
        if (rotation < targetAngle)
        {
            if ((double)(targetAngle - rotation) > 3.1415)
            {
                rotation -= turnSpeed;
            }
            else
            {
                rotation += turnSpeed;
            }
        }
        else if (rotation > targetAngle)
        {
            if ((double)(rotation - targetAngle) > 3.1415)
            {
                rotation += turnSpeed;
            }
            else
            {
                rotation -= turnSpeed;
            }
        }
        if (rotation > targetAngle - turnSpeed && rotation < targetAngle + turnSpeed)
        {
            rotation = targetAngle;
        }
        if (rotation < 0f)
        {
            rotation += 6.283f;
        }
        else if ((double)rotation > 6.283)
        {
            rotation -= 6.283f;
        }
        if (rotation > targetAngle - turnSpeed && rotation < targetAngle + turnSpeed)
        {
            rotation = targetAngle;
        }

        if (currentPhase == 0f)
        {
            if (currentAttackMode == 0f)
            {
                float maxSpeed = 5f;
                float acceleration = 0.04f;

                maxSpeed *= 2;
                acceleration *= 2;
                Vector2 vector = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
                distX = target.position.X - vector.X;
                distY = target.position.Y - 200f * 2 - vector.Y;
                float distScale = (float)Math.Sqrt(distX * distX + distY * distY);
                float targetDistance = distScale;
                distScale = maxSpeed / distScale;
                distX *= distScale;
                distY *= distScale;
                if (velocity.X < distX)
                {
                    velocity.X += acceleration;
                    if (velocity.X < 0f && distX > 0f)
                    {
                        velocity.X += acceleration;
                    }
                }
                else if (velocity.X > distX)
                {
                    velocity.X -= acceleration;
                    if (velocity.X > 0f && distX < 0f)
                    {
                        velocity.X -= acceleration;
                    }
                }
                if (velocity.Y < distY)
                {
                    velocity.Y += acceleration;
                    if (velocity.Y < 0f && distY > 0f)
                    {
                        velocity.Y += acceleration;
                    }
                }
                else if (velocity.Y > distY)
                {
                    velocity.Y -= acceleration;
                    if (velocity.Y > 0f && distY < 0f)
                    {
                        velocity.Y -= acceleration;
                    }
                }
                attackModeTicks += 1f;
                float maxTicksInSpawnAttack = 600f;
                if (attackModeTicks >= maxTicksInSpawnAttack)
                {
                    currentAttackMode = 1f;
                    attackModeTicks = 0f;
                    attackPartTicks = 0f;
                    target = null;
                }
                else if (position.Y + (float)height < target.position.Y && targetDistance < 500f * 2)
                {
                    attackPartTicks += 1f;
                    float ticksToSpawnServant = 110f;
                    if (attackPartTicks >= ticksToSpawnServant)
                    {
                        attackPartTicks = 0f;
                        rotation = targetAngle;
                        maxSpeed = 5f;
                        maxSpeed *= 2;

                        distX = target.position.X - vector.X;
                        distY = target.position.Y - vector.Y;
                        distScale = (float)Math.Sqrt(distX * distX + distY * distY);
                        distScale = maxSpeed / distScale;
                        Vector2 servantPos = vector;
                        Vector2 servantVelocity = default(Vector2);
                        servantVelocity.X = distX * distScale;
                        servantVelocity.Y = distY * distScale;
                        servantPos.X += servantVelocity.X * 10f;
                        servantPos.Y += servantVelocity.Y * 10f;
                        DemonEye servantNPC = new DemonEye(servantPos, "ServantOfCthulhu");
                        servantNPC.velocity.X = servantVelocity.X;
                        servantNPC.velocity.Y = servantVelocity.Y;
                        base.currentLocation.addCharacter(servantNPC);
                        base.currentLocation.playSound($"GlitchedDeveloper-TerrariaBosses-Hit1");
                    }
                }
            }
            else if (currentAttackMode == 1f)
            {
                rotation = targetAngle;
                float maxSpeed = 6f * 2;
                Vector2 eocCenter = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
                distX = target.position.X - eocCenter.X;
                distY = target.position.Y - eocCenter.Y;
                float speedScale = (float)Math.Sqrt(distX * distX + distY * distY);
                speedScale = maxSpeed / speedScale;
                velocity.X = distX * speedScale;
                velocity.Y = distY * speedScale;
                currentAttackMode = 2f;
            }
            else if (currentAttackMode == 2f)
            {
                attackModeTicks += 1f;
                if (attackModeTicks >= 40f)
                {
                    velocity *= 0.98f;
                    if ((double)velocity.X > -0.1 && (double)velocity.X < 0.1)
                    {
                        velocity.X = 0f;
                    }
                    if ((double)velocity.Y > -0.1 && (double)velocity.Y < 0.1)
                    {
                        velocity.Y = 0f;
                    }
                }
                else
                {
                    rotation = (float)Math.Atan2(velocity.Y, velocity.X) - 4.71f;
                }
                int ticksInChargeAttack = 150;
                if (attackModeTicks >= (float)ticksInChargeAttack)
                {
                    attackPartTicks += 1f;
                    attackModeTicks = 0f;
                    target = null;
                    rotation = targetAngle;
                    if (attackPartTicks >= 3f)
                    {
                        currentAttackMode = 0f;
                        attackPartTicks = 0f;
                    }
                    else
                    {
                        currentAttackMode = 1f;
                    }
                }
            }
            float lifeForSecondPhase = 0.75f;
            if (base.Health < base.MaxHealth * lifeForSecondPhase)
            {
                currentPhase = 1f;
                currentAttackMode = 0f;
                attackModeTicks = 0f;
                attackPartTicks = 0f;
            }
            position.Set(new(position.X + velocity.X, position.Y + velocity.Y));
            return;
        }
        if (currentPhase == 1f || currentPhase == 2f)
        {
            if (currentPhase == 1f || attackPartTicks == 1f)
            {
                attackModeTicks += 0.005f;
                if ((double)attackModeTicks > 0.5)
                {
                    attackModeTicks = 0.5f;
                }
            }
            else
            {
                attackModeTicks -= 0.005f;
                if (attackModeTicks < 0f)
                {
                    attackModeTicks = 0f;
                }
            }
            rotation += attackModeTicks;
            currentAttackMode += 1f;
            int howOftenToSpawnServant = 20;
            if (currentAttackMode % (float)howOftenToSpawnServant == 0f)
            {
                float maxServantSpeed = 5f * 2;
                Vector2 eocCenter = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
                float randomTargetX = Game1.random.Next(-400, 400);
                float randomTargetY = Game1.random.Next(-400, 400);
                float servantSpeed = (float)Math.Sqrt(randomTargetX * randomTargetX + randomTargetY * randomTargetY);
                servantSpeed = maxServantSpeed / servantSpeed;
                Vector2 servantPos = eocCenter;
                Vector2 servantVelocity = default(Vector2);
                servantVelocity.X = randomTargetX * servantSpeed;
                servantVelocity.Y = randomTargetY * servantSpeed;
                servantPos.X += servantVelocity.X * 10f * 2;
                servantPos.Y += servantVelocity.Y * 10f * 2;
                DemonEye servantNPC = new DemonEye(servantPos, "ServantOfCthulhu");
                servantNPC.velocity.X = servantVelocity.X;
                servantNPC.velocity.Y = servantVelocity.Y;
                base.currentLocation.addCharacter(servantNPC);
            }
            if (currentAttackMode >= 100f)
            {
                if (attackPartTicks == 1f)
                {
                    attackPartTicks = 0f;
                    currentAttackMode = 0f;
                }
                else
                {
                    currentPhase += 1f;
                    currentAttackMode = 0f;
                    if (currentPhase == 3f)
                    {
                        attackModeTicks = 0f;
                    }
                    else
                    {
                        base.currentLocation.playSound($"GlitchedDeveloper-TerrariaBosses-Hit1");
                        Rectangle hitbox = GetBoundingBox();
                        if (ModEntry.config.SpawnGore)
                        {
                            base.currentLocation.debris.Add(new Gore("Mods\\GlitchedDeveloper.TerrariaBosses\\Gore\\8", new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y + hitbox.Height / 2)));
                            base.currentLocation.debris.Add(new Gore("Mods\\GlitchedDeveloper.TerrariaBosses\\Gore\\8", new Vector2(hitbox.X + hitbox.Width / 2, hitbox.Y + hitbox.Height / 2)));
                        }
                        base.currentLocation.playSound($"GlitchedDeveloper-TerrariaBosses-Roar");
                    }
                }
            }
            velocity.X *= 0.98f;
            velocity.Y *= 0.98f;
            if ((double)velocity.X > -0.1 && (double)velocity.X < 0.1)
            {
                velocity.X = 0f;
            }
            if ((double)velocity.Y > -0.1 && (double)velocity.Y < 0.1)
            {
                velocity.Y = 0f;
            }
            position.Set(new(position.X + velocity.X, position.Y + velocity.Y));
            return;
        }

        resilience.Value = 0;
        int normalDamage = 23;
        damageToFarmer.Value = normalDamage;
        if (currentAttackMode == 0f)
        {
            float maxSpeed = 6f * 2;
            float acceleration = 0.07f * 2;
            Vector2 eocCenter = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
            distX = target.position.X - eocCenter.X;
            distY = target.position.Y - 120f * 2 - eocCenter.Y;
            float targetDistance = (float)Math.Sqrt(distX * distX + distY * distY);
            targetDistance = maxSpeed / targetDistance;
            distX *= targetDistance;
            distY *= targetDistance;
            if (velocity.X < distX)
            {
                velocity.X += acceleration;
                if (velocity.X < 0f && distX > 0f)
                {
                    velocity.X += acceleration;
                }
            }
            else if (velocity.X > distX)
            {
                velocity.X -= acceleration;
                if (velocity.X > 0f && distX < 0f)
                {
                    velocity.X -= acceleration;
                }
            }
            if (velocity.Y < distY)
            {
                velocity.Y += acceleration;
                if (velocity.Y < 0f && distY > 0f)
                {
                    velocity.Y += acceleration;
                }
            }
            else if (velocity.Y > distY)
            {
                velocity.Y -= acceleration;
                if (velocity.Y > 0f && distY < 0f)
                {
                    velocity.Y -= acceleration;
                }
            }
            attackModeTicks += 1f;
            if (attackModeTicks >= 200f)
            {
                currentAttackMode = 1f;
                attackModeTicks = 0f;
                attackPartTicks = 0f;
                target = null;
            }
        }
        else if (currentAttackMode == 1f)
        {
            base.currentLocation.playSound($"GlitchedDeveloper-TerrariaBosses-Roar");
            rotation = targetAngle;
            float maxSpeed = 6.8f * 2;
            Vector2 eocCenter = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
            distX = target.position.X - eocCenter.X;
            distY = target.position.Y - eocCenter.Y;
            float targetDistance = (float)Math.Sqrt(distX * distX + distY * distY);
            targetDistance = maxSpeed / targetDistance;
            velocity.X = distX * targetDistance;
            velocity.Y = distY * targetDistance;
            currentAttackMode = 2f;
        }
        else if (currentAttackMode == 2f)
        {
            float ticksSpentChargingUntilSlower = 40f;
            attackModeTicks += 1f;
            if (attackModeTicks >= ticksSpentChargingUntilSlower)
            {
                velocity *= 0.97f;
                if ((double)velocity.X > -0.1 && (double)velocity.X < 0.1)
                {
                    velocity.X = 0f;
                }
                if ((double)velocity.Y > -0.1 && (double)velocity.Y < 0.1)
                {
                    velocity.Y = 0f;
                }
            }
            else
            {
                rotation = (float)Math.Atan2(velocity.Y, velocity.X) + 1.57f;
            }
            int ticksSpentCharging = 130;
            if (attackModeTicks >= (float)ticksSpentCharging)
            {
                attackPartTicks += 1f;
                attackModeTicks = 0f;
                target = null;
                rotation = targetAngle;
                if (attackPartTicks >= 3f)
                {
                    currentAttackMode = 0f;
                    attackPartTicks = 0f;
                }
                else
                {
                    currentAttackMode = 1f;
                }
            }
        }
        position.Set(new(position.X + velocity.X, position.Y + velocity.Y));
        return;
    }
    public override void updateMovement(GameLocation location, GameTime time)
    {

    }
}