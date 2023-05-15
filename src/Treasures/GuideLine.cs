using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using TwistedDescent.GameElements;

namespace TwistedDescent.Treasures;


public class GuideLine : IDrawableObject {

    private Vector2 a;
    private Vector2 b;
    private RopeGame _game;
    // private Texture2D line;
    // private float rotation;
    // private Vector2 o;
    private const int thickness = 4;

    public GuideLine(RopeGame game, Vector2 start, Vector2 end) {
        a = start;
        b = end;
        _game = game;
        //Code for texture taken from https://stackoverflow.com/questions/72913759/how-can-i-draw-lines-in-monogame
        //int distance = (int)Vector2.Distance(a, b);
        // line  = new Texture2D(game.GraphicsDevice, distance, thickness);
        // Color[] data = new Color[distance * thickness];
        // for (int i = 0; i < data.Length; i++) {
        //     data[i] = Color.Black;
        // }
        // line.SetData(data);

        // rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
        // o = new Vector2(0, thickness / 2);
    }

    public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
    {
        Vector2 aScreen = camera.getScreenPoint(a);
        Vector2 bScreen = camera.getScreenPoint(b);

        int distance = (int)Vector2.Distance(aScreen, bScreen);
        Texture2D line  = new Texture2D(_game.GraphicsDevice, distance, thickness);
        Color[] data = new Color[distance * thickness];
        for (int i = 0; i < data.Length; i++) {
            data[i] = Color.Blue;
        }
        line.SetData(data);

        Vector2 o = new Vector2(0, thickness / 2);
        float rot = (float)Math.Atan2(bScreen.Y - aScreen.Y, bScreen.X - aScreen.X);
        batch.Draw(line, aScreen, null, Color.White, rot, o, 1.0f, SpriteEffects.None, 0.245f);
        if (Keyboard.GetState().IsKeyDown(Keys.H)) {
            Console.WriteLine("a: "+a.ToString()+", aScreen: "+aScreen.ToString()+ ", playerpos: "+_game._gameScreen.TheseusManager.Player.Body.Position.ToString());
        }
    }
}