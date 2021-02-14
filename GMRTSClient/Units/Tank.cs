using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GMRTSClasses;
using GMRTSClasses.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GMRTSClient.Units
{
    /// <summary>
    /// Goes pew pew!
    /// </summary>
    class Tank : Unit
    {
        public Tank(Guid ID, ContentManager content)
            : base(ID, content.Load<Texture2D>("Tank"), content.Load<Texture2D>("SelectionMarker"))
        {
        }
    }
}
