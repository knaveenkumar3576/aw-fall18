﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static BridgeManager;

public class PlayerController : MonoBehaviour {

    // Use this for initialization
    public float speed;

    public Text TotalPoints;
    public Text Calculation;
    public Text Level;
    //public Text TotalPointsText;
    public int totalPoints;
    public int level;


    private int calculationClear;

    private Stack operands;

    private int expectedResultIndex;
    private int expectedResult;

    private string[] operatorList = new string[4] { "+", "-", "x", "/", };

    private int[] answerOptions = new int[4];

    private Rigidbody rb;

	void Start () {
        rb = GetComponent<Rigidbody>();
        operands = new Stack();

        calculationClear = 0; 
        level = 1;
        totalPoints = 0;
        Level.text = "1";
        TotalPoints.text = "0";
    }

	// Update is called once per frame
	void Update () {
		
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
                Debug.Log("Invalid operator");
                break;
        }
        return result;
    }

    public int GetExpectedResult()
    {
        int operand2 = Convert.ToInt32(operands.Pop());
        string operation = (string)operands.Pop();
        int operand1 = Convert.ToInt32(operands.Pop());

        
        return ComputeResult(operand1, operand2, operation);
    }

    public void GenerateAnswerOptions()
    {
        
        Debug.Log("Generatint Options");
        System.Random random = new System.Random();
        expectedResultIndex = random.Next(0, 4);
        answerOptions[expectedResultIndex] = GetExpectedResult();
        expectedResult = answerOptions[expectedResultIndex];

        Debug.Log(BridgeManager.allOperands);

        for (int i = 0; i < 4; i++)
        {
            if (i == expectedResultIndex)
                continue;
            int randomOperandIndex1 = random.Next(8);
            int randomOperandIndex2 = random.Next(8);
            int randomOperatorIndex = random.Next(4);

            Debug.Log(randomOperandIndex1);
            Debug.Log(randomOperandIndex2);
            Debug.Log(randomOperatorIndex);

            answerOptions[i] = ComputeResult(BridgeManager.allOperands[randomOperandIndex1], BridgeManager.allOperands[randomOperandIndex2], operatorList[randomOperatorIndex]);
            Debug.Log("Generated Option " + answerOptions[i]);
            i++;
        }

    }

    public void setAnswerOptionsintoPlane()
    {
        Debug.Log("Setting Options");
        GameObject[] answerBoards = GameObject.FindGameObjectsWithTag("answerboard");
        Debug.Log("Answer baord lenfht:" + answerBoards.Length);
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

    public void OnCollisionEnter(Collision collision)
    {

        string value;

        if (collision.gameObject.CompareTag("rollplane"))
        {
            //UpdateTime();

            value = CollectRollOverAndDestroy(collision);

            Debug.Log("Picked the value " + value);

            operands.Push(value);

            Debug.Log("Operand count: " + operands.Count);

            if (operands.Count == 3)
            {
                GenerateAnswerOptions();
                setAnswerOptionsintoPlane();
            }
        }

        if (collision.gameObject.CompareTag("answerboard"))
        {
            //UpdateTime();

            Debug.Log("Collided with answer board");

            value = CollectRollOverAndDestroy(collision);

            int chosenResult = Convert.ToInt32(value);
            Debug.Log("Chosen Result " + chosenResult);
            Debug.Log("Correct Result " + answerOptions[expectedResultIndex]);

            if (chosenResult == expectedResult)
            {
                totalPoints = totalPoints + 1;
                TotalPoints.text = totalPoints.ToString();
                level = level + 1;
                Level.text = level.ToString();
                Debug.Log("Win");
            }
            else
            {
                totalPoints = totalPoints - 1;
                TotalPoints.text = totalPoints.ToString();
                if (totalPoints < 0) {
                    Debug.Log("Game Over!");
                }
                Debug.Log("Lose");
            }

            GameObject[] answerboards = GameObject.FindGameObjectsWithTag("answerboard");
            for (int i = 0; i < answerboards.Length; i++) {
                Destroy(answerboards[i]);
            }



        }
        
    }


    void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);

    }
}
