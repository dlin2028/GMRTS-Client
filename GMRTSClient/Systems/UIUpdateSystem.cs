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
        private ComponentMapper<Builder> builderMapper;
        private ComponentMapper<Factory> factoryMapper;
        private readonly GameUI gameUI;
        private readonly UIStatus uiStatus;
        private MouseListener mouseListener;
        private BuildFlags currBuildFlags;

        public UIUpdateSystem(GameUI gameUI, UIStatus uiStatus)
            :base(Aspect.All(typeof(Selectable)))
        {
            this.gameUI = gameUI;
            this.uiStatus = uiStatus;
            mouseListener = new MouseListener();
            mouseListener.MouseClicked += MouseListener_MouseClicked;
            mouseListener.MouseDragEnd += MouseListener_MouseDragEnd;
        }

        private void MouseListener_MouseDragEnd(object sender, MouseEventArgs e)
        {
            currBuildFlags = BuildFlags.None;
            foreach (var entityID in ActiveEntities)
            {
                if (selectionMapper.Get(entityID).Selected)
                {
                    if (builderMapper.Has(entityID))
                    {
                        currBuildFlags |= builderMapper.Get(entityID).BuildFlags;
                    }
                    if (factoryMapper.Has(entityID))
                    {
                        currBuildFlags |= factoryMapper.Get(entityID).BuildFlags;
                    }
                }
            }

            gameUI.BuildMenuFlags = currBuildFlags;
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
            builderMapper = mapperService.GetMapper<Builder>();
            factoryMapper = mapperService.GetMapper<Factory>();
        }

        public override void Update(GameTime gameTime)
        {
            mouseListener.Update(gameTime);
        }
    }
}
