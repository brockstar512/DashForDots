using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerMove : MonoBehaviour
{ 
    // how to check the situations of the do
    // make a list of all the connecting dots of the grid manager


    GridManager gridManager;
    List<List<Vector2Int>> validMoves = new List<List<Vector2Int>>();

    Dictionary<Vector2Int, Vector2Int> directionAccordingToGrid = new Dictionary<Vector2Int, Vector2Int>()
    {
        { Vector2Int.up, Vector2Int.left },
        { Vector2Int.right, Vector2Int.up },
        { Vector2Int.down, Vector2Int.right },
        { Vector2Int.left, Vector2Int.down },

    };

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    private void GetAllValidMoves()
    {
        validMoves.Clear();
        foreach (Dot dot in gridManager.dots)
        {
            print(dot.name);
            if (!dot.connectingCompass[Vector2Int.down])
            {
                List<Vector2Int> addToValidList = new List<Vector2Int>();
                Vector2Int dotCoordinate = new Vector2Int(dot.coordinates.X, dot.coordinates.Y);
                addToValidList.Add(dotCoordinate);
                addToValidList.Add(directionAccordingToGrid[ Vector2Int.down]);
                validMoves.Add(addToValidList);
            }

            if (!dot.connectingCompass[Vector2Int.up])
            {
                List<Vector2Int> addToValidList = new List<Vector2Int>();
                Vector2Int dotCoordinate = new Vector2Int(dot.coordinates.X, dot.coordinates.Y);
                addToValidList.Add(dotCoordinate);
                addToValidList.Add(directionAccordingToGrid[Vector2Int.up]);
                validMoves.Add(addToValidList);
            }

            if (!dot.connectingCompass[Vector2Int.right])
            {
                List<Vector2Int> addToValidList = new List<Vector2Int>();
                Vector2Int dotCoordinate = new Vector2Int(dot.coordinates.X, dot.coordinates.Y);
                addToValidList.Add(dotCoordinate);
                addToValidList.Add(directionAccordingToGrid[Vector2Int.right]);
                validMoves.Add(addToValidList);
            }
            if (!dot.connectingCompass[Vector2Int.left])
            {
                List<Vector2Int> addToValidList = new List<Vector2Int>();
                Vector2Int dotCoordinate = new Vector2Int(dot.coordinates.X, dot.coordinates.Y);
                addToValidList.Add(dotCoordinate);
                addToValidList.Add(directionAccordingToGrid[Vector2Int.left]);
                validMoves.Add(addToValidList);

            }
        }

    }


    public void CalculateBestMove()
    {
        int bestIndex = GetBestMove();
        print(bestIndex);
        print("The best line will be between " + validMoves[bestIndex][0] + " and " + validMoves[bestIndex][1]);
        Vector2Int dot1 = validMoves[bestIndex][0];
        Vector2Int dot2 = validMoves[bestIndex][0] + validMoves[bestIndex][1];
        gridManager.SelectDot(dot1.x, dot1.y);
        gridManager.SelectNeighbor(dot2.x, dot2.y);
        gridManager.Confirm();
    }



    private int GetBestMove()
    {
        GetAllValidMoves();
        int mostOptimalMoveValue = -1000;
        int mostOptimalMoveIndex = 0;
        int i = 0;
        foreach (var move in validMoves)
        {
            int moveValue = 0;
            //check number of lines going through the points 



            Vector2Int coordinate1 = move[0];
            Vector2Int coordinate2 = move[0] + move[1];
            Vector2Int direction = move[1];
            print("coordinate 1 is " + coordinate1 + " move is " + direction + "coordinate 2 is " + coordinate2);
            Dot dot1 = gridManager.dots[coordinate1.x, coordinate1.y];
            Dot dot2 = gridManager.dots[coordinate2.x, coordinate2.y];

            //check for square
            if (direction == Vector2Int.left || direction == Vector2Int.right)
            {
                if ((dot1.connectingCompass[Vector2Int.down] == true && dot2.connectingCompass[Vector2Int.down] == true) &&
                   (dot1.connectingCompass[Vector2Int.up] == true && dot2.connectingCompass[Vector2Int.up] == true))
                {

                    if ((gridManager.dots[(coordinate1 + Vector2Int.up).x, (coordinate1 + Vector2Int.up).y] == true &&
                        gridManager.dots[(coordinate2 + Vector2Int.up).x, (coordinate2 + Vector2Int.up).y].connectingCompass[Vector2Int.left] == true)
                        ||
                        (gridManager.dots[(coordinate1 + Vector2Int.down).x, (coordinate1 + Vector2Int.down).y] != null &&
                        gridManager.dots[(coordinate2 + Vector2Int.down).x, (coordinate2 + Vector2Int.down).y].connectingCompass[Vector2Int.right] == true))
                    {
                        moveValue = 1000;
                    }
                    
                    

                }


                //check for same direction parallel lines from both dots
                else if (dot1.connectingCompass[Vector2Int.down] == true && dot2.connectingCompass[Vector2Int.down] == true)
                {

                    if (gridManager.dots[(coordinate1 + Vector2Int.down).x, (coordinate1 + Vector2Int.down).y] != null &&
                        gridManager.dots[(coordinate2 + Vector2Int.down).x, (coordinate2 + Vector2Int.down).y].connectingCompass[Vector2Int.right] == true)
                    {
                        moveValue = 500;
                    }
                    else
                    {
                        moveValue = -100;
                    }
                   
                }
                

                else if (dot1.connectingCompass[Vector2Int.up] == true && dot2.connectingCompass[Vector2Int.up] == true)
                {

                    if (gridManager.dots[(coordinate1 + directionAccordingToGrid[Vector2Int.up]).x, (coordinate1 + directionAccordingToGrid[Vector2Int.up]).y] != null &&
                        gridManager.dots[(coordinate2 + directionAccordingToGrid[Vector2Int.up]).x, (coordinate2 + directionAccordingToGrid[Vector2Int.up]).y].connectingCompass[Vector2Int.right] == true)
                    {
                        moveValue = 500;
                    }
                    else
                    {
                        moveValue = -100;
                    }

                }
                //else if(dot1.connectingCompass.ContainsValue(true) || dot2.connectingCompass.ContainsValue(true))
                //{
                //    moveValue = 250;
                //}
                else
                {
                    moveValue = 100;
                }            
            }
            else
            {
                if ((dot1.connectingCompass[Vector2Int.left] == true && dot2.connectingCompass[Vector2Int.left] == true) &&
                   (dot1.connectingCompass[Vector2Int.right] == true && dot2.connectingCompass[Vector2Int.right] == true))
                {

                    if ((gridManager.dots[(coordinate1 + Vector2Int.right).x, (coordinate1 + Vector2Int.right).y] != null &&
                        gridManager.dots[(coordinate2 + Vector2Int.right).x, (coordinate2 + Vector2Int.right).y].connectingCompass[Vector2Int.down] == true)
                        ||
                        (gridManager.dots[(coordinate1 + Vector2Int.left).x, (coordinate1 + Vector2Int.left).y] != null &&
                        gridManager.dots[(coordinate2 + Vector2Int.left).x, (coordinate2 + Vector2Int.left).y].connectingCompass[Vector2Int.up] == true))
                    {
                        moveValue = 1000;
                    }



                }


                //check for same direction parallel lines from both dots
                else if (dot1.connectingCompass[Vector2Int.left] == true && dot2.connectingCompass[Vector2Int.left] == true)
                {

                    if (gridManager.dots[(coordinate1 + Vector2Int.left).x, (coordinate1 + Vector2Int.left).y] != null &&
                        gridManager.dots[(coordinate2 + Vector2Int.left).x, (coordinate2 + Vector2Int.left).y].connectingCompass[Vector2Int.up] == true)
                    {
                        moveValue = 500;
                    }
                    else
                    {
                        moveValue = -100;
                    }

                }


                else if (dot1.connectingCompass[Vector2Int.right] == true && dot2.connectingCompass[Vector2Int.right] == true)
                {

                    if (gridManager.dots[(coordinate1 + Vector2Int.right).x, (coordinate1 + Vector2Int.right).y] != null &&
                        gridManager.dots[(coordinate2 + Vector2Int.right).x, (coordinate2 + Vector2Int.right).y].connectingCompass[Vector2Int.up] == true)
                    {
                        moveValue = 500;
                    }
                    else
                    {
                        //this will make the third line so restrain from it
                        moveValue = -100;
                    }

                }
                //else if (dot1.connectingCompass.ContainsValue(true) || dot2.connectingCompass.ContainsValue(true))
                //{
                //    //make a second line either perpendicular or alone either way
                //    moveValue = 250;
                //}
                else
                {
                    moveValue = 100;
                }


            }

            if (moveValue > mostOptimalMoveValue)
            {
                mostOptimalMoveIndex = i;
                mostOptimalMoveValue = moveValue;
            }
            i++;
        }
        print("Total moves " + i);
        print(mostOptimalMoveValue);
        return mostOptimalMoveIndex;


    }




}
//print("in the computer move");
//Vector2Int[] move = computerMove.CalculateBestMove();
//Vector2Int dot1 = move[0];
//Vector2Int dot2 = move[1];
//gridManager.SelectDot(dot1.x, dot1.y);
//gridManager.SelectNeighbor(dot2.x, dot2.y);
//gridManager.Confirm();
//int x, y; 

//while(true)
//{
//    x = Random.Range(0, gridManager._height);
//    y = Random.Range(0, gridManager._width);
//    if(gridManager.dots[x,y].connectingCompass.ContainsValue(false))
//    {
//        break;
//    }


//}

//gridManager.SelectDot(x, y);
////neighborDot = null;
//if(!gridManager.dots[x,y].connectingCompass[Vector2Int.down])
//{
//    gridManager.SelectNeighbor(x + 1, y);
//    gridManager.Confirm();
//}

//else if(!gridManager.dots[x, y].connectingCompass[Vector2Int.up])
//{
//    gridManager.SelectNeighbor(x - 1, y);
//    gridManager.Confirm();
//}

//else if(!gridManager.dots[x, y].connectingCompass[Vector2Int.right])
//{
//    gridManager.SelectNeighbor(x, y+1);
//    gridManager.Confirm();
//}
//else if(!gridManager.dots[x, y].connectingCompass[Vector2Int.left])
//{
//    gridManager.SelectNeighbor(x, y-1);
//    gridManager.Confirm();
//}