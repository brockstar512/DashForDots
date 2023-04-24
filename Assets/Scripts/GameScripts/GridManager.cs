using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;
using DG.Tweening;
using System.Threading.Tasks;
using System;
using System.Threading;
using Unity.Netcode;

public class GridManager : MonoBehaviour
{
    public Dot[,] dots { get; private set; }
    public int _height, _width;
    public DotValue currentDot { get; private set; }
    public DotValue neighborDot { get; private set; }
    ScoreKeeper scoreKeeper;
    Action<Button> dotSubscriber;
    public TimerManager timerManager;
    public Action<int, int> OnSelectedDot;
    public Action<int, int> OnSelectedNeighbor;
    public Action OnSelectedCancel;
    public Action OnSelectedConfirm;
    public Action OnSelectedReset;
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
        dotSubscriber += SubscribeButton;
        scoreKeeper = GetComponent<ScoreKeeper>();
        scoreKeeper.Init(this, _height);

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
        OnSelectedDot?.Invoke(x, y);
        await SelectDotLocal(x, y);
    }

    public async Task SelectDotLocal(int x, int y)
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
        OnSelectedNeighbor?.Invoke(x, y);
        await SelectedNeighbor(x, y);
    }

    public async Task SelectedNeighbor(int x, int y)
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
            dot.button.onClick.AddListener(delegate
            {
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
        OnSelectedCancel?.Invoke();
        await OnCancel();
    }

    public async Task OnCancel()
    {
        await dots[currentDot.X, currentDot.Y].ChangeNeighborChoice(dots[neighborDot.X, neighborDot.Y]);
        neighborDot = null;
        dots[currentDot.X, currentDot.Y].OnSelect();
    }

    public async void Confirm()
    {
        if (!MultiplayerController.Instance.IsMultiplayer)
        {
            await OnConfirm(false);
        }
        else
        {
            OnSelectedConfirm?.Invoke();
        }

    }
    public async Task OnConfirm(bool isMultiplayer, ulong senderId = 0)
    {
        await LeaveDot();
        await dots[currentDot.X, currentDot.Y].Confirm(dots[neighborDot.X, neighborDot.Y]);
        SetPlayerTurn(currentDot.X, currentDot.Y, neighborDot.X, neighborDot.Y);
        currentDot = null;
        neighborDot = null;
        int scoreCount = await scoreKeeper.Check();
        bool flag = (senderId.Equals(NetworkManager.Singleton.LocalClientId) && isMultiplayer);
        if (!isMultiplayer || flag)
        {
            //if not true switch players else do nothing or i gues reset the clock
            if (scoreCount > 0)
            {
                if (MultiplayerController.Instance.IsMultiplayer)
                {
                    PlayerHandler.Instance.UpdateScoreServerRpc(scoreCount, GameFinished());
                }
                else
                {
                    Debug.Log("You are good to go");
                    PlayerHandler.Instance.UpdateScore(scoreCount, GameFinished());
                }
            }
            else
            {
                Debug.Log("You did not score");
                if (MultiplayerController.Instance.IsMultiplayer)
                {
                    PlayerHandler.Instance.NextTurnServerRpc();
                }
                else
                {
                    PlayerHandler.Instance.NextPlayer();
                }
                //switch player
            }
        }
    }

    private void SetPlayerTurn(int selectX, int selectY, int neighborX, int neighborY)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            PlayerTurn playerTurn = new PlayerTurn()
            {
                selectedDot = new Vector2(selectX, selectY),
                neighborDot = new Vector2(neighborX, neighborY),
                turn = PlayerHandler.Instance.currentPlayer.Value
            };
            MultiplayerController.Instance.SetPlayerTurn(playerTurn);
        }
    }
    public async Task UpdateRejoinPlayerUI()
    {
        if (MultiplayerController.Instance.IsMultiplayer)
        {
            MultiplayerData multiplayerData = MultiplayerController.Instance.GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
            if (multiplayerData.isRejoin)
            {              
                timerManager.DestoryScreenBlocker();
                await UpdateUIForRejoinPlayerAsync(MultiplayerController.Instance.playerTurnList, 0);
             
            }
        }
    }

    async Task UpdateUIForRejoinPlayerAsync(NetworkList<PlayerTurn> playerTurn, int index)
    {
        if (index < playerTurn.Count)
        {
            Time.timeScale = 20;
            PlayerHandler.Instance.SetPlayerDataSync(playerTurn[index].turn);
            await SelectDotLocal((int)playerTurn[index].selectedDot.x, (int)playerTurn[index].selectedDot.y);
            dots[(int)playerTurn[index].selectedDot.x, (int)playerTurn[index].selectedDot.y].DotStyling.SelectWithoutDelay();
            await SelectedNeighbor((int)playerTurn[index].neighborDot.x, (int)playerTurn[index].neighborDot.y);
            await LeaveDot();
            await dots[currentDot.X, currentDot.Y].Confirm(dots[neighborDot.X, neighborDot.Y]);
            currentDot = null;
            neighborDot = null;
            int scoreCount = await scoreKeeper.Check();
            if (scoreCount > 0)
            {
                PlayerHandler.Instance.UpdateScore(scoreCount, playerTurn[index].turn, GameFinished());
            }           
            Time.timeScale = 1;
            await UpdateUIForRejoinPlayerAsync(playerTurn, index + 1);

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

    internal void Reset()
    {
        OnSelectedReset?.Invoke();
    }
}
