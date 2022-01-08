using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum ActorType
{
    Player = 1,
    Monster,
}

public enum ActorDirection
{
    None = 0,
    Up,
    Down,
    Left,
    Right,
    LeftTop,
    LeftDown,
    RightTop,
    RightDown,
}

public abstract class Actor : BObject
{
    private ActorType actorType;

    private ActorDirection actorDirection;

    private float moveSpeed;

    private float curSpeed;

    private GameObject actorObject;

    private Animator actorAnimator;

    private Rigidbody actorRigidbody;

    private CapsuleCollider actorCollider;

    private bool isPrimitive; //private only

    private bool isStiff;

    public ActorType ActorType
    {
        get => actorType;
        set => actorType = value;
    }

    public ActorDirection ActorDirection
    {
        get => actorDirection;
        set => actorDirection = value;
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public float CurSpeed
    {
        get => curSpeed;
        set => curSpeed = value;
    }

    public GameObject ActorObject
    {
        get => actorObject;
    }

    public Animator ActorAnimator
    {
        get => actorAnimator;
    }

    public Rigidbody ActorRigidbody
    {
        get => actorRigidbody;
    }

    public CapsuleCollider ActorCollider
    {
        get => actorCollider;
    }

    //是否僵硬, 通常被攻击后会进入僵硬时间, 期间无法操控该单位
    public bool IsStiff
    {
        get => isStiff;
        set => isStiff = value;
    }

    public void InitActor(ActorType actorType, float maxHealth, ActorDirection dir, float moveSpeed, string actorPrefabPath)
    {
        InitBObject(BObjectType.Actor, maxHealth);

        ActorType = actorType;
        ActorDirection = dir;
        MoveSpeed = moveSpeed;
        CurSpeed = MoveSpeed; //当前速度为移动速度

        RotateActor(dir);

        string actorPrefabName = null;
        int leftSlashPos = actorPrefabPath.LastIndexOf('/');
        int rightSlashPos = actorPrefabPath.LastIndexOf('\\');
        if (leftSlashPos != -1)
        {
            actorPrefabName = actorPrefabPath.Substring(leftSlashPos + 1, actorPrefabPath.Length - leftSlashPos - 1);
        }
        else if (rightSlashPos != -1)
        {
            actorPrefabName = actorPrefabPath.Substring(rightSlashPos + 1, actorPrefabPath.Length - rightSlashPos - 1);
        }
        //检测自身是否挂载了模型, 如果已经挂载了模型则不动态加载模型:
        Transform child = transform.Find(actorPrefabName);
        if (child && child.gameObject.activeInHierarchy)
        {
            actorObject = child.gameObject;
            actorAnimator = actorObject.GetComponent<Animator>();
            Debug.Log("Find Model Success.");
            if (!actorAnimator)
            {
                Debug.LogError("Missing Animator");
            }
        }
        else
        {
            //如果能够加载预制体则实例化它
            GameObject actorPrefab = Resources.Load<GameObject>(actorPrefabPath);
            if (actorPrefab)
            {
                actorObject = Instantiate(actorPrefab, Vector3.zero, Quaternion.identity, transform);
                actorAnimator = actorObject.GetComponent<Animator>();
                Debug.Log("Load Model Success.");
                if (!actorAnimator)
                {
                    Debug.LogError("Missing Animator");
                }
            }
            //加载失败则创建一个胶囊体当作占位符, 但是没有Animator, 播放动画时会出现警告
            else
            {
                actorObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                actorAnimator = null;
                isPrimitive = true;
                actorObject.transform.parent = transform;
                actorObject.transform.localPosition = new Vector3(0, 1, 0);
                Collider useless_collider = actorObject.GetComponent<Collider>();
                Destroy(useless_collider);
                Debug.LogWarning("Load Model Error, now I will add a capsule for you.");
            }
        }

        actorRigidbody = GetComponent<Rigidbody>();
        actorCollider = GetComponent<CapsuleCollider>();

        if (!actorRigidbody)
        {
            Debug.LogWarning("Missing Rigidbody, Now I will add default one.");
            actorRigidbody = gameObject.AddComponent<Rigidbody>();
            actorRigidbody.mass = 1;
            actorRigidbody.drag = 0;
            actorRigidbody.angularDrag = 0;
            actorRigidbody.useGravity = true;
            actorRigidbody.isKinematic = false;
            actorRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            actorRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            actorRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }

        if (!actorCollider)
        {
            Debug.LogWarning("Missing Collider, Now I will add default one.");
            actorCollider = gameObject.AddComponent<CapsuleCollider>();
            actorCollider.isTrigger = false;
            PhysicMaterial physicMaterial = new PhysicMaterial();
            physicMaterial.dynamicFriction = 0;
            physicMaterial.staticFriction = 0;
            physicMaterial.bounciness = 0;
            physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
            actorCollider.material = physicMaterial;
            actorCollider.center = new Vector3(0, 1, 0);
            actorCollider.radius = 0.5f;
            actorCollider.height = 2;
            actorCollider.direction = 1; //X=0, Y=1, Z=2
        }
    }

    public void RotateActor(ActorDirection dir)
    {
        ActorDirection = dir;

        switch (dir)
        {
            case ActorDirection.Up:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case ActorDirection.Down:
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                break;
            case ActorDirection.Left:
                transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                break;
            case ActorDirection.Right:
                transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case ActorDirection.LeftTop:
                transform.rotation = Quaternion.Euler(new Vector3(0, -45, 0));
                break;
            case ActorDirection.LeftDown:
                transform.rotation = Quaternion.Euler(new Vector3(0, -135, 0));
                break;
            case ActorDirection.RightTop:
                transform.rotation = Quaternion.Euler(new Vector3(0, 45, 0));
                break;
            case ActorDirection.RightDown:
                transform.rotation = Quaternion.Euler(new Vector3(0, 135, 0));
                break;
        }
    }

    public void AnimatorPlaySafe(string clipName)
    {
        if (!actorAnimator)
        {
            if (!isPrimitive)
            {
                Debug.LogWarning("AnimatorPlaySafe failed, can't find Animator component!");
            }
            return;
        }
        actorAnimator.Play(clipName);
    }

    protected override void WhenImHurt(float damage, ExtraHitInfo hitInfo)
    {
        IEnumerator ProcessStiff()
        {
            IsStiff = true;
            yield return new WaitForSeconds(0.1f);
            IsStiff = false;
        }
        StartCoroutine(ProcessStiff());
    }

    protected override void WhenImDead(ExtraHitInfo hitInfo)
    {
        Destroy(gameObject);
    }

    public static ActorDirection GetOppositeDirection(ActorDirection dir)
    {
        switch (dir)
        {
            case ActorDirection.Up:
                return ActorDirection.Down;
            case ActorDirection.Down:
                return ActorDirection.Up;
            case ActorDirection.Left:
                return ActorDirection.Right;
            case ActorDirection.Right:
                return ActorDirection.Left;
            case ActorDirection.LeftTop:
                return ActorDirection.RightDown;
            case ActorDirection.LeftDown:
                return ActorDirection.RightTop;
            case ActorDirection.RightTop:
                return ActorDirection.LeftDown;
            case ActorDirection.RightDown:
                return ActorDirection.LeftTop;
            default:
                return ActorDirection.None;
        }
    }

    public static System.Tuple<ActorDirection, ActorDirection> GetHorizontalDirections(ActorDirection dir)
    {
        System.Tuple<ActorDirection, ActorDirection> tuple;

        switch (dir)
        {
            case ActorDirection.Up:
                tuple = new System.Tuple<ActorDirection, ActorDirection>(ActorDirection.Left, ActorDirection.Right);
                break;
            case ActorDirection.Down:
                tuple = new System.Tuple<ActorDirection, ActorDirection>(ActorDirection.Right, ActorDirection.Left);
                break;
            case ActorDirection.Left:
                tuple = new System.Tuple<ActorDirection, ActorDirection>(ActorDirection.Down, ActorDirection.Up);
                break;
            case ActorDirection.Right:
                tuple = new System.Tuple<ActorDirection, ActorDirection>(ActorDirection.Up, ActorDirection.Down);
                break;
            case ActorDirection.LeftTop:
                tuple = new System.Tuple<ActorDirection, ActorDirection>(ActorDirection.LeftDown, ActorDirection.RightTop);
                break;
            case ActorDirection.LeftDown:
                tuple = new System.Tuple<ActorDirection, ActorDirection>(ActorDirection.RightDown, ActorDirection.LeftTop);
                break;
            case ActorDirection.RightTop:
                tuple = new System.Tuple<ActorDirection, ActorDirection>(ActorDirection.LeftTop, ActorDirection.RightDown);
                break;
            case ActorDirection.RightDown:
                tuple = new System.Tuple<ActorDirection, ActorDirection>(ActorDirection.RightTop, ActorDirection.LeftDown);
                break;
            default:
                tuple = new System.Tuple<ActorDirection, ActorDirection>(ActorDirection.None, ActorDirection.None);
                break;
        }

        return tuple;
    }

    public static System.Tuple<ActorDirection, ActorDirection, ActorDirection> GetBackwardDirections(ActorDirection dir)
    {
        System.Tuple<ActorDirection, ActorDirection, ActorDirection> tuple;

        switch (dir)
        {
            case ActorDirection.Up:
                tuple = new System.Tuple<ActorDirection, ActorDirection, ActorDirection>(ActorDirection.LeftDown, ActorDirection.Down, ActorDirection.RightDown);
                break;
            case ActorDirection.Down:
                tuple = new System.Tuple<ActorDirection, ActorDirection, ActorDirection>(ActorDirection.RightTop, ActorDirection.Up, ActorDirection.LeftTop);
                break;
            case ActorDirection.Left:
                tuple = new System.Tuple<ActorDirection, ActorDirection, ActorDirection>(ActorDirection.RightDown, ActorDirection.Right, ActorDirection.RightTop);
                break;
            case ActorDirection.Right:
                tuple = new System.Tuple<ActorDirection, ActorDirection, ActorDirection>(ActorDirection.LeftTop, ActorDirection.Left, ActorDirection.LeftDown);
                break;
            case ActorDirection.LeftTop:
                tuple = new System.Tuple<ActorDirection, ActorDirection, ActorDirection>(ActorDirection.Down, ActorDirection.RightDown, ActorDirection.Right);
                break;
            case ActorDirection.LeftDown:
                tuple = new System.Tuple<ActorDirection, ActorDirection, ActorDirection>(ActorDirection.Right, ActorDirection.RightTop, ActorDirection.Up);
                break;
            case ActorDirection.RightTop:
                tuple = new System.Tuple<ActorDirection, ActorDirection, ActorDirection>(ActorDirection.Left, ActorDirection.LeftDown, ActorDirection.Down);
                break;
            case ActorDirection.RightDown:
                tuple = new System.Tuple<ActorDirection, ActorDirection, ActorDirection>(ActorDirection.Up, ActorDirection.LeftTop, ActorDirection.Left);
                break;
            default:
                tuple = new System.Tuple<ActorDirection, ActorDirection, ActorDirection>(ActorDirection.None, ActorDirection.None, ActorDirection.None);
                break;
        }

        return tuple;
    }

    public static ActorDirection GetActorDirection(Vector2 vector)
    {
        ActorDirection dir = ActorDirection.None;

        if (vector.x == 0 && vector.y == 0)
        {
            dir = ActorDirection.None;
        }
        else if (vector.x < 0 && vector.y > 0)
        {
            dir = ActorDirection.LeftTop;
        }
        else if (vector.x > 0 && vector.y > 0)
        {
            dir = ActorDirection.RightTop;
        }
        else if (vector.x < 0 && vector.y < 0)
        {
            dir = ActorDirection.LeftDown;
        }
        else if (vector.x > 0 && vector.y < 0)
        {
            dir = ActorDirection.RightDown;
        }
        else if (vector.x > 0)
        {
            dir = ActorDirection.Right;
        }
        else if (vector.x < 0)
        {
            dir = ActorDirection.Left;
        }
        else if (vector.y > 0)
        {
            dir = ActorDirection.Up;
        }
        else if (vector.y < 0)
        {
            dir = ActorDirection.Down;
        }

        return dir;
    }
}