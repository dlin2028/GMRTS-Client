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

            var firstId = value.SelectedEntityIds.First();
            var currBuildFlags = BuildFlags.None;
            IEnumerable<FactoryOrder> orders = null;
            IEnumerable<BuildAction> actions = null;
            if(factoryMapper.Has(value.SelectedEntityIds.First()))
            {
                var factory = factoryMapper.Get(firstId);
                orders = factory.Orders;
            }
            else
            {
                actions = new List<BuildAction>();
                var builder = builderMapper.Get(firstId);
                actions = builder.Unit.Orders.Where(x => x.ActionType == ActionType.Build).Cast<BuildAction>();
            }

            bool displayQueue = true;
            foreach (var entityID in value.SelectedEntityIds)
            {
                if(displayQueue)
                {
                    if(orders == null)
                    {
                        var builder = builderMapper.Get(entityID);
                        var builderActions = builder.Unit.Orders.Where(x => x.ActionType == ActionType.Build).Cast<BuildAction>();
                        if(actions.SequenceEqual(builderActions))
                        {
                            displayQueue = false;
                        }
                    }
                    else
                    {
                        var factory = factoryMapper.Get(entityID);
                        if(orders.SequenceEqual(factory.Orders))
                        {
                            displayQueue = false;
                        }
                    }
                }

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
            if(displayQueue)
            {
                if(orders == null)
                {
                    foreach (var action in actions)
                    {
                        
                    }
                }
                else
                {

                }
            }

            gameUI.BuildMenuFlags = currBuildFlags;
        }
    }
}
