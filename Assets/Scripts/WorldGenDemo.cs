using UnityEngine;
using System.Collections;

public class WorldGenDemo : MonoBehaviour {
    public GameObject floorTile;
    public GameObject obstacleTile;
    private int width = 40;
    private int height = 20;
    NoiseGenerator ng;

    void Start() {
        ng = transform.GetComponent<NoiseGenerator>();
    }

	public void generateMap() {
        if (transform.childCount > 0) {
            foreach (Transform child in transform) GameObject.Destroy(child.gameObject);
        }
        float[,] noiseMap = ng.generateNoise(width + 1, height + 1);
        for (int i = 0; i <= width; i++) {
            for (int j = 0; j <= height; j++) {
                float tempVal = noiseMap[i, j];
                if (tempVal < 0.6f) {
                    GameObject tempFloorTile = (GameObject)Instantiate(floorTile, new Vector3(i - width / 2, 0, j - height / 2), Quaternion.identity);
                    tempFloorTile.transform.SetParent(transform); 
                }
                else {
                    GameObject tempObstacleTile = (GameObject)Instantiate(obstacleTile, new Vector3(i - width / 2, 0, j - height / 2), Quaternion.identity);
                    tempObstacleTile.transform.SetParent(transform);
                }
            }
        }
    }

}
