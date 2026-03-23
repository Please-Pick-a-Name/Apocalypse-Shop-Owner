using UnityEngine;
using UnityEngine.UI;
public class BarricadeHealth : MonoBehaviour
{
    public float health = 75f;
    public float maxHealth = 100f;
    [SerializeField] public Slider slider;

    public GameObject[] visualStates;
    void Start()
    {
        UpdateVisuals();
    }
    void Update()
    {
        if (slider != null && Camera.main != null)
            slider.transform.rotation = Camera.main.transform.rotation;
        UpdateVisuals();
    }
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        UpdateVisuals();
    }
    public void Heal(float healAmount) {
        health = Mathf.Min(maxHealth, health + healAmount);
        UpdateVisuals();
    }
    private void UpdateVisuals() {
        if (slider != null) slider.value = health/maxHealth;

        float ratio = health / maxHealth;
        int stateIndex = Mathf.FloorToInt(ratio * 4);
        stateIndex = Mathf.Clamp(stateIndex, 0, 3);

        if (stateIndex > 3) stateIndex = 3;
        if (stateIndex < 0) stateIndex = 0;

        foreach (GameObject obj in visualStates)
        {
            if (obj != null) obj.SetActive(false);
        }
        
        for (int i = 0; i < visualStates.Length; i++)
        {
            if (visualStates[stateIndex] != null)
            {
                visualStates[stateIndex].SetActive(true);
            }
        }

    }
}
