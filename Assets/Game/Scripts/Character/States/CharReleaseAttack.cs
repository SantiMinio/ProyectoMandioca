﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tools.StateMachine
{
    public class CharReleaseAttack : CharacterStates
    {
        float timer;
        float attackRecall;
        Func<bool> IsHeavy;

        public CharReleaseAttack(EState<CharacterHead.PlayerInputs> myState, EventStateMachine<CharacterHead.PlayerInputs> _sm, float recall, Func<bool> _isHeavy) : base(myState, _sm)
        {
            attackRecall = recall;
            IsHeavy = _isHeavy;
        }

        protected override void Enter(CharacterHead.PlayerInputs input)
        {
            if (IsHeavy())
            {
                charMove.MovementHorizontal(0);
                charMove.MovementVertical(0);
            }
        }

        protected override void Update()
        {
            if (!IsHeavy())
            {
                charMove.MovementHorizontal(LeftHorizontal());
                charMove.MovementVertical(LeftVertical());
            }
            //timer += Time.deltaTime;

            //if (timer >= attackRecall)
            //    sm.SendInput(CharacterHead.PlayerInputs.IDLE);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
        }

        protected override void Exit(CharacterHead.PlayerInputs input)
        {
            timer = 0;
        }
    }
}
