using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;
using DG.Tweening;
using System.Threading.Tasks;

public class GridManager : MonoBehaviour
{
    [SerializeField] Dot[,] dots;
    int _height, _width;
    public bool hasCurrentDot { get; private set; }
    public bool hasNeighborDot { get; private set; }
    public Dot currentDot { get; private set; }
    public Dot neighborDot { get; private set; }
    public Color32 playerColor;
    public Color32 playerOptions;
    public Color32 neighborOption;




    public void Init(Transform dotParent)
    {
        int childIndex = 0;
        _height = _width = (int)Mathf.Sqrt(dotParent.childCount);
        //Debug.Log($"grid size :: {_height}, {_width}");

        dots = new Dot[_height, _width];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                //Debug.Log("Child:: " + childIndex);
                Dot dot = dotParent.GetChild(childIndex).gameObject.AddComponent<Dot>();
                dot.Init(x, y, _height -1,this);
                dots[x, y] = dot;
                DotDefaultState(dot);
                childIndex++;
            }
        }
    }

 

    public void HighlightNeighbors(int x, int y)
    {
        //Debug.Log("HighlightNeighbors");
        dots[x, y].GetComponent<SVGImage>().DOColor(this.playerColor, .15f); ;
        //        if (!dots[x, y].isConnectedLeft && x - 1 >= 0)

        if (!dots[x, y].connectingCompass[Vector2.left])
        {
            Dot dot = dots[x - 1, y];
            DotNeighborState(dot);
        }
        if (!dots[x, y].connectingCompass[Vector2.right])
        {
            Dot dot = dots[x + 1, y];
            DotNeighborState(dot);
        }
        if (!dots[x, y].connectingCompass[Vector2.up])
        {
            Dot dot = dots[x, y + 1];
            DotNeighborState(dot);
        }
        if (!dots[x, y].connectingCompass[Vector2.down])
        {
            //dots[x, y - 1] + Vector2.down
            Dot dot = dots[x, y - 1];
            DotNeighborState(dot);

        }
    }

    public void UnHighlightNeighbors(int x, int y)
    {

        if (!dots[x, y].connectingCompass[Vector2.left])
        {
            Dot dot = dots[x - 1, y];


            DotDefaultState(dot);

        }
        if (!dots[x, y].connectingCompass[Vector2.right])
        {
            Dot dot = dots[x + 1, y];
            DotDefaultState(dot);

        }
        if (!dots[x, y].connectingCompass[Vector2.up])
        {
            Dot dot = dots[x, y + 1];
            DotDefaultState(dot);
        }
        if (!dots[x, y].connectingCompass[Vector2.down])
        {
            Dot dot = dots[x, y - 1];
            DotDefaultState(dot);
        }
    }

    public void SelectDot(int x, int y)
    {
        if(hasCurrentDot != false && currentDot != dots[x, y])
        {
            //leave other dot
            dots[currentDot.X, currentDot.Y].OnDeselect();
        }
        hasCurrentDot = true;
        currentDot = dots[x, y];
        //currentDot.GetComponent<SVGImage>().color = playerColor;
        //HighlightNeighbors(x,y);
    }

    public void SelectNeighbor(int x, int y)
    {
        hasNeighborDot = true;
        neighborDot = dots[x, y];
        neighborDot.GetComponent<SVGImage>().DOColor(this.neighborOption, .15f);

    }

    void DotNeighborState(Dot dot)
    {
        dot.GetComponent<Button>().onClick.RemoveAllListeners();
        dot.GetComponent<SVGImage>().DOColor(this.playerOptions, .15f);
        dot.GetComponent<Button>().onClick.AddListener(dot.NeighboringChoice);
    }

    void DotDefaultState(Dot dot)
    {
        //dot.GetComponent<Button>().onClick.RemoveAllListeners();
        //dot.GetComponent<Button>().onClick.AddListener(delegate { StateManager.Inspect(dot.transform); });
        dot.GetComponent<Button>().onClick.AddListener(dot.OnSelect);
    }

    
    public void Reset()
    {
        Debug.Log("Reset");
        hasNeighborDot = false;

    }
    public void Confirm()
    {
        Debug.Log("Confirm");
    }
}
