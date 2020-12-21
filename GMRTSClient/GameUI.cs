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
        public List<UnitAction> Actions;

        private Camera camera;
        private SelectionRectangle selectionRect;

        private UIElement moveButton;
        private UIElement attackButton;
        private UIElement assistButton;
        private UIElement patrolButton;
        private List<UIElement> actionButtons = new List<UIElement>();

        private ActionType currentAction;

        private List<ClientAction> pendingActions;

        private Texture2D pixel;
        private Texture2D circle;

        public GameUI(Camera camera, GraphicsDevice graphics, Texture2D pixel, Texture2D circle)
        {
            Actions = new List<UnitAction>();
            pendingActions = new List<ClientAction>();

            this.pixel = pixel;
            this.circle = circle;
            this.camera = camera;
            selectionRect = new SelectionRectangle(camera, pixel);

            moveButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 400, graphics.Viewport.Height - 100, 100, 100), Color.Blue);
            moveButton.onClick += (sender, e) => { foreach (var button in actionButtons) button.Release(); moveButton.color = Color.DarkBlue; currentAction = ActionType.Move; };
            moveButton.onRelease += (sender, e) => moveButton.color = Color.Blue;

            attackButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 300, graphics.Viewport.Height - 100, 100, 100), Color.Red);
            attackButton.onClick += (sender, e) => { foreach (var button in actionButtons) button.Release(); attackButton.color = Color.DarkRed; currentAction = ActionType.Attack; };
            attackButton.onRelease += (sender, e) => attackButton.color = Color.Red;

            assistButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 200, graphics.Viewport.Height - 100, 100, 100), Color.Yellow);
            assistButton.onClick += (sender, e) => { foreach (var button in actionButtons) button.Release(); assistButton.color = Color.Gold; currentAction = ActionType.Assist; };
            assistButton.onRelease += (sender, e) => assistButton.color = Color.Yellow;


            patrolButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 100, graphics.Viewport.Height - 100, 100, 100), Color.Green);
            patrolButton.onClick += (sender, e) => { foreach (var button in actionButtons) button.Release(); patrolButton.color = Color.DarkGreen; currentAction = ActionType.Patrol; };
            patrolButton.onRelease += (sender, e) => patrolButton.color = Color.Green;

            actionButtons.Add(moveButton);
            actionButtons.Add(attackButton);
            actionButtons.Add(assistButton);
            actionButtons.Add(patrolButton);
        }

        public void RemoveAction(UnitAction action, Unit unit)
        {
            unit.Orders.Remove(action);
            action.Units.Remove(unit);
            if(action.Units.Count <= 0)
            {
                Actions.Remove(action);
            }
        }

        public void Update(Unit[] units, GameTime gameTime)
        {
            selectionRect.Update(units, actionButtons.ToArray());

            if (InputManager.MouseState.RightButton == ButtonState.Released && InputManager.LastMouseState.RightButton == ButtonState.Pressed)
            {
                var mouseWorldPos = camera.ScreenToWorldSpace(InputManager.MouseState.Position.ToVector2());

                if (InputManager.Keys.IsKeyDown(Keys.LeftShift) && InputManager.Keys.IsKeyDown(Keys.LeftControl))
                {
                    for (int i = 0; i < Actions.Count; i++)
                    {
                        if (Actions[i].Intersecting(mouseWorldPos))
                        {
                            pendingActions.Add(new DeleteAction(Actions[i].ID));
                            foreach (var unit in Actions[i].Units)
                            {
                                unit.Orders.Remove(Actions[i]);
                            }
                            Actions.RemoveAt(i);
                        }
                    }
                }
                else if (selectionRect.SelectedUnits.Count() > 0)
                {
                    if (!InputManager.Keys.IsKeyDown(Keys.LeftShift))
                    {
                        List<UnitAction> oldOrders = new List<UnitAction>();

                        foreach (var unit in selectionRect.SelectedUnits)
                        {
                            var currOrder = unit.Orders.First;
                            while (currOrder != null)
                            {
                                currOrder.Value.Units.Remove(unit);

                                if (!oldOrders.Contains(currOrder.Value))
                                {
                                    oldOrders.Add(currOrder.Value);
                                }
                                currOrder = currOrder.Next;

                                if (currOrder != null)
                                    unit.Orders.Remove(currOrder.Previous);
                                else
                                {
                                    unit.Orders.RemoveLast();
                                }
                            }
                        }
                        pendingActions.AddRange(oldOrders.Select(x => new ReplaceAction(x)));
                    }

                    UnitAction newAction;
                    switch (currentAction)
                    {
                        case ActionType.None:
                            var target = units.FirstOrDefault(x => x.Intersecting(mouseWorldPos));
                            if (target != null) //&& unit is enemy
                            {
                                newAction = new AttackAction(selectionRect.SelectedUnits, pixel, target, circle);
                                Actions.Add(newAction);
                                pendingActions.Add(newAction);
                                break;
                            }
                            else if (target != null) //&&unit is friendly
                            {
                                newAction = new AssistAction(selectionRect.SelectedUnits, pixel, target, circle);
                                Actions.Add(newAction);
                                pendingActions.Add(newAction);
                                break;
                            }
                            newAction = new MoveAction(selectionRect.SelectedUnits, pixel, mouseWorldPos, circle);
                            Actions.Add(newAction);
                            pendingActions.Add(newAction);
                            break;
                        case ActionType.Move:
                            newAction = new MoveAction(selectionRect.SelectedUnits, pixel, mouseWorldPos, circle);
                            Actions.Add(newAction);
                            pendingActions.Add(newAction);
                            break;
                        case ActionType.Attack:
                            var attackTarget = units.FirstOrDefault(x => x.Intersecting(mouseWorldPos));
                            if (attackTarget != null) //&& unit is enemy
                            {
                                newAction = new AttackAction(selectionRect.SelectedUnits, pixel, attackTarget, circle);
                                Actions.Add(newAction);
                                pendingActions.Add(newAction);
                            }
                            break;
                        case ActionType.Assist:
                            var assistTarget = units.FirstOrDefault(x => x.Intersecting(mouseWorldPos));
                            if (assistTarget != null) //&&unit is friendly
                            {
                                newAction = new AssistAction(selectionRect.SelectedUnits, pixel, assistTarget, circle);
                                Actions.Add(newAction);
                                pendingActions.Add(newAction);
                            }
                            break;
                        case ActionType.Patrol:
                            Actions.Add(new PatrolAction(selectionRect.SelectedUnits, pixel, mouseWorldPos, circle));
                            break;
                        default:
                            break;
                    }

                    if (!InputManager.Keys.IsKeyDown(Keys.LeftShift))
                    {
                        selectionRect.DeselectAll();
                    }
                }

            }

            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i].Units.Count > 0)
                {
                    Actions[i].Update(gameTime);
                }
                else
                {
                    Actions.RemoveAt(i--);
                }
            }


            foreach (var element in actionButtons)
            {
                element.Update();
            }
        }

        public List<ClientAction> GetPendingActions()
        {
            var output = new List<ClientAction>(pendingActions);
            pendingActions.Clear();
            return output;
        }

        public void DrawWorld(SpriteBatch sb)
        {
            foreach (UnitAction action in Actions.Where(x => x is UnitAction))
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
