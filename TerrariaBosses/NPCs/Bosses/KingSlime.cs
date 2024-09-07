using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.TargetGame;

namespace TerrariaBosses.NPCs.Bosses
{
    public class KingSlime : ITerrariaBossEntity
    {
        int width = 162;
        int height = 116;
        bool cutscene = true;
        bool startedFading = false;
        bool reflectsProjectiles = false;

        public KingSlime()
        {
        }

        public KingSlime(Vector2 position) : base("King Slime", position)
        {
            InitializeAttributes();
        }

        public KingSlime(Vector2 position, string name) : base(name, position)
        {
            InitializeAttributes();
        }

        public virtual void InitializeAttributes()
        {
            Halt();
            hitSoundID = "Hit 1";
            killSoundID = "Killed 1";
            bossTrack = "GlitchedDeveloper.TerrariaBosses_Boss 1";
            IsWalkingTowardPlayer = false;
            velocity = new(0, 0);
            forceOneTileWide.Value = false;
            switch (ModEntry.config.Difficulty)
            {
                case "Expert":
                    maxHealth.Value = 3640;
                    health.Value = maxHealth.Value;
                    break;
                case "Master":
                    maxHealth.Value = 4641;
                    health.Value = maxHealth.Value;
                    break;
                case "Legendary":
                    maxHealth.Value = 5651;
                    health.Value = maxHealth.Value;
                    break;
            }
            reloadSprite();
        }

        public override void reloadSprite(bool onlyAppearance = false)
        {
            Sprite = new AnimatedSprite("Mods/GlitchedDeveloper.TerrariaBosses/Monsters/King Slime");
            Sprite.SpriteWidth = width;
            Sprite.SpriteHeight = height;
            Sprite.SourceRect = new Rectangle(0, 0, width, height);
            HideShadow = true;
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            float num236 = 1f;
            bool flag8 = false;
            bool flag9 = false;
            float num237 = 2f;
            if (ModEntry.getGoodWorld)
            {
                num237 -= 1f - health.Value / maxHealth.Value;
                num236 *= num237;
            }
            aiAction = 0;
            if (this.ai[3] == 0f && health.Value > 0)
                this.ai[3] = maxHealth.Value;

            if (localAI[3] == 0f && Game1.multiplayerMode != 1)
            {
                this.ai[0] = -100f;
                localAI[3] = 1f;
                TargetClosest();
                netUpdate = true;
            }

            int num238 = 3000;
            if (Main.player[target].dead || Vector2.Distance(base.Center, Main.player[target].Center) > (float)num238)
            {
                TargetClosest();
                if (Main.player[target].dead || Vector2.Distance(base.Center, Main.player[target].Center) > (float)num238)
                {
                    EncourageDespawn(10);
                    if (Main.player[target].Center.X < base.Center.X)
                        base.direction = 1;
                    else
                        base.direction = -1;

                    if (Main.netMode != 1 && this.ai[1] != 5f)
                    {
                        netUpdate = true;
                        this.ai[2] = 0f;
                        this.ai[0] = 0f;
                        this.ai[1] = 5f;
                        localAI[1] = Main.maxTilesX * 16;
                        localAI[2] = Main.maxTilesY * 16;
                    }
                }
            }

            if (!Main.player[target].dead && timeLeft > 10 && this.ai[2] >= 300f && this.ai[1] < 5f && base.velocity.Y == 0f)
            {
                this.ai[2] = 0f;
                this.ai[0] = 0f;
                this.ai[1] = 5f;
                if (Main.netMode != 1)
                {
                    TargetClosest(faceTarget: false);
                    Point point3 = base.Center.ToTileCoordinates();
                    Point point4 = Main.player[target].Center.ToTileCoordinates();
                    Vector2 vector27 = Main.player[target].Center - base.Center;
                    int num239 = 10;
                    int num240 = 0;
                    int num241 = 7;
                    int num242 = 0;
                    bool flag10 = false;
                    if (localAI[0] >= 360f || vector27.Length() > 2000f)
                    {
                        if (localAI[0] >= 360f)
                            localAI[0] = 360f;

                        flag10 = true;
                        num242 = 100;
                    }

                    while (!flag10 && num242 < 100)
                    {
                        num242++;
                        int num243 = Main.rand.Next(point4.X - num239, point4.X + num239 + 1);
                        int num244 = Main.rand.Next(point4.Y - num239, point4.Y + 1);
                        if ((num244 >= point4.Y - num241 && num244 <= point4.Y + num241 && num243 >= point4.X - num241 && num243 <= point4.X + num241) || (num244 >= point3.Y - num240 && num244 <= point3.Y + num240 && num243 >= point3.X - num240 && num243 <= point3.X + num240) || Main.tile[num243, num244].nactive())
                            continue;

                        int num245 = num244;
                        int num246 = 0;
                        if (Main.tile[num243, num245].nactive() && Main.tileSolid[Main.tile[num243, num245].type] && !Main.tileSolidTop[Main.tile[num243, num245].type])
                        {
                            num246 = 1;
                        }
                        else
                        {
                            for (; num246 < 150 && num245 + num246 < Main.maxTilesY; num246++)
                            {
                                int num247 = num245 + num246;
                                if (Main.tile[num243, num247].nactive() && Main.tileSolid[Main.tile[num243, num247].type] && !Main.tileSolidTop[Main.tile[num243, num247].type])
                                {
                                    num246--;
                                    break;
                                }
                            }
                        }

                        num244 += num246;
                        bool flag11 = true;
                        if (flag11 && Main.tile[num243, num244].lava())
                            flag11 = false;

                        if (flag11 && !Collision.CanHitLine(base.Center, 0, 0, Main.player[target].Center, 0, 0))
                            flag11 = false;

                        if (flag11)
                        {
                            localAI[1] = num243 * 16 + 8;
                            localAI[2] = num244 * 16 + 16;
                            flag10 = true;
                            break;
                        }
                    }

                    if (num242 >= 100)
                    {
                        Vector2 bottom = Main.player[Player.FindClosest(base.position, width, height)].Bottom;
                        localAI[1] = bottom.X;
                        localAI[2] = bottom.Y;
                    }
                }
            }

            if (!Collision.CanHitLine(base.Center, 0, 0, Main.player[target].Center, 0, 0) || Math.Abs(base.Top.Y - Main.player[target].Bottom.Y) > 160f)
            {
                this.ai[2]++;
                if (Main.netMode != 1)
                    localAI[0]++;
            }
            else if (Main.netMode != 1)
            {
                localAI[0]--;
                if (localAI[0] < 0f)
                    localAI[0] = 0f;
            }

            if (timeLeft < 10 && (this.ai[0] != 0f || this.ai[1] != 0f))
            {
                this.ai[0] = 0f;
                this.ai[1] = 0f;
                netUpdate = true;
                flag8 = false;
            }

            Dust dust;
            if (this.ai[1] == 5f)
            {
                flag8 = true;
                aiAction = 1;
                this.ai[0]++;
                num236 = MathHelper.Clamp((60f - this.ai[0]) / 60f, 0f, 1f);
                num236 = 0.5f + num236 * 0.5f;
                if (Main.getGoodWorld)
                    num236 *= num237;

                if (this.ai[0] >= 60f)
                    flag9 = true;

                if (this.ai[0] == 60f)
                    Gore.NewGore(base.Center + new Vector2(-40f, -height / 2), base.velocity, 734);

                if (this.ai[0] >= 60f && Main.netMode != 1)
                {
                    base.Bottom = new Vector2(localAI[1], localAI[2]);
                    this.ai[1] = 6f;
                    this.ai[0] = 0f;
                    netUpdate = true;
                }

                if (Main.netMode == 1 && this.ai[0] >= 120f)
                {
                    this.ai[1] = 6f;
                    this.ai[0] = 0f;
                }

                if (!flag9)
                {
                    for (int num248 = 0; num248 < 10; num248++)
                    {
                        int num249 = Dust.NewDust(base.position + Vector2.UnitX * -20f, width + 40, height, 4, base.velocity.X, base.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
                        Main.dust[num249].noGravity = true;
                        dust = Main.dust[num249];
                        dust.velocity *= 0.5f;
                    }
                }
            }
            else if (this.ai[1] == 6f)
            {
                flag8 = true;
                aiAction = 0;
                this.ai[0]++;
                num236 = MathHelper.Clamp(this.ai[0] / 30f, 0f, 1f);
                num236 = 0.5f + num236 * 0.5f;
                if (Main.getGoodWorld)
                    num236 *= num237;

                if (this.ai[0] >= 30f && Main.netMode != 1)
                {
                    this.ai[1] = 0f;
                    this.ai[0] = 0f;
                    netUpdate = true;
                    TargetClosest();
                }

                if (Main.netMode == 1 && this.ai[0] >= 60f)
                {
                    this.ai[1] = 0f;
                    this.ai[0] = 0f;
                    TargetClosest();
                }

                for (int num250 = 0; num250 < 10; num250++)
                {
                    int num251 = Dust.NewDust(base.position + Vector2.UnitX * -20f, width + 40, height, 4, base.velocity.X, base.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
                    Main.dust[num251].noGravity = true;
                    dust = Main.dust[num251];
                    dust.velocity *= 2f;
                }
            }

            dontTakeDamage = (hide = flag9);
            if (base.velocity.Y == 0f)
            {
                base.velocity.X *= 0.8f;
                if ((double)base.velocity.X > -0.1 && (double)base.velocity.X < 0.1)
                    base.velocity.X = 0f;

                if (!flag8)
                {
                    this.ai[0] += 2f;
                    if ((double)life < (double)lifeMax * 0.8)
                        this.ai[0] += 1f;

                    if ((double)life < (double)lifeMax * 0.6)
                        this.ai[0] += 1f;

                    if ((double)life < (double)lifeMax * 0.4)
                        this.ai[0] += 2f;

                    if ((double)life < (double)lifeMax * 0.2)
                        this.ai[0] += 3f;

                    if ((double)life < (double)lifeMax * 0.1)
                        this.ai[0] += 4f;

                    if (this.ai[0] >= 0f)
                    {
                        netUpdate = true;
                        TargetClosest();
                        if (this.ai[1] == 3f)
                        {
                            base.velocity.Y = -13f;
                            base.velocity.X += 3.5f * (float)base.direction;
                            this.ai[0] = -200f;
                            this.ai[1] = 0f;
                        }
                        else if (this.ai[1] == 2f)
                        {
                            base.velocity.Y = -6f;
                            base.velocity.X += 4.5f * (float)base.direction;
                            this.ai[0] = -120f;
                            this.ai[1] += 1f;
                        }
                        else
                        {
                            base.velocity.Y = -8f;
                            base.velocity.X += 4f * (float)base.direction;
                            this.ai[0] = -120f;
                            this.ai[1] += 1f;
                        }
                    }
                    else if (this.ai[0] >= -30f)
                    {
                        aiAction = 1;
                    }
                }
            }
            else if (target < 255)
            {
                float num252 = 3f;
                if (Main.getGoodWorld)
                    num252 = 6f;

                if ((base.direction == 1 && base.velocity.X < num252) || (base.direction == -1 && base.velocity.X > 0f - num252))
                {
                    if ((base.direction == -1 && (double)base.velocity.X < 0.1) || (base.direction == 1 && (double)base.velocity.X > -0.1))
                        base.velocity.X += 0.2f * (float)base.direction;
                    else
                        base.velocity.X *= 0.93f;
                }
            }

            int num253 = Dust.NewDust(base.position, width, height, 4, base.velocity.X, base.velocity.Y, 255, new Color(0, 80, 255, 80), scale * 1.2f);
            Main.dust[num253].noGravity = true;
            dust = Main.dust[num253];
            dust.velocity *= 0.5f;
            if (life <= 0)
                return;

            float num254 = (float)life / (float)lifeMax;
            num254 = num254 * 0.5f + 0.75f;
            num254 *= num236;
            if (num254 != scale)
            {
                base.position.X += width / 2;
                base.position.Y += height;
                scale = num254;
                width = (int)(98f * scale);
                height = (int)(92f * scale);
                base.position.X -= width / 2;
                base.position.Y -= height;
            }

            if (Main.netMode == 1)
                return;

            int num255 = (int)((double)lifeMax * 0.05);
            if (!((float)(life + num255) < this.ai[3]))
                return;

            this.ai[3] = life;
            int num256 = Main.rand.Next(1, 4);
            for (int num257 = 0; num257 < num256; num257++)
            {
                int x = (int)(base.position.X + (float)Main.rand.Next(width - 32));
                int y = (int)(base.position.Y + (float)Main.rand.Next(height - 32));
                int num258 = 1;
                if (Main.expertMode && Main.rand.Next(4) == 0)
                    num258 = 535;

                int num259 = NewNPC(x, y, num258);
                Main.npc[num259].SetDefaults(num258);
                Main.npc[num259].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                Main.npc[num259].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                Main.npc[num259].ai[0] = -1000 * Main.rand.Next(3);
                Main.npc[num259].ai[1] = 0f;
                if (Main.netMode == 2 && num259 < 200)
                    NetMessage.SendData(23, -1, -1, null, num259);
            }
        }
    }
}
