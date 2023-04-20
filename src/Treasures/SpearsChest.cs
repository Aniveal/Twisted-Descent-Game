

using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Treasures {

    public class SpearsChest : Chest
    {
        private int[] content;
        public SpearsChest(RopeGame game, World world, Vector2 position) : base(game, world, position) {
            GenerateLoot();
        }

        public SpearsChest(RopeGame game, World world, Vector2 position, int[] loot) : base(game, world, position) {
            if (loot.Length != 3) {
                GenerateLoot();
            } else {
                content = loot;
            }

        }

        public void GenerateLoot() {
            //TODO randomize?
            content = new int[3] {5, 2, 2};
        }

        public override void Loot() {
            for (int i = 0; i < 3; i++) {
                _game.gameData.spears[i] += content[i];
            }
        }


        
    }
}