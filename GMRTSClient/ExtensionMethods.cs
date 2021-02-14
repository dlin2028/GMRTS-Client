using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GMRTSClient
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Transforms a rectangle from screen to world space
        /// </summary>
        /// <param name="rect">The rectangle to transform</param>
        /// <param name="cam">The camera used for world space</param>
        /// <returns></returns>
        public static Rectangle ScreenToWorldSpace(this Rectangle rect, Camera cam)
        {
            var topRight = cam.ScreenToWorldSpace(rect.Location.ToVector2()).ToPoint();
            var bottomLeft = cam.ScreenToWorldSpace((rect.Location + rect.Size).ToVector2()).ToPoint();

            return new Rectangle(topRight, bottomLeft - topRight);
        }
    }
}
