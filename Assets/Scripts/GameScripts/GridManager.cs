using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;
using DG.Tweening;
using System.Threading.Tasks;

public class GridManager : MonoBehaviour
{
    public Dot[,] dots { get; private set; }
    int _height, _width;
    public bool hasCurrentDot { get; private set; }
    public bool hasNeighborDot { get; private set; }
    public Dot currentDot { get; private set; }
    public Dot neighborDot { get; private set; }

    //keep delegate


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

    async Task LeaveDot()
    {
        dots[currentDot.X, currentDot.Y].OnDeselect();
        //pass this to delegate
        //dots[currentDot.X, currentDot.Y]
        //and the neighbors
        //invoke
        await Task.Yield();
    }

    public async void SelectDot(int x, int y)
    {
        if(hasCurrentDot != false && currentDot != dots[x, y])
        {
            await LeaveDot();
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
    }

    void DotNeighborState(Dot dot)
    {
        dot.GetComponent<Button>().onClick.RemoveAllListeners();
        //dot.GetComponent<SVGImage>().DOColor(this.playerOptions, .15f);
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
