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


    public void HighlightNeighbors(int x, int y, bool isLeaving)
    {
        if(!isLeaving)
            dots[x, y].GetComponent<SVGImage>().color = playerColor;

        if (!dots[x, y].isConnectedLeft && x - 1 >= 0)
        {
            Dot dot = dots[x - 1, y];
            if (isLeaving)
            {
                dot.ColorThemeHelper.Subscribe();

            }
            else
            {
                dot.ColorThemeHelper.UnSubscribe();
                ShowColor(dot.GetComponent<SVGImage>());
            }
        }
        if (!dots[x, y].isConnectedRight && x + 1 < _width)
        {
            Dot dot = dots[x + 1, y];
            if (isLeaving)
            {
                dot.ColorThemeHelper.Subscribe();
            }
            else
            {
                dot.ColorThemeHelper.UnSubscribe();
                ShowColor(dot.GetComponent<SVGImage>());
            }
        }
        if (!dots[x, y].isConnectedUp && y + 1 < _height)
        {
            Dot dot = dots[x, y + 1];

            if (isLeaving)
            {
                dot.ColorThemeHelper.Subscribe();
            }
            else
            {
                dot.ColorThemeHelper.UnSubscribe();
                ShowColor(dot.GetComponent<SVGImage>());
            }

        }
        if (!dots[x, y].isConnectedDown && y - 1 >= 0)
        {
            Dot dot = dots[x, y - 1];
            if (isLeaving)
            {
                dot.ColorThemeHelper.Subscribe();
            }
            else
            {
                dot.ColorThemeHelper.UnSubscribe();
                ShowColor(dot.GetComponent<SVGImage>());
            }
        }
    }

    public void SelectDot(int x, int y)
    {
        if(currentDot != null && currentDot != dots[x, y])
        {
            LeaveDot();
        }

        currentDot = dots[x, y];
        currentDot.GetComponent<SVGImage>().color = playerColor;
        HighlightNeighbors(x,y,false);
        currentDot.ColorThemeHelper.UnSubscribe();
    }

    public void LeaveDot()
    {
        HighlightNeighbors(currentDot.X, currentDot.Y,true);
        currentDot.ColorThemeHelper.Subscribe();
    }

    void ShowColor(SVGImage image)
    {
        image.DOColor(this.playerOptions, .15f);
    }
}
