using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SingleRouteMenu : MonoBehaviour
{
    [SerializeField] private StationListItem stationListItemPrefab;
    [SerializeField] private Transform stationListTransform;
    [SerializeField] private ToggleGroup stationListToggleGroup;
    [SerializeField] private TMP_Text routeName;
    [SerializeField] private Image colorTexture;
    [SerializeField] private Button addButton;
    [SerializeField] private Button doneButton;

    private SolarClusterStruct[] solarClusters;
    private Route route;
    private List<StationListItem> stations;

    private void OnEnable()
    {
        PlayerManagerEventHandler.RoutePartInstantiateEvent.AddListener(RoutePartsInstantiate);
        UIEventHandler.RouteStationDeleteEvent.AddListener(DeleteRouteStation);
    }
    private void OnDisable()
    {
        PlayerManagerEventHandler.RoutePartInstantiateEvent.RemoveListener(RoutePartsInstantiate);
        UIEventHandler.RouteStationDeleteEvent.RemoveListener(DeleteRouteStation);
    }

    public void UpdateDisplay(Route _route, bool isOn)
    {
        route = _route;
        routeName.text = _route.RouteName;
        colorTexture.color = _route.RouteColor;
        StationListInitializer();
    }

    public void StationListInitializer()
    {
        stationListTransform.Clear();
        stations = new List<StationListItem>();
        for (int i = 0; i < route.routeParts.Count; i++)
        {
            var station = Instantiate(stationListItemPrefab, stationListTransform);
            station.transform.GetComponent<Toggle>().group = stationListToggleGroup;
            stations.Add(station);
            station.UpdateDisplay(route.routeParts[i].solars[0].solarSystem);
        }

    }
    public void TakeClusters(SolarClusterStruct[] _solarClusters)
    {
        solarClusters = _solarClusters;
    }
    public void RouteCreatingBegun()
    {
        ButtonChanger(true);
    }

    private void ButtonChanger(bool isActive)
    {
        addButton.gameObject.SetActive(!isActive);
        doneButton.gameObject.SetActive(isActive);
    }

    public void RouteCreatingEnded()
    {
        ButtonChanger(false);
    }

    private void RoutePartsInstantiate(SolarSystem solar)
    {
        if (route.firstSolar == null)
        {
            route.firstSolar = solar;
        }
        route.solarsForRoute.Enqueue(solar);
        List<SolarSystemStruct> solars = new List<SolarSystemStruct>();
        List<SolarSystemStruct> firstSolars = new List<SolarSystemStruct>();

        if (route.solarsForRoute.Count > 1)
        {
            firstSolars = FindPath(route.firstSolar.solarSystemStruct, solar.solarSystemStruct);
            RoutePart routePartEnd = new RoutePart(firstSolars);
            if (route.routeParts.Count < 1)
            {
                route.routeParts.Add(routePartEnd);
            }
            else
            {
                route.routeParts[0] = routePartEnd;
            }
            SolarSystemStruct firstOne = route.solarsForRoute.Dequeue().solarSystemStruct;
            SolarSystemStruct secondOne = route.solarsForRoute.Dequeue().solarSystemStruct;

            solars = FindPath(secondOne, firstOne);
            RoutePart routePart = new RoutePart(solars);
            route.routeParts.Add(routePart);
            route.solarsForRoute.Enqueue(solar);
            CreateRoute();
        }

    }
    private void CreateRoute()
    {
        route.ClearRoute();
        route.InitializeRoute();
        StationListInitializer();
    }
    public void DeleteRoute()
    {
        UIEventHandler.RouteDeleteEvent?.Invoke(route);
    }
    private void DeleteRouteStation(SolarSystem solar)
    {
        if (route.routeParts.Count < 3) return;
        SolarSystemStruct firstSolar = null;
        SolarSystemStruct secondSolar = null;
        List<SolarSystemStruct> solars = new List<SolarSystemStruct>();
        SolarSystem lastSolar = route.solarsForRoute.Dequeue();
        for (int i = 0; i < route.routeParts.Count; i++)
        {
            if (route.routeParts[i].solars[0] == solar.solarSystemStruct)
            {
                firstSolar = route.routeParts[i].solars[route.routeParts[i].solars.Count - 1];
                route.routeParts.RemoveAt(i);
                i--;
            }

        }
        for (int i = 0; i < route.routeParts.Count; i++)
        {

            if (route.routeParts[i].solars[route.routeParts[i].solars.Count - 1] == solar.solarSystemStruct)
            {
                secondSolar = route.routeParts[i].solars[0];

                solars = FindPath(secondSolar, firstSolar);
                RoutePart routePart = new RoutePart(solars);
                route.routeParts[i] = routePart;
            }
        }
        if (solar == lastSolar)
        {
            route.solarsForRoute.Enqueue(firstSolar.solarSystem);
        }
        else
        {
            route.solarsForRoute.Enqueue(lastSolar);
        }
        if (route.firstSolar == solar)
        {
            route.firstSolar = secondSolar.solarSystem;
        }
        for (int i = 0; i < stations.Count; i++)
        {
            if (stations[i].solarSystem == solar)
            {
                Destroy(stations[i].gameObject);
                stations.RemoveAt(i);
                i--;
            }
        }
        CreateRoute();
    }
    private List<SolarSystemStruct> FindPath(SolarSystemStruct startSolar, SolarSystemStruct endSolar)
    {
        List<SolarSystemStruct> routePart = PathFinderWithStruct.pathFindingWithDistance(endSolar, startSolar, solarClusters);
        return routePart;
    }

}
