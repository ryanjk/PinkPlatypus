using UnityEngine;
using System.Collections;

public class MerchantController : MonoBehaviour {
    public static int[,] exampleMap()
    {
        int[,] map = new int[50, 50];
        for (int i = 0; i < 8; i++)
            map[3, i] = 1;
        return map;
    }

	void Start () {
        GameObject g = GameObject.FindGameObjectWithTag("Merchant");
        Merchant merchant = (Merchant) g.GetComponent("Merchant");
        merchant.setMap(exampleMap());
        merchant.setDestination(new Vector3(-8, 0, -9));
        Debug.Log(merchant.getPathLength(0));
        merchant.setDestination(new Vector3(-6, 0, -6));
        Debug.Log(merchant.getPathLength(1));
        merchant.finishedSettingDestinationsAndMap = true;
	}
	
}