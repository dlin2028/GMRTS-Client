using GMRTSClasses.CTSTransferData;
using GMRTSClasses.CTSTransferData.MetaActions;
using GMRTSClient.Component.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GMRTSClient.UI.ClientActions
{
    class BuildAction : UnitGroundAction
    {
        public BuildingType BuildingType;
        public BuildAction(List<Unit> units, Vector2 target, BuildingType buildingType, ContentManager content) : base(units, target)
        {
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
    }
}
