using System.Xml;
using UnityEngine;

public class RoomLogic : MonoBehaviour
{
    [SerializeField] private ScriptableObject _currentEvent;

    private AudioSource _audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        _audioSource.Play();
    }

    void OnTriggerExit(Collider other)
    {
        _audioSource.Stop();
    }
}
