using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public GameObject gameObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            MonkeyController.respawnPoint = gameObject.transform.position;
            print(MonkeyController.respawnPoint);
            Destroy(gameObject);
        }
    }
}
