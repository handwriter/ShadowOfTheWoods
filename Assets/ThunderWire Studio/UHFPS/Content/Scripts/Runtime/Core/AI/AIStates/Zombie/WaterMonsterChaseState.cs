using UnityEngine;
using UHFPS.Scriptable;
using UHFPS.Tools;

namespace UHFPS.Runtime.States
{
    public class WaterMonsterChaseState : AIStateAsset
    {
        public float RunSpeed = 3f;
        public float ChaseStoppingDistance = 1.5f;
        public float PlayerMaxDistance = 30f;
        public float TimeToLeave;
        [Header("Chase")]
        public float LostPlayerPatrolTime = 5f;
        public float LostPlayerPredictTime = 1f;
        public float VeryClosePlayerDetection = 1.5f;

        [Header("Attack")]
        public float AttackFOV = 30f;
        public float AttackDistance = 2f;

        public override FSMAIState InitState(NPCStateMachine machine, AIStatesGroup group)
        {
            return new ChaseState(machine, group, this);
        }

        public override string GetStateKey() => ToString();

        public override string ToString() => "Water Monster Chase";

        public class ChaseState : FSMAIState
        {
            private readonly ZombieStateGroup Group;
            private readonly WaterMonsterChaseState State;

            private bool isChaseStarted;
            private bool isPatrolPending;
            private bool resetParameters;

            private float waitTime;
            private float predictTime;
            private bool playerDied;
            private float _time;

            public ChaseState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine)
            {
                Group = (ZombieStateGroup)group;
                State = (WaterMonsterChaseState)state;

                machine.CatchMessage("Attack", () => AttackPlayer());
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[]
                {
                    Transition.To<WaterMonsterHideState>(() => _time >= State.TimeToLeave),
                    //Transition.To<WaterMonsterHideState>(() => !InPlayerDistance(State.PlayerMaxDistance)),
                    //Transition.To<WaterMonsterPlayerHideState>(() => playerMachine.IsCurrent(PlayerStateMachine.HIDING_STATE))
                };
            }

            public override void OnStateEnter()
            {
                Group.ResetAnimatorPrameters(animator);
                agent.speed = State.RunSpeed;
                agent.stoppingDistance = State.ChaseStoppingDistance;
                machine.RotateAgentManually = true;
                isChaseStarted = true;
                _time = 0;
            }

            public override void OnStateExit()
            {
                machine.RotateAgentManually = false;
                isChaseStarted = false;
                isPatrolPending = false;
                resetParameters = false;
                waitTime = 0f;
                predictTime = 0f;
            }

            public override void OnPlayerDeath()
            {
                animator.ResetTrigger(Group.AttackTrigger);
                playerDied = true;
            }

            public override void OnStateUpdate()
            {
                _time += Time.deltaTime;
                if (playerDied)
                {
                    SetDestination(machine.GetComponent<WaterMonsterController>().StartPosition);
                    animator.SetBool(Group.RunParameter, true);
                    animator.SetBool(Group.IdleParameter, false);
                    agent.isStopped = false;
                    return;
                }
                animator.SetBool("IsOnGround", !machine.GetComponent<WaterMonsterController>().IsInSwamp);
                if (!resetParameters)
                {
                    Group.ResetAnimatorPrameters(animator);
                    animator.SetBool(Group.RunParameter, true);
                    resetParameters = true;
                }

                Chasing();
                SetDestination(PlayerPosition);
                predictTime = State.LostPlayerPredictTime;

                if (PathDistanceCompleted())
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    animator.SetBool(Group.RunParameter, false);
                    animator.SetBool(Group.IdleParameter, true);
                }
                else
                {
                    agent.isStopped = false;
                    animator.SetBool(Group.RunParameter, true);
                    animator.SetBool(Group.IdleParameter, false);
                    animator.ResetTrigger(Group.AttackTrigger);
                }

                isPatrolPending = false;
                isChaseStarted = true;
                waitTime = 0f;
            }
            private void Chasing()
            {
                bool isAttacking = IsAnimation(1, Group.AttackState);
                if (InPlayerDistance(State.AttackDistance) && IsObjectInSights(State.AttackFOV, PlayerPosition) && !isAttacking && !playerHealth.IsDead)
                {
                    animator.SetTrigger(Group.AttackTrigger);
                }
            }

            private bool PlayerInSights()
            {
                if (playerHealth.IsDead)
                    return false;

                if (!isChaseStarted || isPatrolPending)
                    return SeesPlayerOrClose(State.VeryClosePlayerDetection);

                return SeesObject(machine.SightsDistance, PlayerHead);
            }

            private void AttackPlayer()
            {
                Debug.Log("Attack player");
                if (!InPlayerDistance(State.AttackDistance))
                {
                    Debug.Log("Player not in Distance");
                    return;
                }
                int damage = Group.DamageRange.Random();
                playerHealth.OnApplyDamage(damage, machine.transform);
            }
        }
    }
}