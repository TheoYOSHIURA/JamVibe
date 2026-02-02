using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Event", menuName = "Scriptable Objects/Event")]
public class Event : ScriptableObject
{
    [SerializeField] private string _eventName;
    [SerializeField] private AudioClip _choixA;
    [SerializeField] private AudioClip _choixB;
    [SerializeField] private AudioClip _result1choixA;
    [SerializeField] private AudioClip _result2choixA;
    [SerializeField] private AudioClip _result1choixB;
    [SerializeField] private AudioClip _result2choixB;
    [SerializeField] private ScriptableObject _reward1A;
    [SerializeField] private ScriptableObject _reward1B;
     [SerializeField] private ScriptableObject _reward2A;
    [SerializeField] private ScriptableObject _reward2B;
    
    public string EventName => _eventName;
    public AudioClip ChoixA => _choixA; 
    public AudioClip ChoixB => _choixB;
    public AudioClip Result1ChoixA => _result1choixA;   
    public AudioClip Result2ChoixA => _result2choixA;
    public AudioClip Result1ChoixB => _result1choixB;
    public AudioClip Result2ChoixB => _result2choixB;
    public ScriptableObject Reward1A => _reward1A;
    public ScriptableObject Reward1B => _reward1B;
    public ScriptableObject Reward2A => _reward2A;
    public ScriptableObject Reward2B => _reward2B; 
}
