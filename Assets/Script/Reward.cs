using UnityEngine;

[CreateAssetMenu(fileName = "Reward", menuName = "Scriptable Objects/Reward")]
public class Reward : ScriptableObject
{
    [SerializeField] private int _damage;
    [SerializeField] private int _heal;
    [SerializeField] private int _gold;



     public int Damage => _damage;
    public int Heal => _heal;
    public int Gold => _gold;

}
