using UnityEngine;

public class Sword : Item
{
    private void Awake()
    {
        IsWeapon = true;
        WeaponDamage = Random.Range(1, 5);
    }

    public override void ItemBehaviour()
    {
        base.ItemBehaviour();
    }
}
