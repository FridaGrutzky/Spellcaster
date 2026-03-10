using UnityEngine;

[System.Serializable]
public class Spell : MonoBehaviour
{
    public string spellName;
    public GameObject spellPrefab;
    public Transform muzzlePoint;
    public float spellPower = 15f;

    public void CastSpell()
    {
        Debug.Log("Cast Spell!");
    }

    public void SetMuzzlePoint(Transform muzzlePoint)
    {
        this.muzzlePoint = muzzlePoint;
    }
}
