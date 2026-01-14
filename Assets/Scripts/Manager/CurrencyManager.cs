using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    private int currency;
    public TextMeshProUGUI currencyText;
    void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update() {
        currencyText.SetText("$" + currency);
    }
    public void AddCurrency(int amount) {
        currency = currency + amount;
    }

    public bool RemoveCurrency(int amount) {
        if(currency >= amount){
            currency = currency - amount;
            return true;
        } else {
            return false;
        }
    }
}
