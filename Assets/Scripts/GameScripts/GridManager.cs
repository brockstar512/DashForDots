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
    public bool hasCurrentDot { get; private set; }
    public bool hasNeighborDot { get; private set; }
    public Dot currentDot { get; private set; }
    public Dot neighborDot { get; private set; }
    private Action<Button> dotSubscriber;

    public void Init(Transform dotParent, Action<Button> SubscribeButton)
    {
        int childIndex = 0;
        _height = _width = (int)Mathf.Sqrt(dotParent.childCount);
        //Debug.Log($"grid size :: {_height}, {_width}");

        dots = new Dot[_height, _width];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
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
        if(hasCurrentDot != false && currentDot != dots[x, y])
        {
            await LeaveDot();
        }
        hasCurrentDot = true;
        currentDot = dots[x, y];
    }

    public void SelectNeighbor(int x, int y)
    {
        hasNeighborDot = true;
        neighborDot = dots[x, y];
    }





    public void Cancel()
    {
        Debug.Log("Reset");
        //hasNeighborDot = false;
        //await LeaveDot();
        currentDot.OnSelect();

    }
    //public async void Reset()
    //{
    //    Debug.Log("Reset");
    //    hasNeighborDot = false;
    //    await LeaveDot();

    //}
    public void Confirm()
    {
        Debug.Log("Confirm");
        currentDot.Confirm(neighborDot);
    }

    private void OnDestroy()
    {
        //dotSubscriber -= SubscribeButton;

    }
}
