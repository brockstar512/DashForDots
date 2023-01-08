using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Dot : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Dictionary<Vector2Int, bool> connectingCompass { get; private set; }//avaiable direction
    DotStyling DotStyling;
    GridManager GridManager;

    public void Init(int x, int y, int boundaryLimit, GridManager GridManager)
    {
        this.GridManager = GridManager;
        X = x;
        Y = y;
        this.gameObject.name = $"Dot {X},{Y}";

        connectingCompass = new Dictionary<Vector2Int, bool>()
        {
            {Vector2Int.down,false },
            {Vector2Int.up,false },
            {Vector2Int.right,false },
            {Vector2Int.left,false },

        };
        //FIGURE OUT IF FALSE IS CAN CONNECT VS CANNOT
        //true means connected or its the end
        if (x == 0)
            connectingCompass[Vector2Int.up] = true;
        if (x == boundaryLimit)
            connectingCompass[Vector2Int.down] = true;
        if (y == 0)
            connectingCompass[Vector2Int.left] = true;
        if (y == boundaryLimit)
            connectingCompass[Vector2Int.right] = true;

        Debug.Log($"x,y is {x},{y}");
        this.DotStyling = GetComponentInChildren<DotStyling>();
        DotStyling.Init(connectingCompass);
    }

    //when this dot is selected
    public void OnSelect()
    {
        GridManager.SelectDot(X,Y);
        DotStyling.Select();
        GetNeigbors();
    }
    void GetNeigbors()
    {
        if (!connectingCompass[Vector2Int.down])
        {
            GridManager.dots[X + 1, Y].DotStyling.NeighborHighlight();
        }

        if (!connectingCompass[Vector2Int.up])
        {
            GridManager.dots[X - 1, Y].DotStyling.NeighborHighlight();

        }

        if (!connectingCompass[Vector2Int.right])
        {
            GridManager.dots[X, Y + 1].DotStyling.NeighborHighlight();

        }
        if (!connectingCompass[Vector2Int.left])
        {
            GridManager.dots[X, Y - 1].DotStyling.NeighborHighlight();

        }

        //foreach (KeyValuePair<Vector2Int, bool> direction in connectingCompass)
        //{

        //    if (direction.Value == false)
        //    {
        //        //Debug.Log($"Direction: {X + (direction.Key.y * -1)},{Y + (direction.Key.x * -1)} ");
        //        GridManager.dots[X + (direction.Key.y), Y + (direction.Key.x)].DotStyling.NeighborHighlight();
        //    }
        //}
    }
    void LeaveNeigbors()
    {
        if (!connectingCompass[Vector2Int.down])
        {
            GridManager.dots[X + 1, Y].DotStyling.NeighborUnHighlight();
        }
        if (!connectingCompass[Vector2Int.up])
        {
            GridManager.dots[X - 1, Y].DotStyling.NeighborUnHighlight();

        }
        if (!connectingCompass[Vector2Int.right])
        {
            GridManager.dots[X, Y + 1].DotStyling.NeighborUnHighlight();

        }
        if (!connectingCompass[Vector2Int.left])
        {
            GridManager.dots[X, Y - 1].DotStyling.NeighborUnHighlight();

        }
    }

    public void NeighboringChoice()
    {
        GridManager.SelectNeighbor(X, Y);
    }

    public void OnDeselect()
    {
        //leave neighbors
        DotStyling.Deselect();
        LeaveNeigbors();
    }

}
