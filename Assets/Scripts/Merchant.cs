﻿using System.Collections.Generic;
using UnityEngine;
using System;
public class Merchant : MonoBehaviour
{
    private static float speed = 4f;
    private Rigidbody _rbody;
    private Transform _transform;
    private float desiredX;
    private float desiredY;
    private Vector3 currentGoal;
    public Vector3 finalDesiredPoint;
    private Vector3 movementVector;
    private Vector3 currentPosition;
    float vert;
    float hori;
    private const int WALKABLE = 0;
    private const int NUM_DIMENSIONS = 2;
    private int[,] map;
    int[,] directions;
    int[] right;
    int[] left;
    int[] up;
    int[] down;
    bool pathfound;
    int currentGoalIndex;
    List<Vector3> destinations;
    List<int[]> truePath;
    private enum Direction { UP0, DOWN0, UP1, DOWN1 };
    int currentDestinationIndex;
    void Start()
    {
        currentDestinationIndex = 0;
        destinations.Add(new Vector3(1, 1, 1));
        destinations.Add(new Vector3(1, 1, 2));
        destinations.Add(new Vector3(2, 1, 2));
        destinations.Add(new Vector3(2, 1, 1));
        pathfound = false;
        _transform = gameObject.transform;
        Debug.Log(_transform.position);
        map = exampleMap();
        findpath(_transform.position, destinations[0]);
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
    private bool move()
    {

        return false;
    }
    private bool findpath(Vector3 originVector, Vector3 destination)
    {
        Debug.Log("finding path");
        int[] origin = new int[NUM_DIMENSIONS];
        int[] dest = new int[NUM_DIMENSIONS];
        origin[0] = (int)originVector.x;
        origin[1] = (int)originVector.z;
        dest[0] = (int)destination.x;
        dest[1] = (int)destination.z;
        if (map[dest[0], dest[1]] != WALKABLE)
            return false;
        List<int[]> truePath = new List<int[]>();
        List<int[]> tentativePath = new List<int[]>();
        int i = origin[0];
        Direction d;
        if (origin[0] < dest[0])
        {
            d = Direction.UP0;
            movementVector = new Vector3(.1f, 0, 0);
        }
        else if (origin[0] > dest[0])
        {
            d = Direction.DOWN0;
            movementVector = new Vector3(-.1f, 0, 0);
        }
        else if (origin[1] < dest[1])
        {
            d = Direction.UP1;
            movementVector = new Vector3(0, 0, .1f);
        }
        else
        {
            d = Direction.DOWN1;
            movementVector = new Vector3(0, 0, -.1f);
        }

        int[] tentativeGoal = { dest[0], origin[1] };
        tentativePath.Add(tentativeGoal);
        if (asCloseAsHeCanGet(d, origin, dest).Equals(tentativeGoal)) ;
        //else to do later
        tentativePath.Add(tentativeGoal);
        if (origin[1] < dest[1])
            d = Direction.UP1;
        else
            d = Direction.DOWN1;
        if (asCloseAsHeCanGet(d, tentativeGoal, dest).Equals(dest))
        {
            truePath.Add(tentativeGoal);
            truePath.Add(dest);
            pathfound = true;
            currentGoalIndex = 0;
            setVector(currentGoal, truePath[currentGoalIndex]);
            return true;
        }

        return false;
    }
    void setVector(Vector3 vector, int[] array)
    {
        vector.x = array[0];
        vector.z = array[1];
    }
    void Update()
    {
        currentPosition = _transform.position;
        if ((currentPosition - currentGoal).sqrMagnitude < 1)
        {
            //Debug.Log("Hallelujah");
            //if(currentGoal.Equals)
            // currentGoalIndex = (currentGoalIndex + 1) % truePath.Count;
            //setVector(currentGoal, truePath[currentGoalIndex]);
        }
        else
            _transform.Translate(movementVector);


    }

}
