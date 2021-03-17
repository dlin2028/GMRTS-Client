using GMRTSClasses;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Component.Unit
{
    /// <summary>
    /// Units which don't rely on the server for transform data
    /// </summary>
    class ClientOnlyUnit : Unit
    {
        public ClientOnlyUnit(ContentManager content, Transform2 transform)
            : base(Guid.NewGuid(), new Sprite(content.Load<Texture2D>("Builder")))
        {
            Position = new GMRTSClasses.Changing<System.Numerics.Vector2>(new System.Numerics.Vector2(transform.Position.X, transform.Position.Y), System.Numerics.Vector2.Zero, Vector2Changer.VectorChanger, 0);
            Rotation = new GMRTSClasses.Changing<float>(transform.Rotation, 0.05f, FloatChanger.FChanger, 0);
        }
    }
}
