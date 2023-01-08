using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Dot : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool isConnectedRight { get; private set; }
    public bool isConnectedLeft { get; private set; }
    public bool isConnectedUp { get; private set; }
    public bool isConnectedDown { get; private set; }
    public Dictionary<Vector2, bool> connectingCompass { get; private set; }//avaiable direction
    GridManager GridManager;
    DotStyling DotStyling;

    public void Init(int x, int y, int boundaryLimit, GridManager GridManager)
    {
        this.GridManager = GridManager;
        X = x;
        Y = y;
        this.gameObject.name = $"Dot {X},{Y}";


        connectingCompass = new Dictionary<Vector2, bool>()
        {
            {Vector2.down,false },
            {Vector2.up,false },
            {Vector2.right,false },
            {Vector2.left,false },

        };
        //FIGURE OUT IF FALSE IS CAN CONNECT VS CANNOT
        //true means connected or its the end
        if (x == 0)
            connectingCompass[Vector2.up] = true;
        if (x == boundaryLimit)
            connectingCompass[Vector2.down] = true;
        if (y == 0)
            connectingCompass[Vector2.left] = true;
        if (y == boundaryLimit)
            connectingCompass[Vector2.right] = true;
        //connectingCompass[new Vector2(x, 1)];

        Debug.Log($"x,y is {x},{y}");
        this.DotStyling = GetComponentInChildren<DotStyling>();
        DotStyling.Init(connectingCompass);
        DotStyling.DrawLine();
    }

    public void OnSelect()
    {
        GridManager.SelectDot(X,Y);
        DotStyling.OnSelect();
    }

    public void NeighboringChoice()
    {
        GridManager.SelectNeighbor(X, Y);
    }

    public void OnDeselect()
    {

    }

}
