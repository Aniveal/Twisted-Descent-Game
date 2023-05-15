using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace TwistedDescent.Treasures; 

public class HealthChest : Chest {
    private int _contentHealth;
    private int _contentMaxHealth;

    public HealthChest(RopeGame game, World world, Vector2 position) : base(game, world, position) {
        GenerateContent();
    }

    public HealthChest(RopeGame game, World world, Vector2 position, int maxHealth, int health) : base(game, world,
        position) {
        _contentHealth = health;
        _contentMaxHealth = maxHealth;
    }

    public override void LoadContent() {
        LootTexture = Game.Content.Load<Texture2D>("Sprites/UI/hearth_icon");
        
        base.LoadContent();
    }

    public void GenerateContent() {
        _contentMaxHealth = 1;
        _contentHealth = 2;
    }

    public override void Loot() {
        Game.GameData.MaxHealth += _contentMaxHealth;
        Game.GameData.AddHealth(_contentHealth);
        SoundEngine.Instance.ChestSound();
    }
}