using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;
using DG.Tweening;
using System.Threading.Tasks;

public class GridManager : MonoBehaviour
{
    StateManager StateManager;
    [SerializeField] Dot[,] dots;
    int _height, _width;
    public Dot currentDot { get; private set; }
    public Color32 playerColor;
    public Color32 playerOptions;

    public void Init(Transform dotParent, StateManager StateManager)
    {
        this.StateManager = StateManager;
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
                dot.Init(x, y, _height,this);
                dots[x, y] = dot;
                DotDefaultState(dot);
                childIndex++;
            }
        }
    }


    public void HighlightNeighbors(int x, int y)
    {
        Debug.Log("HighlightNeighbors");

        currentDot.ColorThemeHelper.UnSubscribe();
        dots[x, y].GetComponent<SVGImage>().DOColor(this.playerColor, .15f); ;

        if (!dots[x, y].isConnectedLeft && x - 1 >= 0)
        {
            Dot dot = dots[x - 1, y];
            DotNeighborState(dot);
        }
        if (!dots[x, y].isConnectedRight && x + 1 < _width)
        {
            Dot dot = dots[x + 1, y];
            DotNeighborState(dot);
        }
        if (!dots[x, y].isConnectedUp && y + 1 < _height)
        {
            Dot dot = dots[x, y + 1];
            DotNeighborState(dot);
        }
        if (!dots[x, y].isConnectedDown && y - 1 >= 0)
        {
            Dot dot = dots[x, y - 1];
            DotNeighborState(dot);

        }
    }

    public async Task UnHighlightNeighbors(int x, int y, bool isResetting)
    {
        Debug.Log("UnHighlightNeighbors");
        await currentDot.ColorThemeHelper.Subscribe(true);

        if (!dots[x, y].isConnectedLeft && x - 1 >= 0)
        {
            Dot dot = dots[x - 1, y];
            if (!isResetting)
            {
                await dot.ColorThemeHelper.Subscribe(true);
            }
            else
            {
                dot.ColorThemeHelper.Subscribe();
            }
            DotDefaultState(dot);

        }
        if (!dots[x, y].isConnectedRight && x + 1 < _width)
        {
            Dot dot = dots[x + 1, y];
            if (!isResetting)
            {
                await dot.ColorThemeHelper.Subscribe(true);
            }
            else
            {
                dot.ColorThemeHelper.Subscribe();
            }
            DotDefaultState(dot);

        }
        if (!dots[x, y].isConnectedUp && y + 1 < _height)
        {
            Dot dot = dots[x, y + 1];
            if (!isResetting)
            {
                await dot.ColorThemeHelper.Subscribe(true);
            }
            else
            {
                dot.ColorThemeHelper.Subscribe();
            }
            DotDefaultState(dot);
        }
        if (!dots[x, y].isConnectedDown && y - 1 >= 0)
        {
            Dot dot = dots[x, y - 1];
            if (!isResetting)
            {
                await dot.ColorThemeHelper.Subscribe(true);
            }
            else
            {
                dot.ColorThemeHelper.Subscribe();
            }
            DotDefaultState(dot);
        }
        currentDot = null;
        await Task.Yield();
    }

    public async void SelectDot(int x, int y)
    {
        if(currentDot != null && currentDot != dots[x, y])
        {
            await UnHighlightNeighbors(currentDot.X, currentDot.Y,false);
        }

        currentDot = dots[x, y];
        currentDot.GetComponent<SVGImage>().color = playerColor;
        HighlightNeighbors(x,y);
    }

    public void DotNeighborState(Dot dot)
    {
        dot.GetComponent<Button>().onClick.RemoveAllListeners();
        dot.ColorThemeHelper.UnSubscribe();
        dot.GetComponent<SVGImage>().DOColor(this.playerOptions, .15f);

    }

    void DotDefaultState(Dot dot)
    {
        dot.GetComponent<Button>().onClick.RemoveAllListeners();
        dot.GetComponent<Button>().onClick.AddListener(delegate { StateManager.Inspect(dot.transform); });
        dot.GetComponent<Button>().onClick.AddListener(dot.OnSelect);
    }
}
