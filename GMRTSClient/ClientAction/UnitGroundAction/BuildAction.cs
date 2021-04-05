using GMRTSClasses.CTSTransferData;
using GMRTSClasses.CTSTransferData.MetaActions;
using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GMRTSClient.UI.ClientAction
{
    class BuildAction : UnitGroundAction
    {
        public BuildingType BuildingType { get; set; }

        public Sprite Sprite {
            get {
                switch (BuildingType)
                {
                    case BuildingType.Factory:
                        if (factorySprite == null)
                        {
                            factorySprite = new Sprite(content.Load<Texture2D>("Factory"));
                            factorySprite.Alpha = 0.5f;
                        }

                        return factorySprite;

                    case BuildingType.Supermarket:
                        if (supermarketSprite == null)
                        {
                            supermarketSprite = new Sprite(content.Load<Texture2D>("Market"));
                            supermarketSprite.Alpha = 0.5f;
                        }

                        return supermarketSprite;

                    case BuildingType.Mine:
                        if (mineSprite == null)
                        {
                            mineSprite = new Sprite(content.Load<Texture2D>("Mine"));
                            mineSprite.Alpha = 0.5f;
                        }

                        return mineSprite;
                }
                throw new Exception("BuildingType was invalid");
            }
        }

        private static Sprite factorySprite;
        private static Sprite mineSprite;
        private static Sprite supermarketSprite;

        private ContentManager content;

        public BuildAction(List<Unit> units, Vector2 target, BuildingType buildingType, ContentManager content) : base(units, target)
        {
            this.content = content;

            RenderColor = Color.DarkGoldenrod;
            ActionType = ActionType.Build;
            BuildingType = buildingType;
        }

        public override MetaAction ToDTOMetaAction()
        {
            return null;
        }

        public override GMRTSClasses.CTSTransferData.ClientAction ToDTONonmetaAction()
        {
            return new GMRTSClasses.CTSTransferData.UnitGround.BuildBuildingAction() { ActionID = ID, BuildingType = BuildingType, Position = new System.Numerics.Vector2(Position.X, Position.Y), UnitIDs = new List<Guid>() { Units.First().ID } };
        }
        public override GMRTSClasses.CTSTransferData.FactoryActions.FactoryAction ToDTOFactoryAction()
        {
            return null;
        }
    }
}
