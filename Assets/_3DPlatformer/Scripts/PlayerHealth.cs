using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public float invincibleTime = 1f;

    [SerializeField] private int currentHealth;
    private bool isInvincible;
    private bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible || isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
        else StartCoroutine(Invincibility());
    }

    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        currentHealth = 0;
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        GameManager.Instance.GameOver();
    }
}