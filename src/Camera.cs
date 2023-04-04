using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2 {
    public class Camera {
        private readonly GraphicsDevice _graphicsDevice;
        private float _scale;
        private Matrix _transform;
        public Vector2 Pos { get; set; }

        // private readonly Matrix IsometricToEuclidean = Matrix.CreateRotationX((float)-Math.PI / 4) *
        //                                                Matrix.CreateRotationY((float)-Math.PI / 4) *
        //                                                Matrix.CreateScale(1, 0.5f, 1);
        
        public Camera(GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;
            _scale = 1.0f;
            Pos = Vector2.Zero;
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
            
            return new Rectangle((int)(Pos.X - _scale * x), (int)(Pos.Y  -_scale * y / 2), (int)w, (int)h);
        } 
        
        public void Move(Vector2 amount) {
            Pos += amount;
        }
    }
}