﻿using System.Collections.Generic;
using UnityEngine;
using System;

//The merchant has a number of places to go. It finds a path to each of its required places and then goes there.
public class Merchant : MonoBehaviour {
    public SpriteRenderer U, D, L, R; // assigned in Unity editor
    private Direction _direction;
    private Transform _transform; //an object keeping track of the Merchant's position
    /*Where the merchant is heading. Because he only moves horizontally and vertically, it must differ from the merchant's position by only one
     coordinate.*/
    private Vector3 currentGoal;
    private Vector3 currentMovementVector; //the vector he translates by in every frame. It is one of the following four vectors:

    private OriginDestination currentOriginDestination;
  

    //The class needs a map data structure so that the Merchant knows where is walkable and where is not.
    //private static int[,] map;
    private int currentGoalIndex;
    private List<OriginDestination> paths;//for each destination, there is a list of points it should get to.
    private float speed;
    private List<Node> currentPath;
	private const int NUM_DIMENSIONS = 2;


	public int[] getPositionPath(float pos) {
		int estimate = (int)pos* paths.Count;
		return paths[estimate].getDestination();
		
	}

    public void setDestination(Vector3 destinationVector) {
        int[] originArray;
        int pathCount = paths.Count;
        if (pathCount == 0)
            originArray = setArray(gameObject.transform.position);
        else
            originArray = paths[paths.Count - 1].getDestination();
        paths.Add(new OriginDestination(originArray, setArray(destinationVector)));     
    }

    public Vector3[] getOriginDestination(int index) {
        Vector3[] x=new Vector3[2];
        x[0] = setVector(paths[index].getOrigin());
        x[1] = setVector(paths[index].getDestination());
        return x;
    }

    public int getPathLength(int index) {
        return paths[index].getNumberOfPoints();
    }

    public bool finishedSettingDestinationsAndMap;

	public void setMap(TileMapScript aMap) {
		currentOriginDestination.setMap (aMap);
	}

    private Vector3 setVector(int[] array) {
        Vector3 vector;
        vector.x = array[0] - 10;
        vector.y = 1;
        vector.z = array[1] - 10;
        return vector;
    }

    private int[] setArray(Vector3 vector) {
        int[] array = new int[NUM_DIMENSIONS];
        array[0] = (int)vector.x + 10;
        array[1] = (int)vector.z + 10;
        return array;
    }

    private Vector3 directionToVector(int[] direction) {
        return new Vector3(direction[0], 0, direction[1]);
    }

    private void setSprite(Direction newDirection) {
        if (newDirection == Direction.DOWN || newDirection == Direction.NONE) {
            U.enabled = false;
            L.enabled = false;
            R.enabled = false;
            D.enabled = true;
            return;
        } else if (newDirection == Direction.LEFT) {
            U.enabled = false;
            L.enabled = true;
            R.enabled = false;
            D.enabled = false;
            return;
        } else if (newDirection == Direction.RIGHT) {
            U.enabled = false;
            L.enabled = false;
            R.enabled = true;
            D.enabled = false;
            return;
        } else if (newDirection == Direction.UP) {
            U.enabled = true;
            L.enabled = false;
            R.enabled = false;
            D.enabled = false;
            return;
        }
    }

    private Direction getDirection() {
        if (currentMovementVector.x > 0) return Direction.RIGHT;
        else if (currentMovementVector.x < 0) return Direction.LEFT;
        else if (currentMovementVector.z > 0) return Direction.UP;
        else return Direction.DOWN;
    }

    private enum Direction {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }

    public void set_sprite_from_movement(int[] from, int[] to) {
        var direction = Direction.NONE;
        if (from == null && to == null) {
            setSprite(direction);
            return;
        }
        if (from[0] < to[0]) {
            direction = Direction.DOWN;
        }
        else if (from[0] > to[0]) {
            direction = Direction.UP;
        }
        else if (from[1] < to[1]) {
            direction = Direction.RIGHT;
        }
        else if (from[1] > to[1]) {
            direction = Direction.LEFT;
        }
        setSprite(direction);
    }

    void Awake() {
        paths = new List<OriginDestination>();
        finishedSettingDestinationsAndMap = false;
    }

    void Start() {

		currentOriginDestination= new OriginDestination();
        speed = 0.08f;
		currentOriginDestination.currentDestinationIndex = 0;

        _transform = gameObject.transform;
		currentOriginDestination.currentDestinationIndex = 0;
        currentGoalIndex = 1;
        currentGoal = _transform.position;
    }

    void Update() {
        if (finishedSettingDestinationsAndMap) {
            if ((_transform.position - currentGoal).sqrMagnitude < .01) { //If it is at its current goal
                bool notAtCurrentDestination = currentGoalIndex + 1 < paths[currentOriginDestination.currentDestinationIndex].getNumberOfPoints();
				bool currentDestinationNotFinal = currentOriginDestination.currentDestinationIndex + 1 < paths.Count;

                //check whether it needs to change its destination or whether it needs to change its "intermediate goal" to get to its current destination
                if (currentDestinationNotFinal || notAtCurrentDestination) {
                    if (notAtCurrentDestination) {
                        currentGoalIndex++;//In the latter case, increment its "intermediate goal"
                    } else if (currentDestinationNotFinal) {
						currentOriginDestination.currentDestinationIndex++;//In the former case, increment its destination
                        currentGoalIndex = 1;
                    }
					currentGoal = setVector(paths[currentOriginDestination.currentDestinationIndex].getPoint(currentGoalIndex));
                    /*Debug.Log("currentDestinationIndex = "+currentDestinationIndex);
                    Debug.Log("currentDestination=" + paths[currentDestinationIndex].getDestination()[0] + "," + paths[currentDestinationIndex].getDestination()[1]);
                    Debug.Log("currentGoalIndex - "+currentGoalIndex);
                    Debug.Log("currentGoal=" + paths[currentDestinationIndex].getDirection(currentGoalIndex)[0] + "," + paths[currentDestinationIndex].getDirection(currentGoalIndex)[1]);*/
					currentMovementVector = speed * directionToVector(paths[currentOriginDestination.currentDestinationIndex].getDirection(currentGoalIndex)); //In either case, set the movement vector accordingly
                }
                //else, he has reached his final destination, so he should not move.
            }
            else { //if he is not near his current goal, then he should keep moving in the same direction.
                _transform.Translate(currentMovementVector);
            }
            setSprite(getDirection());
        }
    }
} 