using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2 {
    public class Camera {
        private readonly GraphicsDevice _graphicsDevice;
        private float _scale;
        private Vector2 _position;

        // private readonly Matrix IsometricToEuclidean = Matrix.CreateRotationX((float)-Math.PI / 4) *
        //                                                Matrix.CreateRotationY((float)-Math.PI / 4) *
        //                                                Matrix.CreateScale(1, 0.5f, 1);

        public Camera(GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;
            _scale = 60.0f;
            _position = Vector2.Zero;
        }

        public Vector2 Pos {
            get { return _position; }
            set { _position = value; }
        }

        public float Scale {
            get { return _scale; }
            set {
                _scale = value;
                if (_scale < 0.1f) _scale = 0.1f;
            } // Negative zoom will flip image
        }

        public Rectangle getScreenRectangle(float x, float y, float w, float h, bool scaleIsometric = false) {
            w *= _scale;
            h *= _scale;
            if (scaleIsometric) {
                h /= 2;
            }

            return new Rectangle((int)((x - _position.X) * _scale) + _graphicsDevice.Viewport.Width / 2, (int)((y - _position.Y) * _scale / 2) + _graphicsDevice.Viewport.Height / 2, (int)w, (int)h);
        }


        public void Move(Vector2 amount) {
            _position += amount;
        }
    }
}