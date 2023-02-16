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
    public int _height, _width;
    public DotValue currentDot { get; private set; }
    public DotValue neighborDot { get; private set; }
    ScoreKeeper scoreKeeper;
    Action<Button> dotSubscriber;
    public TimerManager timerManager;
    [Header("Grid Basic Data")]
    public int numberOfDots = 0;
    public int numberOfHorizontalLine = 0;
    public int numberOfVerticalLine = 0;
    public int numberOfMoves = 0;
    public int numberOfBoxes = 0;
    public int numberOfBoxThirdLineComplete = 0;
    
    //Fill dot as per x*y input
    public void Init(Transform dotParent, Action<Button> SubscribeButton)
    {
        int childIndex = 0;
        _height = _width = (int)Mathf.Sqrt(dotParent.childCount);

        dots = new Dot[_height, _width];
        Debug.Log($"H : {_height} & W : {_width}");
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Dot dot = dotParent.GetChild(childIndex).gameObject.AddComponent<Dot>();
                dot.Init(x, y, _height - 1, this);
                dots[x, y] = dot;
                dot.button.onClick.AddListener(dot.OnSelect);
                childIndex++;
            }
        }
        GetNumberofMovesAndBoxes(_width, _height);
        dotSubscriber += SubscribeButton;
        scoreKeeper = GetComponent<ScoreKeeper>();
        scoreKeeper.Init(this, _height);
      
    }

    public void GetNumberofMovesAndBoxes(int row, int col)
    {
        numberOfDots = row * col;
        Debug.Log($"<color=yellow> Number of Dots: {numberOfDots}</color>");
        numberOfHorizontalLine = row * (col - 1);
        Debug.Log($"<color=yellow> Number of Horizontal Line: {numberOfHorizontalLine}</color>");
        numberOfVerticalLine = col * (row - 1);
        Debug.Log($"<color=yellow> Number of Vertical Line: {numberOfVerticalLine}</color>");
        numberOfMoves = 2 * row * col - col - row;
        numberOfBoxes = (row - 1) * (col - 1);
        Debug.Log($"<color=yellow> Number of moves: {numberOfMoves} </color>  & <color=pink>Number of Boxes: {numberOfBoxes}</color> ");
        
    }

    async Task LeaveDot()
    {
        if (neighborDot != null)
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
        if (currentDot != null)
        {
            await LeaveDot();
            if (neighborDot != null)
            { //this should not go here its earzing the line. instead it should go where the function is calling this on select dot
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
            dot.button.onClick.AddListener(delegate
            {
                dot.NeighboringChoice();

            });
        }

        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.right])
        {
            Debug.Log($"neighbor dot right : {Vector2Int.right}");
            Dot dot = dots[currentDot.X, currentDot.Y + 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate
            {
                dot.NeighboringChoice();

            });
        }
        if (!dots[currentDot.X, currentDot.Y].connectingCompass[Vector2Int.left])
        {
            Debug.Log($"neighbor dot left : {Vector2Int.left}");
            Dot dot = dots[currentDot.X, currentDot.Y - 1];
            dot.DotStyling.NeighborHighlight();
            dot.button.onClick.RemoveAllListeners();
            dot.button.onClick.AddListener(delegate
            {
                dot.NeighboringChoice();

            });
        }
    }
    public void LeaveNeighbors()
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
        //if not true switch players else do nothing or i gues reset the clock
        int scoreCount = await scoreKeeper.Check();
        if (scoreCount > 0)
        {
            Debug.Log("You are good to go");
            PlayerHandler.Instance.UpdateScore(scoreCount, GameFinished());
        }
        else
        {
            Debug.Log("You did not score");
            PlayerHandler.Instance.NextPlayer();
            //switch player

        }

    }


    public bool GameFinished()
    {
        foreach (Dot dot in dots)
        {
            if (dot.GetComponent<Button>() != null)
                return false;
        }
        return true;

    }

    private void OnDestroy()
    {
        dotSubscriber = null; ;
    }

}
