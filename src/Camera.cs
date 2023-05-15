using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwistedDescent; 

public class Camera {
    private readonly GraphicsDevice _graphicsDevice;
    private readonly float _inverseScale; //let's multiply instead of dividing
    private Vector2 _position;
    private float _scale;
    private Vector2 aa; //top left corner of the camera are in world coords
    private Vector2 bb; //bot right corner of the camera area in world coords
    private Vector2 cornerDist;

    // private readonly Matrix IsometricToEuclidean = Matrix.CreateRotationX((float)-Math.PI / 4) *
    //                                                Matrix.CreateRotationY((float)-Math.PI / 4) *
    //                                                Matrix.CreateScale(1, 0.5f, 1);

    public Camera(GraphicsDevice graphicsDevice) {
        _graphicsDevice = graphicsDevice;
        _scale = 60.0f;
        _inverseScale = 1 / _scale;
        _position = Vector2.Zero;
        cornerDist = getWorldPixel(Vector2.Zero) - new Vector2(10,10); //drawing stuff 10 units beyond camera range so everything gets drawn
    }

    //Deprecated for Set, use SetCameraPos instead
    public Vector2 Pos {
        get => _position;
        set => _position = value;
    }

    public void SetCameraPos(Vector2 pos) {
        _position = pos;
        aa = pos + cornerDist;
        bb = pos - cornerDist;
    }

    //Determines whether an object with radius < 10 is in the camera FOW
    //Does not accurately work for bigger objects, need another method if case arises
    public bool IsVisible(Vector2 pos) {
        if (pos.X < aa.X || pos.Y < aa.Y || pos.X > bb.X || pos.Y > bb.Y) {
            return false;
        }
        return true;
    }

    public float Scale {
        get => _scale;
        set {
            _scale = value;
            if (_scale < 0.1f) _scale = 0.1f;
        } // Negative zoom will flip image
    }


    /// <summary>
    ///     Returns layer depth between 0.25 and 0.75. This range is that there is space up and downwards (for example cliffs)
    /// </summary>
    public float getLayerDepth(float yScreen) {
        var screenFrac = yScreen / _graphicsDevice.Viewport.Height + float.Epsilon;

        if (screenFrac >= 1) screenFrac = 1.0f - float.Epsilon;

        screenFrac = screenFrac / 2.0f + 0.25f;

        return screenFrac;
    }

    /// <summary>
    ///     Gets the screen rectangle for the given rectangle in world space.
    /// </summary>
    /// <param name="x">The world x coordinate</param>
    /// <param name="y">The world y coordinate</param>
    /// <param name="w">The width of the object inside the world</param>
    /// <param name="h">The height of the object inside the world</param>
    /// <param name="scaleIsometric">
    ///     If the texture should be squeezed to fit the isometric projection. This basically halves
    ///     the rectangle height and should only be used for objects being parallel to the floor.
    /// </param>
    /// <returns>A rectangle in screen coordinates which can be directly fed into a draw call.</returns>
    public Rectangle getScreenRectangle(float x, float y, float w, float h, bool scaleIsometric = false) {
        w *= _scale;
        h *= _scale;
        if (scaleIsometric) h /= 2;

        return new Rectangle((int)((x - _position.X) * _scale) + _graphicsDevice.Viewport.Width / 2,
            (int)((y - _position.Y) * _scale / 2) + _graphicsDevice.Viewport.Height / 2, (int)w, (int)h);
    }

    /// <summary>
    ///     Gets the screen rectangle for the given rectangle in world space.
    /// </summary>
    /// <param name="x">The world x coordinate of the bottom left corner</param>
    /// <param name="y">The world y coordinate of the bottom left corner</param>
    /// <param name="w">The width of the object inside the world</param>
    /// <param name="h">The height of the object in the world</param>
    /// <returns>A rectangle in screen coordinates which can be directly fed into a draw call.</returns>
    public Rectangle getSpriteRectangle(float x, float y, float w, float h, bool scaleIsometric = false) {
        y = y - 2 * h;
        w *= _scale;
        h *= _scale;

        return new Rectangle((int)((x - _position.X) * _scale) + _graphicsDevice.Viewport.Width / 2,
            (int)((y - _position.Y) * _scale / 2) + _graphicsDevice.Viewport.Height / 2, (int)w, (int)h);
    }

    /// <summary>
    ///     Calculates the point on the screen for the given world coordinates.
    /// </summary>
    /// <param name="v">The world coordinates</param>
    /// <returns>The screen coordinates corresponding to the given world coordinates</returns>
    public Vector2 getScreenPoint(Vector2 v) {
        v.X = (v.X - _position.X) * _scale + _graphicsDevice.Viewport.Width * 0.5f;
        v.Y = (v.Y - _position.Y) * _scale * 0.5f + _graphicsDevice.Viewport.Height * 0.5f;
        return v;
    }

    public Vector2 getWorldPixel(Vector2 p) {
        var r = new Vector2();
        r.X = (p.X - _graphicsDevice.Viewport.Width * 0.5f) * _inverseScale + _position.X;
        r.Y = (p.Y - _graphicsDevice.Viewport.Height * 0.5f) * _inverseScale * 2 - _position.Y;
        return r;
    }


    public void Move(Vector2 amount) {
        _position += amount;
    }
}