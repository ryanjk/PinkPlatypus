using UnityEngine;
using System.Collections;

/**
* @author Thomas
*/
public class PlayerController : MonoBehaviour {
    private float maxSpeed = 4f;
    private float minSpeed = 0.1f;
    private int direction = 0;

    private Direction prev_direction = Direction.NONE;
    private float speed = 4.0f;
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

        var current_direction = input_to_direction(vert, hori);

        // check if changing direction
        if (current_direction == prev_direction) {
            // not changing, keep going
            _rbody.velocity = direction_to_velocity(current_direction);
            set_sprite(current_direction);
        }

        else if (current_direction != prev_direction) {
            // changing, make sure player is in the center of a tile before changing
            var on_x = _rbody.position.x % 0.5f;
            var on_z = _rbody.position.z % 0.5f;
            var epsilon = 0.05f;
            if (( on_x < epsilon ) && (on_z < epsilon)) {
                // player is in the center, can change
                _rbody.MovePosition(new Vector3(_rbody.position.x - on_x, _rbody.position.y, _rbody.position.z - on_z));
                _rbody.velocity = direction_to_velocity(current_direction);
                set_sprite(current_direction);
                prev_direction = current_direction;
            }
        }

    }

    private void set_sprite (Direction direction) {
        if (direction == Direction.NONE) return;

        transform.GetChild(0).transform.localEulerAngles = new Vector3(90f, 0f, (int) direction * 90f);
    }

    private Direction input_to_direction(float vertical, float horizontal) {
        if (vertical < 0.0f) {
            return Direction.DOWN;
        }

        else if (vertical > 0.0f) {
            return Direction.UP;
        }

        else if (horizontal < 0.0f) {
            return Direction.LEFT;
        }

        else if (horizontal > 0.0f) {
            return Direction.RIGHT;
        }
        else {
            return Direction.NONE;
        }
    }

    private Vector3 direction_to_velocity(Direction dir) {
        switch (dir) {
            case Direction.UP:
                return new Vector3(0.0f, 0.0f, speed);
            case Direction.DOWN:
                return new Vector3(0.0f, 0.0f, -speed);
            case Direction.LEFT:
                return new Vector3(-speed, 0.0f, 0.0f);                
            case Direction.RIGHT:
                return new Vector3(speed, 0.0f, 0.0f);
            case Direction.NONE:
                return new Vector3(0.0f, 0.0f, 0.0f);
            default:
                return new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    private enum Direction {
        UP,
        LEFT,
        DOWN,
        RIGHT,
        NONE
    }
}
