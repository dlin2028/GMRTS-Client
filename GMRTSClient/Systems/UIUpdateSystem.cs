using GMRTSClient.Component;
using GMRTSClient.Component.Unit;
using GMRTSClient.UI;
using GMRTSClient.UI.ClientAction;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input.InputListeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMRTSClient.Systems
{
    class UIUpdateSystem : EntityUpdateSystem
    {
        private ComponentMapper<Selectable> selectionMapper;
        private readonly GameUI gameUI;
        private readonly UIStatus uiStatus;
        private MouseListener mouseListener;

        public UIUpdateSystem(GameUI gameUI, UIStatus uiStatus)
            :base(Aspect.All(typeof(Unit), typeof(Selectable)))
        {
            this.gameUI = gameUI;
            this.uiStatus = uiStatus;
            mouseListener = new MouseListener();
            mouseListener.MouseClicked += MouseListener_MouseClicked;
        }

        private void MouseListener_MouseClicked(object sender, MouseEventArgs e)
        {
            if (ActiveEntities.Count == 0 && uiStatus.MouseHovering == false)
            {
                gameUI.CurrentAction = ActionType.None;
            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            selectionMapper = mapperService.GetMapper<Selectable>();
        }

        public override void Update(GameTime gameTime)
        {
            mouseListener.Update(gameTime);
        }
    }
}
