using UnityEngine;
using UHFPS.Scriptable;
using UHFPS.Tools;

namespace UHFPS.Runtime.States
{
    public class DomovoyRunAwayState : AIStateAsset
    {
        public float[] RestTime;
        public float RestFromStart;
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
            private float _restTimeout;
            private float _targetRestTime;
            
            private float _startRest;
            public enum ChaseStatus { Wait, FromStart };
            private ChaseStatus _status;


            public ChaseState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine)
            {
                Group = (ZombieStateGroup)group;
                State = (DomovoyRunAwayState)state;
                _targetRestTime = State.RestTime[machine.GetComponent<DomovoyController>().RestIndex];
                machine.GetComponent<DomovoyController>().RestIndex += 1;
                if (machine.GetComponent<DomovoyController>().RestIndex == State.RestTime.Length) machine.GetComponent<DomovoyController>().RestIndex = 0;
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[]
                {
                    Transition.To<DomovoyPatrolState>(() => !PlayerManager.Instance.CheckObjectInViewField(machine.gameObject) && _restTimeout >= _targetRestTime)
                };
            }

            public override void OnStateEnter()
            {
                animator.ResetTrigger(Group.AttackTrigger);
                Group.ResetAnimatorPrameters(animator);
                _status = ChaseStatus.FromStart;
                if (machine.GetComponent<DomovoyController>().WaitForRun)
                {
                    machine.GetComponent<DomovoyController>().WaitForRun = false;
                    _status = ChaseStatus.Wait;
                }
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
                switch (_status)
                {
                    case ChaseStatus.Wait:
                        _startRest += Time.deltaTime;
                        agent.isStopped = true;
                        animator.SetBool(Group.RunParameter, false);
                        animator.SetBool(Group.IdleParameter, true);
                        if (_startRest >= State.RestFromStart) _status = ChaseStatus.FromStart;
                        break;
                    case ChaseStatus.FromStart:
                        if (PlayerManager.Instance.CheckObjectInViewField(machine.gameObject))
                        {
                            _restTimeout = 0;
                            animator.SetBool(Group.RunParameter, true);
                            Vector3 direction = playerMachine.MainCamera.transform.right;
                            if (PlayerManager.Instance.GetObjectPositionInViewField(machine.gameObject).x >= 0.5)
                            {
                                direction = Quaternion.AngleAxis(-90, playerMachine.MainCamera.transform.up) * playerMachine.MainCamera.transform.forward;
                            }
                            agent.SetDestination(agent.transform.position - direction * 30);
                            agent.isStopped = false;
                        }
                        else
                        {
                            _restTimeout += Time.deltaTime;
                            animator.SetBool(Group.RunParameter, false);
                            animator.SetBool(Group.IdleParameter, true);
                            agent.isStopped = true;
                        }
                        break;
                }
            }
        }
    }
}