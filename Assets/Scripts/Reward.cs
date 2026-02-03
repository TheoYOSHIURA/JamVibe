using UnityEngine;

[CreateAssetMenu(fileName = "Reward", menuName = "Scriptable Objects/Reward")]
public class Reward : ScriptableObject
{
    [SerializeField] private int _damage;
    [SerializeField] private int _heal;
    [SerializeField] private int _gold;
    [SerializeField] private Armor _armor;
    [SerializeField] private Weapon _weapon;
    



     public int Damage => _damage;
    public int Heal => _heal;
    public Armor Armor => _armor;
    public int Gold => _gold;

    public Weapon Weapon { get => _weapon; set => _weapon = value; }
}
