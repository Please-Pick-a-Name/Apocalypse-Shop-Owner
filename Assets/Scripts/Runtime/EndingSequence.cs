using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class EndingSequence : MonoBehaviour {
    [SerializeField] private AudioClip clip;
    [SerializeField] private Transform source;
    [SerializeField] private float volume = 1f;
    public UnityEvent onEndingSequenceEnd = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    public void TriggerEndingSequence() {
        Debug.Log("starting sequence!");
        StartCoroutine(EndingSequenceCoroutine());
    }
    IEnumerator EndingSequenceCoroutine() {
        Debug.Log("sequence stage 1");
        SoundFXManager.instance.PlaySoundFXClip(clip, source, volume, 0.1f);
        yield return new WaitForSeconds(3);
        Debug.Log("sequence stage 2");
        ZombieSpawner.instance.spawnCount = 0;
        while (!ZombieManager.instance.isZombieClear) {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("sequence stage 3");
        //PlayerController.instance.EnableCursor();
        onEndingSequenceEnd.Invoke();
    }
}
