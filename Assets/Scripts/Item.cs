using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : Item
{

}

[CreateAssetMenu(fileName = "Armor", menuName = "Scriptable Objects/Armor")]
public class Armor : Item
{

}
