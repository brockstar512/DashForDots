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
            });
        }

        if (!connectingCompass[Vector2Int.up])
        {
            Dot dot = GridManager.dots[coordinates.X - 1, coordinates.Y];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {
                LeaveUnselectedNeigbors(dot);
            });
        }

        if (!connectingCompass[Vector2Int.right])
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y + 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {

                LeaveUnselectedNeigbors(dot);
            });
        }
        if (!connectingCompass[Vector2Int.left])
        {
            Dot dot = GridManager.dots[coordinates.X, coordinates.Y - 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {

                LeaveUnselectedNeigbors(dot);
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
        Vector2Int selectedNeighbordirection = GetDifference(selectedNeighbor);
        if (!connectingCompass[Vector2Int.down] )
        {
            Dot dot = GridManager.dots[coordinates.X + 1, coordinates.Y];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            Vector2Int direction = GetDifference(dot);
            if(direction == selectedNeighbordirection)
            {
                //
                //dot.button.onClick.AddListener(delegate {
                //    //change the UI color
                //    dot.NeighboringChoice();
                //    draw line to this
                //    this.DotStyling.DrawLine(direction);
                //});
            }
            else
            {
                //leave and reset
                //reset UI looking
                //add onclick subscription
            }
                
            //dot.button.onClick.AddListener(delegate {
            //    dot.NeighboringChoice();
            //    this.DotStyling.DrawLine(direction);
            //});
        }
        if (!connectingCompass[Vector2Int.down])
        {
            Dot dot = GridManager.dots[coordinates.X + 1, coordinates.Y];
            dot.button.onClick.RemoveAllListeners();
            Vector2Int direction = GetDifference(dot);
            if (direction != selectedNeighbordirection)
            {
                //this is what you are selecting
            }
            else
            {
                if (!connectingCompass[Vector2Int.down])
                {
                    dot = GridManager.dots[coordinates.X + 1, coordinates.Y];
                    dot.button.onClick.RemoveAllListeners();
                    dot.button.onClick.AddListener(delegate {



                    });
                }

                if (!connectingCompass[Vector2Int.up])
                {
                    dot = GridManager.dots[coordinates.X - 1, coordinates.Y];
                    dot.button.onClick.RemoveAllListeners();
                    dot.button.onClick.AddListener(delegate {



                    });
                }

                if (!connectingCompass[Vector2Int.right])
                {
                    dot = GridManager.dots[coordinates.X, coordinates.Y + 1];
                    dot.button.onClick.RemoveAllListeners();
                    dot.button.onClick.AddListener(delegate {



                    });
                }
                if (!connectingCompass[Vector2Int.left])
                {
                    dot = GridManager.dots[coordinates.X, coordinates.Y - 1];
                    dot.button.onClick.RemoveAllListeners();
                    dot.button.onClick.AddListener(delegate {


                        
                    });
                }
            }

            //dot.button.onClick.AddListener(delegate {
            //    dot.NeighboringChoice();
            //    this.DotStyling.DrawLine(direction);
            //});
        }

        if (!connectingCompass[Vector2Int.up])
        {
            Dot dot = GridManager.dots[coordinates.X - 1, coordinates.Y];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            Vector2Int direction = GetDifference(dot);
            if (direction == selectedNeighbordirection)
            {
                //leave
            }
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
            if (direction == selectedNeighbordirection)
            {
                //leave
            }
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
            if (direction == selectedNeighbordirection)
            {
                //leave
            }
            dot.button.onClick.AddListener(delegate {
                dot.NeighboringChoice();
                this.DotStyling.DrawLine(direction);
            });
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

    public void NeighboringChoice()
    {
        GridManager.SelectNeighbor(coordinates.X, coordinates.Y);
        DotStyling.PairingSelected();
    }

    public void OnDeselect()
    {
        button.onClick.AddListener(OnSelect);
        DotStyling.Deselect();
        LeaveNeigbors();
    }

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