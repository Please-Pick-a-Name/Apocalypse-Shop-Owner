using UnityEngine;

public class AmmoController : MonoBehaviour
{
    
    public int ammo;
    public GameObject bullet;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }
    
    public int  getAmmo() {
        return ammo;
    }

    void Update() {
        if (ammo <= 0) {
            bullet.SetActive(false);
        }
    }
}
