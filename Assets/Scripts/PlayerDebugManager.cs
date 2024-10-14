using System.Collections;
using System.Collections.Generic;
using UHFPS.Input;
using UHFPS.Runtime;
using UnityEngine;

public class PlayerDebugManager : MonoBehaviour
{
    public PlayerStateMachine stateMachine;
    public float FastWalkSpeed;
    public float FastRunSpeed;
    private float _defaultWalkSpeed;
    private float _defaultRunSpeed;
    void Start()
    {
        _defaultRunSpeed = stateMachine.PlayerBasicSettings.RunSpeed;
        _defaultWalkSpeed = stateMachine.PlayerBasicSettings.WalkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.ReadButton("input.action.changeSpeed"))
        {
            stateMachine.PlayerBasicSettings.WalkSpeed = FastWalkSpeed;
            stateMachine.PlayerBasicSettings.RunSpeed = FastRunSpeed;
        }
        else
        {
            stateMachine.PlayerBasicSettings.WalkSpeed = _defaultWalkSpeed;
            stateMachine.PlayerBasicSettings.RunSpeed = _defaultRunSpeed;
        }
    }
}
