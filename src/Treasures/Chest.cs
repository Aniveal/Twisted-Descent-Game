using Meridian2.GameElements;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Meridian2.Treasures; 

public class Chest : IDrawableObject {
    public Body Body;
    protected RopeGame Game;

    public bool Open = false;
    public Vector2 Pos;
    protected Texture2D TextureClosed;
    protected Texture2D TextureOpen;
    protected float Width = 0.75f;
    protected float Height = 0.5f;
    protected float spriteWidth = 0.91f;
    protected World World;


    //TODO: modify usage of width and height in Body and draw to fit to proportions of future sprite
    public Chest(RopeGame game, World world, Vector2 position) {
        Game = game;
        World = world;
        Pos = position;
        Body = World.CreateRectangle(Width, Height, 0, Pos, MathHelper.Pi / 4);
        Body.Tag = this;
    }

    public void LoadContent() {
        TextureClosed = Game.Content.Load<Texture2D>("Chest/chest_closed");
        TextureOpen = Game.Content.Load<Texture2D>("Chest/chest_open");
    }

    //Overwrite in subclasses
    public virtual void Loot() { }

    public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var dstRec = new Rectangle();
        dstRec = camera.getSpriteRectangle(Pos.X - spriteWidth*0.5f, Pos.Y+Height, spriteWidth, 1.31f*spriteWidth);
        if (Open)
            batch.Draw(TextureOpen, dstRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(dstRec.Y + dstRec.Height));
        else
            batch.Draw(TextureClosed, dstRec, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(dstRec.Y + dstRec.Height));
    }

    protected bool OnCollision(Fixture sender, Fixture other, Contact contact) {
        Body collider;
        if (sender.Body.Tag == this)
            collider = other.Body;
        else
            collider = sender.Body;
        if (collider.Tag == null) return true;
        ///player collision
        if (collider.Tag is Player)
            //TODO: play opening animation?
            Loot();
            Open = true;
        return true;
    }
}