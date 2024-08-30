using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaBosses.NPCs
{
    public class ITerrariaBossEntity : ITerrariaEntity
    {
        public string bossTrack;

        public ITerrariaBossEntity()
        {
        }

        public ITerrariaBossEntity(Vector2 position) : base("", position)
        {

        }

        public ITerrariaBossEntity(string name, Vector2 position) : base(name, position)
        {

        }
    }
}
