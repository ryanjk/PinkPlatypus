﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
* @author Thomas and Ryan
*/
public class NPlayerController : NetworkBehaviour {

    // time takes to move distance
    private float _speed = 0.3f;

    // distance per step
    private float _distance = 1f;

    private Direction _direction;
    private Rigidbody _rbody;
    public SpriteRenderer U, D, R, L;
	void Start () {
        _rbody = GetComponent<Rigidbody>();
        _direction = Direction.NONE;
    }
	
    void Update () {

    }

	void FixedUpdate () {
        if(!(gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)) {
            return;
        }
        // currently moving, don't even process input
        if (_direction != Direction.NONE) {
            return;
        }
            float vert = Input.GetAxis("Vertical");
            float hori = Input.GetAxis("Horizontal");

        var new_direction = input_to_direction(vert, hori);

        // don't move if nothing is being pressed
        if (new_direction == Direction.NONE) {
            return;
        }

        // start moving (check out iTween library for more info on what this is doing)
        iTween.MoveBy(gameObject, iTween.Hash(
            "amount", _distance * direction_to_velocity(new_direction),
            "time", _speed,
            "oncomplete", "stop_moving",
            "easetype", iTween.EaseType.linear
        ));

        // update player state
        _direction = new_direction;
        set_sprite(new_direction);

    }

    public void stop_moving() {
        _direction = Direction.NONE;

        // see if input is pressed and keep moving if so
        float vert = Input.GetAxis("Vertical");
        float hori = Input.GetAxis("Horizontal");

        if (vert == 0.0f && hori == 0.0f) return;

        FixedUpdate();
    }

    private void set_sprite (Direction direction) {
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
    public void setSprites(bool isHost) {
        if(isHost) {
            U = GameObject.Find("sprite_U").GetComponent<SpriteRenderer>();
            D = GameObject.Find("sprite_D").GetComponent<SpriteRenderer>();
            L = GameObject.Find("sprite_L").GetComponent<SpriteRenderer>();
            R = GameObject.Find("sprite_R").GetComponent<SpriteRenderer>();
            return;
        }
        else {
            U = GameObject.Find("sprite2_U").GetComponent<SpriteRenderer>();
            D = GameObject.Find("sprite2_D").GetComponent<SpriteRenderer>();
            L = GameObject.Find("sprite2_L").GetComponent<SpriteRenderer>();
            R = GameObject.Find("sprite2_R").GetComponent<SpriteRenderer>();
            return;
        }
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
