using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using TwistedDescent.Theseus;

namespace TwistedDescent.Columns; 

public class ActivableColumn : Column {
    protected bool Activated;
    protected int NContacts;

    public ActivableColumn(World world, Vector2 position, float width, Texture2D texture) : base(world, position, width, texture) {
        Activated = false;
        NContacts = 0;
        Body.OnCollision += OnCollision;
        Body.OnSeparation += OnSeparation;
    }
    
    public ActivableColumn(World world, Vector2 position, float width, Texture2D texture, bool isSpear) : base(world, position, width, texture, isSpear) {
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
}