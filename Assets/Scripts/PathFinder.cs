using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder
{
    public List<SelectedTile> FindPath(
        Vector3Int startPos,
        Vector3Int finishPos,
        int jumpHeight,
        int clearance
    )
    {
        foreach (var tile in MapManager.Instance.Map.Values)
        {
            tile.ResetPathfinding();
        }

        List<SelectedTile> openList = new();
        List<SelectedTile> closedList = new();

        SelectedTile start = MapManager.Instance.Map[new Vector2Int(startPos.x, startPos.y)];
        SelectedTile finish = MapManager.Instance.Map[new Vector2Int(finishPos.x, finishPos.y)];
        SelectedTile closestToFinish = start;
        int closestHeuristic = GetManhattanDistance(finish, start);

        openList.Add(start);

        while (openList.Count > 0)
        {
            SelectedTile currentTile = openList.OrderBy(n => n.F).First();
            int currentHeuristic = GetManhattanDistance(finish, currentTile);
            if (currentHeuristic < closestHeuristic)
            {
                closestHeuristic = currentHeuristic;
                closestToFinish = currentTile;
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == finish)
            {
                finish.Show();
                return GetFinishedPath(start, finish);
            }

            foreach (SelectedTile neighbour in GetNeighbours(currentTile))
            {
                Vector3Int? availableTile = MapManager.Instance.FindWalkableTileAt(
                    neighbour.tilePos,
                    maxZDiff: jumpHeight,
                    clearance: clearance
                );

                if (
                    !availableTile.HasValue
                    || closedList.Contains(neighbour)
                    || Mathf.Abs(currentTile.tilePos.z - neighbour.tilePos.z) > 1
                )
                {
                    continue;
                }

                int tentativeG = currentTile.G + neighbour.cost;

                if (tentativeG < neighbour.G || !openList.Contains(neighbour))
                {
                    neighbour.G = tentativeG;
                    neighbour.H = GetManhattanDistance(finish, neighbour);
                    neighbour.previousTile = currentTile;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        finish.Show();
        return GetFinishedPath(start, closestToFinish);
    }

    private List<SelectedTile> GetFinishedPath(SelectedTile start, SelectedTile finish)
    {
        List<SelectedTile> finishedPath = new();
        SelectedTile currentTile = finish;
        while (currentTile != start)
        {
            finishedPath.Add(currentTile);
            currentTile = currentTile.previousTile;
        }
        finishedPath.Reverse();
        return finishedPath;
    }

    private int GetManhattanDistance(SelectedTile a, SelectedTile b)
    {
        return Mathf.Abs(a.tilePos.x - b.tilePos.x) + Mathf.Abs(a.tilePos.y - b.tilePos.y);
    }

    private List<SelectedTile> GetNeighbours(SelectedTile currentTile)
    {
        var map = MapManager.Instance.Map;
        var neighbors = new List<SelectedTile>();
        Vector2Int[] directions = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };

        foreach (var dir in directions)
        {
            Vector2Int checkPos = new(currentTile.tilePos.x + dir.x, currentTile.tilePos.y + dir.y);
            if (map.ContainsKey(checkPos))
            {
                neighbors.Add(map[checkPos]);
            }
        }

        return neighbors;
    }
}
