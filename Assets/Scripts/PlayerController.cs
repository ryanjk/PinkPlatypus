using UnityEngine;
using System.Collections;

/**
* @author Thomas
*/
public class PlayerController : MonoBehaviour {
    private float maxSpeed = 4f;
    private float minSpeed = 0.1f;
    private int direction = 0;
    private Rigidbody _rbody;

	void Start () {
        _rbody = GetComponent<Rigidbody>();
	}
	
    void Update () {

    }

    /**
    * From what I can gather, it seems simpler to implement
    * movement in the FixedUpdate as opposed to Update.
    * It starts by getting the vertical and horizontal input values,
    * then checks to see if the input is greater than the threshold value
    * in the given direction, to facilitate continuous motion.
    * If it's not, then we check for input in any direction, and flip
    * to that direction and start moving.
    * If there's no input, we don't move (obviously).
    */
	void FixedUpdate () {
        float vert = Input.GetAxis("Vertical");
        float hori = Input.GetAxis("Horizontal");

        if ((direction == 0 && vert > minSpeed) || (direction == 2 && vert < -minSpeed)) {
            _rbody.velocity = new Vector3(0f, 0f, maxSpeed * vert);
        } else if ((direction == 1 && hori < -minSpeed) || (direction == 3 && hori > minSpeed)) {
            _rbody.velocity = new Vector3(maxSpeed * hori, 0f, 0f);
        } else {
            if (vert > minSpeed)
            {
                changeDirection(0);
                _rbody.velocity = new Vector3(0f, 0f, maxSpeed * vert);
            }
            else if (hori > minSpeed)
            {
                changeDirection(3);
                _rbody.velocity = new Vector3(maxSpeed * hori, 0f, 0f);
            }
            else if (vert < -minSpeed)
            {
                changeDirection(2);
                _rbody.velocity = new Vector3(0f, 0f, maxSpeed * vert);
            }
            else if (hori < -minSpeed)
            {
                changeDirection(1);
                _rbody.velocity = new Vector3(maxSpeed * hori, 0f, 0f);
            }
            else _rbody.velocity = new Vector3(0f, 0f, 0f);
        }
	}

   private void changeDirection (int pDirection) {
        direction = pDirection;
        transform.GetChild(0).transform.localEulerAngles = new Vector3(90f, 0f, pDirection * 90f);
   }
}
