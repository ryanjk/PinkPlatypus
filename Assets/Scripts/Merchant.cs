using System.Collections.Generic;
using UnityEngine;
using System;
//The merchant has a number of places to go. It finds a path to each of its required places and then goes there.
public class Merchant : MonoBehaviour
{
    private Transform _transform; //an object keeping track of the Merchant's position
    private bool pathfound;
    OriginDestination od;
    /*Where the merchant is heading. Because he only moves horizontally and vertically, it must differ from the merchant's position by only one
     coordinate.*/
    private Vector3 currentGoal; 
    private Vector3 currentMovementVector; //the vector he translates by in every frame. It is one of the following four vectors:

    private OriginDestination currentOriginDestination;
    private const int WALKABLE = 0; //a tile on which the merchant (and the player) can step.
    private const int NUM_DIMENSIONS = 2; //there are two dimensions of movement: up-down, left-right.

    //The class needs a map data structure so that the Merchant knows where is walkable and where is not.
    private int[,] map;
    int currentTurnIndex;
    List<Vector3> destinations;//can be anywhere in the world, provided externally
    List<OriginDestination> paths;//for each destination, there is a list of points it should get to.
    
    public enum Direction { UP0, DOWN0, UP1, DOWN1 };//directions of travel of the merchant
    int currentDestinationIndex;
    //I make each origin-destination pair into an inner class. This class has a method that finds a path between the points and other information.
    public class OriginDestination
    {

        private bool pathFound;
        private int[] origin;
        private int[] destination;

        /*each int[] in the List is of length 2 and represents the coordinates of a point that is a temporary goal for the Merchant as 
         * it travels from origin to destination. Thus, each one is a point at which the Merchant changes direction, except for the last 
         * int[], which is the actual destination.*/
        public List<int[]> path;  

        /*The Merchant iterates through the list and follows the direction at the current position in the list.*/
        private List<Direction> directionList;
        private int[,] map;

        //Returns the length of the List "path". I did not call it getPathLength so as not to confuse it with "path length" as in how much he has to walk.
        public int getNumberOfTurns()
        {
            return path.Count;
        }


        public Direction getDirection(int index) //Returns the Direction in the "directions" list at the specified index.
        {
            return directionList[index];
        }


        public void setMap(int[,] aMap) 
        {
            map = aMap;
        }

        //@param point a point that represents a temporary destination for the Merchant
        //@param d the direction in which the Merchant much travel to get to that point.
        public void addPointToPath(int[] point, Direction d)
        {
            path.Add(point);
            directionList.Add(d);
            Debug.Log("Added point " + path[path.Count - 1][0] + "," + path[path.Count - 1][1] + "to index" + (path.Count - 1));
        }

        //Used when the path is incorrect and needs to be deleted.
        public void clearPath()
        {
            path = null;
        }
        public int[] getPoint(int index)
        {
            Debug.Log("requesting point at index " + index);
            Debug.Log(path[index]);
            return (int[]) path[index].Clone();
        }

        //Constructor of an OriginDestination object.
        public OriginDestination(int[] anOrigin, int[] aDestination)
        {
            origin = anOrigin;
            destination = aDestination;
            path = new List<int[]>();
            directionList = new List<Direction>();
            pathFound = false;
        }

        //Returns true if a path has been found, false otherwise
        public bool getPathFound()
        {
            return pathFound;
        }

        //Sets the pathFound variable to b
        private void setPathFound(bool b)
        {
            pathFound = b;
        }
        public int[] getOrigin()
        {
            return (int[])origin.Clone(); //Returns a Clone so that the origin cannot be modified
        }
        public int[] getDestination()
        {
            return (int[])destination.Clone(); //Returns a Clone so that the destination cannot be modified
        }

        /*private List<int[]> vicinity(int[] point)
        {
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < point.Length; i++)
            {
                if (point[i] + 1 < map.GetLength(i))
                {
                    int[] x = new int[NUM_DIMENSIONS];
                    Array.Copy(point, x, NUM_DIMENSIONS);
                    x[i] += 1;
                    list.Add(x);
                }
                if (point[i] >= 1)
                {
                    int[] y = new int[NUM_DIMENSIONS];
                    Array.Copy(point, y, NUM_DIMENSIONS);
                    y[i] -= 1;
                    list.Add(y);
                }
            }
            return list;
        }*/
        private class PointDirectionPair
        {
            public int[] point;
            public Direction direction;
        }
        /*private Vector3 directionToVector(Direction d)
        {
            if (d == Direction.DOWN0)
                return left;
            if (d == Direction.DOWN1)
                return down;
            if (d == Direction.UP0)
                return right;
            return up;
        }*/
        static int[] up = { 1, 0 };
        static int[] down = { -1, 0 };
        static int[] right = { 0, 1 };
        static int[] left = { 0, -1 };
        private static int[][] directions = { up, down, left, right };
        private List<int[]> vicinity(int[] point)
        {

            List<int[]> list = new List<int[]>();
            for (int i = 0; i < directions.Length; i++)
            {
                int[] sum = add(point, directions[i]);
                bool accept = true;
                for (int j = 0; j < NUM_DIMENSIONS; j++)
                    if (sum[j] < 0 || sum[j] > map.GetLength(j))
                    {
                        accept = false;
                        break;
                    }
                if (accept)
                    list.Add(sum);
            }
            return list;
        }
        private int h(int[] point)
        {
            int sum = 0;
            for (int i = 0; i < NUM_DIMENSIONS; i++)
            {
                sum += Math.Abs(point[i] - destination[i]);
            }
            return sum;
        }

        private Node addNode(int[] aPosition, Node aPrevious)
        {
            if (aPrevious == null)
                return new Node(aPosition, null, 0, h(aPosition));
            return new Node(aPosition, aPrevious, aPrevious.pathLengthFromOrigin + 1, aPrevious.pathLengthFromOrigin + 1 + h(aPosition));
        }
        private class Node
        {
            public int[] position;
            public Node previous;
            public int pathLengthFromOrigin;
            public int f;
            public Node(int[] aPosition, Node aPrevious, int aPathLength, int aF)
            {
                position = aPosition;
                previous = aPrevious;
                pathLengthFromOrigin = aPathLength;
                f = aF;
            }
        }
        public List<int[]> findpath()
        {
            Debug.Log("finding path");
            if (map[destination[0], destination[1]] != WALKABLE)
                return null;
            List<int[]> tentativePath = new List<int[]>();
            Direction d;
            Direction e;
            HashSet<Node> tree = new HashSet<Node>();
            HashSet<Node> frontier = new HashSet<Node>();
            Node originNode = addNode(origin, null);
            frontier.Add(originNode);
            while (frontier.Count>0) 
            {
                int minValue = Int16.MaxValue;
                Node minNode = null;
                foreach (Node n in frontier)
                {
                    int value = n.f;
                    if (minValue > value)
                    {
                        minValue = value;
                        Debug.Log("minValue has been set to " + minValue);
                        minNode = n;
                    }
                }
                frontier.Remove(minNode);
                tree.Add(minNode);
                foreach (int[] y in vicinity(minNode.position))
                {
                    Debug.Log("y=" + y[0]+","+y[1]);
                    //int newDistance = minNode.pathLengthFromOrigin + 1 + h(y);
                    bool found = false;
                    if (map[y[0], y[1]] == WALKABLE)
                    {
                        Debug.Log(y[0] + "," + y[1] + "is walkable.");
                        if (y[0] == destination[0] && y[1] == destination[1])
                        {
                            List<int[]> path = new List<int[]>();
                            Debug.Log("path found");
                            path.Add(y);
                            for (Node n = minNode; n != null; n = n.previous)
                            {
                                path.Insert(0, n.position);
                            }
                            Debug.Log("this is the path");
                            for (int i = 0; i < path.Count; i++)
                            {
                                Debug.Log(path[i][0] + "," + path[i][1]);
                            }
                            return path;
                        }

                        foreach (Node n in tree)
                        {
                            if (n.position[0]==y[0]&&n.position[1]==y[1])
                            {
                                found = true;
                                if (n.pathLengthFromOrigin > minNode.pathLengthFromOrigin + 1)
                                {
                                    n.pathLengthFromOrigin = minNode.pathLengthFromOrigin + 1;
                                    n.previous = minNode;
                                }
                                break;
                            }
                        }
                        if (!found)
                        {
                            Node n = addNode(y, minNode);
                            frontier.Add(n);
                        }
                    }
                    else
                        Debug.Log(y[0] + "," + y[1] + "is unwalkable.");


                }
            }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            return null;
        }
    }
    private Vector3 setVector(int[] array)
    {
        Vector3 vector;
        vector.x = array[0]-10;
        vector.y = 1;
        vector.z = array[1]-10;
        Debug.Log("vector="+vector);
        return vector;
    }
    private int[] setArray(Vector3 vector)
    {
        int[] array = new int[NUM_DIMENSIONS];
        array[0] = (int)vector.x+10;
        array[1] = (int)vector.z+10;
        return array;
    }

    public static int[,] exampleMap()
    {
        int[,] map = new int[50, 50];
        for (int i = 0; i < 8; i++)
            map[3, i] = 1;

            return map;
    }

    private static int[] add(int[] first, int[] second)
        {
            int[] sum = new int[NUM_DIMENSIONS];
            for (int i = 0; i < NUM_DIMENSIONS; i++)
            {
                sum[i] = first[i] + second[i];
            }
            return sum;
        }
    private Vector3 directionToVector(int[] direction)
    {
        return new Vector3(direction[0], 0, direction[1]);
    }
    void Start()
    {
        currentDestinationIndex = 0;
        currentTurnIndex = 0;
        destinations = new List<Vector3>();
        _transform = gameObject.transform;
        //int[] origin = new int[NUM_DIMENSIONS];
        //int[] destination=new int[NUM_DIMENSIONS];
        //origin = setArray(_transform.position);
        //destination=setArray(new Vector3(3, 1, 5));
        int[] origin = { 4,2 };
        int[] destination = { 2,1 };
        od=new OriginDestination(origin,destination);
        currentOriginDestination = od;
        od.setMap(exampleMap());
        paths = new List<OriginDestination>();
        pathfound = false;
        List<int[]> path = od.findpath();
        if (path == null)
            Debug.Log("Uh-oh, a path was not found.");
        else
            paths.Add(od);
        /*OriginDestination od2=new OriginDestination(destination, setArray(new Vector3(-6, 1, 8)));
        paths.Add(od2);
        od.findpath();
        Debug.Log(od.getPoint(0)[0] + "," + od.getPoint(0)[1]);
        currentGoal = setVector(od.getPoint(0));
        Debug.Log("currentGoal=" + currentGoal);
        currentMovementVector = directionToVector(od.getDirection(currentTurnIndex));
        od2.setMap(exampleMap());
        od2.findpath();
        Debug.Log(od.getPoint(0)[0] + "," + od.getPoint(0)[1]);
        currentGoal = setVector(od.getPoint(0));
        Debug.Log("currentGoal=" + currentGoal);
        currentMovementVector = directionToVector(od.getDirection(currentTurnIndex));*/
    }

    void Update()
    {
        if (!pathfound)
        {
            od.findpath();
            pathfound = true;
        }
        //Debug.Log(_transform.position);
        //Debug.Log(currentGoal);
       /* if ((_transform.position - currentGoal).sqrMagnitude < .01)//If it is at its current goal
        {
            if (paths == null)
                Debug.Log("paths==null");
            if (paths[currentDestinationIndex] == null)
                Debug.Log("paths[currentDestinationIndex]==null");
            bool notAtCurrentDestination = currentTurnIndex + 1 < paths[currentDestinationIndex].getNumberOfTurns();
            bool currentDestinationNotFinal = currentDestinationIndex + 1 < paths.Count;

            //check whether it needs to change its destination or whether it needs to change its "intermediate goal" to get to its current destination
            if (currentDestinationNotFinal || notAtCurrentDestination)
            {
                if (notAtCurrentDestination)
                {
                    Debug.Log("notAtCurrentDestination");
                    currentTurnIndex++;//In the latter case, increment its "intermediate goal"
                    currentGoal=setVector(currentOriginDestination.getPoint(currentTurnIndex));
                }
                else if (currentDestinationNotFinal)
                {
                    Debug.Log("currentDestinationNotFinal");
                    currentDestinationIndex++;//In the former case, increment its destination
                    currentOriginDestination = paths[currentDestinationIndex];
                    currentTurnIndex = 0;
                    currentGoal = setVector(currentOriginDestination.getPoint(currentTurnIndex));
                }
                Debug.Log("paths.Count="+paths.Count);
                Debug.Log("currentDestinationIndex" + currentDestinationIndex);
                Debug.Log("currentTurnIndex="+currentTurnIndex);
                currentMovementVector = directionToVector(paths[currentDestinationIndex].getDirection(currentTurnIndex)); //In either case, set the movement vector accordingly
                _transform.Translate(currentMovementVector);
            }
            //else, he has reached his final destination, so he should not move.

        }
        else//if he is not near his current goal, then he should keep moving in the same direction.
            _transform.Translate(currentMovementVector); 
    */}
}
