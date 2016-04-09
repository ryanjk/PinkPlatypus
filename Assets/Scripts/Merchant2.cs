using System.Collections.Generic;
using UnityEngine;
using System;
//The merchant has a number of places to go. It finds a path to each of its required places and then goes there.
public class Merchant2 : MonoBehaviour
{
    private Transform _transform; //an object keeping track of the Merchant's position
    /*Where the merchant is heading. Because he only moves horizontally and vertically, it must differ from the merchant's position by only one
     coordinate.*/
    private Vector3 currentGoal;
    private Vector3 currentMovementVector; //the vector he translates by in every frame. It is one of the following four vectors:

    private OriginDestination currentOriginDestination;
    private const int WALKABLE = 0; //a tile on which the merchant (and the player) can step.
    private const int NUM_DIMENSIONS = 2; //there are two dimensions of movement: up-down, left-right.

    //The class needs a map data structure so that the Merchant knows where is walkable and where is not.
    private static int[,] map;
    private int currentGoalIndex;
    private List<Vector3> destinations;//can be anywhere in the world, provided externally
    private List<OriginDestination> paths;//for each destination, there is a list of points it should get to.
    private float speed;
    private List<Node> currentPath;
    public void setDestination(Vector3 destination)
    {
        paths.Add(new OriginDestination(paths[paths.Count - 1].getDestination(), setArray(destination)));
    }
    public class Node
    {
        public int[] position;
        public Node previous;
        public int pathLengthFromOrigin;
        public int f;
        public int[] directionFromParent;
        public Node(int[] aPosition, Node aPrevious, int aPathLength, int aF, int[] aDirection)
        {
            position = aPosition;
            previous = aPrevious;
            pathLengthFromOrigin = aPathLength;
            f = aF;
            directionFromParent = aDirection == null ? null : (int[])aDirection.Clone();
        }
    }
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
        public List<Node> path;

        /*The Merchant iterates through the list and follows the direction at the current position in the list.*/

        // private int[,] map;

        //Returns the length of the List "path". I did not call it getPathLength so as not to confuse it with "path length" as in how much he has to walk.
        public int getNumberOfTurns()
        {
            return path.Count;
        }

        public int[] getDirection(int index) //Returns the Direction in the "directions" list at the specified index.
        {
            return path[index].directionFromParent;
        }


        public void setMap(int[,] aMap)
        {
            map = aMap;
        }
        //Used when the path is incorrect and needs to be deleted.
        public void clearPath()
        {
            path = null;
        }
        public int[] getPoint(int index)
        {
            return (int[])path[index].position.Clone();
        }

        //Constructor of an OriginDestination object.
        public OriginDestination(int[] anOrigin, int[] aDestination)
        {
            origin = anOrigin;
            destination = aDestination;
            path = new List<Node>();
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
        private class PointDirectionPair
        {
            public int[] point;
            public int[] direction;
        }
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

        private Node addNode(int[] aPosition, Node aPrevious, int[] direction)
        {
            if (aPrevious == null)
                return new Node(aPosition, null, 0, h(aPosition), direction);
            return new Node(aPosition, aPrevious, aPrevious.pathLengthFromOrigin + 1, aPrevious.pathLengthFromOrigin + 1 + h(aPosition), direction);
        }
        private void setParent(Node aNode, Node parent, int[] direction)
        {
            aNode.previous = parent;
            int newPathLength = parent.pathLengthFromOrigin + 1;
            aNode.f = aNode.f - aNode.pathLengthFromOrigin + newPathLength;
            aNode.pathLengthFromOrigin = newPathLength;
            aNode.directionFromParent = direction;
        }
        public void findpath()
        {
            if (map[destination[0], destination[1]] != WALKABLE)
                return;
            List<int[]> tentativePath = new List<int[]>();
            HashSet<Node> tree = new HashSet<Node>();
            HashSet<Node> frontier = new HashSet<Node>();
            Node originNode = addNode(origin, null, null);
            frontier.Add(originNode);
            while (frontier.Count > 0)
            {
                int minValue = Int16.MaxValue;
                Node minNode = null;
                foreach (Node n in frontier)
                {
                    int value = n.f;
                    if (minValue > value)
                    {
                        minValue = value;
                        minNode = n;
                    }
                }
                frontier.Remove(minNode);
                tree.Add(minNode);
                foreach (int[] x in directions)
                {
                    int[] y = add(minNode.position, x);
                    //int newDistance = minNode.pathLengthFromOrigin + 1 + h(y);
                    bool found = false;
                    if (y[0] < 0 || y[1] < 0 || y[0] >= map.GetLength(0) || y[1] >= map.GetLength(1))
                        continue;
                    if (map[y[0], y[1]] == WALKABLE)
                    {
                        if (y[0] == destination[0] && y[1] == destination[1])
                        {
                            path = new List<Node>();
                            Debug.Log("path found");
                            path.Add(addNode(y, minNode, x));
                            for (Node n = minNode; n != null; n = n.previous)
                            {
                                path.Insert(0, n);
                            }
                            Debug.Log("this is the path");
                            for (int i = 0; i < path.Count; i++)
                            {
                                Debug.Log(path[i].position[0] + "," + path[i].position[1]);
                            }
                            pathFound = true;
                        }

                        foreach (Node n in tree)
                        {
                            if (n.position[0] == y[0] && n.position[1] == y[1])
                            {
                                found = true;
                                if (n.pathLengthFromOrigin > minNode.pathLengthFromOrigin + 1)
                                    setParent(n, minNode, x);
                                break;
                            }
                        }
                        if (!found)
                        {
                            Node n = addNode(y, minNode, x);
                            frontier.Add(n);
                            tree.Add(n);
                        }
                    }
                    else
                        Debug.Log(y[0] + "," + y[1] + "is unwalkable.");


                }
            }
            return;
        }
    }
    private Vector3 setVector(int[] array)
    {
        Vector3 vector;
        vector.x = array[0] - 10;
        vector.y = 1;
        vector.z = array[1] - 10;
        Debug.Log("vector=" + vector);
        return vector;
    }
    private int[] setArray(Vector3 vector)
    {
        int[] array = new int[NUM_DIMENSIONS];
        array[0] = (int)vector.x + 10;
        array[1] = (int)vector.z + 10;
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
        destinations = new List<Vector3>();
        _transform = gameObject.transform;
        int[] origin = setArray(_transform.position);
        int[] destination = { 2, 1 };
        int[] destination2 = { 4, 4 };
        OriginDestination od = new OriginDestination(origin, destination);
        currentOriginDestination = od;
        od.setMap(exampleMap());
        paths = new List<OriginDestination>();
        od.findpath();
        speed = 0.1f;
        paths.Add(od);
        OriginDestination od2 = new OriginDestination(destination, destination2);
        od2.findpath();
        paths.Add(od2);
        currentGoalIndex = 0;
        currentGoal = setVector(currentOriginDestination.getPoint(currentGoalIndex));
        currentMovementVector = speed * directionToVector(od.getDirection(1));
    }

    void Update()
    {
        if ((_transform.position - currentGoal).sqrMagnitude < .01)//If it is at its current goal
        {
            bool notAtCurrentDestination = currentGoalIndex + 1 < paths[currentDestinationIndex].getNumberOfTurns();
            bool currentDestinationNotFinal = currentDestinationIndex + 1 < paths.Count;

            //check whether it needs to change its destination or whether it needs to change its "intermediate goal" to get to its current destination
            if (currentDestinationNotFinal || notAtCurrentDestination)
            {
                if (notAtCurrentDestination)
                {
                    currentGoalIndex++;//In the latter case, increment its "intermediate goal"
                }
                else if (currentDestinationNotFinal)
                {
                    currentDestinationIndex++;//In the former case, increment its destination
                    currentOriginDestination = paths[currentDestinationIndex];
                    currentGoalIndex = 0;
                }
                currentGoal = setVector(currentOriginDestination.getPoint(currentGoalIndex));
                currentMovementVector = speed * directionToVector(paths[currentDestinationIndex].getDirection(currentGoalIndex)); //In either case, set the movement vector accordingly
            }
            //else, he has reached his final destination, so he should not move.

        }
        else//if he is not near his current goal, then he should keep moving in the same direction.
        {
            _transform.Translate(currentMovementVector);

        }
    }
}