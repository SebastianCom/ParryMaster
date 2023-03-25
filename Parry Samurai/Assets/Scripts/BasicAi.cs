using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum AiState
{
    None = 0,
    Patrolling,
    Alert,
    Attacking
}

public class BasicAi : MonoBehaviour
{
    // Start is called before the first frame update
    NavMeshAgent NavAgent;
    Animator AiAnimator;
    GameObject Player;

    float StartingZ;
    Vector3 XPosLock;
    Vector3 StartingRot = new Vector3(0, -90, 0);

    public float AttackDistance = 5.0f;

    AiState CurrentState = AiState.None;

    public bool bAttacking = false;
    public bool bBlocking = false;
    public bool bPausing = false;

    public List<Vector2> ParryFrames = new List<Vector2>();

    int AttackAnimNumber = 0;

    bool bPostureBroken = false;

    public int AttackFrameCounter = 0;

    float AttackTimer = 0.0f;
    float PauseTimer = 0.0f;
    float AnimTimer = 0.8f;

    bool bAtPlayer = false;

    public Canvas ParryIconCanvas;

    public int NumberOfWarningFrames = 10;

    void Start()
    {
        NavAgent= GetComponent<NavMeshAgent>();
        Player = GameObject.Find("Player");
        StartingZ = transform.position.z;
        CurrentState= AiState.Attacking;
        AiAnimator = GetComponentInChildren<Animator>();
        AttackTimer = 0.0f;
        if(ParryIconCanvas!= null) 
            ParryIconCanvas.GetComponentInChildren<RawImage>().color = new Color(ParryIconCanvas.GetComponentInChildren<RawImage>().color.r, ParryIconCanvas.GetComponentInChildren<RawImage>().color.g, ParryIconCanvas.GetComponentInChildren<RawImage>().color.b, 0.0f);

    }

    // Update is called once per frame
    void Update()
    {

        switch (CurrentState)
        {
            case AiState.Attacking:
                ResetAttack();
                HandleAttackState();
                break;
        }

        if(AttackAnimNumber == 1)
        {
            if(AttackFrameCounter > ParryFrames[0].x - NumberOfWarningFrames && AttackFrameCounter < ParryFrames[0].y) 
            {
                ParryIconCanvas.GetComponentInChildren<RawImage>().color = new Color (ParryIconCanvas.GetComponentInChildren<RawImage>().color.r, ParryIconCanvas.GetComponentInChildren<RawImage>().color.g, ParryIconCanvas.GetComponentInChildren<RawImage>().color.b, 1.0f);
            }
            else
            {
                ParryIconCanvas.GetComponentInChildren<RawImage>().color = new Color(ParryIconCanvas.GetComponentInChildren<RawImage>().color.r, ParryIconCanvas.GetComponentInChildren<RawImage>().color.g, ParryIconCanvas.GetComponentInChildren<RawImage>().color.b, 0.0f);
            }
        }

        SideScrollerLock();
    }

    private void ResetAttack()
    {
        if(bAtPlayer) 
        {
            if (AttackTimer >= 1.1f && bAttacking && !bPausing)
            {
                AttackTimer = 0.0f;
                bPausing = true;
                bAttacking = false;
                AiAnimator.SetBool("StandAttacking", bAttacking);

            }
            else
            {
                AttackTimer += Time.deltaTime;
                AttackFrameCounter++;
                //Debug.Log(AttackFrameCounter);
            }

            if (bPausing)
            {

                AttackFrameCounter = 0;
                if (PauseTimer >= 1.00f)
                {
                    bPausing = false;
                    PauseTimer = 0.0f;
                }
                else
                {
                    PauseTimer += Time.deltaTime;
                }
            }
        }
    }

    private void HandleAttackState()
    {
        Player.GetComponent<Player>().CurrentEnemy = this.gameObject;

        if (Vector3.Distance(transform.position, Player.transform.position) > AttackDistance)
        {
            NavAgent.SetDestination(Player.transform.position);
            AiAnimator.SetFloat("ForwardSpeed", 0.5f);
            bBlocking = false;
            bAttacking = false;
            bAtPlayer = false;
        }
        else
        {
            bAtPlayer = true;
            NavAgent.ResetPath();
            AiAnimator.SetFloat("ForwardSpeed", 0.0f);
            HandleAttack();

        }
    }

    void HandleAttack()
    {
        if (Player.GetComponent<Player>().bAttacking == false && bAttacking == false && bPausing == false)
        {
            bBlocking = false;
            //AttackAnimNumber = Mathf.FloorToInt(Random.Range(0, 4));
            //AttackAnimNumber = Mathf.Clamp(AttackAnimNumber, 1, 3);
            AttackAnimNumber = 1;

            bAttacking = true;
            AiAnimator.SetBool("StandAttacking", bAttacking);
            AiAnimator.SetInteger("AttackNumber", AttackAnimNumber);
            AiAnimator.SetBool("StandBlocking", bBlocking);
            AttackTimer = 0.0f;
            AttackFrameCounter = 0;
        }
        else if (Player.GetComponent<Player>().bAttacking == true)
        {
            bBlocking = true;
            bAttacking = false;
            AttackTimer = 0.0f;
            AttackFrameCounter = 0;
            AiAnimator.StopPlayback();
            AiAnimator.SetBool("StandAttacking", bAttacking);
            AiAnimator.SetBool("StandBlocking", bBlocking);
        }

    }

    private void SideScrollerLock()
    {
        XPosLock = new Vector3(transform.position.x, transform.position.y, StartingZ);
        transform.position = XPosLock;
        transform.rotation = Quaternion.Euler(StartingRot);
    }
}
