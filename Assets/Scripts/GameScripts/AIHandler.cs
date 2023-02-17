using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashForDots.AI
{
    public class AIHandler : MonoBehaviour
    {
        #region PRIVATE DICTIONARY & LISTS
        GridManager gridManager;
        Dictionary<List<Vector2Int>, int> squareChainMovesWithCount = new Dictionary<List<Vector2Int>, int>();
        Dictionary<Vector2Int, List<Vector2Int>> thirdLineMoves = new Dictionary<Vector2Int, List<Vector2Int>>();
        Dictionary<Vector2Int, List<Vector2Int>> squareLineMoves = new Dictionary<Vector2Int, List<Vector2Int>>();
        Dictionary<Vector2Int, List<Vector2Int>> visitedThirdLineMoves = new Dictionary<Vector2Int, List<Vector2Int>>();
        List<List<Vector2Int>> doubleSquareMoves = new List<List<Vector2Int>>();
        List<List<Vector2Int>> singleSquareMoves = new List<List<Vector2Int>>();
        List<List<Vector2Int>> validMoves = new List<List<Vector2Int>>();

        Dictionary<Vector2Int, Vector2Int> directionAccordingToGrid = new Dictionary<Vector2Int, Vector2Int>()
    {
        { Vector2Int.up, Vector2Int.left },
        { Vector2Int.right, Vector2Int.up },
        { Vector2Int.down, Vector2Int.right },
        { Vector2Int.left, Vector2Int.down },

    };
        Dictionary<Vector2Int, Vector2Int> swasktikaMoves = new Dictionary<Vector2Int, Vector2Int>()
    {
            {new Vector2Int(1, 0),  Vector2Int.right},
            {new Vector2Int(0, 2), Vector2Int.down },
            {new Vector2Int(3,1), Vector2Int.up },
            {new Vector2Int(2, 3), Vector2Int.left },
    };

        Dictionary<Vector2Int, string> directions = new Dictionary<Vector2Int, string>()
    {
        {Vector2Int.up , "up" },
        {Vector2Int.down, "down" },
        {Vector2Int.left, "left" },
        {Vector2Int.right, "right" },
    };
        #endregion

        #region PRIVATE VARIABLES
        int count = 0;
        int longChainsCount = 0;
        int doubleChainsCount = 0;
        int countForNormalMoves = 0;
        #endregion

        #region MONOBEHAVIOUR METHODS
        private void Start()
        {
            gridManager = FindObjectOfType<GridManager>();
        }
        #endregion

        #region CALCULATE BEST MOVES
        public void CalculateBestMove()
        {
            int bestIndex = GetBestMove();
            //print(bestIndex);
            Vector2Int dot1 = validMoves[bestIndex][0];
            Vector2Int dot2 = validMoves[bestIndex][0] + validMoves[bestIndex][1];
            gridManager.SelectDot(dot1.x, dot1.y);
            gridManager.SelectNeighbor(dot2.x, dot2.y);
            gridManager.Confirm();
        }
        IEnumerator ConfirmSelectedDot(int dotXCoordinate, int dotYCoordinate)
        {
            yield return new WaitForSeconds(0.1f);
            gridManager.SelectNeighbor(dotXCoordinate, dotYCoordinate);
            gridManager.Confirm();
        }

        public void GetRandomMove()
        {
            GetAllValidMoves();
            int validMovesCount = validMoves.Count;
            int randomMoveIndex = Random.Range(0, validMovesCount);
            Vector2Int dot1 = validMoves[randomMoveIndex][0];
            Vector2Int dot2 = validMoves[randomMoveIndex][0] + validMoves[randomMoveIndex][1];
            gridManager.SelectDot(dot1.x, dot1.y);
            StartCoroutine(ConfirmSelectedDot(dot2.x, dot2.y));
        }
        private int GetBestMove()
        {
            GetAllValidMoves();
            countForNormalMoves = 0;
            thirdLineMoves.Clear();
            visitedThirdLineMoves.Clear();
            doubleSquareMoves.Clear();
            singleSquareMoves.Clear();
            squareChainMovesWithCount.Clear();
            int mostOptimalMoveValue = -1000;
            int mostOptimalMoveIndex = 0;
            int moveValue = 100;

            int i = 0;
            foreach (var move in validMoves)
            {
                moveValue = 100;


                Vector2Int dotCoordinateFrom = move[0];
                Vector2Int dotCoordinateTo = move[0] + move[1];
                Vector2Int direction = move[2];
                Dot dotRefFrom = gridManager.dots[dotCoordinateFrom.x, dotCoordinateFrom.y];
                Dot dotRefTo = gridManager.dots[dotCoordinateTo.x, dotCoordinateTo.y];
                //print("coordinate 1 is " + dotRefFrom.name + " coordinate 2 is " + dotRefTo.name + " direction is " + directions[direction]);

                ////if the grid is 4x4 then check of swastika moves
                //if (gridManager._width == 4)
                //{
                //    if (CheckIfSwastikaMove(dotCoordinateFrom, direction))
                //    {
                //        moveValue += 10;
                //    }
                //}

                if (direction == Vector2Int.left)
                {

                    moveValue += GetOptimalMove(dotCoordinateFrom, dotCoordinateTo, dotRefFrom, dotRefTo, direction, Vector2Int.down, Vector2Int.up);
                }
                else if (direction == Vector2Int.right)
                {

                    moveValue += GetOptimalMove(dotCoordinateFrom, dotCoordinateTo, dotRefFrom, dotRefTo, direction, Vector2Int.up, Vector2Int.down);

                }
                else if (direction == Vector2Int.down)
                {
                    moveValue += GetOptimalMove(dotCoordinateFrom, dotCoordinateTo, dotRefFrom, dotRefTo, direction, Vector2Int.left, Vector2Int.right);
                }
                else if (direction == Vector2Int.up)
                {
                    moveValue += GetOptimalMove(dotCoordinateFrom, dotCoordinateTo, dotRefFrom, dotRefTo, direction, Vector2Int.right, Vector2Int.left);

                }

                if (moveValue > mostOptimalMoveValue)
                {
                    mostOptimalMoveIndex = i;
                    mostOptimalMoveValue = moveValue;
                }
                if (moveValue > 100 && moveValue < 1000)
                {
                    countForNormalMoves++;
                }
                i++;
            }
            if (mostOptimalMoveValue < 100)
            {
                List<Vector2Int> minSquareChain = CalculateSquareChains();
                if (squareChainMovesWithCount.ContainsValue(1))
                {
                    List<Vector2Int> moveWithChainCountOne = null;
                    foreach (var move in squareChainMovesWithCount)
                    {
                        if (squareChainMovesWithCount[move.Key] == 1)
                        {
                            moveWithChainCountOne = move.Key;
                            break;
                        }
                    }
                    int j = 0;
                    foreach (var line in validMoves)
                    {
                        if (line[0] == moveWithChainCountOne[0] && line[2] == moveWithChainCountOne[1])
                        {

                            return j;
                        }
                        j++;
                    }
                }
                else if (doubleChainsCount > 0)
                {

                    List<Vector2Int> line = null;
                    foreach (var move in doubleSquareMoves)
                    {
                        Vector2Int direction = move[1];
                        Vector2Int dCFrom = move[0];
                        Vector2Int dCTo = move[0] + directionAccordingToGrid[move[1]];
                        //Debug.Log($" the move is dot{move[0]} in the direction {directions[move[1]]}");
                        Vector2 p1 = Vector2.Perpendicular(direction);
                        Vector2Int perpendicular1 = new Vector2Int((int)p1.x, (int)p1.y);
                        Vector2Int perpendicular2 = perpendicular1 * -1;
                        Vector2Int pDC1 = dCFrom + directionAccordingToGrid[perpendicular1];
                        Vector2Int pDC2 = dCFrom + directionAccordingToGrid[perpendicular2];
                        if (CheckIfDoubleThirdLine(move))
                        {
                            line = move;
                        }
                        else
                        {
                            if (CheckIfContainsDirection(thirdLineMoves, dCFrom, perpendicular1))
                            {
                                line = new List<Vector2Int>() { dCFrom, perpendicular1 };
                            }
                            else if (CheckIfContainsDirection(thirdLineMoves, dCFrom, perpendicular2))
                            {
                                line = new List<Vector2Int>() { dCFrom, perpendicular1 };
                            }
                            else if (CheckIfContainsDirection(thirdLineMoves, dCTo, perpendicular1))
                            {
                                line = new List<Vector2Int>() { dCTo, perpendicular1 };
                            }
                            else if (CheckIfContainsDirection(thirdLineMoves, dCTo, perpendicular2))
                            {
                                line = new List<Vector2Int>() { dCTo, perpendicular2 };
                            }
                            else if (CheckIfContainsDirection(thirdLineMoves, pDC1, direction))
                            {
                                line = new List<Vector2Int>() { pDC1, direction };
                            }
                            else if (CheckIfContainsDirection(thirdLineMoves, pDC2, direction))
                            {
                                line = new List<Vector2Int>() { pDC2, direction };
                            }
                        }
                        if (line != null) { break; }
                    }
                    int j = 0;
                    if (line != null)
                    {
                        foreach (var move in validMoves)
                        {
                            if (move[0] == line[0] && move[2] == line[1])
                            {

                                return j;
                            }
                            j++;
                        }
                    }



                }
                else
                {
                    int j = 0;
                    foreach (var line in validMoves)
                    {
                        if (line[0] == minSquareChain[0] && line[2] == minSquareChain[1])
                        {

                            return j;
                        }
                        j++;
                    }
                }
            }
            else if (mostOptimalMoveValue > 1000 && countForNormalMoves <= 0)
            {

                CalculateSquareChains();
                List<Vector2Int> line = null;
                if (longChainsCount > 0 && singleSquareMoves.Count > 0 && doubleChainsCount % 2 == 0)
                {
                    // print("inside the first condition of else if");
                    foreach (var move in singleSquareMoves)
                    {
                        // Debug.Log($"the move is <color=purple> dot {move[0]} and direction is {directions[move[1]]}");
                        Vector2Int dCFrom = move[0];
                        Vector2Int dcTo = dCFrom + directionAccordingToGrid[move[1]];
                        Vector2 p1 = Vector2.Perpendicular(move[1]);
                        Vector2Int perpendicular1 = new Vector2Int((int)p1.x, (int)p1.y);
                        Vector2Int perpendicular2 = perpendicular1 * -1;
                        Vector2Int squareLineDC1 = validMoves[mostOptimalMoveIndex][0];
                        Vector2Int squareLineDC2 = validMoves[mostOptimalMoveIndex][0] + validMoves[mostOptimalMoveIndex][1];
                        Vector2Int pDC1 = dCFrom + directionAccordingToGrid[perpendicular1];
                        Vector2Int pDC2 = dCFrom + directionAccordingToGrid[perpendicular2];
                        //Debug.Log($"dcFrom {dCFrom} dcTo {dcTo} pdc1 {pDC1} pdc2 {pDC2} sqDC1{squareLineDC1} sqdc2{squareLineDC2}");
                        if (dCFrom == squareLineDC1 || dCFrom == squareLineDC2 || dcTo == squareLineDC1 || dcTo == squareLineDC2
                            || pDC1 == squareLineDC1 || pDC1 == squareLineDC2 || pDC2 == squareLineDC1 || pDC2 == squareLineDC2)
                        {
                            //print(" line has been given as move");
                            line = move;
                        }
                    }
                    if (line != null)
                    {
                        int j = 0;
                        foreach (var move in validMoves)
                        {
                            if (move[0] == line[0] && move[2] == line[1])
                            {

                                return j;
                            }
                            j++;
                        }
                    }
                }
            }
            return mostOptimalMoveIndex;
        }
        #endregion

        #region GET ALL VALID MOVES
        private void GetAllValidMoves()
        {
            validMoves.Clear();
            foreach (Dot dot in gridManager.dots)
            {
                //print(dot.name);
                if (!dot.connectingCompass[Vector2Int.down])
                {
                    GetDirection(dot, Vector2Int.down);
                }

                if (!dot.connectingCompass[Vector2Int.up])
                {
                    GetDirection(dot, Vector2Int.up);
                }

                if (!dot.connectingCompass[Vector2Int.right])
                {
                    GetDirection(dot, Vector2Int.right);
                }
                if (!dot.connectingCompass[Vector2Int.left])
                {
                    GetDirection(dot, Vector2Int.left);

                }
            }
        }
        #endregion

        #region GET DIRECTION
        void GetDirection(Dot dot, Vector2Int dir)
        {
            List<Vector2Int> addToValidList = new List<Vector2Int>();
            Vector2Int dotCoordinate = new Vector2Int(dot.coordinates.X, dot.coordinates.Y);

            addToValidList.Add(dotCoordinate);
            addToValidList.Add(directionAccordingToGrid[dir]);
            addToValidList.Add(dir);
            validMoves.Add(addToValidList);
        }
        #endregion

        #region CHECK PARALLEL LINE
        private int CheckParallelLine(Vector2Int parallelDot, int value, Vector2Int dotCoordinateFrom, Vector2Int dotCoordinateTo, Dot dotRefFrom, Dot dotRefTo, Vector2Int direction, Vector2Int perpendicularDirection)
        {
            int moveValue = 0;
            //Debug.Log($"Checking parallel line for dotFrom {dotCoordinateFrom} to dotTo {dotCoordinateTo} in the direction {directions[direction]}");
            if ((parallelDot.x >= 0 && parallelDot.x < gridManager._width) && (parallelDot.y >= 0 && parallelDot.y < gridManager._width)
                 && gridManager.dots[(parallelDot).x, (parallelDot).y].connectingCompass[direction] == true)
            {
                if ((dotRefFrom.connectingCompass[perpendicularDirection] == true && CheckWithinGrid(dotCoordinateFrom, perpendicularDirection)) && (dotRefTo.connectingCompass[perpendicularDirection] == true && CheckWithinGrid(dotCoordinateTo, perpendicularDirection)))
                {
                    moveValue += 2000;
                    AddToGivenMoveSet(squareLineMoves, dotCoordinateFrom, direction);
                    //Debug.Log($"will <color=white> make a square {dotRefFrom} to {dotRefTo} in the direction {directions[direction]} </color>");
                }
                else if ((dotRefFrom.connectingCompass[perpendicularDirection] == true && CheckWithinGrid(dotCoordinateFrom, perpendicularDirection)) || (dotRefTo.connectingCompass[perpendicularDirection] == true && CheckWithinGrid(dotCoordinateTo, perpendicularDirection)))
                {
                    moveValue -= 250;
                    AddToGivenMoveSet(thirdLineMoves, dotCoordinateFrom, direction);
                    //Debug.Log($"will <color=yellow>  make a third line from {dotRefFrom} to {dotRefTo} in the direction {directions[direction]} </color>");
                }
                else if ((dotRefFrom.connectingCompass[direction * -1] && CheckWithinGrid(dotCoordinateFrom, direction * -1)) || (dotRefTo.connectingCompass[direction] && CheckWithinGrid(dotCoordinateTo, direction)))
                {
                    int chainCount = 0;
                    chainCount += CheckForLongChains(dotCoordinateFrom, direction * -1);
                    chainCount += CheckForLongChains(dotCoordinateTo, direction);
                    moveValue += 30 * chainCount;
                    // Debug.Log($"will make a second line in other column from {dotRefFrom} to {dotRefTo} in the direction {directions[direction]}");
                }
                else
                {
                    moveValue += 10;
                    //Debug.Log($"will make a second line in other column from {dotRefFrom} to {dotRefTo} in the direction {directions[direction]}");
                }

            }
            else
            {
                if ((dotRefFrom.connectingCompass[perpendicularDirection] == true && CheckWithinGrid(dotCoordinateFrom, perpendicularDirection)) && (dotRefTo.connectingCompass[perpendicularDirection] == true && CheckWithinGrid(dotCoordinateTo, perpendicularDirection)))
                {
                    moveValue -= 250;
                    AddToGivenMoveSet(thirdLineMoves, dotCoordinateFrom, direction);
                    //Debug.Log($"will <color=yellow> make a third line from {dotRefFrom} to {dotRefTo} in the direction {directions[direction]} </color>");
                }

                // this region is to make line that is perpendicular to current direction line
                //else if ((dotRefFrom.connectingCompass[perpendicularDirection] == true && CheckWithinGrid(dotCoordinateFrom, perpendicularDirection)) || (dotRefTo.connectingCompass[perpendicularDirection] == true && CheckWithinGrid(dotCoordinateTo, perpendicularDirection)))
                //{

                //    moveValue += 120;

                //}
                else if ((dotRefFrom.connectingCompass[direction * -1] && CheckWithinGrid(dotCoordinateFrom, direction * -1)) || (dotRefTo.connectingCompass[direction] && CheckWithinGrid(dotCoordinateTo, direction)))
                {
                    int chainCount = 0;
                    chainCount += CheckForLongChains(dotCoordinateFrom, direction * -1);
                    chainCount += CheckForLongChains(dotCoordinateTo, direction);
                    moveValue += 30 * chainCount;
                    //Debug.Log($" chain count for line {dotRefFrom} to {dotRefTo}  is { chainCount}");
                    // Debug.Log($"will make a second line in other column from {dotRefFrom} to {dotRefTo} in the direction {directions[direction]}");
                }
            }
            return moveValue;
        }
        #endregion

        #region GET OPTIMAL MOVE
        private int GetOptimalMove(Vector2Int dotCoordinateFrom, Vector2Int dotCoordinateTo, Dot dotRefFrom, Dot dotRefTo, Vector2Int direction, Vector2Int p1, Vector2Int p2)
        {

            int moveValue = 100;
            Vector2Int dot1lowerdot = dotCoordinateFrom + directionAccordingToGrid[p1];
            Vector2Int dot1upperdot = dotCoordinateFrom + directionAccordingToGrid[p2];
            moveValue += CheckParallelLine(dot1lowerdot, moveValue, dotCoordinateFrom, dotCoordinateTo, dotRefFrom, dotRefTo, direction, p1);
            moveValue += CheckParallelLine(dot1upperdot, moveValue, dotCoordinateFrom, dotCoordinateTo, dotRefFrom, dotRefTo, direction, p2);
            return moveValue;
        }
        #endregion

        #region CALCULATE SQAURE CHAINS
        private List<Vector2Int> CalculateSquareChains()
        {
            squareChainMovesWithCount.Clear();
            doubleSquareMoves.Clear();
            singleSquareMoves.Clear();
            longChainsCount = 0;
            //squareLineMoves.Clear();
            doubleChainsCount = 0;
            int minSquareChain = 1000;
            List<Vector2Int> minChainPair = new List<Vector2Int>();
            foreach (var line in thirdLineMoves)
            {
                foreach (var dir in thirdLineMoves[line.Key])
                {
                    //Debug.Log($"checking for third line {line.Key} in the direction {directions[dir]}");
                    count = 0;
                    CheckSquareChains(line.Key, dir);
                    //Debug.Log($"Count for square chain for dot {line.Key} in direction{directions[dir]} is {count}");
                    if (count > 0)
                    {
                        // Debug.Log($"Count for square chain for dot {line.Key} in direction{directions[dir]} is {count}");
                        AddToSquareChainDictionary(line.Key, dir, count);
                        if (count > 2)
                        {
                            longChainsCount++;
                        }
                        else if (count == 2)
                        {
                            List<Vector2Int> move = new List<Vector2Int>() { line.Key, dir };
                            doubleSquareMoves.Add(move);
                            doubleChainsCount++;
                        }
                        else if (count == 1)
                        {

                            Vector2 p1 = Vector2.Perpendicular(dir); Vector2Int perpendicular1 = new Vector2Int((int)p1.x, (int)p1.y);
                            Vector2Int perpendicular2 = perpendicular1 * -1;
                            Vector2Int pDC1 = line.Key + directionAccordingToGrid[perpendicular1];
                            Vector2Int pDC2 = line.Key + directionAccordingToGrid[perpendicular2];
                            //Debug.Log($" pDC1 {pDC1} pdc2 {pDC2} p1 {directions[perpendicular1]} and p2 {directions[perpendicular2]}");
                            if (CheckIfContainsDirection(squareLineMoves, line.Key, dir))
                            {
                                if (CheckIfContainsDirection(thirdLineMoves, pDC1, dir))
                                {
                                    List<Vector2Int> moveNew = new List<Vector2Int> { pDC1, dir };
                                    // Debug.Log($"Count for square chain for dot {pDC1} in direction{directions[dir]} is {count}");
                                    singleSquareMoves.Add(moveNew);
                                }
                                else if (CheckIfContainsDirection(thirdLineMoves, pDC2, dir))
                                {

                                    //Debug.Log($"Count for square chain for dot {pDC2} in direction{directions[dir]} is {count}");
                                    List<Vector2Int> moveNew = new List<Vector2Int> { pDC2, dir };
                                    singleSquareMoves.Add(moveNew);
                                }
                                else if (CheckIfContainsDirection(thirdLineMoves, line.Key, perpendicular1))
                                {
                                    List<Vector2Int> moveNew = new List<Vector2Int> { line.Key, perpendicular1 };
                                    singleSquareMoves.Add(moveNew);
                                }
                                else if (CheckIfContainsDirection(thirdLineMoves, line.Key, perpendicular2))
                                {
                                    List<Vector2Int> moveNew = new List<Vector2Int> { line.Key, perpendicular2 };
                                    singleSquareMoves.Add(moveNew);
                                }
                                else if (CheckIfContainsDirection(thirdLineMoves, line.Key + directionAccordingToGrid[dir], perpendicular1))
                                {
                                    List<Vector2Int> moveNew = new List<Vector2Int> { line.Key + directionAccordingToGrid[dir], perpendicular1 };
                                    singleSquareMoves.Add(moveNew);
                                }
                                else if (CheckIfContainsDirection(thirdLineMoves, line.Key + directionAccordingToGrid[dir], perpendicular2))
                                {
                                    List<Vector2Int> moveNew = new List<Vector2Int> { line.Key + directionAccordingToGrid[dir], perpendicular2 };
                                    singleSquareMoves.Add(moveNew);
                                }
                            }

                            else
                            {
                                //Debug.Log($"Count for square chain for dot {line.Key} in direction{directions[dir]} is {count}");
                                List<Vector2Int> move = new List<Vector2Int> { line.Key, dir };
                                singleSquareMoves.Add(move);
                            }


                        }
                        if (count < minSquareChain)
                        {

                            minChainPair = new List<Vector2Int>() { line.Key, dir };
                            minSquareChain = count;
                        }
                    }

                }
            }

            return minChainPair;

        }
        #endregion

        private bool CheckIfDoubleThirdLine(List<Vector2Int> move)
        {
            List<Vector2Int> directions = thirdLineMoves[move[0]];
            int dirAppearCount = 0;
            Vector2Int direction = move[1];
            Vector2Int dCFrom = move[0];
            Vector2Int dCTo = move[0] + directionAccordingToGrid[move[1]];
            Vector2 p1 = Vector2.Perpendicular(direction);
            Vector2Int perpendicular1 = new Vector2Int((int)p1.x, (int)p1.y);
            Vector2Int perpendicular2 = perpendicular1 * -1;
            for (int k = 0; k < directions.Count; k++)
            {
                if (directions[k] == move[1])
                {
                    dirAppearCount++;
                }
            }
            if (dirAppearCount == 2)
            {
                return true;

            }
            return false;
        }

        private bool CheckIfContainsDirection(Dictionary<Vector2Int, List<Vector2Int>> moveSet, Vector2Int dotCoordinate, Vector2Int direction)
        {
            if (moveSet.ContainsKey(dotCoordinate))
            {
                foreach (var line in moveSet[dotCoordinate])
                {
                    if (line == direction)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void CheckSquareChains(Vector2Int dotCoordinate, Vector2Int direction)
        {
            /// get the value and then check for the whole square if any of them have third line and if it present the repeat the process
            /// it should also have a visited list to avoid repetition of counting of the squares 
            /// 
            //Debug.Log($" inside check square chains with dot coordinate {dotCoordinate} and direction {directions[direction]}");

            AddToGivenMoveSet(visitedThirdLineMoves, dotCoordinate, direction);
            //AddToVisitedThirdLineMoves(dotCoordinate, direction);

            Vector2 p1 = Vector2.Perpendicular(direction);
            Vector2 p2 = Vector2.Perpendicular(-1 * direction);

            Vector2Int perpendicular1 = new Vector2Int((int)p1.x, (int)p1.y);
            Vector2Int perpendicular2 = new Vector2Int((int)p2.x, (int)p2.y);

            Vector2Int perpendicularDC1 = dotCoordinate + directionAccordingToGrid[perpendicular1];
            Vector2Int perpendicularDC2 = dotCoordinate + directionAccordingToGrid[perpendicular2];
            Vector2Int dotCoordinateTo = dotCoordinate + directionAccordingToGrid[direction];

            if (thirdLineMoves.ContainsKey(perpendicularDC1))
            {
                //if (dotCoordinate == new Vector2Int(0, 2) && direction == Vector2Int.down) { print("inside first if"); }
                foreach (var line in thirdLineMoves[perpendicularDC1])
                {
                    if (line == direction && !(CheckIfContainsDirection(visitedThirdLineMoves, perpendicularDC1, direction)))
                    {
                        //Debug.Log($" coordinate {perpendicularDC1} in direction {directions[direction]} has been found a third line  congrats ur if is working ");
                        count++;
                        AddToGivenMoveSet(visitedThirdLineMoves, perpendicularDC1, direction);
                        //AddToVisitedThirdLineMoves(perpendicularDC1, direction);
                        CheckSquareChains(perpendicularDC1, direction);
                    }
                    if (line == perpendicular2 && !(CheckIfContainsDirection(visitedThirdLineMoves, perpendicularDC1, perpendicular2)))
                    {
                        //Debug.Log($" coordinate {perpendicularDC1} in direction {directions[perpendicular2]} has been found a third line  congrats ur first elif is working");
                        count++;
                        AddToGivenMoveSet(visitedThirdLineMoves, perpendicularDC1, perpendicular2);
                        //AddToVisitedThirdLineMoves(perpendicularDC1, perpendicular2);
                        CheckSquareChains(perpendicularDC1, perpendicular2);
                    }
                }
            }
            if (thirdLineMoves.ContainsKey(perpendicularDC2))
            {
                foreach (var line in thirdLineMoves[perpendicularDC2])
                {
                    if (line == direction && !(CheckIfContainsDirection(visitedThirdLineMoves, perpendicularDC2, direction)))
                    {
                        // Debug.Log($" coordinate {perpendicularDC2} in direction {directions[direction]} has been found a third line  congrats ur first elif is working");
                        count++;
                        AddToGivenMoveSet(visitedThirdLineMoves, perpendicularDC2, direction);
                        //AddToVisitedThirdLineMoves(perpendicularDC2, direction);
                        CheckSquareChains(perpendicularDC2, direction);
                    }
                    if (line == perpendicular1 && !(CheckIfContainsDirection(visitedThirdLineMoves, perpendicularDC2, perpendicular1)))
                    {
                        // Debug.Log($" coordinate {perpendicularDC2} in direction {directions[direction]} has been found a third line  congrats ur first elif is working");
                        count++;
                        AddToGivenMoveSet(visitedThirdLineMoves, perpendicularDC2, perpendicular1);
                        //AddToVisitedThirdLineMoves(perpendicularDC2, perpendicular1);
                        CheckSquareChains(perpendicularDC2, perpendicular1);
                    }
                }
            }
            if (CheckIfContainsDirection(thirdLineMoves, dotCoordinateTo, perpendicular1) && !(CheckIfContainsDirection(visitedThirdLineMoves, dotCoordinateTo, perpendicular1)))
            {
                // Debug.Log($" coordinate {dotCoordinateTo} in direction {directions[perpendicular1]} has been found a third line  congrats ur second elif is working");
                count++;
                AddToGivenMoveSet(visitedThirdLineMoves, dotCoordinateTo, perpendicular1);
                // AddToVisitedThirdLineMoves(dotCoordinateTo, perpendicular1);
                CheckSquareChains(dotCoordinateTo, perpendicular1);
            }
            if (CheckIfContainsDirection(thirdLineMoves, dotCoordinateTo, perpendicular2) && !(CheckIfContainsDirection(visitedThirdLineMoves, dotCoordinateTo, perpendicular2)))
            {
                //Debug.Log($" coordinate {dotCoordinateTo} in direction {directions[perpendicular2]} has been found a third line  congrats ur third elif is working");
                count++;
                AddToGivenMoveSet(visitedThirdLineMoves, dotCoordinateTo, perpendicular2);
                //AddToVisitedThirdLineMoves(dotCoordinateTo, perpendicular2);
                CheckSquareChains(dotCoordinateTo, perpendicular2);
            }
            AddToGivenMoveSet(visitedThirdLineMoves, dotCoordinateTo, direction * -1);
            //AddToVisitedThirdLineMoves(dotCoordinateTo, direction * -1);

        }
        private void AddToSquareChainDictionary(Vector2Int dotCoordinate, Vector2Int direction, int count)
        {
            List<Vector2Int> line = new List<Vector2Int>() { dotCoordinate, direction };
            squareChainMovesWithCount.Add(line, count);
        }


        private void AddToGivenMoveSet(Dictionary<Vector2Int, List<Vector2Int>> moveSet, Vector2Int dotCoordinate, Vector2Int direction)
        {
            if (moveSet.ContainsKey(dotCoordinate))
            {
                List<Vector2Int> linesThroughDot = moveSet[dotCoordinate];
                linesThroughDot.Add(direction);
                moveSet[dotCoordinate] = linesThroughDot;
            }
            else
            {
                List<Vector2Int> linesThroughDot = new List<Vector2Int>();
                linesThroughDot.Add(direction);
                moveSet.Add(dotCoordinate, linesThroughDot);
            }
        }

        #region N.A SWASTIKA MOVE
        //private bool CheckIfSwastikaMove(Vector2Int dotCoordinate, Vector2Int direction)
        //{

        //    if (swasktikaMoves.ContainsKey(dotCoordinate) && swasktikaMoves[dotCoordinate] == direction)
        //    {
        //        return true;
        //    }


        //    return false;
        //}
        #endregion

        private bool CheckWithinGrid(Vector2 dotCoordinate, Vector2Int dir)
        {
            Vector2 secondDot = dotCoordinate + directionAccordingToGrid[dir];
            if ((secondDot.x >= 0 && secondDot.x < gridManager._width) && (secondDot.y >= 0 && secondDot.y < gridManager._width))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int CheckForLongChains(Vector2Int dotFrom, Vector2Int direction)
        {
            Vector2Int currentDot = dotFrom;
            int chainCount = 0;
            while (CheckWithinGrid(currentDot, direction))
            {
                if (gridManager.dots[currentDot.x, currentDot.y].connectingCompass[direction])
                {
                    chainCount++;
                    currentDot = currentDot + directionAccordingToGrid[direction];
                }
                else
                {
                    break;
                }
            }
            //Debug.Log($"the chain count for dot{dotFrom} in the direction {directions[direction]} is {chainCount}");
            return chainCount;
        }

    }
}