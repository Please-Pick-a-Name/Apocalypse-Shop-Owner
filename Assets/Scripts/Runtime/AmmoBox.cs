using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public Transform player;
    public Gun gun;
    public float interactionDistance = 3.0f;
    public int refillAmount = 60;
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            gun.RefillAmmo(60);
        }
    }
}
