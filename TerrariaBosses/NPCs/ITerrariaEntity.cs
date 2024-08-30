using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;

namespace TerrariaBosses.NPCs
{
    public abstract class ITerrariaEntity : Monster
    {
        public string hitSoundID;

        public string killSoundID;

        public int wasHitCounter;

        public Vector2 velocity;

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
    }
}
