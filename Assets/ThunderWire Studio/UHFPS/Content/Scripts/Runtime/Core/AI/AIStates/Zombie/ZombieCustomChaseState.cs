using UnityEngine;
using UHFPS.Scriptable;
using UHFPS.Tools;

namespace UHFPS.Runtime.States
{
    public class ZombieCustomChaseState : AIStateAsset
    {
        public float RunSpeed = 3f;
        public float ChaseStoppingDistance = 1.5f;

        [Header("Chase")]
        public float LostPlayerPatrolTime = 5f;
        public float LostPlayerPredictTime = 1f;
        public float VeryClosePlayerDetection = 1.5f;

        [Header("Attack")]
        public float AttackFOV = 30f;
        public float AttackDistance = 2f;

        public override FSMAIState InitState(NPCStateMachine machine, AIStatesGroup group)
        {
            return new CustomChaseState(machine, group, this);
        }

        public override string GetStateKey() => ToString();

        public override string ToString() => "CustomChase";

        public class CustomChaseState : FSMAIState
        {
            private readonly ZombieStateGroup Group;
            private readonly ZombieCustomChaseState State;

            private bool isChaseStarted;
            private bool isPatrolPending;
            private bool resetParameters;

            private float waitTime;
            private float predictTime;
            private bool playerDied;

            public CustomChaseState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine)
            {
                Group = (ZombieStateGroup)group;
                State = (ZombieCustomChaseState)state;

                machine.CatchMessage("Attack", () => AttackPlayer());
                machine.CatchMessage("PushAttack", () => PushAttackPlayer());
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[]
                {
                    Transition.To<ZombieCustomPatrolState>(() => waitTime > State.LostPlayerPatrolTime || playerDied || !CheckPlayerDistance()),
                    Transition.To<ZombiePlayerHideState>(() => playerMachine.IsCurrent(PlayerStateMachine.HIDING_STATE))
                };
            }
            private bool CheckPlayerDistance()
            {
                BookHeadController controller = agent.GetComponent<BookHeadController>();
                Vector2 sphereCenter = new Vector2(controller.EnemyAttackSphereCenter.transform.position.x, controller.EnemyAttackSphereCenter.transform.position.z);
                Vector2 playerPos2D = new Vector2(PlayerPosition.x, PlayerPosition.z);
                return Vector2.Distance(sphereCenter, playerPos2D) <= controller.AttackSphereRadius;
            }
            public override void OnStateEnter()
            {
                Group.ResetAnimatorPrameters(animator);
                agent.speed = State.RunSpeed;
                agent.stoppingDistance = State.ChaseStoppingDistance;
                machine.RotateAgentManually = true;
                isChaseStarted = true;
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
                    Debug.Log("Attack PL");
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
                    Debug.Log("Player not in Distance");
                    return;

                int damage = Group.DamageRange.Random();
                playerHealth.OnApplyDamage(damage, machine.transform);
            }

            public void PushAttackPlayer()
            {
                playerMachine.AddForce(machine.transform.forward*40, ForceMode.VelocityChange);
                BookHeadController controller = agent.GetComponent<BookHeadController>();
                controller.CounterUndamageAttacks -= 1;
                animator.SetBool("PushAttack", controller.CounterUndamageAttacks > 0);
            }
        }
    }
}