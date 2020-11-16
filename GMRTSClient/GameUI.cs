using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient
{
    class GameUI
    {
        private Camera camera;
        private SelectionRectangle selectionRect;

        private UIElement moveButton;
        private UIElement attackButton;
        private UIElement assistButton;
        private UIElement patrolButton;
        private List<UIElement> actionButtons = new List<UIElement>();

        private ActionType currentAction;

        private List<UnitAction> actions;


        private Texture2D pixel;

        public GameUI(Camera camera, GraphicsDevice graphics, Texture2D pixel)
        {
            actions = new List<UnitAction>();

            this.pixel = pixel;
            this.camera = camera;
            selectionRect = new SelectionRectangle(camera, pixel);

            moveButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 400, graphics.Viewport.Height - 100, 100, 100), Color.Blue);
            moveButton.onClick += (sender, e) => {  foreach (var button in actionButtons) button.Release();  moveButton.color = Color.DarkBlue; currentAction = ActionType.Move;};
            moveButton.onRelease += (sender, e) =>  moveButton.color = Color.Blue;

            attackButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 300, graphics.Viewport.Height - 100, 100, 100), Color.Red);
            attackButton.onClick += (sender, e) => {  foreach (var button in actionButtons) button.Release(); attackButton.color = Color.DarkRed; currentAction = ActionType.Attack; };
            attackButton.onRelease += (sender, e) =>  attackButton.color = Color.Red;

            assistButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 200, graphics.Viewport.Height - 100, 100, 100), Color.Yellow);
            assistButton.onClick += (sender, e) => {  foreach (var button in actionButtons) button.Release(); assistButton.color = Color.Gold; currentAction = ActionType.Assist; };
            assistButton.onRelease += (sender, e) =>  assistButton.color = Color.Yellow;


            patrolButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 100, graphics.Viewport.Height - 100, 100, 100), Color.Green);
            patrolButton.onClick += (sender, e) => {  foreach (var button in actionButtons) button.Release(); patrolButton.color = Color.DarkGreen; currentAction = ActionType.Patrol; };
            patrolButton.onRelease += (sender, e) => patrolButton.color = Color.Green;

            actionButtons.Add(moveButton);
            actionButtons.Add(attackButton);
            actionButtons.Add(assistButton);
            actionButtons.Add(patrolButton);
        }

        public void Update(Unit[] units)
        {
            if(InputManager.MouseState.RightButton == ButtonState.Released && InputManager.LastMouseState.RightButton == ButtonState.Pressed)
            {
                var mouseWorldPos = camera.ScreenToWorldSpace(InputManager.MouseState.Position.ToVector2());
                switch (currentAction)
                {
                    case ActionType.None:
                        break;
                    case ActionType.Move:
                            actions.Add(new MoveAction(selectionRect.SelectedUnits, pixel, mouseWorldPos));
                        break;
                    case ActionType.Attack:
                            var target = units.FirstOrDefault(x => x.GetSelectionRect().Contains(mouseWorldPos));
                            if(target != null)
                                actions.Add(new AttackAction(selectionRect.SelectedUnits, pixel, target));
                        break;
                    case ActionType.Assist:
                            var target2 = units.FirstOrDefault(x => x.GetSelectionRect().Contains(mouseWorldPos));
                            if (target2 != null)
                                actions.Add(new AssistAction(selectionRect.SelectedUnits, pixel, target2));
                        break;
                    case ActionType.Patrol:
                            actions.Add(new PatrolAction(selectionRect.SelectedUnits, pixel, camera.ScreenToWorldSpace(mouseWorldPos)));
                        break;
                    default:
                        break;
                }
            }

            foreach (var action in actions)
            {
                action.Update();
            }

            foreach (var element in actionButtons)
            {
                element.Update();
            }

            selectionRect.Update(units, actionButtons.ToArray());
        }

        public void DrawWorld(SpriteBatch sb)
        {
            foreach (var action in actions)
            {
                action.Draw(sb);
            }
            selectionRect.Draw(sb);
        }

        public void Draw(SpriteBatch sb)
        {
            moveButton.Draw(sb);
            attackButton.Draw(sb);
            assistButton.Draw(sb);
            patrolButton.Draw(sb);
        }
    }
}
