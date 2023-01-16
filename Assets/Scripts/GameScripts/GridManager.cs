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
    public ScoreKeeper scoreKeeper;
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
        scoreKeeper.Init(this, _height);
    }

    private void Update()
    {
        //Debug.Log("Is dot null?  "+ currentDot ==null);
    }

    async Task LeaveDot()
    {
        if (neighborDot !=null)
        {
            //this should not go here its earzing the line. instead it should go where the function is calling this on select dot
            //consider cancelling or just fading the line away in this instance
            //maybe just cance?
            //await dots[currentDot.X, currentDot.Y].ChangeNeighborChoice(dots[neighborDot.X, neighborDot.Y]);
            
        }

        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.down])
        {
            Dot dot = dots[currentDot.X + 1, currentDot.Y];
            dot.DotStyling.Deselect();
            dot.button.onClick.RemoveAllListeners();
            ResetDot(dot);
        }

        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.up])
        {
            Dot dot = dots[currentDot.X - 1, currentDot.Y];
            dot.DotStyling.Deselect();
            dot.button.onClick.RemoveAllListeners();
            ResetDot(dot);
        }

        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.right])
        {
            Dot dot = dots[currentDot.X, currentDot.Y + 1];
            dot.DotStyling.Deselect();
            dot.button.onClick.RemoveAllListeners();
            ResetDot(dot);
        }
        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.left])
        {
            Dot dot = dots[currentDot.X, currentDot.Y - 1];
            dot.DotStyling.Deselect();
            dot.button.onClick.RemoveAllListeners();
            ResetDot(dot);
        }
        dots[currentDot.X, currentDot.Y].DotStyling.Deselect();
        ResetDot(dots[currentDot.X, currentDot.Y]);
        await Task.Yield();
    }

    public async void SelectDot(int x, int y)
    {
        if(currentDot != null)
        {
            await LeaveDot();
            if (neighborDot != null)
            {
                //this should not go here its earzing the line. instead it should go where the function is calling this on select dot
                //consider cancelling or just fading the line away in this instance
                //maybe just cance?
                await dots[currentDot.X, currentDot.Y].ChangeNeighborChoice(dots[neighborDot.X, neighborDot.Y]);

            }
        }
        currentDot = dots[x, y].coordinates;
        IntroduceNeighbors();
    }

    public async void SelectNeighbor(int x, int y)
    {
        neighborDot = dots[x, y].coordinates;
        LeaveNeighbors();
        await dots[currentDot.X, currentDot.Y].PairWithNeighbor(dots[neighborDot.X, neighborDot.Y]);
    }

    public void ResetDot(Dot dot)
    {
        dot.button.onClick.RemoveAllListeners();
        dot.button.onClick.AddListener(dot.OnSelect);
        dotSubscriber.Invoke(dot.button);

    }
    
    void IntroduceNeighbors()
    {
        neighborDot = null;
        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.down])
        {
            Dot dot = dots[currentDot.X + 1, currentDot.Y];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {
                dot.NeighboringChoice();
            });
        }

        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.up])
        {
            Dot dot = dots[currentDot.X - 1, currentDot.Y];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {
                dot.NeighboringChoice();

            });
        }

        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.right])
        {
            Dot dot = dots[currentDot.X, currentDot.Y + 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {
                dot.NeighboringChoice();

            });
        }
        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.left])
        {
            Dot dot = dots[currentDot.X, currentDot.Y - 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate {
                dot.NeighboringChoice();

            });
        }
    }

    void LeaveNeighbors()
    {
        //is current dot null?
        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.down])
        {
            Dot dot = dots[currentDot.X + 1, currentDot.Y];
            if (neighborDot != dot.coordinates)
            {
                dot.DotStyling.Deselect();
                dot.button.onClick.RemoveAllListeners();
                ResetDot(dot);
            }
        }

        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.up])
        {
            Dot dot = dots[currentDot.X - 1, currentDot.Y];
            if (neighborDot != dot.coordinates)
            {
                dot.DotStyling.Deselect();
                dot.button.onClick.RemoveAllListeners();
                ResetDot(dot);
            }

        }

        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.right])
        {
            Dot dot = dots[currentDot.X, currentDot.Y + 1];
            if (neighborDot != dot.coordinates)
            {
                dot.DotStyling.Deselect();
                dot.button.onClick.RemoveAllListeners();
                ResetDot(dot);
            }

        }
        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.left])
        {
            Dot dot = dots[currentDot.X, currentDot.Y - 1];
            if (neighborDot != dot.coordinates)
            {
                dot.DotStyling.Deselect();
                dot.button.onClick.RemoveAllListeners();
                ResetDot(dot);
            }

        }
    }

    public async void Cancel()
    {
        //hightlight neighbors and null neighbor do
        //Debug.Log("Reset");
        await dots[currentDot.X, currentDot.Y].ChangeNeighborChoice(dots[neighborDot.X, neighborDot.Y]);
        neighborDot = null;
        dots[currentDot.X, currentDot.Y].OnSelect();


    }

    public async void Confirm()
    {
        
        //Debug.Log("Confirm");
        await LeaveDot();
        await dots[currentDot.X, currentDot.Y].Confirm(dots[neighborDot.X, neighborDot.Y]);
        currentDot = null;
        neighborDot = null;
    }

}
