using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns; 

public class Column : DrawableGameElement {
    public Body Body;
    protected Vector2 Center;
    protected Texture2D ColumnTexture;
    protected RopeGame Game;
    protected Texture2D LowerTexture;
    protected bool MultiTexture;
    protected float Radius;
    protected SpriteBatch SpriteBatch;
    protected Texture2D UpperTexture;
    protected World World;

    public Column(RopeGame game, World world, Vector2 center, float radius, Texture2D texture) {
        Game = game;
        World = world;
        Center = center;
        Radius = radius;
        ColumnTexture = texture;
        MultiTexture = false;
        Body = World.CreateCircle(Radius, 0, Center);
    }

    public Column(RopeGame game, World world, Vector2 center, float radius, ColumnTextures texture) {
        Game = game;
        World = world;
        Center = center;
        Radius = radius;
        LowerTexture = texture.Lower;
        UpperTexture = texture.Upper;
        MultiTexture = true;
        Body = World.CreateCircle(Radius, 0, Center);
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        if (MultiTexture) {
            var dstRec = camera.getSpriteRectangle(Center.X - 2 * Radius, Center.Y + Radius, Radius * 4, Radius * 8);
            //batch.Draw(_lowerTexture, dstRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, camera.getLayerDepth(dstRec.Y + dstRec.Height));
            batch.Draw(UpperTexture, dstRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(dstRec.Y + dstRec.Height));
        } else {
            var dstRec = camera.getScreenRectangle(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2, true);
            batch.Draw(ColumnTexture, dstRec, null, Color.Gray, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(dstRec.Y + dstRec.Height));
        }
    }
}