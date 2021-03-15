using GMRTSClient.UI.ClientActions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component.Unit
{
    enum UnitType
    {
        Tank,
        Builder
    }
    /// <summary>
    /// The base class for all units
    /// </summary>
    abstract class Unit : GMRTSClasses.Units.Unit
    {
        public Sprite Sprite { get; set; }
        public UnitType UnitType { get; set; }

        /// <summary>
        /// The UnitActions relevant to this unit
        /// </summary>
        public LinkedList<UnitAction> Orders { get; set; }


        public Unit(Guid id, Sprite sprite)
            :base(id)
        {
            Sprite = sprite;
        }
    }
}
