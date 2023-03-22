using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Meridian2
{
    public class ActivableColumn : Column
    {
        protected bool _activated;
        protected int _nContacts;
        public ActivableColumn(RopeGame game, World world, Vector2 center, float radius, Texture2D texture) : base(game, world, center, radius, texture)
        {
            _activated = false;
            _nContacts = 0;
            Body.OnCollision += OnCollision;
            Body.OnSeparation += OnSeparation;
        }

        //TODO: differentiate string contacts from other contacts
        private bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            _nContacts++;
            _activated = true;
            return true;
        }

        private void OnSeparation(Fixture sender, Fixture other, Contact contact) 
        {
            if (--_nContacts == 0)
            {
                _activated = false;
            }
        }

        public new void Draw(SpriteBatch batch)
        {
            if (_activated)
            {
                //draw activated column
                batch.Draw(_columnTexture, new Rectangle((int)(_center.X - _radius), (int)(_center.Y - _radius), (int)_radius * 2, (int)_radius * 2), Color.Yellow);
            }
            else
            {
                //draw noraml column
                base.Draw(batch);
            }
        }
    }
}
