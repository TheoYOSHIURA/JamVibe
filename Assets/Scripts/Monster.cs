using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Scriptable Objects/Monster")]
public class Monster : ScriptableObject
{
    [SerializeField] private int _hp = 1;
    [SerializeField] private int _armorClass = 0;
    [SerializeField] private int _strength = 0;

    public int Hp { get { return _hp; } set { _hp = value; } }
    public int ArmorClass { get { return _armorClass; } set { _armorClass = value; } }
    public int Strength { get { return _strength; } set { _strength = value; } }
}
