using GMRTSClient.UI.ClientActions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Units
{
    enum UnitType
    {
        Tank,
        Builder
    }
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


        public Unit(Sprite sprite, Sprite selectSprite)
        {
            Sprite = sprite;
            SelectSprite = selectSprite;
        }
    }
}
