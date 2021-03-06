using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class UIEventHandler
{
    public static UnityEvent<float> PauseTextClicked = new UnityEvent<float>();
    public static UnityEvent<Vector2> PauseButtonClicked = new UnityEvent<Vector2>();
    public static UnityEvent<bool> CreatingUniverse = new UnityEvent<bool>();
    public static UnityEvent<int> DayChanged = new UnityEvent<int>();
    public static UnityEvent<int> MonthChanged = new UnityEvent<int>();
    public static UnityEvent<int> YearChanged = new UnityEvent<int>();
    public static UnityEvent<Route, bool> SingleRouteItemClickedEvent = new UnityEvent<Route, bool>();
    public static UnityEvent RouteMenuOpenEvent = new UnityEvent();
    public static UnityEvent RouteCreatingBegunEvent = new UnityEvent();
    public static UnityEvent RouteCreatingEndedEvent = new UnityEvent();
    public static UnityEvent<Route> RouteDeleteEvent = new UnityEvent<Route>();
    public static UnityEvent<int> RouteStationDeleteEvent = new UnityEvent<int>();
    public static UnityEvent<int> RouteStationUpEvent = new UnityEvent<int>();
    public static UnityEvent<int> RouteStationDownEvent = new UnityEvent<int>();
    public static UnityEvent<GameObject> ConstructionBegunEvent = new UnityEvent<GameObject>();
    public static UnityEvent ConstructionEndedEvent = new UnityEvent();
    public static UnityEvent<ConstructionNode> ConstructionPrePlacementEvent = new UnityEvent<ConstructionNode>();
    public static UnityEvent ConstructionPlacementEvent = new UnityEvent();
    public static UnityEvent ConstructionCancelEvent = new UnityEvent();
    public static UnityEvent<IStation, IDestructable> PreDestructionEvent = new UnityEvent<IStation, IDestructable>();

}
