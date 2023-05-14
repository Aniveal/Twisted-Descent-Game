using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns;

internal class ElectricColumn : ActivableColumn {
    private readonly Texture2D _animationTexture;

    SoundEffectInstance SoundEffect;

    public ElectricColumn(World world, Vector2 position, float width, Texture2D texture) : base(world, position, width,
        texture) {
    }

    public ElectricColumn(World world, Vector2 position, float width, Texture2D texture, Texture2D animationTexture) :
        base(world, position, width, texture) {
        _animationTexture = animationTexture;
    }

    // electric spear without animation
    public ElectricColumn(World world, Vector2 position, float width, Texture2D texture, bool isSpear) : base(world,
        position, width, texture, isSpear) {
    }

    // electric spear with animation
    public ElectricColumn(World world, Vector2 position, float width, Texture2D texture, Texture2D animationTexture, bool isSpear) : base(world,
        position, width, texture, isSpear) {
        _animationTexture = animationTexture;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if(SoundEffect == null)
        {
            if (Activated)
            {
                SoundEffect = SoundEngine.Instance.GetElectroColumnInstance(Body.Position);
                SoundEffect.Volume = Math.Max(SoundEngine.Instance.CalculateIntensity(Body.Position) - 0.3f, 0.1f);
                SoundEffect.Play();  
            }
        }
        else
        {

            if (Activated)
            {
                if (SoundEffect.State == SoundState.Stopped)
                {
                    SoundEffect = SoundEngine.Instance.GetElectroColumnInstance(Body.Position);
                    SoundEffect.Volume = Math.Max(SoundEngine.Instance.CalculateIntensity(Body.Position) - 0.3f, 0.1f);
                    SoundEffect.Play();
                }
                else
                {
                    SoundEffect.Volume = Math.Max(SoundEngine.Instance.CalculateIntensity(Body.Position) - 0.3f, 0.1f);
                }
            }
            else SoundEffect = null;
        }
            

    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        if (!camera.IsVisible(Position)) return;
        var screenRec =
            camera.getScreenRectangle(Position.X - SpriteSize.X / 2f, Position.Y, SpriteSize.X, SpriteSize.Y);
        screenRec.Y -= (int)(screenRec.Height * OcclusionHeightFactor);

        batch.Draw(ColumnTexture, screenRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
            camera.getLayerDepth(screenRec.Y + screenRec.Height * OcclusionHeightFactor));
        
        // Draw animation if activated
        if (Activated && _animationTexture != null) {

            var totalTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            var animationLength = 100f;

            var animationIndex = (int)(totalTime / animationLength);

            bool isSpear = (ColumnTexture.Width == 512);

            if (isSpear) {
                animationIndex = animationIndex % 4;
            } else {
                animationIndex = animationIndex % 5;
            }

            var srcRect = new Rectangle(animationIndex * ColumnTexture.Width, 0,
                ColumnTexture.Width, ColumnTexture.Height);

            batch.Draw(_animationTexture, screenRec, srcRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(screenRec.Y + screenRec.Height * OcclusionHeightFactor + 0.1f));
        }
    }
}