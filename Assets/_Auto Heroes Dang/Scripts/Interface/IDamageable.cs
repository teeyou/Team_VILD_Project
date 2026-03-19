
using System.Transactions;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int damage, Transform target);
}
