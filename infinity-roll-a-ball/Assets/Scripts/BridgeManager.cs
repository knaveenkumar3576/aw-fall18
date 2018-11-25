using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeManager : MonoBehaviour {

    // Use this for initialization
    public GameObject[] bridgePreFabs;

    private Transform playerTransform;
    private float spawnZ = 0.0f;
    private float bridgeLength = 10.0f;
    private int bridgeAmount = 2;

	void Start () {

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        for (int i = 0; i < bridgeAmount; i++){
            SpawnBridge();
        }
       

    }
	
	// Update is called once per frame
	void Update () {
        if(playerTransform.position.z > (spawnZ - bridgeAmount * bridgeLength)){
            SpawnBridge();
        }
	}

    void SpawnBridge(int prefabIndex = -1){

        GameObject go;
        go = Instantiate(bridgePreFabs[0]) as GameObject;
        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;
        spawnZ += bridgeLength;

         
    }
}
