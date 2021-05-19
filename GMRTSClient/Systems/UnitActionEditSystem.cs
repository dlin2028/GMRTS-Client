using GMRTSClient.Component;
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
        private readonly OrthographicCamera camera;
        int currentEntityId;
        UnitAction currentAction;
        Vector2 oldActionPosition;
        public UnitActionEditSystem(OrthographicCamera camera)
            :base(Aspect.All(typeof(PlayerAction)))
        {
            this.camera = camera;
            mouseListener = new MouseListener();
            mouseListener.MouseDragStart += MouseListener_MouseDragStart;
            mouseListener.MouseDragEnd += MouseListener_MouseDragEnd;
            mouseListener.MouseClicked += MouseListener_MouseClicked;
        }

        private void MouseListener_MouseDragEnd(object sender, MouseEventArgs e)
        {
            if(currentAction != null)
            {
                var oldId = currentAction.ID;
                currentAction.ID = Guid.NewGuid();
                GetEntity(currentEntityId).Attach(new DTOActionData(new ReplaceAction(currentAction, oldId)));
                currentAction = null;
            }
        }

        private void MouseListener_MouseDragStart(object sender, MouseEventArgs e)
        {
            var keyState = KeyboardExtended.GetState();
            if (e.Button != MouseButton.Right || !keyState.IsShiftDown() || !keyState.IsControlDown())
                return;

            (currentAction, currentEntityId) = getIntersectingAction(e);
            if (currentAction != null)
            {
                oldActionPosition = currentAction.Position;
            }
        }


        private void MouseListener_MouseClicked(object sender, MouseEventArgs e)
        {
            var keyState = KeyboardExtended.GetState();
            if (!keyState.IsControlDown() || !keyState.IsShiftDown() || !(e.Button == MouseButton.Right))
                return;
            
            (var unitAction, var entityId) = getIntersectingAction(e);
            if(unitAction == null)
                return;            

            HashSet<UnitAction> affectedActions = new HashSet<UnitAction>();
            foreach(var unit in unitAction.Units)
            {
                var nextAction = unit.Orders.Find(unitAction).Next;
                if(nextAction != null)
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


        private (UnitAction, int) getIntersectingAction(MouseEventArgs e)
        {
            foreach (var entityID in ActiveEntities)
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

        public override void Initialize(IComponentMapperService mapperService)
        {
            actionMapper = mapperService.GetMapper<PlayerAction>();
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = MouseExtended.GetState();
            mouseListener.Update(gameTime);
            if(currentAction != null)
            {
                currentAction.Position -= mouseState.DeltaPosition.ToVector2() / camera.Zoom;
            }
        }
    }
}
