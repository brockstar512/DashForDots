using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;



public class Dot : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    [SerializeField] public Button button { get; private set; }

    public Dictionary<Vector2Int, bool> connectingCompass { get; private set; }//avaiable direction
    DotStyling DotStyling;
    GridManager GridManager;
    public DotValue coordinates { get; private set; }


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
        this.button = GetComponent<Button>();
        DotStyling.Init(connectingCompass);
        coordinates = new DotValue(x,y);
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
            Dot dot = GridManager.dots[X + 1, Y];
            dot.DotStyling.NeighborHighlight();
            dot.GetComponent<Button>().onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(dot.NeighboringChoice);
        }

        if (!connectingCompass[Vector2Int.up])
        {
            Dot dot = GridManager.dots[X - 1, Y];
            dot.DotStyling.NeighborHighlight();
            dot.GetComponent<Button>().onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(dot.NeighboringChoice);

        }

        if (!connectingCompass[Vector2Int.right])
        {
            Dot dot = GridManager.dots[X, Y + 1];
            dot.DotStyling.NeighborHighlight();
            dot.GetComponent<Button>().onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(dot.NeighboringChoice);

        }
        if (!connectingCompass[Vector2Int.left])
        {
            Dot dot = GridManager.dots[X, Y - 1];
            dot.DotStyling.NeighborHighlight();
            dot.GetComponent<Button>().onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(dot.NeighboringChoice);

        }

    }

    void LeaveNeigbors()
    {
        if (!connectingCompass[Vector2Int.down])
        {
            Dot dot = GridManager.dots[X + 1, Y];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.AddListener(dot.OnSelect);
        }
        if (!connectingCompass[Vector2Int.up])
        {
            //GridManager.dots[X - 1, Y].DotStyling.NeighborUnHighlight();
            Dot dot = GridManager.dots[X - 1, Y];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.AddListener(dot.OnSelect);

        }
        if (!connectingCompass[Vector2Int.right])
        {
            //GridManager.dots[X, Y + 1].DotStyling.NeighborUnHighlight();
            Dot dot = GridManager.dots[X, Y + 1];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.AddListener(dot.OnSelect);

        }
        if (!connectingCompass[Vector2Int.left])
        {
            //GridManager.dots[X, Y - 1].DotStyling.NeighborUnHighlight();
            Dot dot = GridManager.dots[X, Y - 1];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.AddListener(dot.OnSelect);

        }
    }

    public void NeighboringChoice()
    {
        GridManager.SelectNeighbor(X, Y);
    }

    public void OnDeselect()
    {
        button.onClick.AddListener(OnSelect);
        DotStyling.Deselect();
        LeaveNeigbors();
    }

    public async Task Confirm(Dot neighborDot)
    {
        LeaveNeigbors();
        int yDifference = (neighborDot.X - X) * -1;
        int xDifference = (neighborDot.Y - Y);

        Vector2Int direction = new Vector2Int(xDifference, yDifference);
        connectingCompass[direction] = true;
        this.DotStyling.DrawLine(direction);
        neighborDot.ConfirmAsNeighbor(direction * -1);

        await Task.Yield();
    }

    public void ConfirmAsNeighbor(Vector2Int direction)
    {
        connectingCompass[direction] = true;
    }
}

public class DotValue
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public DotValue(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}