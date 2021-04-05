using GMRTSClient.UI.ClientAction;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component.Unit
{
    class Unit : GMRTSClasses.Units.Unit
    {
        public Sprite Sprite { get; set; }

        /// <summary>
        /// The UnitActions relevant to this unit
        /// </summary>
        public LinkedList<UnitAction> Orders { get; set; }


        public Unit(Guid id)
            :base(id)
        {
            Orders = new LinkedList<UnitAction>();
        }
    }
}
