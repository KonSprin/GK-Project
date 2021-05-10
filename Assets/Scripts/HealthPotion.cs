using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            MonkeyController.currentHealth = maxHealth;
            healthBar.SetHealth(maxHealth);
            Destroy(gameObject);
        }
    }
}
