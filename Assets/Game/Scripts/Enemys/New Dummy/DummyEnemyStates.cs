﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.StateMachine
{
    public class DummyEnemyStates : StatesFunctions<TrueDummyEnemy.DummyEnemyInputs>
    {
        protected EState<TrueDummyEnemy.DummyEnemyInputs> lastState;
        protected Animator anim;
        protected Transform target;
        protected Rigidbody rb;

        public DummyEnemyStates(EState<TrueDummyEnemy.DummyEnemyInputs> myState, EventStateMachine<TrueDummyEnemy.DummyEnemyInputs> _sm) : base(myState, _sm)
        {

        }

        protected override void Enter(TrueDummyEnemy.DummyEnemyInputs input)
        {

        }

        protected override void Exit(TrueDummyEnemy.DummyEnemyInputs input)
        {
            lastState = sm.Current;
        }

        protected override void FixedUpdate()
        {

        }

        protected override void LateUpdate()
        {

        }

        protected override void Update()
        {

        }
    }
}
