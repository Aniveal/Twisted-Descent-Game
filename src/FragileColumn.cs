﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Meridian2
{
    public class FragileColumn : ActivableColumn
    {
        bool _broken;
        public FragileColumn(RopeGame game, World world, Vector2 center, float radius, Texture2D texture) : base(game, world, center, radius, texture)
        {
            _broken = false;

        }

        public new void Draw(SpriteBatch batch)
        {
            if (_broken) 
            {
                //TODO: broken texture
                batch.Draw(_columnTexture, new Rectangle((int)(_center.X - _radius), (int)(_center.Y - _radius), (int)_radius * 2, (int)_radius * 2), Color.Orange);
            } else
            {
                base.Draw(batch);
            }
        }

        public void Break()
        {
            _broken = true;
            Body.Enabled = false;
            
        }
    }
}