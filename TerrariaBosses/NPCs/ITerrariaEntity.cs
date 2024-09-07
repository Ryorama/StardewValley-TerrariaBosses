using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using System.IO;

namespace TerrariaBosses.NPCs
{
    public abstract class ITerrariaEntity : Monster
    {
        public string hitSoundID;

        public string killSoundID;

        public int wasHitCounter;

        public Vector2 velocity;

        public static int maxAI = 4;

        public float[] ai = new float[maxAI];

        public float[] localAI = new float[maxAI];

        public int aiAction;

        public bool netUpdate;
        public bool netUpdate2;

        public int width;
        public int height;
        public int direction;
        public int directionY;
        public int oldDirection;
        public int oldDirectionY;
        public bool collideX;
        public bool collideY;
        public bool confused;

        public int target = -1;
        public int oldTarget;

        public Rectangle targetRect;


        public readonly NetFarmerRef killer = new NetFarmerRef().Delayed(interpolationWait: false);

        public ITerrariaEntity()
        {
        }

        public ITerrariaEntity(string name) : base(name, new Vector2(0, 0))
        {

        }

        public ITerrariaEntity(Vector2 position) : base("", position)
        {
           
        }

        public ITerrariaEntity(string name, Vector2 position) : base(name, position)
        {
            
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            float defenseMultiplier = 0.5f;
            switch (ModEntry.config.Difficulty)
            {
                case "Expert":
                    defenseMultiplier = 0.75f;
                    break;
                case "Master":
                    defenseMultiplier = 1f;
                    break;
                case "Legendary":
                    defenseMultiplier = 1.5f;
                    break;
            }
            velocity.X = xTrajectory / 3;
            velocity.Y = yTrajectory / 3;
            int num = (int)(damage - (resilience * defenseMultiplier));
            base.Health -= num;
            wasHitCounter = 500;
            base.currentLocation.playSound($"GlitchedDeveloper.TerrariaBosses_{hitSoundID}");
            if (base.Health <= 0)
            {
                killer.Value = who;
                deathAnimation();
            }
            return num;
        }

        public bool isFarmerDead(Farmer target)
        {
            return target.health > 0;
        }

        public void TargetClosest(bool faceTarget = true)
        {
            float distance = 0f;
            float realDist = 0f;
            bool t = false;
            int tankTarget = -1;
            for (int i = 0; i < 255; i++)
            {
                if (Player.isActive() && isFarmerDead(Player))
                    TryTrackingTarget(ref distance, ref realDist, ref t, ref tankTarget, i);
            }

            SetTargetTrackingValues(faceTarget, realDist, tankTarget);
        }

        private void TryTrackingTarget(ref float distance, ref float realDist, ref bool t, ref int tankTarget, int j)
        {
            float num = Math.Abs(Player.position.X + (float)(Player.GetBoundingBox().Width / 2) - position.X + (float)(Player.GetBoundingBox().Width / 2)) + Math.Abs(Player.position.Y + (float)(Player.GetBoundingBox().Height / 2) - position.Y + (float)(height / 2));
            num -= (float)Main.player[j].aggro;
            if (Main.player[j].npcTypeNoAggro[type] && direction != 0)
                num += 1000f;

            if (!t || num < distance)
            {
                t = true;
                tankTarget = -1;
                realDist = Math.Abs(Player.position.X + (float)(Player.GetBoundingBox().Width / 2) - position.X + (float)(width / 2)) + Math.Abs(Player.position.Y + (float)(Player.GetBoundingBox().Height / 2) - position.Y + (float)(height / 2));
                distance = num;
                target = j;
            }

            if (Main.player[j].tankPet >= 0 && !Main.player[j].npcTypeNoAggro[type])
            {
                int tankPet = Main.player[j].tankPet;
                float num2 = Math.Abs(Main.projectile[tankPet].position.X + (float)(Main.projectile[tankPet].width / 2) - position.X + (float)(width / 2)) + Math.Abs(Main.projectile[tankPet].position.Y + (float)(Main.projectile[tankPet].height / 2) - position.Y + (float)(height / 2));
                num2 -= 200f;
                if (num2 < distance && num2 < 200f && Collision.CanHit(base.Center, 1, 1, Main.projectile[tankPet].Center, 1, 1))
                    tankTarget = tankPet;
            }
        }

        private void SetTargetTrackingValues(bool faceTarget, float realDist, int tankTarget)
        {
            if (tankTarget >= 0)
            {
                targetRect = new Rectangle((int)Main.projectile[tankTarget].position.X, (int)Main.projectile[tankTarget].position.Y, Main.projectile[tankTarget].width, Main.projectile[tankTarget].height);
                direction = 1;
                if ((float)(targetRect.X + targetRect.Width / 2) < position.X + (float)(width / 2))
                    direction = -1;

                directionY = 1;
                if ((float)(targetRect.Y + targetRect.Height / 2) < position.Y + (float)(height / 2))
                    directionY = -1;
            }
            else
            {
                if (target < 0 || target >= 255)
                    target = 0;

                targetRect = new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.GetBoundingBox().Width, Player.GetBoundingBox().Height);
                if (isFarmerDead(Player))
                    faceTarget = false;

                if (Main.player[target].npcTypeNoAggro[type] && direction != 0)
                    faceTarget = false;

                if (faceTarget)
                {
                    _ = Main.player[target].aggro;
                    _ = (Player.GetBoundingBox().Height + Player.GetBoundingBox().Width + height + width) / 4;
                    if (Main.player[target].itemAnimation != 0 || Main.player[target].aggro >= 0 || oldTarget < 0 || oldTarget > 254)
                    {
                        direction = 1;
                        if ((float)(targetRect.X + targetRect.Width / 2) < position.X + (float)(width / 2))
                            direction = -1;

                        directionY = 1;
                        if ((float)(targetRect.Y + targetRect.Height / 2) < position.Y + (float)(height / 2))
                            directionY = -1;
                    }
                }
            }

            if (confused)
                direction *= -1;

            if ((direction != oldDirection || directionY != oldDirectionY || target != oldTarget) && !collideX && !collideY)
                netUpdate = true;
        }

    }
}
