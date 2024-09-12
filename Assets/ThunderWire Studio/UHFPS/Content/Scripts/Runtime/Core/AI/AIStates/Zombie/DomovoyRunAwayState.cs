using UnityEngine;
using UHFPS.Scriptable;
using UHFPS.Tools;

namespace UHFPS.Runtime.States
{
    public class DomovoyRunAwayState : AIStateAsset
    {
        public float RestTime;

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

            private float _RunAwayTime;

            public ChaseState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine)
            {
                Group = (ZombieStateGroup)group;
                State = (DomovoyRunAwayState)state;
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[]
                {
                    Transition.To<DomovoyPatrolState>(() => _RunAwayTime > State.RestTime)
                };
            }

            public override void OnStateEnter()
            {
                animator.ResetTrigger(Group.AttackTrigger);
                Group.ResetAnimatorPrameters(animator);
                _RunAwayTime = 0;
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
                _RunAwayTime += Time.deltaTime;
                if (_RunAwayTime <= 5)
                {
                    animator.SetBool(Group.RunParameter, true);
                    agent.SetDestination(agent.transform.position + agent.transform.forward * 30);
                    agent.isStopped = false;
                    GameObject newObj = new GameObject();
                    newObj.transform.position = agent.transform.position + agent.transform.forward * 10;
                }
                else
                {
                    animator.SetBool(Group.RunParameter, false);
                    animator.SetBool(Group.IdleParameter, true);
                    agent.isStopped = true;
                }
            }
        }
    }
}