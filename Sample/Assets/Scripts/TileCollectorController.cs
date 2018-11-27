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
        transform.position = player.transform.position + offset;
    }


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnCollisionEnterCollector" + collision.gameObject);

        if (other.gameObject.CompareTag("rolloverplane") || other.gameObject.CompareTag("answerboard"))
        {
            Destroy(other.gameObject);
        }
    }
}