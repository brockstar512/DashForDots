using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;


public class Dot : MonoBehaviour
{
    [SerializeField] SVGImage circle;
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool isConnectedRight;
    public bool isConnectedLeft;
    public bool isConnectedUp;
    public bool isConnectedDown;
    Dictionary<Vector2, bool> connectingCompass;//avaiable direction

    public void Init(int x, int y, int boundaryLimit)
    {
        this.gameObject.name = $"Node {x},{y}";
        X = x;
        Y = y;
        circle = GetComponent<SVGImage>();

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
        Debug.Log(connectingCompass[Vector2.down]);

    }
}
