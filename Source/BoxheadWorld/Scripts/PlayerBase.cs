using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum LocalPlayerID
{
    Player1 = 1,
    Player2 = 2,
    Player3 = 3,
    Player4 = 4,
}

public enum PlayerInputKey
{
    Unknown = 0,
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Shoot,
    Use,
    NextWeapon,
    PrevWeapon,
    Strafe,
}

public enum PlayerState
{
    Dead = 1,
    Idle,
    Walk,
    Strafe,
    Stiff,
}

public abstract class PlayerBase : Actor
{
    //Animations:
    public string AnimationDie = "die";
    public string AnimationBeHit = "hit03";

    public string AnimationIdle = "idle01";
    public string AnimationWalk = "gun_walk";
    public string AnimationStrafe = "double_walk";

    private LocalPlayerID localPlayerID;

    private Dictionary<PlayerInputKey, KeyCode> inputKeys = new Dictionary<PlayerInputKey, KeyCode>();

    private float horizontalStrafeMoveSpeed;

    private float oppositeStrafeMoveSpeed;

    private float forwardStrafeMoveSpeed;

    private float backwardStrafeMoveSpeed;

    private PlayerState playerState;

    private Vector2 inputVector;

    private bool isMoving;

    public LocalPlayerID LocalPlayerID
    {
        get => localPlayerID;
        set => localPlayerID = value;
    }

    public Dictionary<PlayerInputKey, KeyCode> InputKeys
    {
        get => inputKeys;
    }

    public float HorizontalStrafeMoveSpeed
    {
        get => horizontalStrafeMoveSpeed;
        set => horizontalStrafeMoveSpeed = value;
    }

    public float OppositeStrafeMoveSpeed
    {
        get => oppositeStrafeMoveSpeed;
        set => oppositeStrafeMoveSpeed = value;
    }

    public float ForwardStrafeMoveSpeed
    {
        get => forwardStrafeMoveSpeed;
        set => forwardStrafeMoveSpeed = value;
    }

    public float BackwardStrafeMoveSpeed
    {
        get => backwardStrafeMoveSpeed;
        set => backwardStrafeMoveSpeed = value;
    }

    public PlayerState PlayerState
    {
        get => playerState;
        set => playerState = value;
    }

    public Vector2 InputVector
    {
        get => inputVector;
        set => inputVector = value;
    }

    public bool IsMoving
    {
        get => isMoving;
        set => isMoving = value;
    }

    public void InitPlayerBase(LocalPlayerID localPlayerID, float maxHealth, ActorDirection dir, float moveSpeed, string actorPrefabPath, float horizontalStrafeMoveSpeed, float oppositeStrafeMoveSpeed, float forwardStrafeMoveSpeed, float backwardStrafeMoveSpeed)
    {
        InitActor(ActorType.Player, maxHealth, dir, moveSpeed, actorPrefabPath);

        LocalPlayerID = localPlayerID;

        HorizontalStrafeMoveSpeed = horizontalStrafeMoveSpeed;
        OppositeStrafeMoveSpeed = oppositeStrafeMoveSpeed;
        ForwardStrafeMoveSpeed = forwardStrafeMoveSpeed;
        BackwardStrafeMoveSpeed = backwardStrafeMoveSpeed;

        PlayerState = PlayerState.Idle;
    }

    public void SetInputKey(PlayerInputKey inputKey, KeyCode keyCode)
    {
        InputKeys[inputKey] = keyCode;
    }

    public void InitDefaultInputKeys()
    {
        SetInputKey(PlayerInputKey.MoveUp, KeyCode.W);
        SetInputKey(PlayerInputKey.MoveDown, KeyCode.S);
        SetInputKey(PlayerInputKey.MoveLeft, KeyCode.A);
        SetInputKey(PlayerInputKey.MoveRight, KeyCode.D);
        SetInputKey(PlayerInputKey.Shoot, KeyCode.J);
        SetInputKey(PlayerInputKey.Use, KeyCode.H);
        SetInputKey(PlayerInputKey.NextWeapon, KeyCode.L);
        SetInputKey(PlayerInputKey.PrevWeapon, KeyCode.K);
        SetInputKey(PlayerInputKey.Strafe, KeyCode.Space);
    }

    public Vector2 ReceiveMoveInput(out bool isMoving)
    {
        Vector2 moveInput = Vector2.zero;

        if (Input.GetKey(InputKeys[PlayerInputKey.MoveLeft]))
        {
            moveInput.x--;
        }
        if (Input.GetKey(InputKeys[PlayerInputKey.MoveRight]))
        {
            moveInput.x++;
        }
        if (Input.GetKey(InputKeys[PlayerInputKey.MoveUp]))
        {
            moveInput.y++;
        }
        if (Input.GetKey(InputKeys[PlayerInputKey.MoveDown]))
        {
            moveInput.y--;
        }

        isMoving = moveInput.x != 0 || moveInput.y != 0;

        return moveInput;
    }

    public bool ReceiveStrafeInput()
    {
        return Input.GetKey(InputKeys[PlayerInputKey.Strafe]);
    }

    public PlayerState UpdatePlayerState(bool isMoving, bool strafe)
    {
        if (IsDead)
        {
            PlayerState = PlayerState.Dead;
        }
        else if (IsStiff)
        {
            PlayerState = PlayerState.Stiff;
        }
        else
        {
            if (isMoving)
            {
                if (strafe)
                {
                    PlayerState = PlayerState.Strafe;
                }
                else
                {
                    PlayerState = PlayerState.Walk;
                }
            }
            else
            {
                PlayerState = PlayerState.Idle;
            }
        }

        return PlayerState;
    }

    public void SetPlayerMoveSpeed(PlayerState state, ActorDirection dir)
    {
        if (state == PlayerState.Dead || state == PlayerState.Idle || state == PlayerState.Stiff)
        {
            CurSpeed = 0;
        }
        else if (state == PlayerState.Walk)
        {
            CurSpeed = MoveSpeed;
        }
        else if (state == PlayerState.Strafe)
        {
            System.Tuple<ActorDirection, ActorDirection> horizontalDirs = Actor.GetHorizontalDirections(ActorDirection);
            ActorDirection oppDir = Actor.GetOppositeDirection(ActorDirection);
            System.Tuple<ActorDirection, ActorDirection, ActorDirection> backwardDirs = Actor.GetBackwardDirections(ActorDirection);

            //水平移动时速度:
            if (dir == horizontalDirs.Item1 || dir == horizontalDirs.Item2)
            {
                CurSpeed = HorizontalStrafeMoveSpeed;
            }
            //反方向移动时速度:
            else if (dir == oppDir)
            {
                CurSpeed = OppositeStrafeMoveSpeed;
            }
            //向斜后方向移动时速度:
            else if (dir == backwardDirs.Item1 || dir == backwardDirs.Item3)
            {
                CurSpeed = BackwardStrafeMoveSpeed;
            }
            //向前方移动时速度:
            else if (dir == ActorDirection)
            {
                CurSpeed = MoveSpeed;
            }
            //向斜前方移动时速度:
            else
            {
                CurSpeed = ForwardStrafeMoveSpeed;
            }
        }
    }

    public void RotatePlayerBase(PlayerState state, ActorDirection dir)
    {
        if (state == PlayerState.Dead) return;
        if (state == PlayerState.Idle) return;
        if (state == PlayerState.Strafe) return;
        if (state == PlayerState.Stiff) return;

        if (dir == ActorDirection.None) return;

        RotateActor(dir);
    }

    private void Start()
    {
        InitDefaultInputKeys();
        StartPlayer();
    }

    private void Update()
    {
        //Input:
        InputVector = ReceiveMoveInput(out this.isMoving);
        bool strafe = ReceiveStrafeInput();

        //Update State:
        ActorDirection targetDir = Actor.GetActorDirection(InputVector);
        UpdatePlayerState(isMoving, strafe);

        //Move & Rotate & Animation:
        RotatePlayerBase(PlayerState, targetDir);
        SetPlayerMoveSpeed(PlayerState, targetDir);
        UpdatePlayerAnimation(PlayerState);

        //Logic:
        UpdatePlayer(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        UpdatePlayerPhysics();
    }

    protected override void WhenImDead(ExtraHitInfo hitInfo)
    {
        AnimatorPlaySafe(AnimationDie);
    }

    protected override void WhenImHurt(float damage, ExtraHitInfo hitInfo)
    {
        AnimatorPlaySafe(AnimationBeHit);
    }

    protected abstract void StartPlayer();

    protected abstract void UpdatePlayer(float deltaTime);

    protected virtual void UpdatePlayerAnimation(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                AnimatorPlaySafe(AnimationIdle);
                break;
            case PlayerState.Walk:
                AnimatorPlaySafe(AnimationWalk);
                break;
            case PlayerState.Strafe:
                AnimatorPlaySafe(AnimationStrafe);
                break;

            case PlayerState.Dead:
            case PlayerState.Stiff:
                //这里什么也不播放, 死亡跟受伤动画放在虚方法中播放
                break;
            default:
                //这里什么也不播放
                break;
        }
    }

    protected virtual void UpdatePlayerPhysics()
    {
        Vector3 vel = new Vector3(InputVector.x, 0, InputVector.y).normalized * CurSpeed;
        this.ActorRigidbody.velocity = vel;
    }
}