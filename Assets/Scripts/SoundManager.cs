
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    
    [SerializeField] private AudioClip _clip;
    [SerializeField] private AudioSource _audioSource;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //NoteChecker.Instance.OnMiss += PlaySoundEffectClick;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlaySoundEffectClick()
    {
        _audioSource.Play();
    }
    void OnDestroy()
    {
        //NoteChecker.Instance.OnMiss -= PlaySoundEffectMiss;
    }

   
}