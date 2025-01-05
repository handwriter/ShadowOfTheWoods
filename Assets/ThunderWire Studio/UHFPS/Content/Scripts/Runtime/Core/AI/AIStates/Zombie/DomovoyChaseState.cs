using UnityEngine;
using UHFPS.Scriptable;
using UHFPS.Tools;
using Unity.VisualScripting;
using UnityEngine.AI;

namespace UHFPS.Runtime.States
{
    public class DomovoyChaseState : AIStateAsset
    {
        public float RunSpeed = 3f;
        public float ChaseStoppingDistance = 1.5f;
        public float UnderFlashlightMaxTime;

        public float PlayerLostDistance;
        public float StartTransparentDistance;

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

        public override string ToString() => "DomovoyChase";

        public class ChaseState : FSMAIState
        {
            private readonly ZombieStateGroup Group;
            private readonly DomovoyChaseState State;
            private DomovoyController _controller;
            private float _timeInFlashLight;


            private bool isChaseStarted;
            private bool isPatrolPending;
            private bool resetParameters;

            private float waitTime;
            private float predictTime;
            private bool playerDied;
            private bool _HideFromPlayer;
            private float _timeFromAttack;
            private bool _isChangeToOpaque;
            public bool isAttacked = false;
            public ChaseState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine)
            {
                Group = (ZombieStateGroup)group;
                State = (DomovoyChaseState)state;
                _controller = machine.GetComponent<DomovoyController>();
                isAttacked = false;
                machine.CatchMessage("Attack", () => AttackPlayer());
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[]
                {
                    Transition.To<DomovoyRunAwayState>(() => CheckRunAwayState()),
                };
            }

            private bool CheckRunAwayState()
            {
                
                bool isPlayerAvailable = !(float.IsInfinity(agent.remainingDistance) || agent.remainingDistance == 0);
                bool changeState = _timeInFlashLight >= State.UnderFlashlightMaxTime || isAttacked || !isPlayerAvailable;
                if (changeState)
                {
                    _controller.AttackCount += 1;
                }
                if (!isPlayerAvailable) _controller.PlayerIsUnavalilable = true;
                return changeState;
            }

            public override void OnStateEnter()
            {
                Group.ResetAnimatorPrameters(animator);
                agent.speed = State.RunSpeed;
                agent.stoppingDistance = State.ChaseStoppingDistance;
                machine.RotateAgentManually = true;
                isChaseStarted = true;
                isAttacked = false;
                _isChangeToOpaque = false;
                _timeInFlashLight = 0;
                _controller = machine.GetComponent<DomovoyController>();
            }

            public override void OnStateExit()
            {
                machine.RotateAgentManually = false;
                isChaseStarted = false;
                isPatrolPending = false;
                resetParameters = false;
                waitTime = 0f;
                predictTime = 0f;
                _HideFromPlayer = false;
            }

            public override void OnPlayerDeath()
            {
                animator.ResetTrigger(Group.AttackTrigger);
                playerDied = true;
            }

            private bool IsUnderFlashlight()
            {
                return _controller.IsInLightZone && ModelController.IsUsingFlashlight;
            }

            public override void OnStateUpdate()
            {
                _timeFromAttack += Time.deltaTime;
                if (!resetParameters)
                {
                    Group.ResetAnimatorPrameters(animator);
                    animator.SetBool(Group.RunParameter, true);
                    resetParameters = true;
                }
                Chasing();
                float distance = PlayerManager.Instance.CalculateDistanceToObj(machine.transform);
                if (PlayerManager.Instance.CheckObjectInViewField(machine.gameObject))
                {
                    SetDestination(PlayerPosition);
                }
                else
                {
                    Vector3 targetCoord = PlayerManager.Instance.CalculateBackPoint(Mathf.Max(0, distance - 2), PlayerManager.Instance.transform.position.y);
                    SetDestination(targetCoord);
                }
                
                predictTime = State.LostPlayerPredictTime;

                if (IsUnderFlashlight())
                {
                    _timeInFlashLight += Time.deltaTime;
                }
                else
                {
                    _timeInFlashLight = 0;
                }

                if (PathDistanceCompleted() || IsUnderFlashlight())
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

                if (distance <= State.StartTransparentDistance)
                {
                    if (!_isChangeToOpaque)
                    {
                        _controller.TransparentMaterialsManager.SetTransparentState(false);
                        _isChangeToOpaque = true;
                    }
                    float alpha = Mathf.Clamp01((distance - State.StartTransparentDistance) / (State.PlayerLostDistance - State.StartTransparentDistance));
                    _controller.TransparentMaterialsManager.SetAlpha(alpha);
                }

                isPatrolPending = false;
                isChaseStarted = true;
                waitTime = 0f;
            }
            private void Chasing()
            {
                bool isAttacking = IsAnimation(1, Group.AttackState);
                if(InPlayerDistance(State.AttackDistance) && IsObjectInSights(State.AttackFOV, PlayerPosition) && !isAttacking && !playerHealth.IsDead)
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
                if (!InPlayerDistance(State.AttackDistance) || isAttacked)
                {
                    return;
                }
                int damage = Group.DamageRange.Random();
                playerHealth.OnApplyDamage(damage, machine.transform);
                isAttacked = true;
            }
        }
    }
}