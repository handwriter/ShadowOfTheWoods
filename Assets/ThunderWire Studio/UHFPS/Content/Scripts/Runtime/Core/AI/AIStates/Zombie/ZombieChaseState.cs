using UnityEngine;
using UHFPS.Scriptable;
using UHFPS.Tools;

namespace UHFPS.Runtime.States
{
    public class ZombieChaseState : AIStateAsset
    {
        public float RunSpeed = 3f;
        public float ChaseStoppingDistance = 1.5f;
        public float MaxTimeAtOnePlace = 2f;
        public float ExtremeTimeAtOnePlace = 10f;

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

        public override string ToString() => "Chase";

        public class ChaseState : FSMAIState
        {
            private readonly ZombieStateGroup Group;
            private readonly ZombieChaseState State;

            private bool isChaseStarted;
            private bool isPatrolPending;
            private bool isRagePending;
            private bool resetParameters;

            private float waitTime;
            private float predictTime;
            private bool playerDied;
            private float _timeAtOnePlace;
            private Vector3 _previousPosition;
            private float _timeForLastAnim = 0;
            private const float _timeForPatrolAnim = 2f;

            public ChaseState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine)
            {
                Group = (ZombieStateGroup)group;
                State = (ZombieChaseState)state;

                machine.CatchMessage("Attack", () => AttackPlayer());
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[]
                {
                    Transition.To<ZombiePatrolState>(() => waitTime > State.LostPlayerPatrolTime || playerDied),
                    Transition.To<ZombiePlayerHideState>(() => playerMachine.IsCurrent(PlayerStateMachine.HIDING_STATE)),
                    Transition.To<StuckState>(() => (_timeAtOnePlace >= State.MaxTimeAtOnePlace && !IsAnimation(1, Group.AttackState) && PlayerInSights()) || _timeAtOnePlace >= State.ExtremeTimeAtOnePlace)
                };
            }

            public override void OnStateEnter()
            {
                Group.ResetAnimatorPrameters(animator);
                agent.speed = State.RunSpeed;
                agent.stoppingDistance = State.ChaseStoppingDistance;
                agent.destination = PlayerPosition;
                machine.RotateAgentManually = true;
                isChaseStarted = true;
            }

            public override void OnStateExit()
            {
                machine.RotateAgentManually = false;
                isChaseStarted = false;
                isPatrolPending = false;
                resetParameters = false;
                isRagePending = false;
                animator.SetBool(Group.RageParameter, false);
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
                if (machine.transform.position == _previousPosition)
                {
                    _timeAtOnePlace += Time.deltaTime;
                }
                else
                {
                    _timeAtOnePlace = 0f;
                    _previousPosition = machine.transform.position;
                }
                if (PlayerInSights())
                {
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
                else if(predictTime > 0f)
                {
                    SetDestination(PlayerPosition);
                    predictTime -= Time.deltaTime;
                }
                else
                {
                    if (isPatrolPending) _timeForLastAnim += Time.deltaTime;
                    if (!PathCompleted())
                        return;

                    if (!isPatrolPending)
                    {
                        Group.ResetAnimatorPrameters(animator);
                        animator.SetBool(Group.PatrolParameter, true);
                        agent.velocity = Vector3.zero;
                        agent.isStopped = true;

                        resetParameters = false;
                        isPatrolPending = true;
                        isChaseStarted = false;
                        _timeForLastAnim = 0f;
                    }
                    else if (!isRagePending && _timeForLastAnim >= _timeForPatrolAnim)
                    {
                        Group.ResetAnimatorPrameters(animator);
                        animator.SetBool(Group.RageParameter, true);
                        agent.velocity = Vector3.zero;
                        agent.isStopped = true;

                        resetParameters = false;
                        isRagePending = true;
                        isChaseStarted = false;
                    }
                    else
                    {
                        waitTime += Time.deltaTime;
                    }
                }
            }

            private void Chasing()
            {
                bool isAttacking = IsAnimation(1, Group.AttackState);
                if(InPlayerDistance(State.AttackDistance) && IsObjectInSights(State.AttackFOV, PlayerPosition) && !isAttacking && !playerHealth.IsDead)
                {
                    _timeAtOnePlace = 0;
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
                    return;

                int damage = Group.DamageRange.Random();
                playerHealth.OnApplyDamage(damage, machine.transform);
            }
        }
    }
}