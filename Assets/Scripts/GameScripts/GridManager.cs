using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;
using DG.Tweening;
using System.Threading.Tasks;
using System;


public class GridManager : MonoBehaviour
{
    public Dot[,] dots { get; private set; }
    int _height, _width;
    public DotValue currentDot { get; private set; }
    public DotValue neighborDot { get; private set; }


    private Action<Button> dotSubscriber;

    public void Init(Transform dotParent, Action<Button> SubscribeButton)
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
                dot.button.onClick.AddListener(dot.OnSelect);
                childIndex++;
            }
        }
        dotSubscriber += SubscribeButton;
    }


    async Task LeaveDot()
    {
        dots[currentDot.X, currentDot.Y].button.onClick.RemoveAllListeners();

        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.down])
        {
            Dot dot = dots[currentDot.X + 1, currentDot.Y];
            dot.button.onClick.RemoveAllListeners();
            dotSubscriber.Invoke(dot.button);
        }
        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.up])
        {
            Dot dot = dots[currentDot.X - 1, currentDot.Y];
            dot.button.onClick.RemoveAllListeners();
            dotSubscriber.Invoke(dot.button);
        }
        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.right])
        {
            Dot dot = dots[currentDot.X, currentDot.Y + 1];
            dot.button.onClick.RemoveAllListeners();
            dotSubscriber.Invoke(dot.button);
        }
        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.left])
        {
            Dot dot = dots[currentDot.X, currentDot.Y - 1];
            dot.button.onClick.RemoveAllListeners();
            dotSubscriber.Invoke(dot.button);
        }
        dots[currentDot.X, currentDot.Y].OnDeselect();
        dotSubscriber.Invoke(dots[currentDot.X, currentDot.Y].button);
        await Task.Yield();
    }

    public async void SelectDot(int x, int y)
    {
        if(currentDot != null && !currentDot.Equals(dots[x, y].coordinates))
        {
            await LeaveDot();
        }
        currentDot = dots[x, y].coordinates;
    }

    public void SelectNeighbor(int x, int y)
    {
        neighborDot = dots[x, y].coordinates;
    }




    public async void Cancel()
    {
        //hightlight neighbors and null neighbor do
        Debug.Log("Reset");
        await dots[currentDot.X, currentDot.Y].ChangeNeighborChoice(dots[neighborDot.X, neighborDot.Y]);
        neighborDot = null;
        dots[currentDot.X, currentDot.Y].OnSelect();


    }

    public async void Confirm()
    {
        
        Debug.Log("Confirm");
        await LeaveDot();
        await dots[currentDot.X, currentDot.Y].Confirm(dots[neighborDot.X, neighborDot.Y]);
        currentDot = null;
        neighborDot = null;
    }

}
