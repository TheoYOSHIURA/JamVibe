using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : Item
{
    [SerializeField] private int _strength = 0;

    public int Strength { get => _strength; set => _strength = value; }
}

[CreateAssetMenu(fileName = "Armor", menuName = "Scriptable Objects/Armor")]
public class Armor : Item
{
    [SerializeField] private int _armorClass = 0;

    public int ArmorClass { get => _armorClass; set => _armorClass = value; }
}
