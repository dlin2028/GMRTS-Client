using GMRTSClient.ClientAction;
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
using System.Linq;
using System.Text;

namespace GMRTSClient.Systems
{
    class UIUpdateSystem : EntityUpdateSystem, IObserver<SelectableData>
    {
        private ComponentMapper<Selectable> selectionMapper;
        private ComponentMapper<Builder> builderMapper;
        private ComponentMapper<Factory> factoryMapper;
        private readonly GameUI gameUI;
        private readonly UIStatus uiStatus;
        private IDisposable unsubscriber;

        public UIUpdateSystem(GameUI gameUI, UIStatus uiStatus)
            :base(Aspect.All(typeof(Selectable)))
        {
            this.gameUI = gameUI;
            this.uiStatus = uiStatus;
            unsubscriber = SelectionSystem.Instance.Subscribe(this);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            selectionMapper = mapperService.GetMapper<Selectable>();
            builderMapper = mapperService.GetMapper<Builder>();
            factoryMapper = mapperService.GetMapper<Factory>();
        }

        public override void Update(GameTime gameTime)
        {
            gameUI.Update((ulong)ServerUpdateSystem.Instance.Stopwatch.ElapsedMilliseconds);
        }

        public void OnCompleted()
        {
            unsubscriber.Dispose();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(SelectableData value)
        {
            if(value.SelectedEntityIds.Count == 0 && uiStatus.MouseHovering == false)
            {
                gameUI.CurrentAction = ActionType.None;
                gameUI.BuildMenuFlags = BuildFlags.None;
                return;
            }

            var currBuildFlags = BuildFlags.None;
            List<FactoryOrder> orders;
            if(builderMapper.Has(value.SelectedEntityIds.First()))
            {
                orders = new List<FactoryOrder>();
                var factory = factoryMapper.Get(entityId);
                orders.Add(factory.Unit.Orders)
            }

            foreach (var entityID in value.SelectedEntityIds)
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
    }
}
