﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ToolsMandioca.StateMachine
{
    public class CharReleaseAttack : CharacterStates
    {
        float attackRecall;
        Func<bool> IsHeavy;
        Action<bool> ChangeHeavy;
        Animator anim;
        Func<bool> WaitAttack;

        public CharReleaseAttack(EState<CharacterHead.PlayerInputs> myState, EventStateMachine<CharacterHead.PlayerInputs> _sm, float recall,
                                 Func<bool> _isHeavy, Action<bool> _ChangeHeavy, Animator _anim, Func<bool> _WaitAttack) : base(myState, _sm)
        {
            attackRecall = recall;
            IsHeavy = _isHeavy;
            ChangeHeavy = _ChangeHeavy;
            anim = _anim;
            WaitAttack = _WaitAttack;
        }

        protected override void Enter(EState<CharacterHead.PlayerInputs> input)
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

            var info = anim.GetCurrentAnimatorStateInfo(2);

            if (info.IsName("CentralState"))
            {
                if (WaitAttack())
                {
                    sm.SendInput(CharacterHead.PlayerInputs.CHARGE_ATTACK);
                }
                else
                {
                    if (LeftHorizontal() == 0 && LeftVertical() == 0)
                    {
                        sm.SendInput(CharacterHead.PlayerInputs.IDLE);
                    }
                    else
                    {
                        sm.SendInput(CharacterHead.PlayerInputs.MOVE);
                    }
                }
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
            ChangeHeavy(false);
        }
    }
}