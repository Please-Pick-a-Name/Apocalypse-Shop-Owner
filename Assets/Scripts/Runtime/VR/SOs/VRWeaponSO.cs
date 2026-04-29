using UnityEngine;

[CreateAssetMenu(fileName = "VRWeaponSO", menuName = "Scriptable Objects/VRWeaponSO")]
public class VRWeaponSO : ScriptableObject
{    
    public ProjectileOptions projectileOptions;
    public ProjectileVisualOptions visualOptions;
    public float roundsPerMinute;
    public AudioClip gunFireSFX;
    public AudioClip gunDryAmmoSFX;
    public bool fireMode;// 0 for semi, 1 for full auto
}
