using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunShopManager : MonoBehaviour {
    public static GunShopManager Instance { get; private set; }

    [Serializable]
    public class GunShopItem {
        public int cost;
        public GameObject gameObject;
        public string displayName;
    }

    public GameObject shopUIGroup;
    public GameObject shoppingListGroup;
    public GameObject shopItemUiPrefab;
    public List<GunShopItem> gunShopItems;
    public List<GameObject> itemUIs = new();

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    void UpdateShopItemUI(GameObject UI, GunShopItem item) {
        var textMeshProUGUIs = UI.GetComponentsInChildren<TextMeshProUGUI>();
        var images = UI.GetComponentsInChildren<Image>();
        var buttons = UI.GetComponentsInChildren<Button>();
        
        var displayNameUI = textMeshProUGUIs[0];
        var costUI = textMeshProUGUIs[1];
        var iconUI = images[0];

        displayNameUI.text = item.displayName;
        //iconUI
        costUI.text = $"${item.cost}";

        buttons[0].onClick.AddListener(() => {
            if (CurrencyManager.Instance.RemoveCurrency(item.cost)){
                SpawnSomething(item.gameObject);
            } else {
                // some not enough money feedback here, or disable buy button entirely in Update()
            }
        });

    }
    void OnValidate() {
        int i = 0;
        for (; i < itemUIs.Count; i++){
            if (i >= gunShopItems.Count) {
                for (int j = i; j < itemUIs.Count; j++){
                    StartCoroutine(Destroy(itemUIs[j]));
                }
                itemUIs.RemoveRange(i, itemUIs.Count - 1);
                break;
            }
            UpdateShopItemUI(itemUIs[i], gunShopItems[i]);
        }
        for (; i < gunShopItems.Count; i++){
            var item = gunShopItems[i];
            itemUIs.Add(Instantiate(shopItemUiPrefab, shoppingListGroup.transform, false));
            itemUIs[i].transform.localPosition = new(0, i*-64, 0);
            UpdateShopItemUI(itemUIs[i], item);
        }
    }
    IEnumerator Destroy(GameObject go) {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
    }

    public void SpawnSomething(GameObject toSpawn) {
        var gameObject = Instantiate(toSpawn);
        gameObject.transform.position = new Vector3(-9.867f, 1.084f, -4.805f);
    }

    /* void Update() {
        for (int i = 0; i < itemUIs.Count; i++){
            var itemUI = itemUIs[i];
            if itemUI.GetComponentInChildren<Button>().
        }
    } */

}
