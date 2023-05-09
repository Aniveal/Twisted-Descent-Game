using System.Collections.Generic;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Columns; 

/**
 * Propagates Update(), DrawFirst(), DrawSecond() to all columns
 * 
 * TODO: add a system to only work on columns on or close to the screen once we have a bigger map
 * This will also bring up the question of when to initialize the columns, currently done by the columns in thei constructor
 */
public class ColumnsManager {
    public readonly List<DrawableGameElement> Columns;
    private readonly RopeGame _game;

    public ColumnsManager(RopeGame game) {
        Columns = new List<DrawableGameElement>();
        _game = game;
    }

    public void Add(DrawableGameElement column) {
        Columns.Add(column);
    }

    public void Remove(DrawableGameElement column) {
        Columns.Remove(column);
    }

    public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        foreach (var element in Columns) element.Draw(gameTime, batch, camera);
    }

    public void Update(GameTime gameTime) {
        foreach (var element in Columns) element.Update(gameTime);
    }

    public void LoadContent() {
        FragileColumn.ControlsTexture =
            _game.Content.Load<Texture2D>("Sprites/Controller/Triggers/button_xbox_analog_trigger_dark_2");
    }
    
    public void Clear()
    {
        Columns.Clear();
    }
}