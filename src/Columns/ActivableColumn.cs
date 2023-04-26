using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Meridian2.Columns; 

public class ActivableColumn : Column {
    protected bool Activated;
    protected int NContacts;

    public ActivableColumn(RopeGame game, World world, Vector2 center, float radius, Texture2D texture) : base(game,
        world, center, radius, texture) {
        Activated = false;
        NContacts = 0;
        Body.OnCollision += OnCollision;
        Body.OnSeparation += OnSeparation;
    }

    public ActivableColumn(RopeGame game, World world, Vector2 center, float radius, ColumnTextures texture) : base(
        game, world, center, radius, texture) {
        Activated = false;
        NContacts = 0;
        Body.OnCollision += OnCollision;
        Body.OnSeparation += OnSeparation;
    }

    protected bool OnCollision(Fixture sender, Fixture other, Contact contact) {
        if (sender.Body.Tag != null && sender.Body.Tag is RopeSegment) {
            var segment = (RopeSegment)sender.Body.Tag;
            NContacts++;
            segment.ColumnCallback(this, true, !Activated);
            Activated = true;
        } else if (other.Body.Tag != null && other.Body.Tag is RopeSegment) {
            var segment = (RopeSegment)other.Body.Tag;
            NContacts++;
            segment.ColumnCallback(this, true, !Activated);
            Activated = true;
        }

        return true;
    }

    protected void OnSeparation(Fixture sender, Fixture other, Contact contact) {
        if (sender.Body.Tag != null && sender.Body.Tag is RopeSegment) {
            var segment = (RopeSegment)sender.Body.Tag;
            if (--NContacts == 0) Activated = false;
            segment.ColumnCallback(this, false, !Activated);
        } else if (other.Body.Tag != null && other.Body.Tag is RopeSegment) {
            var segment = (RopeSegment)other.Body.Tag;
            if (--NContacts == 0) Activated = false;
            segment.ColumnCallback(this, false, !Activated);
        }
    }

    // public override void DrawFirst(GameTime gameTime, SpriteBatch batch, Camera camera)
    // {
    //     if (_activated)
    //     {
    //         Rectangle dstRec = camera.getScreenRectangle(_center.X - _radius, _center.Y - _radius, _radius * 2, _radius*2, true);
    //         //draw activated column
    //         batch.Draw(_columnTexture, dstRec, Color.Yellow);
    //     }
    //     else
    //     {
    //         //draw noraml column
    //         base.DrawFirst(gameTime, batch, camera);
    //     }
    // }

    // public override void DrawSecond(GameTime gameTime, SpriteBatch batch, Camera camera)
    // {
    //     //TODO: update once sprites are available
    // }
}