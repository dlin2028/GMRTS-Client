using GMRTSClient.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public GameUI(Camera camera, GraphicsDevice graphics)
        {
            this.camera = camera;
            Texture2D pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });
            selectionRect = new SelectionRectangle(camera, pixel);

            moveButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 400, graphics.Viewport.Height - 100, 100, 100), Color.Blue);
            moveButton.onClick += (sender, e) => {  foreach (var button in actionButtons) button.Release();  moveButton.blue = Color.Black; currentAction = ActionType.Move;};
            moveButton.onRelease += (sender, e) =>  moveButton.blue = Color.Blue;

            attackButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 300, graphics.Viewport.Height - 100, 100, 100), Color.Red);
            attackButton.onClick += (sender, e) => {  foreach (var button in actionButtons) button.Release(); attackButton.blue = Color.Black; currentAction = ActionType.Attack; };
            attackButton.onRelease += (sender, e) =>  attackButton.blue = Color.Red;

            assistButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 200, graphics.Viewport.Height - 100, 100, 100), Color.Yellow);
            assistButton.onClick += (sender, e) => {  foreach (var button in actionButtons) button.Release(); assistButton.blue = Color.Black; currentAction = ActionType.Assist; };
            assistButton.onRelease += (sender, e) =>  assistButton.blue = Color.Yellow;


            patrolButton = new UIElement(pixel, new Rectangle(graphics.Viewport.Width - 100, graphics.Viewport.Height - 100, 100, 100), Color.Green);
            patrolButton.onClick += (sender, e) => {  foreach (var button in actionButtons) button.Release(); patrolButton.blue = Color.Black; currentAction = ActionType.Patrol; };
            patrolButton.onRelease += (sender, e) => patrolButton.blue = Color.Green;

            actionButtons.Add(moveButton);
            actionButtons.Add(attackButton);
            actionButtons.Add(assistButton);
            actionButtons.Add(patrolButton);
        }

        public void Update(Unit[] units)
        {
            


            foreach (var element in actionButtons)
            {
                element.Update();
            }

            selectionRect.Update(units.Where(x => x is ISelectable).Cast<ISelectable>().ToArray(), actionButtons.ToArray());
        }

        public void DrawSelectionRect(SpriteBatch sb)
        {
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
