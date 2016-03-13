using System.Collections.Generic;
using UnityEngine;
using System;
//The merchant has a number of places to go. It finds a path to each of its required places and then goes there.
public class Merchant : MonoBehaviour
{
    private Transform _transform; //an object keeping track of the Merchant's position

    /*Where the merchant is heading. Because he only moves horizontally and vertically, it must differ from the merchant's position by only one
     coordinate.*/
    private Vector3 currentGoal; 
    private Vector3 currentMovementVector; //the vector he translates by in every frame. It is one of the following four vectors:
    Vector3 up = new Vector3(0, 0, .1f);
    Vector3 down = new Vector3(0, 0, -.1f);
    Vector3 right = new Vector3(.1f, 0, 0);
    Vector3 left = new Vector3(-.1f, 0, 0);

    private OriginDestination currentOriginDestination;
    private const int WALKABLE = 0; //a tile on which the merchant (and the player) can step.
    private const int NUM_DIMENSIONS = 2; //there are two dimensions of movement: up-down, left-right.
    private int[,] map;
    int currentTurnIndex;
    List<Vector3> destinations;//can be anywhere in the world, provided externally
    List<OriginDestination> paths;//for each destination, there is a list of points it should get to.
    
    public enum Direction { UP0, DOWN0, UP1, DOWN1 };
    int currentDestinationIndex;
    public class OriginDestination
    {
        private bool pathFound;
        private int[] origin;
        private int[] destination;
        private List<int[]> path;
        private List<Direction> directions;
        private int[,] map;
        public int getNumberOfTurns()
        {
            return path.Count;
        }
        public Direction getDirection(int index)
        {
            return directions[index];
        }
        public void setMap(int[,] aMap)
        {
            map = aMap;
        }
        public void addPointToPath(int[] point, Direction d)
        {
            path.Add(point);
            directions.Add(d);
            Debug.Log("Added point " + path[path.Count - 1][0] + "," + path[path.Count - 1][1] + "to index"+(path.Count-1));
        }
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
        public OriginDestination(int[] anOrigin, int[] aDestination)
        {
            origin = anOrigin;
            destination = aDestination;
            path = new List<int[]>();
            directions = new List<Direction>();
            pathFound = false;
        }
        public bool getPathFound()
        {
            return pathFound;
        }
        public void setPathFound(bool b)
        {
            pathFound = b;
        }
        public int[] getOrigin()
        {
            return (int[])origin.Clone();
        }
        public int[] getDestination()
        {
            return (int[])destination.Clone();
        }
        private int[] thinkOfStepping(Direction d, int[] position)
        {
            int[] result = new int[NUM_DIMENSIONS];
            if (d == Direction.UP0)
            {
                result[0] = position[0] + 1;
                result[1] = position[1];
            }
            else if (d == Direction.DOWN0)
            {
                result[0] = position[0] - 1;
                result[1] = position[1];
            }
            else if (d == Direction.UP1)
            {
                result[0] = position[0];
                result[1] = position[1] + 1;
            }
            else
            {
                result[0] = position[0];
                result[1] = position[1] - 1;
            }

            return result;
        }
        private int[] asCloseAsHeCanGet(Direction d, int[] start, int[] dest)
        {
            int[] x = new int[NUM_DIMENSIONS];
            Array.Copy(start, x, NUM_DIMENSIONS);
            Debug.Log("x[0]=" + x[0] + "x[1]=" + x[1]);
            int[] next;
            while (x[0]!=dest[0]||x[1]!=dest[1])
            {
                Debug.Log("In while loop:" + "x[0]=" + x[0] + "x[1]=" + x[1]);
                next = thinkOfStepping(d, x);
                Debug.Log("d=" + d + "next[0]=" + next[0] + "next[1]=" + next[1]);
                if (next[0] >= 0 && next[1] >= 0 && next[0] <map.GetLength(0)
                    && next[1] < map.GetLength(1) && map[next[0], next[1]] == WALKABLE)
                {
                    Array.Copy(next, x, NUM_DIMENSIONS);
                }
                else
                    break;
            }
            Debug.Log("Return value:" + "x[0]=" + x[0] + "x[1]=" + x[1]);
            return x;
        }
       
        public int[] add(int[] first, int[] second)
        {
            int[] sum = new int[NUM_DIMENSIONS];
            for (int i = 0; i < NUM_DIMENSIONS; i++)
            {
                sum[i] = first[i] + second[i];
            }
            return sum;
        }
        public bool findpath()
        {
            Debug.Log("finding path");
            int[] origin = getOrigin();
            int[] destination = getDestination();
            if (map == null) Debug.Log("Map is null");
            if (destination == null) Debug.Log("destination is null");
            if (map[destination[0], destination[1]] != WALKABLE)
                return false;
            List<int[]> tentativePath = new List<int[]>();
            int i = origin[0];
            Direction d;
            Direction e;
            int[] tentativeGoal = { destination[0], origin[1] };
            if (origin[0] != destination[0])
            {
                if (origin[0] < destination[0])
                    d = Direction.UP0;
                else
                    d = Direction.DOWN0;
                if (asCloseAsHeCanGet(d, origin, tentativeGoal)[0] != tentativeGoal[0]){
                    Debug.Log("Didn't work 1.");
                    return false;
                }
                addPointToPath(tentativeGoal, d);
            }
            if (origin[1] != destination[1])
            {
                if (origin[1] < destination[1])
                    e = Direction.UP1;
                else
                    e = Direction.DOWN1;
                if (asCloseAsHeCanGet(e, tentativeGoal, destination)[1] != destination[1])
                {
                    clearPath();
                    Debug.Log("Didn't work 2.");
                    return false;
                }
                addPointToPath(origin, e);
            }
            Debug.Log("I'm pleased to say it has worked.");
            return true;

        }
    }
    void setVector(Vector3 vector, int[] array)
    {
        vector.x = array[0];
        vector.z = array[1];
    }
    void setArray(int[] array, Vector3 vector)
    {
        array[0] = (int)vector.x;
        array[1] = (int)vector.z;
    }
    void Start()
    {
        currentDestinationIndex = 0;
        currentTurnIndex = 0;
        destinations = new List<Vector3>();
        _transform = gameObject.transform;
        int[] origin=new int[NUM_DIMENSIONS];
        int[] destination=new int[NUM_DIMENSIONS];
        setArray(origin, new Vector3(1, 1, 1));
        setArray(destination, new Vector3(3, 1, 3));
        OriginDestination od=new OriginDestination(origin,destination);
        od.setMap(exampleMap());
        od.findpath();
        setVector(currentGoal,od.getPoint(0));
        currentMovementVector = directionToVector(od.getDirection(currentTurnIndex));
    }

    public static int[,] exampleMap()
    {
        int[,] map = new int[10, 10];
        return map;
    }
    private Vector3 directionToVector(Direction d)
    {
        if (d == Direction.DOWN0)
            return left;
        if (d == Direction.DOWN1)
            return down;
        if (d == Direction.UP0)
            return right;
        return up;
    }
    void Update()
    {
        if ((_transform.position - currentGoal).sqrMagnitude < .1)
        {
            bool notAtCurrentDestination = currentTurnIndex + 1 < paths[currentDestinationIndex].getNumberOfTurns();
            bool currentDestinationNotFinal = currentDestinationIndex + 1 < paths.Count;
            if (currentDestinationNotFinal || notAtCurrentDestination)
            {
                if (notAtCurrentDestination)
                {
                    currentTurnIndex++;
                }
                else if (currentDestinationNotFinal)
                {
                    currentDestinationIndex++;
                    currentTurnIndex = 0;
                }
                currentMovementVector=directionToVector(paths[currentDestinationIndex].getDirection(currentTurnIndex));
                _transform.Translate(currentMovementVector);
            }
        }
        else
            _transform.Translate(currentMovementVector); 
    }
}
