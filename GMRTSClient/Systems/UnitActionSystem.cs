using GMRTSClasses.CTSTransferData;
using GMRTSClient.Component;
using GMRTSClient.Component.Unit;
using GMRTSClient.UI;
using GMRTSClient.UI.ClientAction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        private readonly OrthographicCamera camera;
        private readonly ContentManager content;

        private ComponentMapper<Unit> unitMapper;
        private ComponentMapper<Selectable> selectMapper;
        private ComponentMapper<FancyRect> rectMapper;
        private MouseListener mouseListener;

        private GameUI gameui;
        private UIStatus uiStatus;
        private ActionType currentAction => uiStatus.CurrentAction;
        private BuildingType currentBuilding => uiStatus.CurrentBuilding;

        public UnitActionSystem(UIStatus uiStatus, GameUI gameui, OrthographicCamera camera, ContentManager content)
            : base(Aspect.All(typeof(Unit)))
        {
            this.uiStatus = uiStatus;
            this.camera = camera;
            this.content = content;
            this.gameui = gameui;

            mouseListener = new MouseListener();
            mouseListener.MouseClicked += MouseListener_MouseClicked;
            mouseListener.MouseDoubleClicked += MouseListener_MouseClicked;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            unitMapper = mapperService.GetMapper<Unit>();
            selectMapper = mapperService.GetMapper<Selectable>();
            rectMapper = mapperService.GetMapper<FancyRect>();
        }

        private IEnumerable<int> GetIntersectingUnits(Point position)
        {
            return ActiveEntities.Where(x => rectMapper.Get(x).Contains(camera.ScreenToWorld(position.ToVector2())));
        }

        private void MouseListener_MouseClicked(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButton.Left && !uiStatus.MouseHovering)
            {
                gameui.CurrentAction = ActionType.None;
            }

            if (e.Button != MouseButton.Right || uiStatus.MouseHovering) return;

            List<int> selectedEntities = new List<int>();
            List<Unit> selectedUnits = new List<Unit>();
            foreach (var entityId in ActiveEntities)
            {
                if (selectMapper.Get(entityId).Selected)
                {
                    selectedEntities.Add(entityId);
                    selectedUnits.Add(unitMapper.Get(entityId));
                }
            }
            if(selectedEntities.Count <= 0)
            {
                return;
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
                    var clickedUnits = GetIntersectingUnits(e.Position);
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
                    Unit attackTarget = null;
                    var unitsToAttack = GetIntersectingUnits(e.Position);
                    if (unitsToAttack.Count() > 0)
                        attackTarget = unitMapper.Get(unitsToAttack.First());

                    if (attackTarget != null) //&& unit is enemy
                    {
                        newAction = new AttackAction(selectedUnits, attackTarget);
                        newEntity.Attach(newAction);
                        newEntity.Attach(new DTOActionData(newAction));
                    }
                    break;
                case ActionType.Assist:
                    Unit assistTarget = null;
                    var unitsToAssist = GetIntersectingUnits(e.Position);
                    if (unitsToAssist.Count() > 0)
                        assistTarget = unitMapper.Get(unitsToAssist.First());

                    if (assistTarget != null) //&&unit is friendly
                    {
                        newAction = new AssistAction(selectedUnits, assistTarget);
                        newEntity.Attach(newAction);
                        newEntity.Attach(new DTOActionData(newAction));
                    }
                    break;
                case ActionType.Patrol:
                    DestroyEntity(newEntity.Id); //so that the extraEntities are sent before the newEntity
                    List<(UnitAction?, IEnumerable<Unit>)> prevActions = new List<(UnitAction?, IEnumerable<Unit>)>();
                    foreach (var uniqueAction in selectedUnits.Select(x => x.Orders).Select(x => x.LastOrDefault()).Distinct())
                    {
                        prevActions.Add((uniqueAction, selectedUnits.Where(x => x.Orders.LastOrDefault() == uniqueAction)));
                    }
                    foreach (var prevAction in prevActions)
                    {
                        var extraEntity = CreateEntity();
                        if (prevAction.Item1 == null)
                        {
                            var posList = prevAction.Item2.Select(x => x.Position.Value);
                            var avgpos = new Vector2(posList.Average(x => x.X), posList.Average(x => x.Y));
                            newAction = new PatrolAction(prevAction.Item2.ToList(), avgpos);
                            extraEntity.Attach(newAction);
                            extraEntity.Attach(new DTOActionData(newAction));
                        }
                        else if(prevAction.Item1.ActionType != ActionType.Patrol)
                        {
                            newAction = new PatrolAction(prevAction.Item2.ToList(), prevAction.Item1.Position);
                            extraEntity.Attach(newAction);
                            extraEntity.Attach(new DTOActionData(newAction));
                        }
                    }
                    newEntity = CreateEntity();
                    newAction = new PatrolAction(selectedUnits, mouseWorldPos);
                    newEntity.Attach(newAction);
                    newEntity.Attach(new DTOActionData(newAction));
                    break;
                case ActionType.Build:
                    newAction = new BuildAction(selectedUnits.Where(x => x == x /*is Builder*/).ToList(), mouseWorldPos, currentBuilding, content);
                    newEntity.Attach(newAction);
                    newEntity.Attach(new DTOActionData(newAction));
                    break;
                default:
                    break;
            }

            if (!keyState.IsShiftDown())
            {
                SelectionSystem.Instance.DeselectAllUnits();
                gameui.CurrentAction = ActionType.None;
            }
        }

        public override void Update(GameTime gameTime)
        {
            mouseListener.Update(gameTime);
        }
    }
}
