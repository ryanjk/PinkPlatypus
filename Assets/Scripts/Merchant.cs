using System.Collections.Generic;
using UnityEngine;
using System;
public class Merchant : MonoBehaviour
{
    private static float speed = 4f;
    private Transform _transform;
    private Vector3 currentGoal;
    private Vector3 movementVector;
    private const int WALKABLE = 0;
    private const int NUM_DIMENSIONS = 2;
    private int[,] map;
    List<bool> pathfound;
    int currentTurnIndex;
    List<Vector3> destinations;//can be anywhere in the world, provided externally
    List<int[]> truePath;
    List<List<int[]>> paths;//for each destination, there is a list of points it should get to.
    
    private enum Direction { UP0, DOWN0, UP1, DOWN1 };
    int currentDestinationIndex;
    private bool pathFound(int[] origin, int[] destination)
    {
        //for (int i = 0; i < paths.Count;i++)
          //  if
            return false;
    }
    public class OriginDestination
    {
        private bool pathFound;
        private int[] origin;
        private int[] destination;
        public OriginDestination(int[] anOrigin, int[] aDestination)
        {
            origin = anOrigin;
            destination = aDestination;
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
            return (int[]) origin.Clone();
        }
        public int[] getDestination()
        {
            return (int[]) destination.Clone();
        }
    }
    void Start()
    {
        currentDestinationIndex = 0;
        addDestination(new Vector3(1, 1, 1));
        addDestination(new Vector3(1, 1, 2));
        _transform = gameObject.transform;
        int[] origin=new int[NUM_DIMENSIONS];
        int[] destination=new int[NUM_DIMENSIONS];
        setArray(origin, new Vector3(1, 1, 1));
        setArray(origin, new Vector3(3, 1, 3));
        OriginDestination od=new OriginDestination(origin,destination);
        //addDestination(new Vector3(2, 1, 2));
        //addDestination(new Vector3(2, 1, 1));
        
        Debug.Log(_transform.position);
        map = exampleMap();
        paths = new List<List<int[]>>();
        findpath(od);
    }
    public void addDestination(Vector3 destination)
    {
        destinations.Add(destination);
        pathfound.Add(false);
    }
    public static int[,] exampleMap()
    {
        int[,] map = new int[10, 10];
        return map;
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
        while (!dest.Equals(x))
        {
            next = thinkOfStepping(d, x);
            if (next[0] >= 0 && next[1] >= 0 && next[0] <= map.GetLength(0)
                && next[1] <= map.GetLength(1) && map[next[0], next[1]] == WALKABLE)
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
    private bool findpath(OriginDestination od)
    {
        Debug.Log("finding path");
        int[] origin = od.getOrigin();
        int[] destination = od.getDestination();
        if (map[destination[0], destination[1]] != WALKABLE)
            return false;
        List<int[]> tentativePath = new List<int[]>();
        int i = origin[0];
        Direction d;
        if (origin[0] < destination[0])
        {
            d = Direction.UP0;
            movementVector = new Vector3(.1f, 0, 0);
        }
        else if (origin[0] > destination[0])
        {
            d = Direction.DOWN0;
            movementVector = new Vector3(-.1f, 0, 0);
        }
        else if (origin[1] < destination[1])
        {
            d = Direction.UP1;
            movementVector = new Vector3(0, 0, .1f);
        }
        else
        {
            d = Direction.DOWN1;
            movementVector = new Vector3(0, 0, -.1f);
        }

        int[] tentativeGoal = { destination[0], origin[1] };
        tentativePath.Add(tentativeGoal);
        if (asCloseAsHeCanGet(d, origin, destination).Equals(tentativeGoal)) ;
        //else to do later
        tentativePath.Add(tentativeGoal);
        if (origin[1] < destination[1])
            d = Direction.UP1;
        else
            d = Direction.DOWN1;
        if (asCloseAsHeCanGet(d, tentativeGoal, destination).Equals(destination))
        {
            List<int[]> thisPath=new List<int[]>();
            thisPath.Add(tentativeGoal);
            thisPath.Add(destination);
            paths.Add(thisPath);
            od.setPathFound(true);
            currentTurnIndex = 0;
            setVector(currentGoal, truePath[currentTurnIndex]);
            return true;
        }

        return false;
    }
    void setVector(Vector3 vector, int[] array)
    {
        vector.x = array[0];
        vector.z = array[1];
    }
    void setArray(int[] array, Vector3 vector)
    {
        array[0] = (int) vector.x;
        array[1] = (int) vector.z;
    }
    void Update()
    {
        Debug.Log("Transform position "+_transform.position);
        Debug.Log("currentGoal "+currentGoal);
        //if ((_transform.position - currentGoal).sqrMagnitude < 0.5)
        //{

            //if(currentGoalIndex<truePath.)
            //Debug.Log("Hallelujah");
            //if(currentGoal.Equals)
            // currentGoalIndex = (currentGoalIndex + 1) % truePath.Count;
            //setVector(currentGoal, truePath[currentGoalIndex]);
        //}
        //else
          //  _transform.Translate(movementVector);


    }

}
