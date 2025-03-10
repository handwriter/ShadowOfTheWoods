using UnityEngine;
using UnityEngine.UI;
using UHFPS.Scriptable;
using static UHFPS.Runtime.States.HidingStateAsset;
using UHFPS.Runtime;
using System;
using System.Drawing.Text;

public class WaterMonsterHideState : AIStateAsset
{
    
    public float HideTime = 1;
    

    public override FSMAIState InitState(NPCStateMachine machine, AIStatesGroup group)
    {
        return new PlayerHideState(machine, group, this);
    }

    public override string GetStateKey() => ToString();

    public override string ToString() => "Water Monster Hide";

    public class PlayerHideState : FSMAIState
    {
        private readonly ZombieStateGroup Group;
        private readonly WaterMonsterHideState State;

        private HidingPlayerState PlayerHide;
        private HideInteract HidingPlace;
        private float _time;
        private WaterMonsterController _controller;

        public PlayerHideState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine)
        {
            Group = (ZombieStateGroup)group;
            State = (WaterMonsterHideState)state;
            _controller = machine.GetComponent<WaterMonsterController>();
        }

        public override Transition[] OnGetTransitions()
        {
            return new Transition[]
            {
            };
        }

        public override void OnStateEnter()
        {
            _controller.SetupMaterials();
            SetDestination(machine.GetComponent<WaterMonsterController>().StartPosition);
        }

        public override void OnStateExit()
        {
        }

        public override void OnStateUpdate()
        {
            if (!PathDistanceCompleted())
            {
                SetDestination(machine.GetComponent<WaterMonsterController>().StartPosition);
                animator.SetBool(Group.RunParameter, true);
                animator.SetBool(Group.IdleParameter, false);
                agent.isStopped = false;

            }
            else
            {
                _time += Time.deltaTime;
                float value = Mathf.Clamp01(_time / State.HideTime);
                _controller.SetAlphaValue(Mathf.Lerp(1, 0, value));
                if (value == 1)
                {
                    Destroy(machine.gameObject);
                }
            }
            
        }
    }
}