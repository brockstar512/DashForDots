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
        //GetNeigbors();

        if (!connectingCompass[Vector2Int.down])
        {
            Dot dot = GridManager.dots[coordinates.X + 1, coordinates.Y];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {
                LeaveUnselectedNeigbors(dot);
                dot.NeighboringChoice();

            });
        }

        if (!connectingCompass[Vector2Int.up])
        {
            Dot dot = GridManager.dots[coordinates.X - 1, coordinates.Y];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {
                this.LeaveUnselectedNeigbors(dot);
                dot.NeighboringChoice();
            });
        }

        if (!connectingCompass[Vector2Int.right])
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y + 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {
                LeaveUnselectedNeigbors(dot);
                dot.NeighboringChoice();

            });
        }
        if (!connectingCompass[Vector2Int.left])
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y - 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {
                LeaveUnselectedNeigbors(dot);
                dot.NeighboringChoice();
            });
        }
    }

    void GetNeigbors()
    {
        if (!connectingCompass[Vector2Int.down])
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

    }
    //depending which dot i select i need to tell this dot i selected it

    //i selected your neighboring direction
    void LeaveUnselectedNeigbors(Dot selectedNeighbor)
    {
        //this is your direction you are looking at
        Vector2Int selectedNeighbordirection = GetDifference(selectedNeighbor);
        Debug.Log("Selected neighbopr  " + selectedNeighbordirection);
        this.DotStyling.DrawLine(selectedNeighbordirection);

        //if you did not select the dot in this direction
        //and there is a dot that exists
        if (!connectingCompass[Vector2Int.down] && selectedNeighbordirection != Vector2Int.down)
        {
            Dot dot = GridManager.dots[coordinates.X + 1, coordinates.Y];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.RemoveAllListeners();
        }
        if (!connectingCompass[Vector2Int.up] && selectedNeighbordirection != Vector2Int.up)
        {
            Dot dot = GridManager.dots[coordinates.X - 1, coordinates.Y];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.RemoveAllListeners();
        }
        if (!connectingCompass[Vector2Int.left] && selectedNeighbordirection != Vector2Int.left)
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y - 1];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.RemoveAllListeners();
        }
        if (!connectingCompass[Vector2Int.right] && selectedNeighbordirection != Vector2Int.right)
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y + 1];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.RemoveAllListeners();
        }
  

    }

    public async Task ChangeNeighborChoice(Dot oldNeighborChoice)
    {
        DotStyling.EraseLine(GetDifference(oldNeighborChoice));
        await Task.Yield();
    }


    void LeaveNeigbors()
    {
        if (!connectingCompass[Vector2Int.down])
        {

            Dot dot = GridManager.dots[coordinates.X + 1, coordinates.Y];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.AddListener(dot.OnSelect);
        }
        if (!connectingCompass[Vector2Int.up])
        {
            Dot dot = GridManager.dots[coordinates.X - 1, coordinates.Y];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.AddListener(dot.OnSelect);

        }
        if (!connectingCompass[Vector2Int.right])
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y + 1];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.AddListener(dot.OnSelect);

        }
        if (!connectingCompass[Vector2Int.left])
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y - 1];
            dot.DotStyling.NeighborUnHighlight();
            dot.button.onClick.AddListener(dot.OnSelect);

        }
    }

    //unsure about this one
    public void OnDeselect()
    {
        button.onClick.AddListener(OnSelect);
        DotStyling.Deselect();
        //LeaveNeigbors();
    }

    //good -> triggers grid manager and styling for the nighor
    public void NeighboringChoice()
    {
        GridManager.SelectNeighbor(coordinates.X, coordinates.Y);
        DotStyling.PairingSelected();
    }

    //good
    public async Task Confirm(Dot neighborDot)
    {
        Vector2Int direction = GetDifference(neighborDot);
        connectingCompass[direction] = true;
        //this.DotStyling.DrawLine(direction);
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