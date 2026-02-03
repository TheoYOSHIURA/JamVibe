
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;



public class RoomLogic : MonoBehaviour
{
    [SerializeField] private Event _currentEvent;
    private int _diceRoll;
    private bool _leftInputChosen = false;
    private bool _rightInputChosen = false;
    private AudioSource _currentAudioSource;
    private CombatLogic _combatLogic = null;

    private bool _leftChoiceHappened = false;
    private bool _rightChoiceHappened = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentAudioSource = GetComponent<AudioSource>();
        _combatLogic = GetComponent<CombatLogic>();
        //ne pas oublier de se subscribe aux touches

    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator OnRoomEnter()
    {
        if (_currentEvent != null)
        {
            _currentAudioSource.PlayOneShot(_currentEvent.Description);
            StartCoroutine(WaitForSound(_currentAudioSource));

            _currentAudioSource.panStereo = -1f;
            _currentAudioSource.PlayOneShot(_currentEvent.ChoixA);
            StartCoroutine(WaitForSound(_currentAudioSource));

            _currentAudioSource.panStereo = 1f;
            _currentAudioSource.PlayOneShot(_currentEvent.ChoixB);
            StartCoroutine(WaitForSound(_currentAudioSource));

            _currentAudioSource.panStereo = 0f;

            switch (_currentEvent.EventType)
            {
                case Event.EEventType.Base:

                    yield return StartCoroutine(WaitForChoice());

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
                    yield return StartCoroutine(WaitForChoice());

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
                    }
                    break;

                case Event.EEventType.Trésor:

                    if (_rightChoiceHappened)
                    {
                        if (RollDice(6))
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
                        StartCoroutine(OnsuccessChoice(false));
                    }
                    break;

                case Event.EEventType.Combat:
                    _combatLogic.StartCombat();
                    break;
            }
        }
        
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
        StartCoroutine(OnRoomEnter());
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
        
        if (CharaController.Instance.Weapon.Strength < reward.Weapon.Strength)
        {
            CharaController.Instance.EquipItem(reward.Weapon, null);
        }

        if (CharaController.Instance.Armor.ArmorClass < reward.Armor.ArmorClass)
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
        int random = UnityEngine.Random.Range(1, maxValue + 1);
        return random >= successOn;
    }
}
