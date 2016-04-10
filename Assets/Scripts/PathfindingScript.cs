using System.Collections.Generic;
using UnityEngine;
using System;
//The merchant has a number of places to go. It finds a path to each of its required places and then goes there.
public class PathfindingScript : MonoBehaviour {
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

    //The class needs a map data structure so that the Merchant knows where is walkable and where is not.
    private int[,] map;
    int currentTurnIndex;
    List<Vector3> destinations;//can be anywhere in the world, provided externally
    List<OriginDestination> paths;//for each destination, there is a list of points it should get to.

    public enum Direction { UP0, DOWN0, UP1, DOWN1 };//directions of travel of the merchant
    int currentDestinationIndex;

    //I make each origin-destination pair into an inner class. This class has a method that finds a path between the points and other information.
    public class OriginDestination {
        private bool pathFound;
        private int[] origin;
        private int[] destination;

        /*each int[] in the List is of length 2 and represents the coordinates of a point that is a temporary goal for the Merchant as 
         * it travels from origin to destination. Thus, each one is a point at which the Merchant changes direction, except for the last 
         * int[], which is the actual destination.*/
        public List<int[]> path;

        /*The Merchant iterates through the list and follows the direction at the current position in the list.*/
        private List<Direction> directions;
        private int[,] map;

        //Returns the length of the List "path". I did not call it getPathLength so as not to confuse it with "path length" as in how much he has to walk.
        public int getNumberOfTurns() {
            return path.Count;
        }


        public Direction getDirection(int index) //Returns the Direction in the "directions" list at the specified index.
        {
            return directions[index];
        }


        public void setMap(int[,] aMap) {
            map = aMap;
        }

        //@param point a point that represents a temporary destination for the Merchant
        //@param d the direction in which the Merchant much travel to get to that point.
        public void addPointToPath(int[] point, Direction d) {
            path.Add(point);
            directions.Add(d);
            Debug.Log("Added point " + path[path.Count - 1][0] + "," + path[path.Count - 1][1] + "to index" + (path.Count - 1));
        }

        //Used when the path is incorrect and needs to be deleted.
        public void clearPath() {
            path = null;
        }
        public int[] getPoint(int index) {
            Debug.Log("requesting point at index " + index);
            Debug.Log(path[index]);
            return (int[])path[index].Clone();
        }

        //Constructor of an OriginDestination object.
        public OriginDestination(int[] anOrigin, int[] aDestination) {
            origin = anOrigin;
            destination = aDestination;
            path = new List<int[]>();
            directions = new List<Direction>();
            pathFound = false;
        }

        //Returns true if a path has been found, false otherwise
        public bool getPathFound() {
            return pathFound;
        }

        //Sets the pathFound variable to b
        private void setPathFound(bool b) {
            pathFound = b;
        }
        public int[] getOrigin() {
            return (int[])origin.Clone(); //Returns a Clone so that the origin cannot be modified
        }
        public int[] getDestination() {
            return (int[])destination.Clone(); //Returns a Clone so that the origin cannot be modified
        }

        //@param position the starting position
        //@param d the direction of motion
        //@return the position after a movement of 1 unit in that direction
        private int[] thinkOfStepping(Direction d, int[] position) {
            int[] result = new int[NUM_DIMENSIONS];
            if (d == Direction.UP0) {
                result[0] = position[0] + 1;
                result[1] = position[1];
            }
            else if (d == Direction.DOWN0) {
                result[0] = position[0] - 1;
                result[1] = position[1];
            }
            else if (d == Direction.UP1) {
                result[0] = position[0];
                result[1] = position[1] + 1;
            }
            else {
                result[0] = position[0];
                result[1] = position[1] - 1;
            }

            return result;
        }
        private int[] asCloseAsHeCanGet(Direction d, int[] start, int[] dest) //If the merchant walks in one direction from start to dest, the point at which he stops.
        {
            int[] x = new int[NUM_DIMENSIONS];
            Array.Copy(start, x, NUM_DIMENSIONS);
            Debug.Log("x[0]=" + x[0] + "x[1]=" + x[1]);
            int[] next;
            while (x[0] != dest[0] || x[1] != dest[1]) {
                //Debug.Log("In while loop:" + "x[0]=" + x[0] + "x[1]=" + x[1]);
                next = thinkOfStepping(d, x);
                //Debug.Log("d=" + d + "next[0]=" + next[0] + "next[1]=" + next[1]);
                if (next[0] >= 0 && next[1] >= 0 && next[0] < map.GetLength(0)
                    && next[1] < map.GetLength(1) && map[next[0], next[1]] == WALKABLE) {
                    Array.Copy(next, x, NUM_DIMENSIONS);
                }
                else
                    break;
            }
            Debug.Log("Return value:" + "x[0]=" + x[0] + "x[1]=" + x[1]);
            return x;
        }

        private int[] add(int[] first, int[] second) {
            int[] sum = new int[NUM_DIMENSIONS];
            for (int i = 0; i < NUM_DIMENSIONS; i++) {
                sum[i] = first[i] + second[i];
            }
            return sum;
        }
        public bool findpath() {
            Debug.Log("finding path");
            int[] origin = getOrigin();
            int[] destination = getDestination();
            if (map[destination[0], destination[1]] != WALKABLE)
                return false;
            List<int[]> tentativePath = new List<int[]>();
            Direction d;
            Direction e;
            int[] tentativeGoal = { destination[0], origin[1] };//tries to move "horizontally" first
            if (origin[0] != destination[0]) {
                if (origin[0] < destination[0])
                    d = Direction.UP0;
                else
                    d = Direction.DOWN0;
                if (asCloseAsHeCanGet(d, origin, tentativeGoal)[0] != tentativeGoal[0]) {
                    Debug.Log("Didn't work 1.");
                    return false;
                }
                addPointToPath(tentativeGoal, d);
            }
            if (origin[1] != destination[1]) {
                if (origin[1] < destination[1])
                    e = Direction.UP1;
                else
                    e = Direction.DOWN1;
                if (asCloseAsHeCanGet(e, tentativeGoal, destination)[1] != destination[1]) {
                    clearPath();
                    Debug.Log("Didn't work 2.");
                    return false;
                }
                addPointToPath(destination, e);
            }
            Debug.Log("I'm pleased to say it has worked.");
            return true;

        }
    }
    private Vector3 setVector(int[] array) {
        Vector3 vector;
        vector.x = array[0] - 10;
        vector.y = 1;
        vector.z = array[1] - 10;
        Debug.Log("vector=" + vector);
        return vector;
    }
    private int[] setArray(Vector3 vector) {
        int[] array = new int[NUM_DIMENSIONS];
        array[0] = (int)vector.x + 10;
        array[1] = (int)vector.z + 10;
        return array;
    }

    public static int[,] exampleMap() {
        int[,] map = new int[50, 50];
        return map;
    }
    private Vector3 directionToVector(Direction d) {
        if (d == Direction.DOWN0)
            return left;
        if (d == Direction.DOWN1)
            return down;
        if (d == Direction.UP0)
            return right;
        return up;
    }
    public Vector3 dest_pt;
    public bool going = false;

    void Start() {
        _transform = gameObject.transform;
    }

    private void Go() {
        going = true;
        currentDestinationIndex = 0;
        currentTurnIndex = 0;
        destinations = new List<Vector3>();        
        int[] origin = new int[NUM_DIMENSIONS];
        int[] destination = new int[NUM_DIMENSIONS];
        // origin=setArray(new Vector3(-5, 1, -2));
        origin = setArray(_transform.position);
        destination = setArray(new Vector3(dest_pt.x, 1, dest_pt.z));
        OriginDestination od = new OriginDestination(origin, destination);
        currentOriginDestination = od;
        od.setMap(exampleMap());

        paths = new List<OriginDestination>();
        paths.Add(od);
        od.findpath();
        Debug.Log(od.getPoint(0)[0] + "," + od.getPoint(0)[1]);
        currentGoal = setVector(od.getPoint(0));
        Debug.Log("currentGoal=" + currentGoal);
        currentMovementVector = directionToVector(od.getDirection(currentTurnIndex));
        Debug.Log(od.getPoint(0)[0] + "," + od.getPoint(0)[1]);
        currentGoal = setVector(od.getPoint(0));
        Debug.Log("currentGoal=" + currentGoal);
        currentMovementVector = directionToVector(od.getDirection(currentTurnIndex));
    }

    void Update() {
        if (!going && Input.GetMouseButtonDown(0)) {
            var world_pt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var new_dest = new Vector3(Mathf.RoundToInt(world_pt.x), world_pt.y, Mathf.RoundToInt(world_pt.z));
            dest_pt = new_dest;
            Go();
        }
        //Debug.Log(_transform.position);
        //Debug.Log(currentGoal);
        else if ((_transform.position - currentGoal).sqrMagnitude < .01)//If it is at its current goal
        {
            if (paths == null)
                Debug.Log("paths==null");
            if (paths[currentDestinationIndex] == null)
                Debug.Log("paths[currentDestinationIndex]==null");
            bool notAtCurrentDestination = currentTurnIndex + 1 < paths[currentDestinationIndex].getNumberOfTurns();
            bool currentDestinationNotFinal = currentDestinationIndex + 1 < paths.Count;

            //check whether it needs to change its destination or whether it needs to change its "intermediate goal" to get to its current destination
            if (currentDestinationNotFinal || notAtCurrentDestination) {
                if (notAtCurrentDestination) {
                    Debug.Log("notAtCurrentDestination");
                    currentTurnIndex++;//In the latter case, increment its "intermediate goal"
                    currentGoal = setVector(currentOriginDestination.getPoint(currentTurnIndex));
                }
                else if (currentDestinationNotFinal) {
                    Debug.Log("currentDestinationNotFinal");
                    currentDestinationIndex++;//In the former case, increment its destination
                    currentOriginDestination = paths[currentDestinationIndex];
                    currentTurnIndex = 0;
                    currentGoal = setVector(currentOriginDestination.getPoint(currentTurnIndex));
                }
                Debug.Log("paths.Count=" + paths.Count);
                Debug.Log("currentDestinationIndex" + currentDestinationIndex);
                Debug.Log("currentTurnIndex=" + currentTurnIndex);
                currentMovementVector = directionToVector(paths[currentDestinationIndex].getDirection(currentTurnIndex)); //In either case, set the movement vector accordingly
                _transform.Translate(currentMovementVector);
            }
            else { // he has reached his final destination
                going = false;
                _transform.position = new Vector3(Mathf.RoundToInt(_transform.position.x), _transform.position.y, Mathf.RoundToInt(_transform.position.z));
            } 

        }
        else { //if he is not near his current goal, then he should keep moving in the same direction.
            _transform.Translate(currentMovementVector);
        }
    }
}