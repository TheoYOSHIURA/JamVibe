using UnityEngine;
using UnityEngine.InputSystem;
using XInputDotNetPure; 

public class VibrationControllerWrong : MonoBehaviour
{
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") > 0.5f)
        {
            GamePad.SetVibration(playerIndex, 0f, 1f);
            //Debug.Log("D-Pad droite");
        }

        if (Input.GetAxis("Horizontal") < -0.5f)
        {
            GamePad.SetVibration(playerIndex, 1f, 0f);
            //Debug.Log("D-Pad gauche");
        }

        if (Input.GetAxis("Horizontal") == 0)
        {
            GamePad.SetVibration(playerIndex, 0f, 0f);
            //Debug.Log("D-Pad neutre");
        }

        float x = Input.GetAxis("Horizontal");
        if (Mathf.Abs(x) > 0.01f)
        {
            Debug.Log(x);
        }
    }
}
