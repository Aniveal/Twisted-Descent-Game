

using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Treasures {

    public class HealthChest : Chest
    {
        private int contentMaxHealth;
        private int contentHealth;
        public HealthChest(RopeGame game, World world, Vector2 position) : base(game, world, position) {
            GenerateContent();
        }

        public HealthChest(RopeGame game, World world, Vector2 position, int maxHealth, int health) : base(game, world, position) {
            contentHealth = health;
            contentMaxHealth = maxHealth;
        }   

        public void GenerateContent() {
            contentMaxHealth = 1;
            contentHealth = 2;
        }

        public override void Loot()
        {
            _game.gameData.maxHealth += contentMaxHealth;
            _game.gameData.AddHealth(contentHealth);
        }
    }
}