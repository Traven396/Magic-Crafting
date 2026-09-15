using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Events;


public enum EntityType
{
    Player,
    Enemy,
    Non_Living
}
[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour, IDamageable
{
    [SerializeField] EntityType _type;
    [SerializeField] bool Invulnerable;
    [HideIf("Invulnerable")][SerializeField] float _maxHealth;
    [HideIf("Invulnerable")][ReadOnly][SerializeField] float _currentHealth;

    Rigidbody _rigidbody;

    public EntityType Type => _type;
    public Rigidbody Rigidbody => _rigidbody;
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;

    public UnityEvent OnDeath;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentHealth = MaxHealth;
    }
    public void ApplyDamage(float damageAmount)
    {
        _currentHealth -= damageAmount;

        if (!Invulnerable && _currentHealth <= 0)
        {
            Debug.Log("Blehhh, we died");
            OnDeath?.Invoke();
        }
        else
        {
            Debug.Log("Ouch! We just took " + damageAmount + " damage!");
        }
    }

}
