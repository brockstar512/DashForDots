using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    int _height, _width;

    public void Init(GridManager GridManager)
    {
        int childIndex = 0;
        _height = _width = GridManager.dots.Length;
        //Debug.Log($"grid size :: {_height}, {_width}");

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Dot currentDot = GridManager.dots[x, y];
                PatrolParameter(GridManager.dots,x,y);
                //does this dot have a score hlper? if so run the check, else continue

                //Debug.Log("Child:: " + childIndex);
                //dots[x, y] = dot;
                childIndex++;
            }
        }
    }

    void PatrolParameter(Dot[,] dots, int x, int y)
    {
        Dot currentDot = dots[x, y];
        if(currentDot.connectingCompass[Vector2Int.right] == true)
        {
            currentDot = dots[x, y + 1];
            if(currentDot.connectingCompass[Vector2Int.down] == true)
            {
                currentDot = dots[x + 1, y];
                if (currentDot.connectingCompass[Vector2Int.left] == true)
                {
                    currentDot = dots[x, y - 1];

                    if (currentDot.connectingCompass[Vector2Int.up] == true)
                    {
                        //is a connected square
                        //run a function that handles the ui
                        //handle the score
                        //once that is done remove the patrol helper script on the original dot
                        //check if the player scored if so... go again. oyherwie switch
                        //switch players

                    }

                }

            }
        }

    }
}
