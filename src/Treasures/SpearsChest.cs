using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Treasures; 

public class SpearsChest : Chest {
    private int[] _content;
    private int spearType;

    public SpearsChest(RopeGame game, World world, Vector2 position) : base(game, world, position) {
        GenerateLoot();
    }

    public SpearsChest(RopeGame game, World world, Vector2 position, int[] loot) : base(game, world, position) {
        if (loot.Length != 3)
            GenerateLoot();
        else
            _content = loot;
    }
    
    public override void LoadContent() {
        switch (spearType) {
            case 0:
                LootTexture = Game.Content.Load<Texture2D>("Sprites/Chest/icon_metal_" + _content[spearType]);
                break;
            case 1:
                LootTexture = Game.Content.Load<Texture2D>("Sprites/Chest/icon_electric_" + _content[spearType]);
                break;
            case 2:
                LootTexture = Game.Content.Load<Texture2D>("Sprites/Chest/icon_wooden_" + _content[spearType]);
                break;
        }

        base.LoadContent();
    }

    public void GenerateLoot() {
        _content = new int[3];
        spearType = RnGsus.Instance.Next(3);
        var spearAmount = RnGsus.Instance.Next(3) + 1;
        _content[spearType] = spearAmount;
    }

    public override void Loot() {
        for (var i = 0; i < 3; i++) Game.GameData.Spears[i] += _content[i];
        SoundEngine.Instance.ChestSound();
    }
}