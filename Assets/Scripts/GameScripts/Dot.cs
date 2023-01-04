using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Dot : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool isConnectedRight;
    public bool isConnectedLeft;
    public bool isConnectedUp;
    public bool isConnectedDown;
    public ColorThemeHelper ColorThemeHelper { get; private set; }
    Dictionary<Vector2, bool> connectingCompass;//avaiable direction
    GridManager GridManager;

    public void Init(int x, int y, int boundaryLimit, GridManager GridManager)
    {
        ColorThemeHelper = GetComponent<ColorThemeHelper>();
        this.GridManager = GridManager;
        this.gameObject.name = $"Node {x},{y}";
        X = x;
        Y = y;

        connectingCompass = new Dictionary<Vector2, bool>()
        {
            {Vector2.down,true },
            {Vector2.up,true },
            {Vector2.right,true },
            {Vector2.left,true },

        };

        if (x == 0)
            connectingCompass[Vector2.left] = false;
        if (x == boundaryLimit)
            connectingCompass[Vector2.right] = false;
        if (y == 0)
            connectingCompass[Vector2.down] = false;
        if (y == boundaryLimit)
            connectingCompass[Vector2.up] = false;
        //connectingCompass[new Vector2(x, 1)];

        Debug.Log($"x,y is {x},{y}");
        //Debug.Log(connectingCompass[Vector2.down]);
        this.GetComponent<Button>().onClick.AddListener(OnSelect);
    }

    public void OnSelect()
    {
        GridManager.SelectDot(X,Y);
    }

}
