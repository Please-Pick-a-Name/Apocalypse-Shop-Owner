using UnityEngine;
using UnityEngine.UI;
public class BarricadeHealth : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    [SerializeField] public Slider slider;
    void Start()
    {
        health = maxHealth;
        UpdateHealthBar(health, maxHealth);
    }
    void Update()
    {
        slider.transform.rotation = Camera.main.transform.rotation;
    }
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        UpdateHealthBar(health, maxHealth);
        if (health <= 0)
        {

        }
    }
}
