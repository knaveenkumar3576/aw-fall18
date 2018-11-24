using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;
    
    private Rigidbody rb;

    //private int expectedResult;

    //private int touchedResult;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        //expectedResult = 0;
    } 

    void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
    
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * 10.0f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("rolloverplane")) {
            Console.WriteLine("COllided with rollover plane");

            //TextMeshPro textMeshPro = 
            //int childCount = collision.gameObject;
            //Console.WriteLine("Child Count: " + childCount);
            //.gameObject.GetComponent<TextMeshPro>();

            //string textMeshString = textMeshPro.text;

            //Console.WriteLine("Text: " + textMeshString);
            //collision.gameObject.

            Destroy(collision.gameObject);

        }
    }
    
}
