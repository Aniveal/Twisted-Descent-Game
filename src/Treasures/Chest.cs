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
    protected Texture2D LootTexture;
    protected float Width = 1.5f;
    protected float Height = 1f;
    protected float spriteWidth = 1.8f;
    protected float spriteHeight;
    protected World World;
    
    protected Texture2D TextureAnimation;
    protected double AnimationStart;
    protected double FadeEnd;
    protected double FadeDuration = 1000;

    //TODO: modify usage of width and height in Body and draw to fit to proportions of future sprite
    public Chest(RopeGame game, World world, Vector2 position) {
        Game = game;
        World = world;
        Pos = position;
        Body = World.CreateRectangle(Width, Height, 0, Pos, MathHelper.Pi / 4);
        Body.Tag = this;
        Body.OnCollision += OnCollision;
    }

    public void LoadContent() {
        TextureClosed = Game.Content.Load<Texture2D>("Sprites/Chest/chest_closed_updated");
        TextureOpen = Game.Content.Load<Texture2D>("Sprites/Chest/chest_open_updated");
        TextureAnimation = Game.Content.Load<Texture2D>("Sprites/Chest/chest_opening");
        
        spriteHeight = (TextureClosed.Height / (float)TextureClosed.Width) * spriteWidth;
    }

    //Overwrite in subclasses
    public virtual void Loot() 
    {
        
    }

    protected void DrawLoot(GameTime gameTime, SpriteBatch batch, Camera camera) {
        if (LootTexture != null) {
            
        }
    }
    
    public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var dstRec = new Rectangle();
        dstRec = camera.getSpriteRectangle(Pos.X - spriteWidth*0.5f, Pos.Y+Height*0.88f, spriteWidth, spriteHeight);
        if (Open) {
            if (AnimationStart == 0)
                AnimationStart = gameTime.TotalGameTime.TotalMilliseconds;

            var animFrame = (int)((gameTime.TotalGameTime.TotalMilliseconds - AnimationStart) / 100f);
            if (animFrame < 5) {
                var animRect = new Rectangle(animFrame * TextureOpen.Width, 0, TextureOpen.Width, TextureOpen.Height);
                batch.Draw(TextureAnimation, dstRec, animRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                    camera.getLayerDepth(dstRec.Y + dstRec.Height));
            } else {
                batch.Draw(TextureOpen, dstRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                    camera.getLayerDepth(dstRec.Y + dstRec.Height));

                if (FadeEnd == 0)
                    FadeEnd = gameTime.TotalGameTime.TotalMilliseconds + FadeDuration;

                var opacity = (FadeEnd - gameTime.TotalGameTime.TotalMilliseconds) / FadeDuration;

                if (opacity > 0) {
                    var animRect = new Rectangle(4 * TextureOpen.Width, 0, TextureOpen.Width, TextureOpen.Height);
                    batch.Draw(TextureAnimation, dstRec, animRect, new Color(255, 255, 255, (float)opacity), 0f, Vector2.Zero, SpriteEffects.None,
                        camera.getLayerDepth(dstRec.Y + dstRec.Height) + 0.01f);
                }
            }
        } else {
            batch.Draw(TextureClosed, dstRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(dstRec.Y + dstRec.Height));
        }
    }

    protected bool OnCollision(Fixture sender, Fixture other, Contact contact) {
        if (Open) return true;

        Body collider;
        if (sender.Body.Tag == this)
            collider = other.Body;
        else
            collider = sender.Body;
        if (collider.Tag == null) return true;
        ///player collision
        if (collider.Tag is Player) {
            Loot();
            Open = true;
        }
        return true;
    }
}