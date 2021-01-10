using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient
{
    public enum BuildingType
    {
        Factory,
        Mine,
        Market
    }
    class BuildPreviewElement
    {
        private Rectangle rect;
        private Texture2D factoryTexture;
        private Texture2D mineTexture;
        private Texture2D marketTexture;
        private Texture2D currentTexture;

        public bool Enabled;
        private BuildingType currentBuilding;
        private float scale;

        public BuildingType CurrentBuilding
        {
            get { return currentBuilding; }
            set { currentBuilding = value;

                switch (currentBuilding)
                {
                    case BuildingType.Factory:
                        currentTexture = factoryTexture;
                        break;
                    case BuildingType.Mine:
                        currentTexture = mineTexture;
                        break;
                    case BuildingType.Market:
                        currentTexture = marketTexture;
                        break;
                    default:
                        break;
                }
                rect.Width = (int)(currentTexture.Width * scale);
                rect.Height = (int)(currentTexture.Height * scale);
            }
        }


        public BuildPreviewElement(Texture2D factoryTexture, Texture2D mineTexture, Texture2D marketTexture, float scale)
        {
            this.scale = scale;
            this.factoryTexture = factoryTexture;
            this.mineTexture = mineTexture;
            this.marketTexture = marketTexture;
            rect = new Rectangle();
            CurrentBuilding = BuildingType.Factory;
        }

        public void Update()
        {
            if(Enabled)
            {
                rect.X = (int)(InputManager.MouseState.Position.X - rect.Width / 2f);
                rect.Y = (int)(InputManager.MouseState.Position.Y - rect.Height / 2f);
            }
        }

        public void Draw(SpriteBatch sb, BuildingType buildingType)
        {
            if(Enabled)
            {
                sb.Draw(currentTexture, rect, Color.White);
            }
        }
    }
}
