using UnityEngine;
using UHFPS.Scriptable;
using UHFPS.Tools;

namespace UHFPS.Runtime.States
{
    public class DomovoyRunAwayState : AIStateAsset
    {
        public float PlayerLostDistance;
        public float StartTransparentDistance;
        public override FSMAIState InitState(NPCStateMachine machine, AIStatesGroup group)
        {
            return new ChaseState(machine, group, this);
        }

        public override string GetStateKey() => ToString();

        public override string ToString() => "DomovoyRunAway";

        public class ChaseState : FSMAIState
        {
            private readonly ZombieStateGroup Group;
            private readonly DomovoyRunAwayState State;
            private DomovoyController _controller;
            private bool _isLost = false;
            private bool _isChangeToTransparent = false;

            public ChaseState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine)
            {
                Group = (ZombieStateGroup)group;
                State = (DomovoyRunAwayState)state;
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[]
                {
                    Transition.To<DomovoyChaseState>(() => CheckIsLost())
                };
            }

            private bool CheckIsLost()
            {
                if (_isLost && _controller.AttackCount >= _controller.MaxAttackCount)
                {
                    Debug.Log("DESTR");
                    Destroy(machine.gameObject);
                }
                return _isLost;
            }

            public override void OnStateEnter()
            {
                animator.ResetTrigger(Group.AttackTrigger);
                Group.ResetAnimatorPrameters(animator);
                _controller = machine.GetComponent<DomovoyController>();
                _isLost = false;
                _isChangeToTransparent = false;
            }

            public override void OnStateExit()
            {
                
            }

            public override void OnPlayerDeath()
            {
                animator.ResetTrigger(Group.AttackTrigger);
            }

            public override void OnStateUpdate()
            {
                animator.SetBool(Group.RunParameter, true);
                float distance = PlayerManager.Instance.CalculateDistanceToObj(machine.transform);
                Vector2 targetDelta = PlayerManager.Instance.CalculateDelta(distance + 2, machine.transform);
                Vector3 targetPoint = new Vector3(PlayerPosition.x - targetDelta.x, machine.transform.position.y, PlayerPosition.z - targetDelta.y);
                agent.SetDestination(targetPoint);
                agent.isStopped = false;
                
                if (distance >= State.StartTransparentDistance)
                {
                    if (!_isChangeToTransparent)
                    {
                        _controller.TransparentMaterialsManager.SetTransparentState(true);
                        _isChangeToTransparent = true;
                    }
                    float alpha = Mathf.Clamp01(1 - ((distance - State.StartTransparentDistance) / (State.PlayerLostDistance - State.StartTransparentDistance)));
                    _controller.TransparentMaterialsManager.SetAlpha(alpha);
                    _isLost = alpha == 0;
                }
            }
        }
    }
}