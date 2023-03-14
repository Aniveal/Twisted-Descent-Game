using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class Rope {
    private Body body;
    private RopePoint _ropeStart;
    private RopePoint _ropeEnd;

    private RopeGame _game;
    private World _world;

    private Texture2D _baseTexture;
    private const int BaseLength = 100;
    private const int BaseWidth = 5;


    public Rope(RopeGame game, World world, Vector2 pos, int length, float strength, float weight) {
        this._game = game;
        this._world = world;
        createSegments(pos, length, length / 8, strength, weight);
        createBaseTexture();

        _ropeStart.setFixed(true);
    }

    private void createSegments(Vector2 pos, int targetLength, int segments, float strength, float weight) {
        RopePoint tmp = null;
        float weightPerSegment = targetLength / 100f * weight / segments;
        for (int i = 0; i < segments; i++) {
            RopePoint seg = new RopePoint(this, new Vector2(pos.X + (targetLength / segments - 1) * i, pos.Y),
                (targetLength / segments - 1), strength, weightPerSegment);
            if (_ropeStart == null) {
                _ropeStart = seg;
            }

            if (tmp != null) {
                tmp.setNext(seg);
            }

            seg.setLast(tmp);
            tmp = seg;
        }

        _ropeEnd = tmp;
    }

    private void createBaseTexture() {
        _baseTexture = new Texture2D(_game.GraphicsDevice, BaseLength, BaseWidth);
        Color[] data = new Color[_baseTexture.Width * _baseTexture.Height];
        for (int i = 0; i < data.Length; i++) {
            data[i] = Color.Chocolate;
        }

        _baseTexture.SetData(data);
    }

    public void Update(GameTime gameTime) {
        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed) {
            _ropeEnd.addForce(new Vector2(mouse.X, mouse.Y) - _ropeEnd.getPos());
            _ropeEnd.setPos(new Vector2(mouse.X, mouse.Y));

            //ropeEnd.addForce(new Vector2(100 * gameTime.ElapsedGameTime.Milliseconds / 1000f, 0));
        }

        if (mouse.RightButton == ButtonState.Pressed) {
            _ropeEnd.addForce(new Vector2(-100 * gameTime.ElapsedGameTime.Milliseconds / 1000f, 0));
        }

        
        updateForce(gameTime);
        updatePhysics(gameTime);
    }

    private void updatePhysics(GameTime gameTime) {
        RopePoint current = _ropeStart;

        while (current != null) {
            current.updatePhysics(gameTime);
            current = current.getNext();
        }
    }

    private void updateForce(GameTime gameTime) {
        RopePoint current = _ropeStart;

        while (current != null) {
            current.updateForce(gameTime);
            current = current.getNext();
        }
    }

    public void Draw(SpriteBatch batch) {
        RopePoint current = _ropeStart;

        while (current.getNext() != null) {
            drawBetweenSegments(current, current.getNext(), batch);
            current = current.getNext();
        }
    }

    private void drawBetweenSegments(RopePoint one, RopePoint two, SpriteBatch batch) {
        Vector2 dist = two.getPos() - one.getPos();
        float rotation = (float)Math.Atan2(dist.Y, dist.X);
        Vector2 scale = new Vector2(dist.Length() / BaseLength, 1);
        float sc = (one.targetLength / dist.Length());
        if (sc < 0) sc = 0;
        Color color = new Color(1 - sc, sc, 0);
        batch.Draw(_baseTexture, sourceRectangle: null, position: one.getPos(), scale: scale, rotation: rotation, color: color, origin: new Vector2(0, 2.5f), effects: SpriteEffects.None, layerDepth: 0.0f);
    }

    public void setEnd(Vector2 pos) {
        _ropeEnd.setPos(pos);
    }
}