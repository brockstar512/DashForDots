using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using DG.Tweening;
using System.Threading.Tasks;

public class GridManager : MonoBehaviour
{
    [SerializeField] Dot[,] dots;
    int _height, _width;
    Dot currentDot;
    public Color32 playerColor;
    public Color32 playerOptions;

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
                dot.Init(x, y, _height,this);
                dots[x, y] = dot;
                dot.gameObject.name = $"Dot {x},{y}";
                childIndex++;
            }
        }
    }


    public void HighlightNeighbors(int x, int y)
    {

        currentDot.ColorThemeHelper.UnSubscribe();
        dots[x, y].GetComponent<SVGImage>().color = playerColor;

        if (!dots[x, y].isConnectedLeft && x - 1 >= 0)
        {

            Dot dot = dots[x - 1, y];
            dot.ColorThemeHelper.UnSubscribe();
            dot.GetComponent<SVGImage>().color = this.playerOptions;


        }
        if (!dots[x, y].isConnectedRight && x + 1 < _width)
        {

            Dot dot = dots[x + 1, y];
            dot.ColorThemeHelper.UnSubscribe();
            dot.GetComponent<SVGImage>().color = this.playerOptions;


        }
        if (!dots[x, y].isConnectedUp && y + 1 < _height)
        {

            Dot dot = dots[x, y + 1];
            dot.ColorThemeHelper.UnSubscribe();
            dot.GetComponent<SVGImage>().color = this.playerOptions;
            //image.DOColor(this.playerOptions, .15f);


        }
        if (!dots[x, y].isConnectedDown && y - 1 >= 0)
        {
            Dot dot = dots[x, y - 1];
            dot.ColorThemeHelper.UnSubscribe();
            dot.GetComponent<SVGImage>().color = this.playerOptions;
        }
    }
    //it's unsubscribing but not changing color
    public async Task UnHighlightNeighbors(int x, int y)
    {
        await currentDot.ColorThemeHelper.Subscribe(true);

        if (!dots[x, y].isConnectedLeft && x - 1 >= 0)
        {
            Dot dot = dots[x - 1, y];
            await dot.ColorThemeHelper.Subscribe(true);

        }
        if (!dots[x, y].isConnectedRight && x + 1 < _width)
        {
            Dot dot = dots[x + 1, y];
            await dot.ColorThemeHelper.Subscribe(true);

        }
        if (!dots[x, y].isConnectedUp && y + 1 < _height)
        {
            Dot dot = dots[x, y + 1];
            await dot.ColorThemeHelper.Subscribe(true);


        }
        if (!dots[x, y].isConnectedDown && y - 1 >= 0)
        {
            Dot dot = dots[x, y - 1];
            await dot.ColorThemeHelper.Subscribe(true);
        }
        await Task.Yield();
    }

    public async void SelectDot(int x, int y)
    {
        if(currentDot != null && currentDot != dots[x, y])
        {
            await UnHighlightNeighbors(currentDot.X, currentDot.Y);


        }

        currentDot = dots[x, y];
        currentDot.GetComponent<SVGImage>().color = playerColor;
        HighlightNeighbors(x,y);
    }
    //maybe one that doensnt rely on start
    //on start it forces color
    public async Task LeaveDot()
    {
        await UnHighlightNeighbors(currentDot.X, currentDot.Y);
        await Task.Yield();
    }

    void ShowColor(SVGImage image)
    {
        image.color = this.playerOptions;
        //image.DOColor(this.playerOptions, .15f);
    }
}
