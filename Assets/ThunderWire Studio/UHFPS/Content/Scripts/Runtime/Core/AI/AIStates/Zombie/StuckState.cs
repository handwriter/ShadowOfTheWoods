using UnityEngine;
using UHFPS.Scriptable;
using UHFPS.Tools;

namespace UHFPS.Runtime.States
{
    public class StuckState : AIStateAsset
    {
        public int EnemyDataIndex;

        public override FSMAIState InitState(NPCStateMachine machine, AIStatesGroup group)
        {
            return new ChaseState(machine, group, this);
        }

        public override string GetStateKey() => ToString();

        public override string ToString() => "Stuck";

        public class ChaseState : FSMAIState
        {
            private readonly ZombieStateGroup Group;
            private readonly StuckState State;

            private float _timeAtOnePlace;
            private Vector3 _previousPosition;

            //private bool _playerMoved;

            public ChaseState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine)
            {
                Group = (ZombieStateGroup)group;
                State = (StuckState)state;
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[]
                {
                    Transition.To<ZombiePatrolState >(() => PathDistanceCompleted())
                };
            }

            public override void OnStateEnter()
            {
                machine.RotateAgentManually = true;
                _previousPosition = PlayerManager.Instance.transform.position;
                agent.SetDestination(EnemiesSpawnManager.Instance.GetDefaultEnemyPosition(State.EnemyDataIndex));
                //_playerMoved = false;
            }

            public override void OnStateExit()
            {
            }

            public override void OnPlayerDeath()
            {
            }

            public override void OnStateUpdate()
            {
                //if (_previousPosition != PlayerManager.Instance.transform.position) _playerMoved = true;
                //_previousPosition = PlayerManager.Instance.transform.position;
                agent.isStopped = false;
                animator.SetBool(Group.RunParameter, true);
                animator.SetBool(Group.IdleParameter, false);
                animator.ResetTrigger(Group.AttackTrigger);
                //if (PathDistanceCompleted() || !IsPathPossible(agent.destination))
                //{
                //    agent.SetDestination(machine.GetComponent<LeshyController>().GetRandomStuckPoint());
                //    agent.isStopped = true;
                //    agent.velocity = Vector3.zero;
                //}
                //else
                //{
                //    agent.isStopped = false;
                //    animator.SetBool(Group.RunParameter, true);
                //    animator.SetBool(Group.IdleParameter, false);
                //    animator.ResetTrigger(Group.AttackTrigger);
                //}
            }
        }
    }
}