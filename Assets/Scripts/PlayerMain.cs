using UnityEngine;
using System.Collections;
public class PlayerMain : MonoBehaviour {
    private Rigidbody rb;
    public float speed;
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        //TODO: fix this, can't create a script with new. should be AddComponent.
		//_inventory = new ItemInventory ();
        _sceneLoadData = new SceneLoadData();

	}
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }

	private ItemInventory _inventory;

    //TODO: change to private, add access methods
    public SceneLoadData _sceneLoadData;

} 
	