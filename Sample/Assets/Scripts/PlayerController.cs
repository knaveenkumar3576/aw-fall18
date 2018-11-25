using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;
    
    private Rigidbody rb;

    private Stack operands;
    private int expectedResult;
    private int touchedResult;

    private string[] operatorList = new string[4] {"+", "-", "*", "/",};
    
    //TextMeshPro textMeshPro; 

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        operands = new Stack();
        //expectedResult = 0;
    } 

    void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * 10.0f);
    }

    public string GetRollOverValue(Collision collision)
    {
        GameObject originalGameObject = Instantiate(collision.gameObject);
        GameObject childObject = originalGameObject.transform.GetChild(0).gameObject;
        TextMeshPro textMeshPro = (TextMeshPro)childObject.GetComponent("TextMeshPro");

        Destroy(collision.gameObject);
        Destroy(originalGameObject);
        return (string)textMeshPro.text;
    }

    public int GetExpectedResult()
    {
        int firstOperand = Convert.ToInt32(operands.Pop());
        string operatorSelected = (string) operands.Pop();
        int secondOperand = Convert.ToInt32(operands.Pop());

        int result=0;

        switch (operatorSelected)
        {
            case "+":
                result = firstOperand + secondOperand;
                break;

            case "-":
                result = firstOperand - secondOperand;
                break;

            case "x":
                result = firstOperand * secondOperand;
                break;

            case "/":
                result = firstOperand / secondOperand;
                break;

            default:
                Console.WriteLine("Invalid operator");
                break;
        }

        return result;
    }

    public void OnCollisionEnter(Collision collision)
    {
        string value;

        if (collision.gameObject.CompareTag("rolloverplane")) {

            value = GetRollOverValue(collision);

            Debug.Log("Collected" + value);

            operands.Push(value);
        }


        if (collision.gameObject.CompareTag("answerboard"))
        {
            value = GetRollOverValue(collision);

            if (operands.Count == 3)
            {
                int chosenResult = Convert.ToInt32(value);
                int result = GetExpectedResult();
                if (chosenResult == result)
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
    
}
