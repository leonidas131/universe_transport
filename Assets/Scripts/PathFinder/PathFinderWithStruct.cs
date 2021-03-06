using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;

public static class PathFinderWithStruct
{
    [BurstCompile]
    public static List<SolarSystem> pathFindingWithDistance(SolarSystemStruct _targetSolar, SolarSystemStruct _startSolar, List<SolarClusterStruct> solarClusters)
    {
        ResetDistances(solarClusters);
        CalculateDistances(_targetSolar, _startSolar);
        SolarSystemStruct startsolar = _startSolar;
        List<SolarSystemStruct> route = new List<SolarSystemStruct>();
        route.Add(startsolar);
        while (startsolar.solarLocation != _targetSolar.solarLocation)
        {
            var tempstartsolar = startsolar.connectedSolars.Find(solar => solar.solarDistance == startsolar.connectedSolars.Min(solar => solar.solarDistance));
            //Debug.DrawLine(startsolar.solarLocation, tempstartsolar.solarLocation, Color.red, 360.0f);
            route.Add(tempstartsolar);
            startsolar = tempstartsolar;
        }
        List<SolarSystem> solarRoute = new List<SolarSystem>();
        foreach (var solarStruct in route)
        {
            foreach (var solarCluster in SolarCluster.SolarClusterList)
            {
                foreach (var solar in solarCluster.solarSystems)
                {
                    if (solar.solarSystemStruct == solarStruct)
                    {
                        solarRoute.Add(solar);
                    }
                }
            }
        }
        return solarRoute;
    }
    private static void ResetDistances(List<SolarClusterStruct> solarClusters)
    {
        foreach (var cluster in solarClusters)
        {
            foreach (var solar in cluster.solarSystemsStruct)
            {
                solar.solarDistanceChange(float.MaxValue);
            }
        }
    }
    [BurstCompile]
    private static void CalculateDistances(SolarSystemStruct _targetSolar, SolarSystemStruct _startSolar)
    {
        var visitedSolars = new List<SolarSystemStruct>();
        var solarToVisitQueue = new Queue<SolarSystemStruct>();
        solarToVisitQueue.Enqueue(_targetSolar);
        while (solarToVisitQueue.Count > 0)
        {
            var currentSolar = solarToVisitQueue.Dequeue();
            if (currentSolar == _targetSolar)
            {
                currentSolar.solarDistanceChange(0);
            }
            if (currentSolar == _startSolar)
            {
                solarToVisitQueue.Clear();
                return;
            }
            var nextSolars = currentSolar.connectedSolars;
            var filteredSolars = nextSolars.Where(solar => !visitedSolars.Contains(solar)).ToList();
            foreach (var solar in filteredSolars)
            {

                solarToVisitQueue.Enqueue(solar);

                var distance = (currentSolar.solarLocation - solar.solarLocation).magnitude;
                solar.solarDistanceChange(math.min(solar.solarDistance, currentSolar.solarDistance + distance));
            }
            visitedSolars.Add(currentSolar);
        }
    }
    public static void CalculateAllDistances(SolarSystemStruct _targetSolar)
    {
        var visitedSolars = new List<SolarSystemStruct>();
        var solarToVisitQueue = new Queue<SolarSystemStruct>();
        solarToVisitQueue.Enqueue(_targetSolar);
        while (solarToVisitQueue.Count > 0)
        {
            var currentSolar = solarToVisitQueue.Dequeue();
            if (currentSolar == _targetSolar)
            {
                currentSolar.solarDistanceChange(0);
            }
            var nextSolars = currentSolar.connectedSolars;
            var filteredSolars = nextSolars.Where(solar => !visitedSolars.Contains(solar)).ToList();
            foreach (var solar in filteredSolars)
            {

                solarToVisitQueue.Enqueue(solar);

                var distance = (currentSolar.solarLocation - solar.solarLocation).magnitude;
                solar.solarDistanceChange(math.min(solar.solarDistance, currentSolar.solarDistance + distance));
            }

            //add to queue
            visitedSolars.Add(currentSolar);
        }
    }
    private static float CalculateSolarDistance(SolarSystemStruct currentSolar, SolarSystemStruct solar)
    {
        return (currentSolar.solarLocation - solar.solarLocation).magnitude;
    }


}
