using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
* @author Thomas, Ryan and Harry
*/
public class NPlayerController : NetworkBehaviour {

    // time takes to move distance
    private float _speed = 0.3f;

    // distance per step
    private float _distance = 1f;
    private Direction _direction;

    private Rigidbody _rbody;
    public SpriteRenderer U, D, R, L, U2, D2, R2, L2; // Sprites for all 4 directions
    public Camera cam;
    public bool ignoreInput;
    private bool host;

    void Start() {
        _rbody = GetComponent<Rigidbody>();
        _direction = Direction.NONE;
        ignoreInput = false;
        U2.enabled = false;
        R2.enabled = false;
        L2.enabled = false;
        D2.enabled = false;
        host = true;
    }

    void Update() {
    }

    public void switchToPlayer2() {
        U.enabled = false;
        R.enabled = false;
        L.enabled = false;
        D.enabled = false;
        U = U2;
        R = R2;
        L = L2;
        D = D2;
        D.enabled = true;
    }
    public bool isHost() {
        return host;
    }

    public void otherPlayer() {
        cam.enabled = false;
        ignoreInput = true;
        host = false;
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

    public void on_tween_complete() {
        _direction = Direction.NONE;

        // see if input is pressed and keep moving if so
        float vert = Input.GetAxis("Vertical");
        float hori = Input.GetAxis("Horizontal");

        if (vert == 0.0f && hori == 0.0f) return;

        FixedUpdate();
    }

    private void set_sprite(Direction direction) {
        if (direction == Direction.NONE || direction == Direction.DOWN) {
            R.enabled = false;
            L.enabled = false;
            U.enabled = false;
            D.enabled = true;
            return;
        }
        if (direction == Direction.UP) {
            R.enabled = false;
            L.enabled = false;
            U.enabled = true;
            D.enabled = false;
            return;
        }
        if (direction == Direction.RIGHT) {
            R.enabled = true;
            L.enabled = false;
            U.enabled = false;
            D.enabled = false;
            return;
        }
        if (direction == Direction.LEFT) {
            R.enabled = false;
            L.enabled = true;
            U.enabled = false;
            D.enabled = false;
            return;
        }

        //transform.GetChild(0).transform.localEulerAngles = new Vector3(90f, 0f, (int) direction * 90f);
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
