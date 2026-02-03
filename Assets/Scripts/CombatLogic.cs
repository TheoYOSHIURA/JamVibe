using System.Collections;
using UnityEngine;

public class CombatLogic : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int _enemyHP = 10;
    [SerializeField] private int _enemyArmorClass = 5;
    [SerializeField] private int _enemyStrength = 1;

    private bool playerTurn = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayerAttack()
    {
        int attackRoll = Random.Range(1, 7);
        attackRoll += CharaController.Instance.Strength;

        if (attackRoll >= _enemyArmorClass)
        {
            int damageRoll = Random.Range(1, 5);
            _enemyHP -= damageRoll;
        }
    }

    private void EnemyAttack()
    {
        int attackRoll = Random.Range(1, 7);
        attackRoll += _enemyStrength;

        if (attackRoll >= CharaController.Instance.ArmorClass)
        {
            int damageRoll = Random.Range(1, 5);
            CharaController.Instance.Hp -= damageRoll;
        }
    }
}
