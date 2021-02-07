using GMRTSClasses.CTSTransferData;

using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public GMRTSClasses.Changing<float> Money { get; set; }
        public GMRTSClasses.Changing<float> Minerals { get; set; }

        public List<UnitAction> Actions { get; set; }

        private Camera camera;
        private SelectionRectangle selectionRect;

        private Button moveButton;
        private Button attackButton;
        private Button assistButton;
        private Button patrolButton;

        private ToggleButton buildButton;

        private Button buildFactoryButton;
        private Button buildMarketButton;
        private Button buildMineButton;

        private List<Button> actionButtons = new List<Button>();
        private List<Button> buildButtons = new List<Button>();
        private List<UIElement> uiElements = new List<UIElement>();

        private ActionType currentAction;
        private BuildingType currentBuilding;

        private List<ClientAction> pendingActions;

        private Texture2D pixel;
        private Texture2D circle;

        private BuildPreviewElement buildPreview;


        ContentManager content;

        public GameUI(Camera camera, GraphicsDevice graphics, ContentManager content)
        {
            this.content = content;

            Actions = new List<UnitAction>();
            pendingActions = new List<ClientAction>();

            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });
            this.circle = content.Load<Texture2D>("circle");
            this.camera = camera;
            selectionRect = new SelectionRectangle(camera, pixel);

            buildPreview = new BuildPreviewElement(content.Load<Texture2D>("Factory"), content.Load<Texture2D>("Mine"), content.Load<Texture2D>("Market"), 0.25f);
            buildPreview.Enabled = false;

            moveButton = new Button(pixel, new Rectangle(graphics.Viewport.Width - 400, graphics.Viewport.Height - 100, 100, 100), Color.Blue);
            moveButton.onClick += (sender, e) => { foreach (var button in actionButtons) button.Release(); moveButton.Color = Color.DarkBlue; currentAction = ActionType.Move; };
            moveButton.onRelease += (sender, e) => moveButton.Color = Color.Blue;

            attackButton = new Button(pixel, new Rectangle(graphics.Viewport.Width - 300, graphics.Viewport.Height - 100, 100, 100), Color.Red);
            attackButton.onClick += (sender, e) => { foreach (var button in actionButtons) button.Release(); attackButton.Color = Color.DarkRed; currentAction = ActionType.Attack; };
            attackButton.onRelease += (sender, e) => attackButton.Color = Color.Red;

            assistButton = new Button(pixel, new Rectangle(graphics.Viewport.Width - 200, graphics.Viewport.Height - 100, 100, 100), Color.Yellow);
            assistButton.onClick += (sender, e) => { foreach (var button in actionButtons) button.Release(); assistButton.Color = Color.Gold; currentAction = ActionType.Assist; };
            assistButton.onRelease += (sender, e) => assistButton.Color = Color.Yellow;


            patrolButton = new Button(pixel, new Rectangle(graphics.Viewport.Width - 100, graphics.Viewport.Height - 100, 100, 100), Color.Green);
            patrolButton.onClick += (sender, e) => { foreach (var button in actionButtons) button.Release(); patrolButton.Color = Color.DarkGreen; currentAction = ActionType.Patrol; };
            patrolButton.onRelease += (sender, e) => patrolButton.Color = Color.Green;

            buildButton = new ToggleButton(pixel, new Rectangle(0, graphics.Viewport.Height - 100, 100, 100), Color.Gray);
            buildButton.OnToggleOn += (sender, e) => { foreach (var button in actionButtons) button.Enabled = false; foreach (var button in buildButtons) { button.Enabled = true; button.Release(); } buildButton.Color = Color.DarkGray; currentAction = ActionType.Build; buildPreview.Enabled = true; };
            buildButton.OnToggleOff += (sender, e) => { foreach (var button in actionButtons) { button.Enabled = true; button.Release(); } foreach (var button in buildButtons) button.Enabled = false; buildButton.Color = Color.Gray; buildPreview.Enabled = false; };

            buildFactoryButton = new Button(content.Load<Texture2D>("Factory"), new Rectangle(graphics.Viewport.Width - 300, graphics.Viewport.Height - 100, 100, 100), Color.White);
            buildFactoryButton.onClick += (sender, e) => { foreach (var button in buildButtons) button.Release(); moveButton.Color = Color.Gray; currentBuilding = BuildingType.Factory; buildPreview.CurrentBuilding = currentBuilding; };
            buildFactoryButton.onRelease += (sender, e) => moveButton.Color = Color.White;
            buildFactoryButton.Enabled = false;

            buildMarketButton = new Button(content.Load<Texture2D>("Market"), new Rectangle(graphics.Viewport.Width - 200, graphics.Viewport.Height - 100, 100, 100), Color.White);
            buildMarketButton.onClick += (sender, e) => { foreach (var button in buildButtons) button.Release(); attackButton.Color = Color.Gray; currentBuilding = BuildingType.Supermarket; buildPreview.CurrentBuilding = currentBuilding; };
            buildMarketButton.onRelease += (sender, e) => attackButton.Color = Color.White;
            buildMarketButton.Enabled = false;

            buildMineButton = new Button(content.Load<Texture2D>("Mine"), new Rectangle(graphics.Viewport.Width - 100, graphics.Viewport.Height - 100, 100, 100), Color.White);
            buildMineButton.onClick += (sender, e) => { foreach (var button in buildButtons) button.Release(); assistButton.Color = Color.Gray; currentBuilding = BuildingType.Mine; buildPreview.CurrentBuilding = currentBuilding; };
            buildMineButton.onRelease += (sender, e) => assistButton.Color = Color.White;
            buildMineButton.Enabled = false;

            actionButtons.Add(moveButton);
            actionButtons.Add(attackButton);
            actionButtons.Add(assistButton);
            actionButtons.Add(patrolButton);

            buildButtons.Add(buildFactoryButton);
            buildButtons.Add(buildMarketButton);
            buildButtons.Add(buildMineButton);

            uiElements.Add(buildButton);

            uiElements.AddRange(actionButtons);
            uiElements.AddRange(buildButtons);
            uiElements.Add(buildPreview);
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
            selectionRect.Update(units, uiElements.ToArray());

            if (InputManager.MouseState.RightButton == ButtonState.Released && InputManager.LastMouseState.RightButton == ButtonState.Pressed)
            {
                var mouseWorldPos = camera.ScreenToWorldSpace(InputManager.MouseState.Position.ToVector2());

                if (InputManager.Keys.IsKeyDown(Keys.LeftShift) && InputManager.Keys.IsKeyDown(Keys.LeftControl))
                {
                    for (int i = 0; i < Actions.Count; i++)
                    {
                        if (Actions[i].Intersecting(mouseWorldPos))
                        {
                            pendingActions.Add(new DeleteAction(Actions[i].Units.ToArray(), Actions[i]));
                            foreach (var unit in Actions[i].Units)
                            {
                                unit.Orders.Remove(Actions[i]);
                            }
                            Actions.RemoveAt(i);
                        }
                    }
                }
                else if (selectionRect.SelectedUnits.Count > 0)
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
                        pendingActions.AddRange(oldOrders.Select(x => new DeleteAction(selectionRect.SelectedUnits.ToArray(), x)));
                    }

                    UnitAction newAction;
                    switch (currentAction)
                    {
                        case ActionType.None:
                            var target = units.FirstOrDefault(x => x.Rect.Intersecting(mouseWorldPos));
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
                            var attackTarget = units.FirstOrDefault(x => x.Rect.Intersecting(mouseWorldPos));
                            if (attackTarget != null) //&& unit is enemy
                            {
                                newAction = new AttackAction(selectionRect.SelectedUnits, pixel, attackTarget, circle);
                                Actions.Add(newAction);
                                pendingActions.Add(newAction);
                            }
                            break;
                        case ActionType.Assist:
                            var assistTarget = units.FirstOrDefault(x => x.Rect.Intersecting(mouseWorldPos));
                            if (assistTarget != null) //&&unit is friendly
                            {
                                newAction = new AssistAction(selectionRect.SelectedUnits, pixel, assistTarget, circle);
                                Actions.Add(newAction);
                                pendingActions.Add(newAction);
                            }
                            break;
                        case ActionType.Patrol:
                            List<(UnitAction?, IEnumerable<Unit>)> prevActions = new List<(UnitAction?, IEnumerable<Unit>)>();
                            foreach (var uniqueAction in selectionRect.SelectedUnits.Select(x => x.Orders).Select(x => x.LastOrDefault()).Distinct())
                            {
                                prevActions.Add((uniqueAction, selectionRect.SelectedUnits.Where(x => x.Orders.LastOrDefault() == uniqueAction)));
                            }
                            foreach (var prevAction in prevActions)
                            {
                                if(prevAction.Item1 == null)
                                {
                                    var posList = prevAction.Item2.Select(x => x.CurrentPosition);
                                    var avgpos = new Vector2(posList.Average(x => x.X), posList.Average(x => x.Y));
                                    newAction = new PatrolAction(prevAction.Item2.ToList(), pixel, avgpos, circle);
                                    Actions.Add(newAction);
                                    pendingActions.Add(newAction);
                                }
                                else
                                {
                                    newAction = new PatrolAction(prevAction.Item2.ToList(), pixel, prevAction.Item1.Position, circle);
                                    Actions.Add(newAction);
                                    pendingActions.Add(newAction);
                                }
                            }

                            newAction = new PatrolAction(selectionRect.SelectedUnits, pixel, mouseWorldPos, circle);
                            Actions.Add(newAction);
                            pendingActions.Add(newAction);
                            break;
                        case ActionType.Build:
                            newAction = new BuildAction(selectionRect.SelectedUnits.Where(x => x == x /* uncomment this later ------------------------------------------
                                                                                                   is Builder */).ToList(), pixel, mouseWorldPos, currentBuilding, circle, content);
                            Actions.Add(newAction);
                            pendingActions.Add(newAction);
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

            foreach (var element in uiElements)
            {
                element.Update();
            }
            buildPreview.Update();
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
            foreach (var element in uiElements)
            {
                element.Draw(sb);
            }
        }
    }
}
