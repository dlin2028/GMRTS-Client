using Microsoft.Xna.Framework;

namespace GMRTSClient.Units
{
    internal interface ISelectable
    {
        public Rectangle SelectionRect { get; } 
        
        public bool Selected { get; set; }
    }
}