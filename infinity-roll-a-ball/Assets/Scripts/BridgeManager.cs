using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BridgeManager : MonoBehaviour {

    //const int ANSWERBOARD = 2;
    //const int OPERATOR = 1;

    // Use this for initialization
    public GameObject[] bridgePreFabs;

    private Transform playerTransform;
    private float spawnZ = 0.0f;
    private float bridgeLength = 10.0f;
    private int bridgeAmount = 2;

    string[] operators = new string[] {"+", "-", "x", "/"};
    public int bridgeCount;
    public static List<int> operandList1;
    public static List<int> operandList2;
    public static List<int> allOperands;

    System.Random random = new System.Random();
    int numDigits = 1;
    int finalAnswer = 0;

    List<int> generateNumbers() {
        Debug.Log("GeneratingNumbers");

        List<int> numberList = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            int num = 0;
            while (num == 0)
                num = random.Next(10 * (numDigits - 1), (10 * numDigits) - 1);
            allOperands.Add(num);
            numberList.Add(num);
            //Debug.Log(numberList[i]);
        }
        return numberList;
    }


    void Start () {
        bridgeCount = 1;
        allOperands = new List<int>();
        operandList1 = new List<int>();
        operandList2 = new List<int>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        for (int i = 0; i < bridgeAmount; i++){
            SpawnBridge();
        }
       

    }

    public List<GameObject> FindRollPlanes(Transform parent, string tag) {
        List<GameObject> rollPlanes = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            if (child.tag == tag) {
                rollPlanes.Add(child.gameObject);
            }
            if (child.childCount > 0) {
                rollPlanes.AddRange(FindRollPlanes(child, tag));
            }
        }
        return rollPlanes;
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
        // Get all roll planes in the game object
        List<GameObject> rollplanes = FindRollPlanes(go.transform, "rollplane");
        Debug.Log("Rollplanes count: " + rollplanes.Count);

        if (bridgeCount == 1 || bridgeCount == 3)
        {
            List<int> numbers = generateNumbers();
            if (bridgeCount == 1) {
                operandList1 = new List<int>(numbers);
            }
            else if (bridgeCount == 3) {
                operandList2 = new List<int>(numbers);
            }

            for (int i = 0; i < 4; i++)
            {
                Debug.Log(rollplanes[i]);
                Debug.Log(rollplanes[i].transform.childCount);
                GameObject childObject = rollplanes[i].transform.GetChild(0).gameObject;
                TextMeshPro textMeshPro = (TextMeshPro)childObject.GetComponent("TextMeshPro");
                textMeshPro.SetText(numbers[i].ToString());
            }
        }
        else if (bridgeCount == 2) {
            for (int i = 0; i < 4; i++)
            {
                Debug.Log(rollplanes[i]);
                Debug.Log(rollplanes[i].transform.childCount);
                GameObject childObject = rollplanes[i].transform.GetChild(0).gameObject;
                TextMeshPro textMeshPro = (TextMeshPro)childObject.GetComponent("TextMeshPro");
                textMeshPro.SetText(operators[i]);
            }
        }
        else if (bridgeCount == 4) {
            for (int i = 0; i < 4; i++) {
                rollplanes[i].tag = "answerboard";
            }
        }

        if (bridgeCount >= 4)
        {
            bridgeCount = 1;
            //BridgeManager.allOperands.Clear();
        }
        else
            bridgeCount++;
        Debug.Log("Bridge count: " + bridgeCount);
        

        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;
        spawnZ += bridgeLength;
         
    }
}
