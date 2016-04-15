using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
* Player Controller
* @author Thomas, Ryan and Harry
*
* Class converting player input into character movement
*
* This class takes the default Unity input, and translates it into movement
* for the player character, including tweening provided by an external library.
* The class also handles switching between sprites to match the direction
* the player character is facing.
*/
public class PlayerController : MonoBehaviour {

    // time takes to move distance
    private float _speed = 0.22f;

    // distance per step
    private float _distance = 1f;

    private Direction _direction;
    private Rigidbody _rbody;
    public SpriteRenderer U, D, R, L; // Sprites for all 4 directions
    public bool ignoreInput;

    void Start() {
        // initialize some fields
        _rbody = GetComponent<Rigidbody>();
        _direction = Direction.NONE;
        ignoreInput = false;
    }

    void Update() {

    }

    void FixedUpdate() {

        // currently moving, don't even process input
        if (_direction != Direction.NONE) {
            return;
        }
        float vert = 0f;
        float hori = 0f;
        if (!ignoreInput) {
            vert = Input.GetAxis("Vertical");
            hori = Input.GetAxis("Horizontal");
        }
            var new_direction = input_to_direction(vert, hori);

        // don't move if nothing is being pressed
        if (new_direction == Direction.NONE) {
            return;
        }

        // start moving (check out iTween library for more info on what this is doing)
        iTween.MoveBy(gameObject, iTween.Hash(
            "amount", _distance * direction_to_velocity(new_direction),
            "name", "player_move_tween",
            "time", _speed,
            "oncomplete", "on_tween_complete",
            "easetype", iTween.EaseType.linear
        ));

        // update player state
        _direction = new_direction;
        set_sprite(new_direction);

    }

    public void stop_moving() {
        _direction = Direction.NONE;
        iTween.StopByName("player_move_tween");
    }

    public bool isStopped() {
        return (_direction == Direction.NONE);
    }

    public void on_tween_complete() {
        _direction = Direction.NONE;

        // see if input is pressed and keep moving if so
            float vert = Input.GetAxis("Vertical");
            float hori = Input.GetAxis("Horizontal");
        
        if (vert == 0.0f && hori == 0.0f) return;

        FixedUpdate();
    }

    /**
    * Simple method to switch the active sprite
    * 
    * @param direction takes in a direction enum and switches the sprite to the appropriate direction
    */
    private void set_sprite(Direction direction) {
        // most of the details of sprites are handled in Unity
        // down-facing/default case
        if (direction == Direction.NONE || direction == Direction.DOWN) {
            // disable every sprite except down sprite
            R.enabled = false;
            L.enabled = false;
            U.enabled = false;
            D.enabled = true;
            return;
        }
        // up-facing case
        if (direction == Direction.UP) {
            R.enabled = false;
            L.enabled = false;
            U.enabled = true;
            D.enabled = false;
            return;
        }
        // right-facing case
        if (direction == Direction.RIGHT) {
            R.enabled = true;
            L.enabled = false;
            U.enabled = false;
            D.enabled = false;
            return;
        }
        // left-facing case
        if (direction == Direction.LEFT) {
            R.enabled = false;
            L.enabled = true;
            U.enabled = false;
            D.enabled = false;
            return;
        }

        //transform.GetChild(0).transform.localEulerAngles = new Vector3(90f, 0f, (int) direction * 90f);
    }

    /**
    * converting the input into a direction enum
    *
    * @param vertical the value of the vertical input
    * @param horizontal the value of the horizontal input
    * @return the corresponding Direction enum
    */
    private Direction input_to_direction(float vertical, float horizontal) {
        // negative vertical? -> facing down
        if (vertical < 0.0f) {
            return Direction.DOWN;
        }
        // positive vertical? -> facing up
        else if (vertical > 0.0f) {
            return Direction.UP;
        }
        // negative horizontal? -> facing left
        else if (horizontal < 0.0f) {
            return Direction.LEFT;
        }
        // positive horizontal? -> facing right
        else if (horizontal > 0.0f) {
            return Direction.RIGHT;
        }
        // zero input? -> default direction
        else {
            return Direction.NONE;
        }
    }

    /**
    * simple switch block mapping a direction enum to a direction vector
    *
    * @param dir the direction to convert
    * @return a unit vector in the correct direction (or the 0-vector)
    */
    private Vector3 direction_to_velocity(Direction dir) {
        switch (dir) {
            case Direction.UP:
                return new Vector3(0.0f, 0.0f, 1.0f);
            case Direction.DOWN:
                return new Vector3(0.0f, 0.0f, -1.0f);
            case Direction.LEFT:
                return new Vector3(-1.0f, 0.0f, 0.0f);
            case Direction.RIGHT:
                return new Vector3(1.0f, 0.0f, 0.0f);
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
