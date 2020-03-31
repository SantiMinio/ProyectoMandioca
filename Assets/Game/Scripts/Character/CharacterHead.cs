using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DevelopTools;

public class CharacterHead : CharacterControllable
{
    Action<float> MovementHorizontal;
    Action<float> MovementVertical;
    Action<float> RotateHorizontal;
    Action<float> RotateVertical;
    Action Dash;
    Action ChildrensUpdates;
    Func<bool> InDash;

    #region SCRIPT TEMPORAL, BORRAR
    Action<float> changeCDDash; public void ChangeDashCD(float _cd) => changeCDDash.Invoke(_cd);
    #endregion

    CharacterBlock charBlock;
    Action OnBlock;
    Action UpBlock;
    Action Parry;

    [Header("Dash Options")]
    [SerializeField] bool directionalDash;
    [SerializeField] float dashTiming;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDeceleration;
    [SerializeField] float dashCD;


    [Header("Movement Options")]
    [SerializeField] float speed;
    [SerializeField] Transform rot;
    CharacterMovement move;

    [Header("Parry & Block Options")]
    [SerializeField] float _timerOfParry;
    [SerializeField] ParticleSystem parryParticle;
    [SerializeField] ParticleSystem hitParticle;

    [Header("Life Options")]
    [SerializeField] LifeSystem lifesystem;

    [Header("Feedbacks")]
    public GameObject feedbackParry;
    public GameObject feedbackBlock;
    [SerializeField] Text txt;


    [Header("Animations")]
    [SerializeField] Animator anim_base;
    [SerializeField] AnimEvent charAnimEvent;
    CharacterAnimator charanim;

    [Header("Attack Options")]
    [SerializeField] ParticleSystem feedbackHeavy;
    [SerializeField] float dmg_normal = 5;
    [SerializeField] float dmg_heavy = 20;
    [SerializeField] float attackRange;
    [SerializeField] float timeToHeavyAttack;
    float dmg = 5;
    Action OnAttackBegin;
    Action OnAttackEnd;
    CharacterAttack charAttack;

    CustomCamera customCam;

    private void Awake()
    {
        charanim = new CharacterAnimator(anim_base);
        customCam = FindObjectOfType<CustomCamera>();

        move = new CharacterMovement(GetComponent<Rigidbody>(), rot, IsDirectionalDash, charanim)
            .SetSpeed(speed)
            .SetTimerDash(dashTiming).SetDashSpeed(dashSpeed)
            .SetDashCD(dashCD)
            .SetRollDeceleration(dashDeceleration);

        MovementHorizontal += move.LeftHorizontal;
        MovementVertical += move.LeftVerical;
        RotateHorizontal += move.RightHorizontal;
        RotateVertical += move.RightVerical;
        Dash += move.Roll;
        InDash += move.IsDash;
        ChildrensUpdates += move.OnUpdate;

        charBlock = new CharacterBlock(_timerOfParry, OnBeginParry, OnEndParry, charanim);
        OnBlock += charBlock.OnBlockDown;
        UpBlock += charBlock.OnBlockUp;
        Parry += charBlock.Parry;
        ChildrensUpdates += charBlock.OnUpdate;

        charAttack = new CharacterAttack(attackRange, timeToHeavyAttack, charanim, rot, ReleaseInNormal, ReleaseInHeavy, feedbackHeavy);
        OnAttackBegin += charAttack.OnattackBegin;
        OnAttackEnd += charAttack.OnAttackEnd;


        charAnimEvent.Add_Callback("CheckAttackType", CheckAttackType);
        charAnimEvent.Add_Callback("DealAttack", DealAttack);
        charAnimEvent.Add_Callback("RompeCoco", RompeCoco);
        charAnimEvent.Add_Callback("BeginBlock", charBlock.OnBlockSucesfull);
        charAnimEvent.Add_Callback("BeginBlock", BlockFeedback);


        #region SCRIPT TEMPORAL, BORRAR
        changeCDDash += move.ChangeDashCD;
        #endregion
    }

    void RompeCoco()
    {
        if (customCam != null) customCam.BeginShakeCamera();
    }

    private void Update()
    {
        ChildrensUpdates();
        charAttack.Refresh();

        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            RollDash();
    }

    #region Attack
    /////////////////////////////////////////////////////////////////

    public void EVENT_OnAttackBegin() { OnAttackBegin(); }
    public void EVENT_OnAttackEnd() { OnAttackEnd(); }

    //tengo la espada arriba
    public void CheckAttackType()
    {
        charAttack.BeginCheckAttackType();
    }

    public void DealAttack()
    {
        charAttack.Attack((int)dmg, 2);
    }

    void ReleaseInNormal()
    {
        dmg = dmg_normal;
        charanim.NormalAttack();
    }
    void ReleaseInHeavy()
    {
        dmg = dmg_heavy;
        charanim.HeavyAttack();
    }

    /////////////////////////////////////////////////////////////////
    #endregion



    #region Block & Parry

    public void EVENT_OnBlocking()
    {
        if (!charBlock.onParry && !InDash() && !charAttack.inAttack)
        {
            move.SetSpeed(speed / 2);
            OnBlock();
        }
    }
    public void EVENT_UpBlocking()
    {
        feedbackBlock.SetActive(false);
        move.SetSpeed(speed);
        UpBlock();
    }

    public void BlockFeedback()
    {
        feedbackBlock.SetActive(true);
    }
    public void EVENT_Parry()
    {
        if (!charBlock.onBlock && !InDash() && !charAttack.inAttack)
        {
            charanim.Parry();
            Parry();
        }
    }
    public void PerfectParry()
    {
        parryParticle.Play();
    }
    void OnBeginParry() => feedbackParry.SetActive(true);
    void OnEndParry() => feedbackParry.SetActive(false);

    #endregion


    #region Movimiento y Rotacion
    public void LeftHorizontal(float axis)
    {
        if (!InDash())
            MovementHorizontal(axis);
    }

    public void LeftVerical(float axis)
    {
        if (!InDash())
            MovementVertical(axis);
    }

    //Joystick Derecho, Rotacion
    public void RightHorizontal(float axis)
    {
        if (!InDash())
            RotateHorizontal(axis);
    }
    public void RightVerical(float axis)
    {
        if (!InDash())
            RotateVertical(axis);
    }
    #endregion

    #region Roll 
    bool IsDirectionalDash()
    {
        return directionalDash;
    }

    public void ChangeDashForm()
    {
        directionalDash = !directionalDash;
        txt.text = "Directional Dash = " + directionalDash.ToString();
    }
    public void RollDash()
    {
        if (!InDash())
        {
            UpBlock();
            Dash();
        }
    }

    #endregion

    #region Take Damage
    public override Attack_Result TakeDamage(int dmg)
    {
        if (InDash())
            return Attack_Result.inmune;

        if (charBlock.onParry)
        {
            PerfectParry();
            return Attack_Result.parried;
        }
        else if (charBlock.onBlock)
        {
            charanim.BlockSomething();
            return Attack_Result.blocked;
        }
        else
        {
            hitParticle.Play();

            lifesystem.Hit(dmg);
            return Attack_Result.sucessful;
        }

    }
    #endregion

    #region Fuera de uso
    protected override void OnUpdateEntity() { }
    protected override void OnTurnOn() { }
    protected override void OnTurnOff() { }
    protected override void OnFixedUpdate() { }
    protected override void OnPause() { }
    protected override void OnResume() { }
    #endregion 
}