using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2 {
    public class Camera {
        private readonly GraphicsDevice _graphicsDevice;
        private float _zoom;
        private Matrix _transform;
        public Vector2 Pos { get; set; }

        private readonly Matrix IsometricToEuclidean = Matrix.Identity; // TODO: Figure out isometric transformation matrix
        
        public Camera(GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;
            _zoom = 1.0f;
            Pos = Vector2.Zero;
        }

        public float Zoom {
            get { return _zoom; }
            set {
                _zoom = value;
                if (_zoom < 0.1f) _zoom = 0.1f;
            } // Negative zoom will flip image
        }

        public void Move(Vector2 amount) {
            Pos += amount;
        }

        public Matrix GetTransformation(GraphicsDevice graphicsDevice) {
            _transform =
                Matrix.CreateTranslation(new Vector3(-Pos.X, -Pos.Y, 0)) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f,
                    graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }
    }
}