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
    //public static List<int> operandList1;
    //public static List<int> operandList2;
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
                num = random.Next((int)Math.Pow(10, numDigits - 1), (int)Math.Pow(10,numDigits));
            allOperands.Add(num);
            numberList.Add(num);
            //Debug.Log(numberList[i]);
        }
        return numberList;
    }


    void Start () {
        bridgeCount = 0;
        allOperands = new List<int>();
        //operandList1 = new List<int>();
        //operandList2 = new List<int>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        for (int i = 0; i < bridgeAmount; i++){
            SpawnBridge();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (playerTransform.position.z > (spawnZ - bridgeAmount * bridgeLength))
        {
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

        Debug.Log("Rollplanes count: " + rollPlanes.Count);

        return rollPlanes;
    }

    void GenerateAndSetSelectionNumbers(GameObject bridgeObject)
    {
        List<int> numbers = generateNumbers();
        List<GameObject> rollPlanes = FindRollPlanes(bridgeObject.transform, "rollplane");
        
        //if (bridgeCount == 1) {
        //    operandList1 = new List<int>(numbers);
        //}
        //else if (bridgeCount == 3) {
        //    operandList2 = new List<int>(numbers);
        //}

        for (int i = 0; i < 4; i++)
        {
            Debug.Log(rollPlanes[i]);
            Debug.Log(rollPlanes[i].transform.childCount);
            GameObject childObject = rollPlanes[i].transform.GetChild(0).gameObject;
            TextMeshPro textMeshPro = (TextMeshPro)childObject.GetComponent("TextMeshPro");
            textMeshPro.SetText(numbers[i].ToString());
        }
    }


    void SetSelectionOperators(GameObject bridgeObject)
    {
        List<GameObject> rollPlanes = FindRollPlanes(bridgeObject.transform, "rollplane");

        for (int i = 0; i < 4; i++)
        {
            Debug.Log(rollPlanes[i]);
            Debug.Log(rollPlanes[i].transform.childCount);
            GameObject childObject = rollPlanes[i].transform.GetChild(0).gameObject;
            TextMeshPro textMeshPro = (TextMeshPro)childObject.GetComponent("TextMeshPro");
            textMeshPro.SetText(operators[i]);
        }
    }
    void GeneratSelectionAnswers(GameObject bridgeObject)
    {
        List<GameObject> rollPlanes = FindRollPlanes(bridgeObject.transform, "rollplane");
        for (int i = 0; i < 4; i++)
        {
            rollPlanes[i].tag = "answerboard";
            Renderer[] rend = rollPlanes[i].GetComponentsInChildren<Renderer>();
            rend[0].material.shader = Shader.Find("_Color");
            rend[0].material.SetColor("_Color", Color.green);
            rend[0].material.shader = Shader.Find("Specular");
            rend[0].material.SetColor("_SpecColor", Color.red);
        }
    }

    void SpawnBridge(int prefabIndex = -1){

        GameObject bridgeObject;
        bridgeObject = Instantiate(bridgePreFabs[0]) as GameObject;

        bridgeCount++;

        if (bridgeCount == 1 || bridgeCount == 3)
        {
            GenerateAndSetSelectionNumbers(bridgeObject);
        }
        else if (bridgeCount == 2) {
            SetSelectionOperators(bridgeObject);
        }

        else if (bridgeCount == 4) {
            GeneratSelectionAnswers(bridgeObject);
            bridgeCount = 0;
        }

        Debug.Log("Bridge count: " + bridgeCount);

        bridgeObject.transform.SetParent(transform);
        bridgeObject.transform.position = Vector3.forward * spawnZ;
        spawnZ += bridgeLength;
         
    }
}
