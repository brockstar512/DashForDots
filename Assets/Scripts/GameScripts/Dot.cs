using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;



public class Dot : MonoBehaviour
{
    //public int X { get; private set; }
    //public int Y { get; private set; }
    [SerializeField] public Button button { get; private set; }

    public Dictionary<Vector2Int, bool> connectingCompass { get; private set; }//avaiable direction
    public DotStyling DotStyling { get; private set; }
    GridManager GridManager;
    public DotValue coordinates { get; private set; }


    public void Init(int x, int y, int boundaryLimit, GridManager GridManager)
    {
        this.GridManager = GridManager;
        this.gameObject.name = $"Dot {x},{y}";

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
        GridManager.SelectDot(coordinates.X, coordinates.Y);
        DotStyling.Select();
        button.onClick.RemoveAllListeners();
    }

 
    public async Task ChangeNeighborChoice(Dot oldNeighborChoice)
    {
        DotStyling.EraseLine(GetDifference(oldNeighborChoice));
        await Task.Yield();
    }

    public void NeighboringChoice()
    {
        GridManager.SelectNeighbor(coordinates.X, coordinates.Y);
    }


    public async Task Confirm(Dot neighborDot)
    {
        Vector2Int direction = GetDifference(neighborDot);
        connectingCompass[direction] = true;
        neighborDot.ConfirmAsNeighbor(direction * -1);

        await Task.Yield();
    }

    public void ConfirmAsNeighbor(Vector2Int direction)
    {
        connectingCompass[direction] = true;
    }

    Vector2Int GetDifference(Dot neighborDot)
    {
        int yDifference = (neighborDot.coordinates.X - coordinates.X) * -1;
        int xDifference = (neighborDot.coordinates.Y - coordinates.Y);
        return new Vector2Int(xDifference, yDifference);
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

    public bool Equals(DotValue dotValue)
    {
        return (this.X == dotValue.X && this.Y == dotValue.Y);
    }
}

/*
 *         if (!connectingCompass[Vector2Int.down])
        {
            Dot dot = GridManager.dots[coordinates.X + 1, coordinates.Y];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            Vector2Int direction = GetDifference(dot);
            dot.button.onClick.AddListener(delegate {
                dot.NeighboringChoice();
                this.DotStyling.DrawLine(direction);
            });
        }

        if (!connectingCompass[Vector2Int.up])
        {
            Dot dot = GridManager.dots[coordinates.X - 1, coordinates.Y];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            Vector2Int direction = GetDifference(dot);
            dot.button.onClick.AddListener(delegate {
                dot.NeighboringChoice();
                this.DotStyling.DrawLine(direction);
            });
        }

        if (!connectingCompass[Vector2Int.right])
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y + 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            Vector2Int direction = GetDifference(dot);
            dot.button.onClick.AddListener(delegate {
                dot.NeighboringChoice();
                this.DotStyling.DrawLine(direction);
            });
        }
        if (!connectingCompass[Vector2Int.left])
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y - 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            Vector2Int direction = GetDifference(dot);
            dot.button.onClick.AddListener(delegate {
                dot.NeighboringChoice();
                this.DotStyling.DrawLine(direction);
            });
        }
 * 
 */