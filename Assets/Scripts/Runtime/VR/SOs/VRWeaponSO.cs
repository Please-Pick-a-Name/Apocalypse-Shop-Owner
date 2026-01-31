using UnityEngine;

[CreateAssetMenu(fileName = "VRWeaponSO", menuName = "Scriptable Objects/VRWeaponSO")]
public class VRWeaponSO : ScriptableObject
{    
    public float roundsPerMinute;
    public float damage;
    public AudioClip gunFireSFX;
    public bool fireMode;// 0 for semi, 1 for full auto
}
