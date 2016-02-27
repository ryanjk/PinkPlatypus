using System;
using UnityEngine;
public class Merchant:MonoBehaviour
{
    private static float speed = 4f;
    private Rigidbody _rbody;
    private Transform _transform;
    private float desiredX;
    private float desiredY;
    private Vector2 tempDesiredPoint;
    public Vector2 finalDesiredPoint;
    private Vector2 movementVector;
    private Vector2 currentPosition;
    float vert;
    float hori;
    private const int WALKABLE = 0;

    void Start()
    {
        _transform = gameObject.transform;
        _rbody = GetComponent<Rigidbody>();
        vert = Input.GetAxis("Vertical");
        hori = Input.GetAxis("Horizontal");
    }
    public static int[,] exampleMap()
    {
        int[,] map=new int[,]{{0,0,1},{0,0,0},{2,0,0}};
        return map;
    }
    //Among other things, findpath sets the tempDesiredPoint and the movementVector.
	public void findpath(Vector2 origin, Vector2 destination, int[,] map)
	{
        int originX=(int)origin.x;
        int originY=(int)origin.y;
        if (originX < destination.x)
        {
            for (int i = (int) origin.x; i < destination.x; i++)
            {
                if (map[i, originY] != WALKABLE)
                {

                }
            }
        }
        tempDesiredPoint = finalDesiredPoint;
        movementVector = .1f * (tempDesiredPoint - Vector3.ProjectOnPlane(_transform.position, new Vector3(0,1,0)));                                                                                                                                                                              
        /*int[,] map = exampleMap();
        Boolean nonzero = false;
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                if (map[i, j] != 0)
                    nonzero = true;
        if (nonzero||!nonzero)
        {
            _rbody.velocity = new Vector3(0, 0, speed);

        }*/
                
	}
    void Update()
    {

        if (_transform.position == tempDesiredPoint)
        {
            if (tempDesiredPoint != finalDesiredPoint)
                findpath(_transform.position,tempDesiredPoint,exampleMap());
        }
        else
            _transform.Translate(movementVector);


    }
    
}
