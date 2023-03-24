using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    
    Animator PlayerAniamtor;
    AnimatorController PlayerAnimatorController;
    public float Speed = 20;
    public float MaxWalkSpeed = 20;
    public float MaxRunSpeed = 30;
    public float BackPeddleSpeed = 15;

    public float WalkToRunEase = 1.0f;
    public float RunToWalkEase = 1.0f;

    public float WalkAcceleration = 1.0f;


    public bool bMoving;

    Quaternion ForwardRot = Quaternion.Euler(0,90,0);
    Quaternion BackwardsRot = Quaternion.Euler(0, -90, 0);

    public float StartAimingEaseAmount = 5.0f;
    int m_AttackAnimLayerIndex;
    float m_AimAnimLayerWeight;

    float AttackTimer = 2.0f;
    bool bAttacking;
    bool bSprinting;

    void Start()
    {
        PlayerAniamtor = transform.GetChild(0).GetComponent<Animator>();
        m_AttackAnimLayerIndex = PlayerAniamtor.GetLayerIndex("AttackingLayer");
    }

    // Update is called once per frame
    void Update()
    {
        ResetAttack();
        HandleMovement();
        CheckForAttack();
    }

    private void ResetAttack()
    {
        if(AttackTimer >= 1.1f && bAttacking)
        {
            m_AimAnimLayerWeight = 0.0f;
            PlayerAniamtor.SetLayerWeight(m_AttackAnimLayerIndex, m_AimAnimLayerWeight);
            AttackTimer = 0.0f;
            bAttacking= false;
            PlayerAniamtor.SetBool("Attacking", bAttacking);
            PlayerAniamtor.SetBool("StandAttacking", bAttacking);

        }
        else
        {
            AttackTimer += Time.deltaTime;
        }
    }

    void HandleMovement()
    {
        HandleSprinting();

        if (Input.GetKey(KeyCode.D))
        {
            if(Speed < MaxWalkSpeed && !bSprinting)
                Speed += WalkAcceleration;
            
            transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0), Space.World);

            float Animspeed = Speed / MaxRunSpeed;

            PlayerAniamtor.SetFloat("ForwardSpeed", Animspeed);
            bMoving = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-BackPeddleSpeed * Time.deltaTime, 0, 0), Space.World);
            PlayerAniamtor.SetFloat("ForwardSpeed", -1.0f);

            bMoving = true;
        }
        else
        {
            PlayerAniamtor.SetFloat("ForwardSpeed", 0.0f);
            Speed = 0.0f;
            bMoving = false;
        }

        if(bSprinting)
            Speed = Mathf.Clamp(Speed, 0.0f, MaxRunSpeed);

        PlayerAniamtor.SetBool("Moving", bMoving);

    }

    private void HandleSprinting()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            bSprinting = true;

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            bSprinting = false;

        }

        if (bSprinting)
        {
            Speed += WalkToRunEase;
            Speed = Mathf.Clamp(Speed, 0.0f, MaxRunSpeed);
        }
        else
        {
            if (Speed > MaxWalkSpeed)
            {
                Speed -= RunToWalkEase;

            }
        }
    }

    void CheckForAttack()
    {
        if(Input.GetKey(KeyCode.Mouse0) && bAttacking == false)
        {
            bAttacking = true;
            
            if(Speed != 0.0f || PlayerAniamtor.GetFloat("ForwardSpeed") == -1.0f)
            {
                m_AimAnimLayerWeight = 1.0f;
                PlayerAniamtor.SetLayerWeight(m_AttackAnimLayerIndex, m_AimAnimLayerWeight);
            }
            else
            {
                PlayerAniamtor.SetBool("StandAttacking", bAttacking);

            }

            PlayerAniamtor.SetBool("Attacking", bAttacking);
            AttackTimer = 0.0f;
        }
    }
}
