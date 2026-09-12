using UnityEngine;

public interface IDamageable
{
    //This class is for any entity that could take damage from something.
    //This includes breakable objects, enemies, spell constructs, anything of the sort.

    //Maybe later on I will add damage types to the game so that resistances can be a thing

    public void ApplyDamage(float damageAmount);
}
