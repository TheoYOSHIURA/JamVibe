
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;



public class RoomLogic : MonoBehaviour
{
    [SerializeField] private Event _currentEvent;
    [SerializeField] private AudioClip _diceSound;
    private int _diceRoll;
    private bool _leftInputChosen = false;
    private bool _rightInputChosen = false;
    private AudioSource _currentAudioSource;
    private CombatLogic _combatLogic = null;

    private bool _leftChoiceHappened = false;
    private bool _rightChoiceHappened = false;

    private bool _eventFinished = false;


    void Start()
    {
        _currentAudioSource = GetComponent<AudioSource>();
        _combatLogic = GetComponent<CombatLogic>();
    }

    void Update()
    {

    }
    IEnumerator OnRoomEnter()
    {
        if (_currentEvent != null)
        {
            CharaController.Instance.CantMove = true;
            Debug.Log("Entered a event room");
            _currentAudioSource.PlayOneShot(_currentEvent.Description);
            yield return StartCoroutine(WaitForSound(_currentAudioSource));

            _currentAudioSource.panStereo = -1f;
            _currentAudioSource.PlayOneShot(_currentEvent.ChoixA);
            yield return StartCoroutine(WaitForSound(_currentAudioSource));

            _currentAudioSource.panStereo = 1f;
            _currentAudioSource.PlayOneShot(_currentEvent.ChoixB);
            yield return StartCoroutine(WaitForSound(_currentAudioSource));

            _currentAudioSource.panStereo = 0f;

            Debug.Log("Wainting for choice");
            yield return StartCoroutine(WaitForChoice());

            switch (_currentEvent.EventType)
            {
                case Event.EEventType.Base:
                    if (_rightChoiceHappened)
                    {
                        if (RollDice(6))
                        {
                            StartCoroutine(OnsuccessChoice(true));
                        }
                        else
                        {
                            StartCoroutine(OnfailChoice(true));
                        }
                    }
                    else
                    {
                        if (RollDice(6))
                        {
                            StartCoroutine(OnsuccessChoice(false));
                        }
                        else
                        {
                            StartCoroutine(OnfailChoice(false));
                        }
                    }
                    break;

                case Event.EEventType.Fontaine:
                    if (_rightChoiceHappened)
                    {
                        if (CharaController.Instance.Gold > 0)
                        {
                            CharaController.Instance.Gold--;
                            StartCoroutine(OnsuccessChoice(true)); // ON DEVRAIT PAS PROPOSER LE CHOIX SI LE JOUEUR A PAS ASSEZ D'ARGENT
                        }
                    }
                    else
                    {
                        StartCoroutine(OnsuccessChoice(false));
                        CharaController.Instance.MoveBackwards();
                    }
                    break;

                case Event.EEventType.Trésor:
                    if (_rightChoiceHappened)
                    {
                        if (RollDice(6, 3))
                        {
                            StartCoroutine(OnsuccessChoice(true));
                        }
                        else
                        {
                            StartCoroutine(OnsuccessChoice(false));
                        }
                    }
                    else
                    {
                        CharaController.Instance.MoveBackwards();
                        StartCoroutine(OnsuccessChoice(false));
                    }
                    break;

                case Event.EEventType.Combat:
                    _combatLogic.StartCombat();
                    break;
            }
        }

        Debug.Log("Event finished");
        _eventFinished = true;
        CharaController.Instance.CantMove = false;
        yield return null;
    }

    private void OnLeftChoice()
    {
        _leftChoiceHappened = true;
    }

    private void OnRightChoice()
    {
        _rightChoiceHappened = true;
    }

    IEnumerator OnsuccessChoice(bool right)
    {
        Debug.Log("Success");
        if (!right)
        {
            _currentAudioSource.PlayOneShot(_currentEvent.Result1ChoixA);
            StartCoroutine(WaitForSound(_currentAudioSource));
            ApplyReward(_currentEvent.Reward1A);

        }
        else
        {
            _currentAudioSource.PlayOneShot(_currentEvent.Result1ChoixB);
            StartCoroutine(WaitForSound(_currentAudioSource));
            ApplyReward(_currentEvent.Reward1B);
        }
        yield return null;
    }

    IEnumerator OnfailChoice(bool right)
    {
        Debug.Log("Fail");
        if (!right)
        {
            _currentAudioSource.PlayOneShot(_currentEvent.Result2ChoixA);
            StartCoroutine(WaitForSound(_currentAudioSource));
            ApplyReward(_currentEvent.Reward2A);
        }
        else
        {
            _currentAudioSource.PlayOneShot(_currentEvent.Result2ChoixB);
            StartCoroutine(WaitForSound(_currentAudioSource));
            ApplyReward(_currentEvent.Reward2B);
        }
        yield return null;
    }

    IEnumerator WaitForSound(AudioSource other)
    {
        //Wait Until Sound has finished playing
        while (_currentAudioSource.isPlaying)
        {
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_eventFinished && _currentEvent != null)
        {
            StartCoroutine(OnRoomEnter());
        }
    }

    void OnTriggerExit(Collider other)
    {
        _currentAudioSource.Stop();
    }

    void ApplyReward(Reward reward)
    {
        CharaController.Instance.Hp += reward.Heal;
        CharaController.Instance.Hp -= reward.Damage;
        CharaController.Instance.Gold += reward.Gold;
        
        if (CharaController.Instance.Weapon != null)
        {
            if (CharaController.Instance.Weapon.Strength < reward.Weapon.Strength)
            {
                CharaController.Instance.EquipItem(reward.Weapon, null);
            }
        }
        else
        {
            CharaController.Instance.EquipItem(reward.Weapon, null);
        }

        if (CharaController.Instance.Armor != null)
        {
            if (CharaController.Instance.Armor.ArmorClass < reward.Armor.ArmorClass)
            {
                CharaController.Instance.EquipItem(null, reward.Armor);
            }
        }
        else
        {
            CharaController.Instance.EquipItem(null, reward.Armor);
        }
    }

    private IEnumerator WaitForChoice()
    {
        VibrationController.Instance.OnConfirmLeft += OnLeftChoice;
        VibrationController.Instance.OnConfirmRight += OnRightChoice;

        yield return new WaitUntil(() => _leftChoiceHappened || _rightChoiceHappened);

        VibrationController.Instance.OnConfirmLeft -= OnLeftChoice;
        VibrationController.Instance.OnConfirmRight -= OnRightChoice;
    }

    private bool RollDice(int maxValue, int successOn = 4)
    {
        _currentAudioSource.PlayOneShot(_diceSound);
        StartCoroutine(WaitForSound(_currentAudioSource));
        int random = UnityEngine.Random.Range(1, maxValue + 1);
        VibrationController.Instance.RumbleXTime(random);
        return random >= successOn;    }
}
