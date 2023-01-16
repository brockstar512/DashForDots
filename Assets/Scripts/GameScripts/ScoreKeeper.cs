using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    int _height, _width;
    GridManager GridManager;
   
    public void Init(GridManager GridManager, int limit)
    {
        this.GridManager = GridManager;
        _height = _width = limit;
        //Debug.Log($"grid size :: {_height}, {_width}");
    }

    [ContextMenu("Check")]
    public void Check()
    {
        int childIndex = 0;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                ScoreHelper scoreHelper = GridManager.dots[x, y].GetComponent<ScoreHelper>();
                if (scoreHelper != null)
                {
                    PatrolParameter(GridManager.dots, x, y);
                    //does this dot have a score hlper? if so run the check, else continue
                }

                //Debug.Log("Child:: " + childIndex);
                childIndex++;
            }
        }
    }
    bool IsInBounds(int index, int limit)
    {
        //y == boundaryLimit
        //y == 0
        if (index == limit)
        {
            return false;
        }
        return true;
    }


    void PatrolParameter(Dot[,] dots, int x, int y)
    {
        //Debug.Log($"Index:{x},{y}");

        Dot currentDot = dots[x, y];

        //0,0 is the 0,1 in bounds?
        if (!IsInBounds(currentDot.coordinates.Y + 1, _height))
            return;
        

        if (currentDot.connectingCompass[Vector2Int.right] == true)
        {
            //0,1 is 1,1 in bounds
            if (!IsInBounds(currentDot.coordinates.X + 1, _height))
                return;
            
            currentDot = dots[currentDot.coordinates.X, currentDot.coordinates.Y + 1];//check the bounds too
            if(currentDot.connectingCompass[Vector2Int.down] == true)
            {

                //1,1 is 1,0 in boundss?
                if (!IsInBounds(currentDot.coordinates.Y - 1, -1))
                    return;

                currentDot = dots[currentDot.coordinates.X + 1, currentDot.coordinates.Y];
                if (currentDot.connectingCompass[Vector2Int.left] == true)
                {

                    //1,0 is 0,0 in bounds
                    if (!IsInBounds(currentDot.coordinates.X - 1, -1))
                        return;

                    currentDot = dots[currentDot.coordinates.X, currentDot.coordinates.Y - 1];

                    if (currentDot.connectingCompass[Vector2Int.up] == true)
                    {


                        //is a connected square
                        //run a function that handles the ui
                        dots[x, y].GetComponent<ScoreHelper>().ShowFill();
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
