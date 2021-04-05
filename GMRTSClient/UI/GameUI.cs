/* Generated by MyraPad at 3/16/2021 11:29:51 PM */
using GMRTSClasses.CTSTransferData;
using GMRTSClient.Component.Unit;
using GMRTSClient.UI.ClientAction;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;

namespace GMRTSClient.UI
{
	public class UIStatus
	{
		public ActionType CurrentAction { get; set; }
		public BuildingType CurrentBuilding { get; set; }
		public bool MouseHovering { get; set; }
        public UIStatus(ActionType currentAction, BuildingType currentBuilding, bool mouseHovering)
        {
			CurrentAction = currentAction;
			CurrentBuilding = currentBuilding;
			MouseHovering = mouseHovering;
        }

		public void Update(ActionType currentAction, BuildingType currentBuilding, bool mouseHovering)
		{
			CurrentAction = currentAction;
			CurrentBuilding = currentBuilding;
			MouseHovering = mouseHovering;
		}
	}
	[Flags] public enum BuildFlags
	{
		None,
		Building,
		Unit
	} 

	public partial class GameUI
	{
		private ActionType currentAction;

		private BuildFlags buildMenuFlags;

		public ActionType CurrentAction
        {
            get { return currentAction; }
            set {
				currentAction = value;
				resetActionButtons();
			}
        }

		public BuildFlags BuildMenuFlags
		{
			get { return buildMenuFlags; }
			set
			{
				buildMenuFlags = value;
				updateBuildableUnits();
			}
		}
        private BuildingType currentBuilding;

        public BuildingType CurrentBuilding
        {
            get { return currentBuilding; }
            set {
				CurrentAction = ActionType.Build;
				currentBuilding = value;
			}
        }


        List<ImageButton> activeBuildButtons;
		List<ImageButton> buildButtons;

		public GameUI()
		{
			BuildUI();

			MoveButton.Click += (s, a) => { CurrentAction = MoveButton.IsPressed ? ActionType.Move : ActionType.None; };
			AssistButton.Click += (s, a) => { CurrentAction = AssistButton.IsPressed ? ActionType.Assist : ActionType.None; };
			AttackButton.Click += (s, a) => { CurrentAction = AttackButton.IsPressed ? ActionType.Attack : ActionType.None; };
			PatrolButton.Click += (s, a) => { CurrentAction = PatrolButton.IsPressed ? ActionType.Patrol : ActionType.None; };

			BuildFactoryButton.Click += (s, a) => { CurrentBuilding = BuildingType.Factory; };
			BuildMineButton.Click += (s, a) => { CurrentBuilding = BuildingType.Mine; };

			buildButtons = new List<ImageButton>();
			buildButtons.Add(BuildFactoryButton);
			buildButtons.Add(BuildMineButton);
			buildButtons.Add(BuildMarketButton);
			buildButtons.Add(BuildTankButton);
			buildButtons.Add(BuildBuilderButton);

			BuildMenuFlags = BuildFlags.None;
		}

		private void resetActionButtons()
        {
			MoveButton.IsPressed = false;
			AssistButton.IsPressed = false;
			AttackButton.IsPressed = false;
			PatrolButton.IsPressed = false;

			switch (CurrentAction)
			{
				case ActionType.Move:
					MoveButton.IsPressed = !MoveButton.IsPressed;
					break;
				case ActionType.Attack:
					AttackButton.IsPressed = !AttackButton.IsPressed;
					break;
				case ActionType.Assist:
					AssistButton.IsPressed = !AssistButton.IsPressed;
					break;
				case ActionType.Patrol:
					PatrolButton.IsPressed = !PatrolButton.IsPressed;
					break;
				default:
					break;
			}
		}

		private void updateBuildableUnits()
        {
            foreach (var button in buildButtons)
            {
				button.Visible = false;
            }
			activeBuildButtons = new List<ImageButton>();
			if(buildMenuFlags.HasFlag(BuildFlags.Building))
            {
				activeBuildButtons.Add(BuildFactoryButton);
				activeBuildButtons.Add(BuildMineButton);
				activeBuildButtons.Add(BuildMarketButton);
            }
			if(buildMenuFlags.HasFlag(BuildFlags.Unit))
            {
				activeBuildButtons.Add(BuildTankButton);
				activeBuildButtons.Add(BuildBuilderButton);
            }

            for (int i = 0; i < activeBuildButtons.Count; i++)
            {
				activeBuildButtons[i].GridColumn = i;
				activeBuildButtons[i].Visible = true;
            }
		}
	}
}