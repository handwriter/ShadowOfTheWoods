using System;
using System.Linq;
using UnityEngine;
using UHFPS.Tools;
using UHFPS.Scriptable;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Rendering.HighDefinition;

namespace UHFPS.Runtime.States
{
    public class ZombiePatrolState : AIStateAsset
    {
        public enum WaypointPatrolEnum { InOrder, Random, RandomInSafeZone }
        public enum PatrolTypeEnum { None, WaitTime }


        public WaypointPatrolEnum Patrol = WaypointPatrolEnum.InOrder;
        public PatrolTypeEnum PatrolType = PatrolTypeEnum.None;

        [Header("Settings")]
        public float PatrolTime = 3f;
        public float WalkSpeed = 0.5f;
        public float PatrolStoppingDistance = 1f;
        public float VeryClosePlayerDetection = 1f;
        public float FindAnimTime;
        public float RageChance;

        public override FSMAIState InitState(NPCStateMachine machine, AIStatesGroup group)
        {
            return new PatrolState(machine, group, this);
        }

        public override string GetStateKey() => ToString();

        public override string ToString() => "Patrol";

        public class PatrolState : FSMAIState
        {
            private readonly ZombieStateGroup Group;
            private readonly ZombiePatrolState State;

            private AIWaypointsGroup waypointsGroup;
            private AIWaypoint currWaypoint;
            private AIWaypoint prevWaypoint;

            private float waitTime;
            private bool isWaypointSet;
            private bool isPatrolPending;
            private bool WaitForMove = false;
            private float _waitForMoveTime;

            public PatrolState(NPCStateMachine machine, AIStatesGroup group, AIStateAsset state) : base(machine) 
            {
                Group = (ZombieStateGroup)group;
                State = (ZombiePatrolState)state;
            }

            public override Transition[] OnGetTransitions()
            {
                return new Transition[] { Transition.To<ZombieChaseState>(() => !playerMachine.IsCurrent(PlayerStateMachine.HIDING_STATE)
                        && (SeesPlayer() || InDistance(State.VeryClosePlayerDetection, PlayerPosition) || (machine.GetComponent<LeshyController>().IsInLightZone && ModelController.IsUsingFlashlight)) && !IsPlayerDead)
                        };

            }

            public override void OnStateEnter()
            {
                var closestWaypointsGroup = FindClosestWaypointsGroup();
                waypointsGroup = closestWaypointsGroup.Key;

                agent.speed = State.WalkSpeed;
                agent.stoppingDistance = State.PatrolStoppingDistance;
                Group.ResetAnimatorPrameters(animator);
            }

            public override void OnStateExit()
            {
                waitTime = 0f;
                isWaypointSet = false;
                isPatrolPending = false;
                animator.SetBool(Group.RageParameter, false);
                if (currWaypoint != null)
                    currWaypoint.ReservedBy = null;
            }

            public override void OnStateUpdate()
            {
                if (State.Patrol == WaypointPatrolEnum.RandomInSafeZone)
                {
                    if (_waitForMoveTime >= State.FindAnimTime) WaitForMove = false;
                    if (WaitForMove)
                    {
                        agent.isStopped = true;
                        animator.SetBool(Group.WalkParameter, false);
                        _waitForMoveTime += Time.deltaTime;
                        return;
                    }
                    animator.SetBool(Group.RageParameter, false);
                    Vector3 waypointPos = machine.GetComponent<LeshyController>().GetTargetPoint().transform.position;
                    agent.isStopped = false;
                    agent.SetDestination(waypointPos);
                    animator.SetBool(Group.WalkParameter, true);
                    animator.SetBool(Group.RageParameter, false);
                    if (machine.GetComponent<LeshyController>().IsLostSafeZone)
                    {
                        machine.GetComponent<LeshyController>().IsLostSafeZone = false;
                        bool isNeedRage = UnityEngine.Random.value <= State.RageChance;
                        if (isNeedRage)
                        {
                            WaitForMove = true;
                            _waitForMoveTime = 0f;
                            animator.SetBool(Group.RageParameter, true);
                        }
                    }
                    //currWaypoint.ReservedBy = machine.gameObject;
                    return;
                }
                if (waypointsGroup == null)
                    return;
                if (!isWaypointSet)
                {
                    SetNextWaypoint();
                    if(currWaypoint != null)
                    {
                        Vector3 waypointPos = currWaypoint.transform.position;
                        agent.isStopped = false;
                        agent.SetDestination(waypointPos);
                        animator.SetBool(Group.WalkParameter, true);
                        currWaypoint.ReservedBy = machine.gameObject;
                    }

                    isWaypointSet = true;
                }
                else
                {
                    if (!PathCompleted() && !isPatrolPending)
                        return;

                    if (State.PatrolType == PatrolTypeEnum.None)
                    {
                        isWaypointSet = false;
                        Group.ResetAnimatorPrameters(animator);
                    }
                    else if (State.PatrolType == PatrolTypeEnum.WaitTime)
                    {
                        if (!isPatrolPending)
                        {
                            Group.ResetAnimatorPrameters(animator);
                            animator.SetBool(Group.PatrolParameter, true);
                            agent.velocity = Vector3.zero;
                            agent.isStopped = true;
                            isPatrolPending = true;
                        }
                        else
                        {
                            waitTime += Time.deltaTime;

                            if (waitTime > State.PatrolTime)
                            {
                                waitTime = 0f;
                                isPatrolPending = false;
                                isWaypointSet = false;
                                Group.ResetAnimatorPrameters(animator);
                            }
                        }
                    }
                }
            }

            private void SetNextWaypoint()
            {
                prevWaypoint = currWaypoint;
                if(prevWaypoint != null) 
                    prevWaypoint.ReservedBy = null;

                var freeWaypoints = GetFreeWaypoints(waypointsGroup);
                if(State.Patrol == WaypointPatrolEnum.InOrder)
                {
                    if (currWaypoint == null) currWaypoint = freeWaypoints[0];
                    else
                    {
                        int currIndex = Array.IndexOf(freeWaypoints, currWaypoint);
                        int nextIndex = currIndex + 1 >= freeWaypoints.Length ? 0 : currIndex + 1;
                        currWaypoint = freeWaypoints[nextIndex];
                    }
                }
                else if(State.Patrol == WaypointPatrolEnum.Random)
                {
                    freeWaypoints = freeWaypoints.Except(new[] { prevWaypoint }).ToArray();
                    currWaypoint = freeWaypoints.Random();
                }
            }
        }
    }
}