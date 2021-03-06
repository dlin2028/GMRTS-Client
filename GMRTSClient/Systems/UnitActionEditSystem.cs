using GMRTSClient.Component;
using GMRTSClient.Component.Unit;
using GMRTSClient.UI.ClientAction;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMRTSClient.Systems
{
    class UnitActionEditSystem : EntityUpdateSystem
    {
        private MouseListener mouseListener;
        private ComponentMapper<PlayerAction> actionMapper;
        private ComponentMapper<FancyRect> rectMapper;
        private ComponentMapper<Unit> unitMapper;
        private readonly OrthographicCamera camera;
        private static UnitActionEditSystem instance;
        public static UnitActionEditSystem Instance{
            get
            {
                return instance;
            }
        }

        int currentEntityId;
        UnitAction currentAction;
        Vector2 oldActionPosition;
        public UnitActionEditSystem(OrthographicCamera camera)
            : base(Aspect.One(typeof(PlayerAction), typeof(Unit)))
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                throw new Exception("systems are singletons");
            }
            this.camera = camera;
            mouseListener = new MouseListener();
            mouseListener.MouseDragStart += MouseListener_MouseDragStart;
            mouseListener.MouseDragEnd += MouseListener_MouseDragEnd;
            mouseListener.MouseClicked += MouseListener_MouseClicked;
        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            actionMapper = mapperService.GetMapper<PlayerAction>();
            rectMapper = mapperService.GetMapper<FancyRect>();
            unitMapper = mapperService.GetMapper<Unit>();
        }

        private void MouseListener_MouseDragEnd(object sender, MouseEventArgs e)
        {
            if (currentAction == null)
                return;

            if (currentAction is UnitUnitAction)
            {
                ((UnitUnitAction)currentAction).PauseUpdate = false;
                var newTargets = getIntersectingUnits(camera.ScreenToWorld(e.Position.ToVector2()));
                if (newTargets.Count() != 0)
                {
                    var newTarget = newTargets.First();
                    if (currentAction.ActionType == ActionType.Assist) //&& is friendly )
                    {
                        ((UnitUnitAction)currentAction).Target = unitMapper.Get(newTarget);
                    }
                    else if (currentAction.ActionType == ActionType.Attack) //&& is enemy )
                    {
                        ((UnitUnitAction)currentAction).Target = unitMapper.Get(newTarget);
                    }
                }
                else
                {
                    return;
                }
            }

            var oldId = currentAction.ID;
            currentAction.ID = Guid.NewGuid();
            GetEntity(currentEntityId).Attach(new DTOActionData(new ReplaceAction(currentAction, oldId)));
            currentAction = null;

        }

        private void MouseListener_MouseDragStart(object sender, MouseEventArgs e)
        {
            var keyState = KeyboardExtended.GetState();
            if (e.Button != MouseButton.Right || !keyState.IsShiftDown() || !keyState.IsControlDown())
                return;

            (currentAction, currentEntityId) = getIntersectingAction(e);
            if (currentAction != null)
            {
                if (currentAction is UnitUnitAction)
                {
                    ((UnitUnitAction)currentAction).PauseUpdate = true;
                }
                oldActionPosition = currentAction.Position;
            }
        }


        private void MouseListener_MouseClicked(object sender, MouseEventArgs e)
        {
            var keyState = KeyboardExtended.GetState();
            if (!keyState.IsControlDown() || !keyState.IsShiftDown() || !(e.Button == MouseButton.Right))
                return;

            (var unitAction, var entityId) = getIntersectingAction(e);
            if (unitAction == null)
                return;

            HashSet<UnitAction> affectedActions = new HashSet<UnitAction>();
            foreach (var unit in unitAction.Units)
            {
                var nextAction = unit.Orders.Find(unitAction).Next;
                if (nextAction != null)
                {
                    affectedActions.Add(nextAction.Value);
                }
                unit.Orders.Remove(unitAction);
            }
            foreach (var a in affectedActions)
            {
                a.UpdateCollections();
            }
            var deleteEntity = CreateEntity();
            var deleteAction = new DeleteAction(unitAction.Units.ToArray(), unitAction);
            deleteEntity.Attach(deleteAction);
            deleteEntity.Attach(new DTOActionData(deleteAction));
            DestroyEntity(entityId);
        }

        public void DeleteAction(UnitAction unitAction)
        {
             HashSet<UnitAction> affectedActions = new HashSet<UnitAction>();
            foreach (var unit in unitAction.Units)
            {
                var nextAction = unit.Orders.Find(unitAction).Next;
                if (nextAction != null)
                {
                    affectedActions.Add(nextAction.Value);
                }
                unit.Orders.Remove(unitAction);
            }
            foreach (var a in affectedActions)
            {
                a.UpdateCollections();
            }
            var deleteEntity = CreateEntity();
            var deleteAction = new DeleteAction(unitAction.Units.ToArray(), unitAction);
            deleteEntity.Attach(deleteAction);
            deleteEntity.Attach(new DTOActionData(deleteAction));
            DestroyEntity(ActiveEntities.First(x => actionMapper.Get(x) == unitAction));
        }


        private (UnitAction, int) getIntersectingAction(MouseEventArgs e)
        {
            foreach (var entityID in ActiveEntities.Where(x => actionMapper.Has(x)))
            {
                var action = actionMapper.Get(entityID);
                if (!action.IsUnitAction)
                    return (null, 0);

                var unitAction = (UnitAction)action;
                if (Vector2.Distance(unitAction.Position, camera.ScreenToWorld(e.Position.ToVector2())) <= 10)
                {
                    return (unitAction, entityID);
                }
            }
            return (null, 0);
        }
        private IEnumerable<int> getIntersectingUnits(Vector2 position)
        {
            return ActiveEntities.Where(x => rectMapper.Has(x) && rectMapper.Get(x).Contains(position));
        }


        public override void Update(GameTime gameTime)
        {
            var mouseState = MouseExtended.GetState();
            mouseListener.Update(gameTime);
            if (currentAction != null)
            {
                currentAction.Position -= mouseState.DeltaPosition.ToVector2() / camera.Zoom;
            }
        }
    }
}
