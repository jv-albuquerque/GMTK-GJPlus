using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Range(1f, 10f)]
    public float horizontalSpeed = 5f;
    [Range(1f, 20f)]
    public float jumpSpeed = 5f;
    [Range(0f, 1f)]
    public float horizontalDecelerationRate = 0.5f;

    public static Player instance = null;

    private Rigidbody2D _rigidbody2d;
    private bool canMove = true, canJump = true, hiding = false;
    private float horizontalForce = 0f, horizontalAcceleration = 0f;
    private WaitForSeconds timer = new WaitForSeconds(0.1f);

    public bool Hiding()
    {
        return hiding;
    }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0f)
            {
                horizontalAcceleration = _rigidbody2d.mass * horizontalSpeed / Time.fixedDeltaTime;
                horizontalForce = horizontalAcceleration * Input.GetAxisRaw("Horizontal") * (1f - Mathf.Min(Mathf.Abs(_rigidbody2d.velocity.x), horizontalSpeed) / horizontalSpeed);
                _rigidbody2d.AddForce(Vector2.right * horizontalForce, ForceMode2D.Force);
            }
            else
            {
                horizontalAcceleration = _rigidbody2d.mass * horizontalSpeed / Time.fixedDeltaTime;
                horizontalForce = horizontalAcceleration * horizontalDecelerationRate * -Mathf.Sign(_rigidbody2d.velocity.x) * (Mathf.Min(Mathf.Abs(_rigidbody2d.velocity.x), horizontalSpeed) / horizontalSpeed);
                _rigidbody2d.AddForce(Vector2.right * horizontalForce, ForceMode2D.Force);
            }
            if (Input.GetAxisRaw("Vertical") > 0f)
            {
                if (IsOnTheGround() && canJump)
                {
                    StartCoroutine(JumpDelayToAvoidDoubleJump());
                    _rigidbody2d.AddForce(Vector2.up * jumpSpeed * _rigidbody2d.mass, ForceMode2D.Impulse);
                }
            }
        }
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetAxisRaw("Vertical") < 0f && other.CompareTag("JumpDownPlatform"))
        {
            other.GetComponentInParent<BoxCollider2D>().isTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("JumpDownPlatform"))
        {
            other.GetComponentInParent<BoxCollider2D>().isTrigger = false;
        }
    }

    private bool IsOnTheGround()
    {
        ContactPoint2D[] contacts = new ContactPoint2D[5];
        int contactCount = _rigidbody2d.GetContacts(contacts);
        foreach (ContactPoint2D contact in contacts)
        {
            if (contact.normal.y >= 0.9f)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator JumpDelayToAvoidDoubleJump()
    {
        canJump = false;
        yield return timer;
        canJump = true;
    }
}
