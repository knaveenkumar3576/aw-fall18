using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCollectorController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        Vector3 temp = player.transform.position;
        temp.x = 0.0f;
        transform.position = temp + offset;
    }


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnCollisionEnterCollector" + other.gameObject);

        if (other.gameObject.CompareTag("rollplane") || other.gameObject.CompareTag("answerboard"))
        {
            Destroy(other.gameObject);
        }
    }
}