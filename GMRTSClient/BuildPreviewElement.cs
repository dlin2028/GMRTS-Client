using GMRTSClasses.CTSTransferData;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient
{
    class BuildPreviewElement : UIElement
    {
        private Texture2D factoryTexture;
        private Texture2D mineTexture;
        private Texture2D marketTexture;

        private BuildingType currentBuilding;
        private float scale;

        public BuildingType CurrentBuilding
        {
            get { return currentBuilding; }
            set { currentBuilding = value;

                switch (currentBuilding)
                {
                    case BuildingType.Factory:
                        Texture = factoryTexture;
                        break;
                    case BuildingType.Mine:
                        Texture = mineTexture;
                        break;
                    case BuildingType.Supermarket:
                        Texture = marketTexture;
                        break;
                    default:
                        break;
                }
                rect.Width = (int)(Texture.Width * scale);
                rect.Height = (int)(Texture.Height * scale);
            }
        }


        public BuildPreviewElement(Texture2D factoryTexture, Texture2D mineTexture, Texture2D marketTexture, float scale)
            :base(factoryTexture, new Rectangle(), Color.White)
        {
            this.scale = scale;
            this.factoryTexture = factoryTexture;
            this.mineTexture = mineTexture;
            this.marketTexture = marketTexture;
            CurrentBuilding = BuildingType.Factory;
        }

        protected override void update()
        {
            if(Enabled)
            {
                rect.X = (int)(InputManager.MouseState.Position.X - rect.Width / 2f);
                rect.Y = (int)(InputManager.MouseState.Position.Y - rect.Height / 2f);
            }
        }
    }
}
