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
    class UnitActionSystem : EntityUpdateSystem
    {
        private ComponentMapper<Unit> unitMapper;
        private ComponentMapper<Selectable> selectMapper;
        private ComponentMapper<FancyRect> rectMapper;
        private MouseListener mouseListener;

        private readonly ActionType currentAction;
        private readonly OrthographicCamera camera;

        public UnitActionSystem(int currentAction, OrthographicCamera camera)
            :base(Aspect.All(typeof(Unit)))
        {
            this.currentAction = (ActionType)currentAction;
            mouseListener = new MouseListener();
            mouseListener.MouseClicked += MouseListener_MouseClicked;
            mouseListener.MouseDoubleClicked += MouseListener_MouseClicked;
            this.camera = camera;
        }

        private ComponentMapper<PlayerAction> pam;
        public override void Initialize(IComponentMapperService mapperService)
        {
            pam = mapperService.GetMapper<PlayerAction>();
            unitMapper = mapperService.GetMapper<Unit>();
            selectMapper = mapperService.GetMapper<Selectable>();
            rectMapper = mapperService.GetMapper<FancyRect>();
        }

        private void MouseListener_MouseClicked(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButton.Right) return;

            List<int> selectedEntities = new List<int>();
            List<Unit> selectedUnits = new List<Unit>();
            foreach (var entityId in ActiveEntities)
            {
                if(selectMapper.Get(entityId).Selected)
                {
                    selectedEntities.Add(entityId);
                    selectedUnits.Add(unitMapper.Get(entityId));
                }
            }

            var keyState = KeyboardExtended.GetState();
            if (!keyState.IsShiftDown())
            {
                List<UnitAction> oldOrders = new List<UnitAction>();

                foreach (var unitId in selectedEntities)
                {
                    var unit = unitMapper.Get(unitId);
                    var currOrder = unitMapper.Get(unitId).Orders.First;
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
                            unit.Orders.RemoveLast();
                    }
                }
                var selectedUnitArr = selectedEntities.Select(x => unitMapper.Get(x)).ToArray();
                foreach (var oldOrder in oldOrders)
                {
                    var delActionEntity = CreateEntity();
                    var delAction = new DeleteAction(selectedUnitArr, oldOrder);
                    delActionEntity.Attach<PlayerAction>(delAction);
                }
            }

            var newEntity = CreateEntity();
            PlayerAction newAction;
            var mouseWorldPos = camera.ScreenToWorld(e.Position.ToVector2());
            switch (currentAction)
            {
                case ActionType.None:
                    Unit target = null;
                    var clickedUnits = ActiveEntities.Where(x => rectMapper.Get(x).Contains(camera.ScreenToWorld(e.Position.ToVector2())));
                    if (clickedUnits.Count() > 0)
                        target = unitMapper.Get(clickedUnits.First());

                    if (target != null) //&& unit is enemy
                    {
                        newAction = new AttackAction(selectedUnits, target);
                    }
                    else if (target != null) //&&unit is friendly
                    {
                        newAction = new AssistAction(selectedUnits, target);
                    }
                    else
                    {
                        newAction = new MoveAction(selectedUnits, mouseWorldPos);
                    }
                    newEntity.Attach(newAction);
                    newEntity.Attach(new DTOActionData(newAction));
                    break;
                case ActionType.Move:
                    newAction = new MoveAction(selectedUnits, mouseWorldPos);
                    newEntity.Attach(newAction);
                    newEntity.Attach(new DTOActionData(newAction));
                    break;
                case ActionType.Attack:
                    var attackTarget = unitMapper.Get(ActiveEntities.FirstOrDefault(x => rectMapper.Get(x).Contains(camera.ScreenToWorld(e.Position.ToVector2()))));
                    if (attackTarget != null) //&& unit is enemy
                    {
                        newAction = new AttackAction(selectedUnits, attackTarget);
                        newEntity.Attach(newAction);
                        newEntity.Attach(new DTOActionData(newAction));
                    }
                    break;
                case ActionType.Assist:
                    var assistTarget = unitMapper.Get(ActiveEntities.FirstOrDefault(x => rectMapper.Get(x).Contains(camera.ScreenToWorld(e.Position.ToVector2()))));
                    if (assistTarget != null) //&&unit is friendly
                    {
                        newAction = new AssistAction(selectedUnits, assistTarget);
                        newEntity.Attach(newAction);
                        newEntity.Attach(new DTOActionData(newAction));
                    }
                    break;
                case ActionType.Patrol:
                    List<(UnitAction?, IEnumerable<Unit>)> prevActions = new List<(UnitAction?, IEnumerable<Unit>)>();
                    foreach (var uniqueAction in selectedUnits.Select(x => x.Orders).Select(x => x.LastOrDefault()).Distinct())
                    {
                        prevActions.Add((uniqueAction, selectedUnits.Where(x => x.Orders.LastOrDefault() == uniqueAction)));
                    }
                    foreach (var prevAction in prevActions)
                    {
                        if (prevAction.Item1 == null)
                        {
                            var posList = prevAction.Item2.Select(x => x.Position.Value);
                            var avgpos = new Vector2(posList.Average(x => x.X), posList.Average(x => x.Y));
                            newAction = new PatrolAction(prevAction.Item2.ToList(), avgpos);
                        }
                        else
                        {
                            newAction = new PatrolAction(prevAction.Item2.ToList(), prevAction.Item1.Position);
                        }
                        newEntity.Attach(newAction);
                        newEntity.Attach(new DTOActionData(newAction));
                    }

                    newAction = new PatrolAction(selectedUnits, mouseWorldPos);
                    newEntity.Attach(newAction);
                    newEntity.Attach(new DTOActionData(newAction));
                    break;
                case ActionType.Build:
                    //newAction = new BuildAction(selectionRect.SelectedUnits.Where(x => x is Builder).ToList(), pixel, mouseWorldPos, currentBuilding, circle, content);
                    //Actions.Add(newAction);
                    //pendingActions.Add(newAction);
                    break;
                default:
                    break;
            }

            if (!keyState.IsShiftDown())
            {
                foreach (var entityId in selectedEntities)
                {
                    selectMapper.Get(entityId).Selected = false;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            mouseListener.Update(gameTime);
        }
    }
}
