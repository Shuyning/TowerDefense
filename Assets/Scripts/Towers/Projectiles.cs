using UnityEngine;

public enum projectileType
{
    rock, arrow, fireball
};

public class Projectiles : MonoBehaviour
{
    [SerializeField] float attackDamage;
    [SerializeField] projectileType pType;

    public float AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }

    public projectileType PType
    {
        get
        {
            return pType;
        }
    }
}
