using System.Collections.Generic;
using UnityEngine;
using System;
public class Merchant : MonoBehaviour
{
    private static float speed = 4f;
    private Transform _transform;
    private Vector3 currentGoal;
    private List<Vector3> movementVectors;
    private Vector3 currentMovementVector;
    private OriginDestination currentOriginDestination;
    private const int WALKABLE = 0;
    private const int NUM_DIMENSIONS = 2;
    private int[,] map;
    int currentTurnIndex;
    List<Vector3> destinations;//can be anywhere in the world, provided externally
    List<OriginDestination> paths;//for each destination, there is a list of points it should get to.
    Vector3 up = new Vector3(0, 0, .1f);
    Vector3 down = new Vector3(0, 0, -.1f);
    Vector3 right = new Vector3(.1f, 0, 0);
    Vector3 left = new Vector3(-.1f, 0, 0);
    
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
        }
        public void clearPath()
        {
            path = null;
        }
        public int[] getPoint(int index)
        {
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
            int[] next;
            while (x[0]!=dest[0]||x[1]!=dest[1])
            {
                next = thinkOfStepping(d, x);
                if (next[0] >= 0 && next[1] >= 0 && next[0] <map.GetLength(0)
                    && next[1] < map.GetLength(1) && map[next[0], next[1]] == WALKABLE)
                {
                    Array.Copy(next, x, NUM_DIMENSIONS);
                }
                else
                    break;
            }
            return x;
        }
        private int[] asFarAsHeCanGo(int[] direction, int[] start)
        {
            int[] x = new int[NUM_DIMENSIONS];
            Array.Copy(start, x, NUM_DIMENSIONS);
            int[] next;
            while (true)
            {
                next = add(x, direction);
                if (next[0] >= 0 && next[1] >= 0 && next[0] <= map.GetLength(0)
                    && next[1] <= map.GetLength(1) && map[next[0], next[1]] == WALKABLE)
                {
                    Array.Copy(next, x, NUM_DIMENSIONS);
                }
                else break;
            }
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
            int place = map[destination[0], destination[1]];
            int x = WALKABLE + 1;
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
                if (asCloseAsHeCanGet(d, origin, tentativeGoal)[0] != tentativeGoal[0])
                    return false;
                addPointToPath(tentativeGoal, d);
            }
            if (origin[1] != destination[1])
            {
                if (origin[1] < destination[1])
                    e = Direction.UP1;
                else
                    e = Direction.DOWN1;
                if (asCloseAsHeCanGet(e, tentativeGoal, destination)[1] != tentativeGoal[1])
                {
                    clearPath();
                    return false;
                }
                addPointToPath(origin, e);
            }
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
        destinations = new List<Vector3>();
        addDestination(new Vector3(1, 1, 1));
        addDestination(new Vector3(1, 1, 2));
        _transform = gameObject.transform;
        int[] origin=new int[NUM_DIMENSIONS];
        int[] destination=new int[NUM_DIMENSIONS];
        setArray(origin, new Vector3(1, 1, 1));
        setArray(destination, new Vector3(3, 1, 3));
        OriginDestination od=new OriginDestination(origin,destination);
        od.setMap(exampleMap());
        od.findpath();
        
        setVector(currentGoal,od.getPoint(0));
    }
    public void addDestination(Vector3 destination)
    {
        destinations.Add(destination);
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
        if ((_transform.position - currentGoal).sqrMagnitude < 0.1)
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
