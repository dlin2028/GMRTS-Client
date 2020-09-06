using Microsoft.Xna.Framework;

namespace GMRTSClient
{
    internal interface ISelectable
    {
        public Rectangle SelectionRect { get; set; }
        public void Select();
        public void Deselect();
    }
}