using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CombatLogic : MonoBehaviour
{
  
    [Header("Enemy Stats")]
    [SerializeField] private Monster _monster;

    [Header("Combat System")]
    private bool _playerHasflee = false;

    [Header("Listen System")]
    private bool _attackEventHappened;
    private bool _fleeEventHappened;

     private AudioSource _currentAudioSource;
    [SerializeField] private AudioClip _damagePlayer;
    [SerializeField] private AudioClip _deathPlayer;
    [SerializeField] private AudioClip _damageMonster;
    [SerializeField] private AudioClip _deathMonster;
    [SerializeField] private AudioClip _runAway;

    void Start()
    {
        _currentAudioSource = GetComponent<AudioSource>();
        //For testing purpose
        //StartCoroutine(CombatSystem());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void StartCombat()
    {
        StartCoroutine(CombatSystem());
    }

    private void PlayerAttack()
    {
        int attackRoll = UnityEngine.Random.Range(1, 7);
        attackRoll += CharaController.Instance.Strength;

        Debug.Log("Player rolled a " + attackRoll + " on atk roll");

        if (attackRoll >= _monster.ArmorClass)
        {
            int damageRoll = UnityEngine.Random.Range(1, 5);
            _monster.Hp -= damageRoll;
            Debug.Log("Player rolled a " + damageRoll + " on damage roll");
            _currentAudioSource.PlayOneShot(_damageMonster);
        }
        else
        {
            Debug.Log("Armor class is too high!");
        }
    }

    private void EnemyAttack()
    {
        int attackRoll = UnityEngine.Random.Range(1, 7);
        attackRoll += _monster.Strength;

        Debug.Log("Enemy rolled a " + attackRoll + " on atk roll");

        if (attackRoll >= CharaController.Instance.ArmorClass)
        {
            int damageRoll = UnityEngine.Random.Range(1, 5);
            CharaController.Instance.Hp -= damageRoll;
            Debug.Log("Enemy rolled a " + damageRoll + " on damage roll");
            _currentAudioSource.PlayOneShot(_damagePlayer);
        }
        else
        {
            Debug.Log("Armor class is too high!");
        }
    }

    private void PlayerFlee()
    {
        Debug.Log("Le joueur s'est enfui");
        CharaController.Instance.MoveBackwards();
        CharaController.Instance.ForceRotate();
        CharaController.Instance.ForceRotate();
        _currentAudioSource.PlayOneShot(_runAway);
        
    }

    private IEnumerator CombatSystem()
    {
        CharaController.Instance.CantMove = true;
        // Description du monstre
        // --
        Debug.Log("Description du monstre");

        // Boucle de combat
        while (CharaController.Instance.Hp > 0 && _monster.Hp > 0 && !_playerHasflee)
        {
            // Choix attaque ou fuite
            Debug.Log("Waiting for choice");
            yield return StartCoroutine(WaitForChoice());
            yield return new WaitForSeconds(1);

            if (_playerHasflee)
            {
                PlayerFlee();
                yield break;
            }
            if (_monster.Hp > 0)
            {
                EnemyAttack();
                yield return new WaitForSeconds(1);

            }
        }

        if (CharaController.Instance.Hp < 0)
        {
            _currentAudioSource.PlayOneShot(_deathPlayer);
            Debug.Log("Player died");
        }

        if (_monster.Hp < 0)
        {
            _currentAudioSource.PlayOneShot(_deathMonster);
            Debug.Log("Enemy died");
        }
    }

    //Event subscriber that sets the flag
    private void OnAttack()
    {
        _attackEventHappened = true;
    }

    private void OnFlee()
    {
        _fleeEventHappened = true;
    }

    //Coroutine that waits until the flag is set
    private IEnumerator WaitForChoice()
    {
        VibrationController.Instance.OnConfirmLeft += OnFlee;
        VibrationController.Instance.OnConfirmRight += OnAttack;
        yield return new WaitUntil(() => _attackEventHappened || _fleeEventHappened);

        if (_attackEventHappened)
        {
            _attackEventHappened = false;
            PlayerAttack();
        }
        else
        {
            if (UnityEngine.Random.Range(1, 7) >= 3)
            {
                _playerHasflee = true;
                Debug.Log("Fuite !");
            }
            else
            {
                Debug.Log("Fuite rat�e");
            }
                _fleeEventHappened = false;
            
        }

        VibrationController.Instance.OnConfirmLeft -= OnFlee;
        VibrationController.Instance.OnConfirmRight -= OnAttack;
    }
}
