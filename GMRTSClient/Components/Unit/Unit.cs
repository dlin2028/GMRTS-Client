using GMRTSClient.UI.ClientActions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Components.Unit
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
        public Sprite SelectSprite { get; set; }
        public UnitType UnitType { get; set; }

        /// <summary>
        /// The UnitActions relevant to this unit
        /// </summary>
        public LinkedList<UnitAction> Orders { get; set; }
        /// <summary>
        /// Whether or not the unit is currently selected
        /// </summary>
        public bool Selected { get; set; }


        public Unit(Guid id, Sprite sprite, Sprite selectSprite)
            :base(id)
        {
            Sprite = sprite;
            SelectSprite = selectSprite;
        }
    }
}
