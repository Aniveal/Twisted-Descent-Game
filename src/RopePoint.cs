using Microsoft.Xna.Framework;

namespace Meridian2;

public class RopePoint {
    private Rope mother; //Be strong for mother

    private RopePoint next;
    private RopePoint last;

    private Vector2 pos;
    private Vector2 force;
    private Vector2 postUpdateForce;
    private Color color;

    public int targetLength;
    private float strength;

    private bool isFix; //Wther the position can be changed or not
    private float mass;
    private Vector2 gravForce;

    public RopePoint(Rope mother, Vector2 pos, int targetLength, float strength, float weight) {
        this.pos = pos;
        this.targetLength = targetLength;
        this.strength = strength;
        this.mother = mother;

        isFix = false;

        force = Vector2.Zero;
        postUpdateForce = Vector2.Zero;
        mass = weight;
        gravForce = new Vector2(0, 9.81f * 50 * mass);
    }

    public void updateForce(GameTime time) {
        force -= force * 1f * time.ElapsedGameTime.Milliseconds / 1000f;

        Vector2 forceLeft = getForceTo(last);
        Vector2 forceRight = getForceTo(next);

        addForce(forceLeft + forceRight);
        //addForce(gravForce * time.ElapsedGameTime.Milliseconds / 1000f);
    }

    public void updatePhysics(GameTime time) {
        Vector2 newPos = pos + force * time.ElapsedGameTime.Milliseconds / 1000f;

        setPos(newPos);
    }

    private Vector2 getDistVector(Vector2 from, Vector2 to) {
        return to - from;
    }

    private Vector2 getForceTo(RopePoint seg) {
        Vector2 force = Vector2.Zero;

        if (seg != null) {
            Vector2 dist = seg.getPos() - getPos();
            float len = dist.Length();
            float overlength = len - targetLength;
            dist.Normalize();
            force = dist * overlength * strength;
        }

        return force;
    }

    public void setForce(Vector2 force) {
        this.force = force;
    }

    public void addForce(Vector2 force) {
        this.force += force;
    }

    public void setPos(Vector2 pos) {
        if (!isFix) {
            this.pos = pos;
        }
    }

    public Vector2 getPos() {
        return pos;
    }

    public void setLast(RopePoint last) {
        this.last = last;
    }

    public void setNext(RopePoint next) {
        this.next = next;
    }

    public RopePoint getNext() {
        return next;
    }

    public RopePoint getLast() {
        return last;
    }

    public void setFixed(bool fix) {
        this.isFix = fix;
    }

    public bool getFixed() {
        return this.isFix;
    }
}