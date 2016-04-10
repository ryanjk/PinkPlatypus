using System.Collections.Generic;
using UnityEngine;
using System;
public class MerchantDebug:MonoBehaviour
{
    private static float speed = 4f;
    private Rigidbody _rbody;
    private Transform _transform;
    private float desiredX;
    private float desiredY;
    private Vector2 currentGoal;
    public Vector2 finalDesiredPoint;
    private Vector2 movementVector;
    private Vector2 currentPosition;
    float vert;
    float hori;
    private const int WALKABLE = 0;
    private const int NUM_DIMENSIONS=2;
    private int[,] map;
    int[,] directions;
    int[] right;
    int[] left;
    int[] up;
    int[] down;

    private enum Direction { UP0, DOWN0, UP1, DOWN1 };
    void Start()
    {
        _transform = gameObject.transform;
        Debug.Log(_transform.position); 
        map = exampleMap();
        currentGoal = new Vector2(1, 1);
    }
    public static int[,] exampleMap()
    {
        int[,] map=new int[,]{{0,0,1},{0,0,0},{2,0,0}};
        return map;
    }
    public int[] add(int[] first, int[] second)
    {
        int[] sum = new int[NUM_DIMENSIONS];
        for(int i=0; i < NUM_DIMENSIONS; i++)
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
    public int[] asCloseAsHeCanGet(Direction d, int[] start, int[] dest)
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
    public int[] asFarAsHeCanGo(int[] direction, int[] start)
    {
        int[] x = new int[NUM_DIMENSIONS];
        Array.Copy(start, x,NUM_DIMENSIONS);
        int[] next;
        while (true)
        {
            next = add(x, direction);
            if (next[0]>=0&&next[1]>=0&&next[0]<=map.GetLength(0)
                &&next[1]<=map.GetLength(1)&&map[next[0], next[1]] == WALKABLE)
            {
                Array.Copy(next, x,NUM_DIMENSIONS);
            }
            else break;
        }
        return x;
    }
    //Among other things, findpath sets the tempDesiredPoint and the movementVector.
	public bool findpath(Vector2 originVector, Vector2 destination)
	{
        int[] origin = new int[NUM_DIMENSIONS];
        int[] dest = new int[NUM_DIMENSIONS];
        origin[0] = (int)originVector.x;
        origin[1] = (int)originVector.y;
        dest[0] = (int)destination.x;
        dest[1] = (int)destination.y;
        if (map[dest[0], dest[1]] != WALKABLE)
            return false;
        List<int[]> truePath=new List<int[]>();
        List<int[]> tentativePath = new List<int[]>();
        int i = origin[0];
        Direction d;
        if (origin[0] < dest[0])
            d = Direction.UP0;
        else
            d = Direction.DOWN0;
        int[] tentativeGoal={dest[0],origin[1]};
        tentativePath.Add(tentativeGoal);
        if (asCloseAsHeCanGet(d, origin, dest).Equals(tentativeGoal)) ;
        //else to do later
        tentativePath.Add(tentativeGoal);
        if (origin[1] < dest[1])
            d = Direction.UP1;
        else
            d = Direction.DOWN1;
        if (asCloseAsHeCanGet(d, origin, dest).Equals(dest))
        {
            currentGoal.x = tentativeGoal[0];
            currentGoal.y = tentativeGoal[1];
            truePath.Add(tentativeGoal);
            truePath.Add(dest);
            return true;
        }
        
        return false;
	}
    void Update()
    {
        if (currentPosition == currentGoal)
        {
            if (currentGoal != finalDesiredPoint)
                findpath(_transform.position, currentGoal);
        }
        else
            _transform.Translate(movementVector);


    }
    
}

/*tentativePath.Add(new Vector2(i, originY));
tempDesiredPoint = finalDesiredPoint;
currentPosition=Vector3.ProjectOnPlane(_transform.position, new Vector3(0, 1, 0));
movementVector = .1f * (tempDesiredPoint - currentPosition);       */
