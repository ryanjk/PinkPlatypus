using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OriginDestination
{
	//private bool pathFound;
	public bool leave;
	private int[] origin;
	private int[] destination;
	private static int[,] map;
	private const int WALKABLE = 0; //a tile on which the merchant (and the player) can step.
	private const int NUM_DIMENSIONS = 2; //there are two dimensions of movement: up-down, left-right.
	/*each int[] in the List is of length 2 and represents the coordinates of a point that is a temporary goal for the Merchant as 
         * it travels from origin to destination. Thus, each one is a point at which the Merchant changes direction, except for the last 
         * int[], which is the actual destination.*/
	public List<Node> path;

	public OriginDestination(){
	}
	/*The Merchant iterates through the list and follows the direction at the current position in the list.*/
	
	// private int[,] map;
	
	//Returns the length of the List "path". I did not call it getPathLength so as not to confuse it with "path length" as in how much he has to walk.
	public void setMap(int[,] aMap)
	{
		map = aMap;
	}
	public int currentDestinationIndex;
	//I make each origin-destination pair into an inner class. This class has a method that finds a path between the points and other information.

	public int getNumberOfPoints()
	{
		return path.Count;
	}
	
	public int[] getDirection(int index) //Returns the Direction in the "directions" list at the specified index.
	{
		return path[index].directionFromParent;
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
	private static int[] add(int[] first, int[] second)
	{
		int[] sum = new int[NUM_DIMENSIONS];
		for (int i = 0; i < NUM_DIMENSIONS; i++)
		{
			sum[i] = first[i] + second[i];
		}
		return sum;
	}
	//Constructor of an OriginDestination object.
	public OriginDestination(int[] anOrigin, int[] aDestination)
	{
		origin = anOrigin;
		destination = aDestination;
		path = new List<Node>();
		//pathFound = false;
		findpath();
	}
	
	//Returns true if a path has been found, false otherwise
	/*public bool getPathFound()
        {
            return pathFound;
        }*/
	
	//Sets the pathFound variable to b
	/*private void setPathFound(bool b)
        {
            pathFound = b;
        }*/
	public int[] getOrigin()
	{
		return (int[])origin.Clone(); //Returns a Clone so that the origin cannot be modified
	}
	public int[] getDestination()
	{
		return (int[])destination.Clone(); //Returns a Clone so that the destination cannot be modified
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
						path.Add(addNode(y, minNode, x));
						for (Node n = minNode; n != null; n = n.previous)
							path.Insert(0, n);
						//pathFound = true;
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
				
				
			}
		}
		return;
	}
}
