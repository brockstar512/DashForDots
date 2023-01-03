using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Dot[,] dots;
    int _height, _width;

    public void Init(Transform dotParent)
    {
        int childIndex = 0;
        _height = _width = (int)Mathf.Sqrt(dotParent.childCount);
        Debug.Log($"grid size :: {_height}, {_width}");

        dots = new Dot[_height, _width];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Debug.Log("Child:: " + childIndex);
                Dot dot = dotParent.GetChild(childIndex).gameObject.AddComponent<Dot>();
                dot.Init(x, y, _height);
                dots[x, y] = dot;
                dot.gameObject.name = $"Dot {x},{y}";
                childIndex++;
            }
        }
    }


    public void HighlightNeighbors(int x, int y)
    {
        dots[x, y].GetComponent<SpriteRenderer>().color = Color.red;
        if (!dots[x, y].isConnectedLeft && x - 1 >= 0)
            dots[x - 1, y].GetComponent<SpriteRenderer>().color = Color.black;
        if (!dots[x, y].isConnectedRight && x + 1 < _width)
            dots[x + 1, y].GetComponent<SpriteRenderer>().color = Color.black;
        if (!dots[x, y].isConnectedUp && y + 1 < _height)
            dots[x, y + 1].GetComponent<SpriteRenderer>().color = Color.black;
        if (!dots[x, y].isConnectedDown && y - 1 >= 0)
            dots[x, y - 1].GetComponent<SpriteRenderer>().color = Color.black;

    }
}
