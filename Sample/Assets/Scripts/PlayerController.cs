using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;

    private Rigidbody rb;

    private Stack operands;
    private int expectedResultIndex;

    public GameObject[] allRollPlanes;

    private int[] allOperands = new int[8];
    private string[] operatorList = new string[4] { "+", "-", "x", "/", };

    private int[] answerOptions = new int[4];
    DateTime startTime;

    //TextMeshPro textMeshPro; 

    // Use this for initialization
    void Start() {

        startTime = DateTime.Now;

        rb = GetComponent<Rigidbody>();
        operands = new Stack();
        collectAllPlaneObjects();
    }

    void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * 10.0f);
    }

    public void collectAllPlaneObjects()
    {
        Debug.Log("collectAllPlaneObjects");
        allRollPlanes = GameObject.FindGameObjectsWithTag("rolloverplane");
        int i = 0;
        foreach (GameObject rollPlane in allRollPlanes)
        {
            string operand = GetRollPlaneValue(rollPlane);
            if (!Array.Exists(operatorList, element => element == operand))
            {
                allOperands[i] = Convert.ToInt32(operand);
                Debug.Log(allOperands[i]);
                i++;
            }
        }
    }

    public void GenerateAnswerOptions()
    {
        Debug.Log("Generatint Options");
        System.Random random = new System.Random(); 
        expectedResultIndex = random.Next(4);
        answerOptions[expectedResultIndex] = GetExpectedResult();

        for (int i = 0; i < 4; i++)
        {
            if (i == expectedResultIndex)
                continue;
            int randomOperandIndex1 = random.Next(8);
            int randomOperandIndex2 = random.Next(8);
            int randomOperatorIndex = random.Next(4);

            answerOptions[i] = ComputeResult(allOperands[randomOperandIndex1], allOperands[randomOperandIndex2], operatorList[randomOperatorIndex]);
            Debug.Log("Generated Option " + answerOptions[i]);
            i++;
        }

    }

    public void setAnswerOptionsintoPlane()
    {
        Debug.Log("Setting Options");
        GameObject[] answerBoards = GameObject.FindGameObjectsWithTag("answerboard");
        for (int i = 0; i < answerBoards.Length; i++)
        {
            TextMeshPro textMeshPro = GetTextMeshReference(answerBoards[i]);
            textMeshPro.SetText(answerOptions[i].ToString());
        }
    }

    public TextMeshPro GetTextMeshReference(GameObject gameObject)
    {
        GameObject childObject = gameObject.transform.GetChild(0).gameObject;
        TextMeshPro textMeshPro = (TextMeshPro)childObject.GetComponent("TextMeshPro");
        return textMeshPro;
    }

    public string GetRollPlaneValue(GameObject gameObject)
    {
        TextMeshPro textMeshPro = GetTextMeshReference(gameObject);
        string value = (string)textMeshPro.text;
        return value;
    }

    public string CollectRollOverAndDestroy(Collision collision)
    {
        string value;
        value = GetRollPlaneValue(collision.gameObject);
        Destroy(collision.gameObject);
        return value;
    }

    public int ComputeResult(int operand1, int operand2, string operation)
    {

        int result = 0;

        switch (operation)
        {
            case "+":
                result = operand1 + operand2;
                break;

            case "-":
                result = operand1 - operand2;
                break;

            case "x":
                result = operand1 * operand2;
                break;

            case "/":
                result = operand1 / operand2;
                break;

            default:
                Console.WriteLine("Invalid operator");
                break;
        }
        return result;
    }

    public int GetExpectedResult()
    {
        int operand2 = Convert.ToInt32(operands.Pop());
        string operation = (string) operands.Pop();
        int operand1 = Convert.ToInt32(operands.Pop());

        return ComputeResult(operand1, operand2, operation);
    }


    public void UpdateTime()
    {
        DateTime oldStartTime = startTime;

        startTime = DateTime.Now;

        TimeSpan currentSpan = startTime - oldStartTime;

        Debug.Log("Time Elapsed" + currentSpan);
    }


    public void OnCollisionEnter(Collision collision)
    {

        string value;

        if (collision.gameObject.CompareTag("rolloverplane"))
        {
            UpdateTime();

            value = CollectRollOverAndDestroy(collision);

            Debug.Log("Picked the value " + value);

            operands.Push(value);

            if (operands.Count == 3)
            {
                GenerateAnswerOptions();
                setAnswerOptionsintoPlane();
            }
        }

        if (collision.gameObject.CompareTag("answerboard"))
        {
            UpdateTime();

            value = CollectRollOverAndDestroy(collision);

            int chosenResult = Convert.ToInt32(value);
            Debug.Log("Chosen Result " + chosenResult);
            Debug.Log("Correct Result " + answerOptions[expectedResultIndex]);

            if (chosenResult == answerOptions[expectedResultIndex])
            {
                Debug.Log("Win");
            }
            else
            {
                Debug.Log("Lose");
            }
        }
    }
}
