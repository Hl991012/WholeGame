using System;
using System.Collections.Generic;
using NMNH.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D curRigidbody;
    [SerializeField] private Transform curTrigger;
    [SerializeField] private ConstantForce2D constantForce2D;
    
    public float moveSpeed = 15;
    
    public Direction CurDirection { get; private set; } = Direction.None;

    private Queue<Direction> moveOrderQueue = new();

    private Direction lastAddDirection = Direction.None;

    private bool isPlayerMoving;

    private Vector2 moveDir = Vector2.zero;
    
    private Vector2 recentRevivePoint = Vector2.zero;
    
    private Direction lastTouchMoveDir = Direction.None;

    private void OnEnable()
    {
        canInputMoveDir = true;
    }

    private void Awake()
    {
        CameraController.Instance.Register(transform);
        TextAdventureGameController.Instance.Register(this);
    }

    private void AddMoveOrder(Direction moveDirection)
    { 
        // if(lastAddDirection == moveDirection) return;
        if(TextAdventureGameController.Instance.CurGameState != TextAdventureGameController.GameState.Playing) return;
        if(moveOrderQueue.Count > 2) return;
        lastAddDirection = moveDirection;
        moveOrderQueue.Enqueue(moveDirection);
    }

    private bool canInputMoveDir;

    private void Update()
    {
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            return;
        }
#endif
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddMoveOrder(Direction.Left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            AddMoveOrder(Direction.Right);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddMoveOrder(Direction.Up);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddMoveOrder(Direction.Down);
        }
        
        if (CurDirection == Direction.None && moveOrderQueue.Count > 0)
        {
            var curDirection = moveOrderQueue.Dequeue();
            Move(curDirection);
        }

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Moved:
                    Vector2 deltaPosition = touch.deltaPosition;
                    var tempDir = Direction.None;
                    // 检查滑动方向
                    if (Mathf.Abs(deltaPosition.x) > Mathf.Abs(deltaPosition.y))
                    { 
                        // 水平滑动
                        tempDir = deltaPosition.x < 0 ? Direction.Right : Direction.Left;
                    }
                    else
                    {
                        // 垂直滑动
                        tempDir = deltaPosition.y < 0 ? Direction.Up : Direction.Down;
                    }

                    if (tempDir != lastTouchMoveDir && canInputMoveDir)
                    {
                        canInputMoveDir = false;
                        AddMoveOrder(tempDir);
                        lastTouchMoveDir = tempDir;
                    }
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    canInputMoveDir = true;
                    break;
            }
        }
        else
        {
            canInputMoveDir = true;
        }
    }

    private void Move(Direction moveDirection)
    {
        CurDirection = moveDirection;
        switch (moveDirection)
        {
            case Direction.Up:
                curTrigger.localPosition = Vector2.up * 0.3f;
                moveDir = Vector2.up;
                constantForce2D.force = (Vector2.up) * moveSpeed;
                curRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
                break;
            case Direction.Down:
                curTrigger.localPosition = Vector2.down * 0.3f;
                moveDir = Vector2.down;
                constantForce2D.force = (Vector2.down) * moveSpeed;
                curRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
                break;
            case Direction.Left:
                curTrigger.localPosition = Vector2.left * 0.3f;
                moveDir = Vector2.left;
                constantForce2D.force = (Vector2.left) * moveSpeed;
                curRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
                break;
            case Direction.Right:
                curTrigger.localPosition = Vector2.right * 0.3f;
                moveDir = Vector2.right;
                constantForce2D.force = (Vector2.right) * moveSpeed;
                curRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
                break;
        }

        curRigidbody.constraints |= RigidbodyConstraints2D.FreezeRotation;
        curTrigger.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "RevivePoint":
                recentRevivePoint = other.transform.position;
                break;
            case "Electricity":
            case "Arrow":
            case "Stab":
            case "Ghost":
            case "PoisonTrigger":
                Death();
                break;
            case "SpringTrigger":
                moveOrderQueue.Clear();
                var addDir = other.GetComponent<SpringOrgan>().GetMoveDir(CurDirection);
                AddMoveOrder(addDir);
                break;
            case "Wall":
                ResetState();
                AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.TouchWall);
                break;
            case "StabTrigger":
                var stab = other.GetComponentInParent<Stab>();
                if (stab != null)
                {
                    stab.TriggerOrgan(other.name);
                }
                break;
            case "Door":
                ResetState();
                TextAdventureGameController.Instance.EndGame(true);
                break;
            case "Star":
                Destroy(other.gameObject);
                TextAdventureGameController.Instance.AddStar();
                break;
            case "Coin":
                Destroy(other.gameObject);
                WealthManager.Instance.AddCoin(1);
                break;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "GhostTrigger":
                var ghost = other.GetComponentInParent<Ghost>();
                if (ghost != null)
                {
                    ghost.BeRelease(transform);
                }
                break;
        }
    }

    private void ResetState()
    {
        lastAddDirection = Direction.None;
        CurDirection = Direction.None;
        curTrigger.localPosition = Vector2.zero;
        curTrigger.gameObject.SetActive(false);
        constantForce2D.force = Vector2.zero;
        curRigidbody.velocity = Vector2.zero;
        curRigidbody.angularVelocity = 0;

        curRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        var position = transform.position;
        var x = Mathf.Round(position.x * 2) / 2f;
        var y = Mathf.Round(position.y * 2) / 2f;
        transform.position = new Vector3(x, y, 0);
    }

    private void Death()
    {
        ResetState();
        TextAdventureGameController.Instance.GameDeath();
    }

    public void Revive()
    {
        transform.position = recentRevivePoint;
    }
}
