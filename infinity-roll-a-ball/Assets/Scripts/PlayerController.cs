using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static BridgeManager;

public class PlayerController : MonoBehaviour {

    private float[] thresholdSpeed = new float[2] {14, 25};
    private float[] thresholdNumberComplexity = new float[2] {1, 10};
    private float[] thresholdWallHeightScaleY = new float[2] { 0.2f, 1.0f };
    private float[] thresholdPlaneLengthScaleZ = new float[2] { 0.5f, 1.0f };

    // Use this for initialization
    public float speed;

    public Text TotalPoints;
    public Text Calculation;
    public Text Level;
    public Text Display;
    public Text GameOver;
    public GameObject Menu;
    //public Text TotalPointsText;
    public int totalPoints;
    public int level;
    private string operation;

    DateTime startTime;

    private int calculationClear;

    private Stack operands;

    private int expectedResultIndex;
    private int expectedResult;

    SqlliteSetup sqlclient;

    private string[] operatorList = new string[4] { "+", "-", "x", "/", };

    private Vector2 touchStart, touchEnd;

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
        GameOver.text = "Game Over!";
        sqlclient = new SqlliteSetup();

        Menu.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        // Swipe start
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchStart = Input.GetTouch(0).position;
        }
        // Swipe end
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            touchEnd = Input.GetTouch(0).position;
            float cameraFacing = Camera.main.transform.eulerAngles.y;
            Vector2 swipeVector = touchEnd - touchStart;
            Vector3 inputVector = new Vector3(swipeVector.x, 0.0f, swipeVector.y);
            Vector3 movement = Quaternion.Euler(0.0f, cameraFacing, 0.0f) * Vector3.Normalize(inputVector);
            rb.velocity = movement * speed/1.50f;
        }
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
        operation = (string)operands.Pop();
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

        Debug.Log("All operands" + BridgeManager.allOperands.Count);

        for (int i = 0; i < 4; i++)
        {
            if (i == expectedResultIndex)
                continue;
            int randomOperandIndex1 = random.Next(8);
            int randomOperandIndex2 = random.Next(8);
            int randomOperatorIndex = random.Next(4);

            Debug.Log("Operand1 " + BridgeManager.allOperands[randomOperandIndex1]);
            Debug.Log("Operand2 " + BridgeManager.allOperands[randomOperandIndex2]);
            Debug.Log("Operator" +  operatorList[randomOperatorIndex]);

            answerOptions[i] = ComputeResult(BridgeManager.allOperands[randomOperandIndex1], BridgeManager.allOperands[randomOperandIndex2], operatorList[randomOperatorIndex]);
            Debug.Log("Generated Option " + answerOptions[i]);
        }

        BridgeManager.allOperands.RemoveRange(0, 8);

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

    public void clearDisplay() {
        Display.text = "";
    }

    public void HideMenu()
    {
        Menu.SetActive(false);
        //.alpha = 0f; //this makes everything transparent
        //canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
    }

    public void ShowMenu()
    {
        Menu.SetActive(true);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public int UpdateTime()
    {
        DateTime oldStartTime = startTime;

        startTime = DateTime.Now;

        TimeSpan currentSpan = startTime - oldStartTime;

        Debug.Log("Time Elapsed" + currentSpan);

        int milliseconds = currentSpan.Milliseconds;

        return milliseconds;
    }

    
    public void ModifyNumbers(int rate)
    {
        int newComplexity = BridgeManager.numComplexity + rate;

        if (newComplexity >= thresholdNumberComplexity[0] && newComplexity <= thresholdNumberComplexity[1])
        {
            BridgeManager.numComplexity = newComplexity;
        }

    }

    public void ModifyWallHeightScale(float rate)
    {
        float newWallHeightScaleY = BridgeManager.wallHeightScaleY + rate;

        if (newWallHeightScaleY >= thresholdWallHeightScaleY[0] && newWallHeightScaleY <= thresholdWallHeightScaleY[1])
        {
            BridgeManager.wallHeightScaleY = newWallHeightScaleY;
        }
    }

    public void ModifyPlaneScale(float rate)
    {
        float newPlaneLengthScaleZ = BridgeManager.planeLengthScaleZ + rate;

        if (newPlaneLengthScaleZ >= thresholdPlaneLengthScaleZ[0] && newPlaneLengthScaleZ <= thresholdPlaneLengthScaleZ[1])
        {
            BridgeManager.planeLengthScaleZ = newPlaneLengthScaleZ;
        }
        
    }

    public void ModifySpeed(float rate)
    {
        float newSpeed = speed + speed / rate;

        if(newSpeed >= thresholdSpeed[0] && newSpeed <= thresholdSpeed[1])
        {
            speed = newSpeed;
        }
    }

    public void IncreaseGameplayDifficulty()
    {

        System.Random random = new System.Random();
        int difficultyIndex = random.Next(0, 4);

        switch (difficultyIndex)
        {
            case 0:
                ModifySpeed(4);
                Display.text += " Speed up";
                break;

            case 1:
                ModifyPlaneScale(-0.2f);
                Display.text += " Planes squeezed";
                break;

            case 2:
                ModifyNumbers(1);
                Display.text += " Big Numbers coming up";
                break;

            case 3:
                ModifyWallHeightScale(-0.1f);
                Display.text += " Walls Lowered";
                break;

            default:
                break;
        }
    }


    public void DecreaseGameplayDifficulty()
    {
        System.Random random = new System.Random();
        int difficultyIndex = random.Next(0, 4);

        switch (difficultyIndex)
        {
            case 0:
                ModifySpeed(-8);
                Display.text += " Speed down";
                break;
            case 1:
                ModifyPlaneScale(0.2f);
                Display.text += " Planes stretched";
                break;

            case 2:
                ModifyNumbers(-1);
                Display.text += " Small Numbers coming up";
                break;

            case 3:
                ModifyWallHeightScale(0.1f);
                Display.text += " Walls Raised";
                break;

            default:
                break;
        }
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

            Calculation.text += (value + " ");
            calculationClear++;
            if (calculationClear == 3)
                Calculation.text += "= ?";

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
                Display.color = Color.green;
                Display.text = "Right Answer!";
                Invoke("clearDisplay", 1.5f);
                Debug.Log("Win");

                // increasing the difficulty
                IncreaseGameplayDifficulty();

                //int time = UpdateTime();
                //sqlclient.insertData(level, "", operation, time, "win");
                //sqlclient.getData(operation);
            }
            else
            {
                totalPoints = totalPoints - 1;
                TotalPoints.text = totalPoints.ToString();
                if (totalPoints < 0) {
                    ShowMenu();
                    speed = 0;
                    Debug.Log("Game Over!");
                }
                Display.color = Color.red;
                Display.text = "Wrong Answer!";
                Invoke("clearDisplay", 1.5f);
                Debug.Log("Lose");

                // increasing the difficulty
                DecreaseGameplayDifficulty();

                //int time = UpdateTime();
                //sqlclient.insertData(level, "", operation, time, "lose");
                //sqlclient.getData(operation);
            }

            GameObject[] answerboards = GameObject.FindGameObjectsWithTag("answerboard");
            for (int i = 0; i < answerboards.Length; i++) {
                Destroy(answerboards[i]);
            }

            calculationClear = 0;
            Calculation.text = "";
        }

    }

    //void FixedUpdate() {
    //    if (SystemInfo.deviceType == DeviceType.Desktop)
    //    {
    //        // Player movement in desktop devices
    //        // Definition of force vector X and Y components
    //        float moveHorizontal = Input.GetAxis("Horizontal");
    //        float moveVertical = Input.GetAxis("Vertical");
    //        // Building of force vector
    //        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
    //        // Adding force to rigidbody
    //        rb.AddForce(movement * speed);
    //    }
    //    else
    //    {
    //        // Player movement in mobile devices
    //        // Building of force vector 
    //        Vector3 movement = new Vector3(Input.acceleration.x, 0.0f, Input.acceleration.y);
    //        // Adding force to rigidbody
    //        rb.AddForce(movement * speed);
    //    }
    //}
}
