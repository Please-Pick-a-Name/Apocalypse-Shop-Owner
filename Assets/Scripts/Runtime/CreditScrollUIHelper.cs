using System;
using System.Collections;
using TMPro;
using UnityEngine;

[Serializable]
public struct CreditStruct {
    [TextArea(1, 10)] // minLines, maxLines
    public string title;
    public CreditStruct[] sections;
}

[RequireComponent(typeof(RectTransform))]
public class CreditScrollUIHelper : MonoBehaviour {
    public GameObject creditGameObject;
    public float cursorY = 0;
    public float scrollSpeed = 5f;
    [SerializeField] private  TMP_FontAsset font;
    [Tooltip("height offset to place the thx for playing text and quit/restart button just in bound")]
    public float halfHeight = 100f;
    public CreditStruct credit;

    [Header("debug")]
    [SerializeField] private float heightY = 0;
    [SerializeField] private float scrollY = 0;
    [SerializeField] private bool scrolling = false;

    public void SetScrolling(bool val) {
        Debug.Log("starting credit scroll");
        scrolling = val;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnValidate() {
        var cursorYStart = cursorY;
        for (int i = 0; i < creditGameObject.transform.childCount; i++) {
            var child = creditGameObject.transform.GetChild(i);
            StartCoroutine(Destroy(child.gameObject));
            print("destoried smth");
        }
        SpawnCredit(credit, 30f);
        heightY = cursorYStart - cursorY + halfHeight;
        cursorY = cursorYStart;
    }
    IEnumerator Destroy(GameObject go) {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
    }

    void SpawnCredit(CreditStruct creditStruct, float fontSize) {
        var textHeight = SpawnTextHelper(creditStruct.title, fontSize, out var tmpText);
        cursorY -= textHeight + fontSize * 0.5f;
        foreach (var section in creditStruct.sections) {
            SpawnCredit(section, fontSize * 0.5f);
        }
        if (creditStruct.sections.Length > 0) {
            cursorY -= fontSize;
        }
    }

    float SpawnTextHelper(string text, float fontSize, out TextMeshProUGUI creditTextTMP) {
        var creditTextGO = new GameObject("credit text", typeof(TextMeshProUGUI));
        creditTextGO.transform.SetParent(creditGameObject.transform, false);
        creditTextGO.transform.localPosition = new(0, cursorY);
        creditTextTMP = creditTextGO.GetComponent<TextMeshProUGUI>();
        creditTextTMP.alignment = TextAlignmentOptions.Top;
        creditTextTMP.text = text;
        creditTextTMP.fontSize = fontSize;
        creditTextTMP.font = font;

        creditTextTMP.ForceMeshUpdate();
        float height = creditTextTMP.preferredHeight;
        return height;
    }

    // Update is called once per frame
    void Update() {
        if (scrolling && scrollY <= heightY) {
            transform.localPosition = new(0, cursorY + (scrollY += Time.deltaTime * scrollSpeed));
        }
    }
}
