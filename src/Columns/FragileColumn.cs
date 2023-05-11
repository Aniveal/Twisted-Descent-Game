using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns;

public class FragileColumn : ActivableColumn {
    private readonly Texture2D _brokenTexture;
    private readonly Texture2D _animationTexture;
    public static Texture2D ControlsTexture;
    private bool _animation;
    private bool _broken;

    private int AnimationFrameWidth = 1060;
    private double _animationStart = 0;
    private bool _showControls = false;

    public FragileColumn(World world, Vector2 position, float width, Texture2D texture, Texture2D brokenTexture) : base(
        world, position, width, texture) {
        _brokenTexture = brokenTexture;
        _broken = false;
        _animation = false;
    }

    public FragileColumn(World world, Vector2 position, float width, Texture2D texture, Texture2D brokenTexture,
        Texture2D animationTexture) : base(world, position, width, texture) {
        _brokenTexture = brokenTexture;
        _animationTexture = animationTexture;
        _broken = false;
        _animation = false;
    }

    public FragileColumn(World world, Vector2 position, float width, Texture2D texture, Texture2D brokenTexture,
        bool isSpear) : base(world, position, width, texture, isSpear) {
        _brokenTexture = brokenTexture;
        _broken = false;
        _animation = false;
    }

    // Spear with Animation
    public FragileColumn(World world, Vector2 position, float width, Texture2D texture, Texture2D brokenTexture,
        Texture2D animationTexture, bool isSpear) : base(world, position, width, texture, isSpear) {
        _brokenTexture = brokenTexture;
        _animationTexture = animationTexture;
        _broken = false;
        _animation = false;
        AnimationFrameWidth = 512;
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        if (!camera.IsVisible(Position)) return;
        Rectangle screenRec;
        if (_animation && _animationTexture != null) {
            if (_animationStart == 0) {
                _animationStart = gameTime.TotalGameTime.TotalMilliseconds;
            }

            var animationTime = gameTime.TotalGameTime.TotalMilliseconds - _animationStart;
            var frameLength = 100f;
            var animationIndex = (int)(animationTime / frameLength);

            var srcRect = new Rectangle(animationIndex * AnimationFrameWidth, 0,
                AnimationFrameWidth, ColumnTexture.Height);

            var animationScreenWidth = (float)AnimationFrameWidth / ColumnTexture.Width * Width;
            
            screenRec = camera.getScreenRectangle(Position.X - animationScreenWidth / 2f, Position.Y, animationScreenWidth, SpriteSize.Y);
            screenRec.Y -= (int)(screenRec.Height * OcclusionHeightFactor);
            
            batch.Draw(_animationTexture, screenRec, srcRect, Color.LightGray, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(screenRec.Y + screenRec.Height * OcclusionHeightFactor + 0.1f));
            
            if (animationIndex >= 7) {
                _animation = false;
            }
            if (AnimationFrameWidth == 512 && animationIndex >= 6){
                _animation = false;
            }
            
        }
        
        screenRec = camera.getScreenRectangle(Position.X - SpriteSize.X / 2f, Position.Y, SpriteSize.X, SpriteSize.Y);
        screenRec.Y -= (int)(screenRec.Height * OcclusionHeightFactor);
        
        if (_broken) {
            batch.Draw(_brokenTexture, screenRec, null, Color.LightGray, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(screenRec.Y + screenRec.Height * OcclusionHeightFactor));
        } else {
            batch.Draw(ColumnTexture, screenRec, null, Color.LightGray, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(screenRec.Y + screenRec.Height * OcclusionHeightFactor));

            if (_showControls && ControlsTexture != null) {
                var controlsSize = 48;
                var xMargin = (screenRec.Width - controlsSize) / 2;
                var yMargin = (controlsSize + 5) * -1;
                var controlsRect =
                    new Rectangle(screenRec.X + (int)xMargin, screenRec.Y + (int)yMargin, (int)controlsSize, (int)controlsSize);
                batch.Draw(ControlsTexture, controlsRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            }
        }
    }

    public void ShowControls() {
        _showControls = true;
    }
    
    public void HideControls() {
        _showControls = false;
    }
    
    public void Break() {
        if (_broken) return; //don't repeat animation
        _animation = true;
        _broken = true;
        Body.Enabled = false;
        SoundEngine.Instance.CollapseColumn(this.Body.Position);
        SoundEngine.Instance.FlingSound(this.Body.Position);
    }
}