
using UnityEngine;
using System.Collections;



public class RoomLogic : MonoBehaviour
{
    [SerializeField] private Event _currentEvent;
    private int _diceRoll;
    private bool _leftInputChosen = false;
    private bool _rightInputChosen = false;
    private AudioSource _currentAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator OnRoomEnter()
    {
        _currentAudioSource.panStereo = -1f;
        _currentAudioSource.PlayOneShot(_currentEvent.ChoixA);
        StartCoroutine(waitForSound(_currentAudioSource));
        _currentAudioSource.panStereo = 1f;
        _currentAudioSource.PlayOneShot(_currentEvent.ChoixB);
        StartCoroutine(waitForSound(_currentAudioSource));
        _currentAudioSource.panStereo = 0f;
        yield return null;
    }

    void OnInputLeftChoice()
    {
        _leftInputChosen = true;
        _diceRoll = Random.Range(1, 7);
        //rumbleXtime(_diceRoll);
        if (_diceRoll >= 4)
        {
            StartCoroutine(OnsuccessChoice());
        }
        else
        {
            StartCoroutine(OnfailChoice());
        }

    }

    void OnInputRightChoice()
    {
        _rightInputChosen = true;
        _diceRoll = Random.Range(1, 7);
        //rumbleXtime(_diceRoll);
        if (_diceRoll >= 4)
        {
            StartCoroutine(OnsuccessChoice());
        }
        else
        {
            StartCoroutine(OnfailChoice());
        }
    }

    IEnumerator OnsuccessChoice()
    {
        if (_leftInputChosen)
        {
            _currentAudioSource.PlayOneShot(_currentEvent.Result1ChoixA);
            StartCoroutine(waitForSound(_currentAudioSource));
            //_currentEvent.Reward1A

        }
        else if (_rightInputChosen)
        {
            _currentAudioSource.PlayOneShot(_currentEvent.Result1ChoixB);
            StartCoroutine(waitForSound(_currentAudioSource));
            //_currentEvent.Reward1B
        }
        yield return null;
    }

    IEnumerator OnfailChoice()
    {
        if (_leftInputChosen)
        {
            _currentAudioSource.PlayOneShot(_currentEvent.Result2ChoixA);
            StartCoroutine(waitForSound(_currentAudioSource));
            //_currentEvent.Reward2A
        }
        else if (_rightInputChosen)
        {
            _currentAudioSource.PlayOneShot(_currentEvent.Result2ChoixB);
            StartCoroutine(waitForSound(_currentAudioSource));
            //_currentEvent.Reward2B
        }
        yield return null;
    }
    IEnumerator waitForSound(AudioSource other)
    {
        //Wait Until Sound has finished playing
        while (_currentAudioSource.isPlaying)
        {
            yield return null;
        }

        //Auidio has finished playing, disable GameObject
        other.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(OnRoomEnter());

    }

    void OnTriggerExit(Collider other)
    {
        _currentAudioSource.Stop();

    }
}
