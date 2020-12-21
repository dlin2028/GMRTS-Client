using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GMRTSClient
{
    static class ExtensionMethods
    {
        public static Rectangle ScreenToWorldSpace(this Rectangle rect, Camera cam)
        {
            var topRight = cam.ScreenToWorldSpace(rect.Location.ToVector2()).ToPoint();
            var bottomLeft = cam.ScreenToWorldSpace((rect.Location + rect.Size).ToVector2()).ToPoint();

            return new Rectangle(topRight, bottomLeft - topRight);
        }
    }
}
