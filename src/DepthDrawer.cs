using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2; 

/// <summary>
/// This class can be used as a drop-in replacement for SpriteBatch and properly handles the draw order of all sprites.
/// </summary>
public class DepthDrawer {
    private SpriteBatch _batch;
    /// <summary>
    /// A sorted list of DrawJobs ready to be filled into the SpriteBuffer. The key is the depth of the sprite.
    /// </summary>
    private SortedList<float, DrawJob> _depthBuffer;

    public DepthDrawer(GraphicsDevice graphicsDevice) {
        _batch = new SpriteBatch(graphicsDevice);
        _depthBuffer = new SortedList<float, DrawJob>();
    }

    private void DrawFromJob(DrawJob job) {
        
    }
    
    public void Begin() {
        _batch.Begin();
    }

    public void End() {
        // Put all draws sorted by depth into the sprite batch
        foreach (KeyValuePair<float,DrawJob> kvp in _depthBuffer) {
            var drawJob = kvp.Value;
            DrawFromJob(drawJob);
        }
        
        _batch.End();
    }
    
    public void Draw() {
        
    }
}