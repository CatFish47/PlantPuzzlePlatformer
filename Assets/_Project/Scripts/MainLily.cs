using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLily : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;
    public float speedMod;
    public float maxVelocity;
    public float accelerate;
    public float drag;

    private float vel;
    private float start_x;
    private float start_y;
    // Start is called before the first frame update
    void Start()
    {
        vel = 0;

        start_x = transform.position.x;
        start_y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    public void Move()
    {
        if (vel > 0)
        {
            if (!IsTouchingRightWall())
            {
                transform.position = new Vector2(transform.position.x + vel * speedMod * Time.deltaTime, transform.position.y);
            } else
            {
                vel = -vel / 4;
            }
        } else if (vel < 0)
        {
            if (!IsTouchingLeftWall())
            {
                transform.position = new Vector2(transform.position.x + vel * speedMod * Time.deltaTime, transform.position.y);
            } else
            {
                vel = -vel / 4;
            }
        }

        if (vel > 0)
        {
            vel = Mathf.Max(vel - drag * Time.deltaTime, 0);
        } else if (vel < 0)
        {
            vel = Mathf.Min(vel + drag * Time.deltaTime, 0);
        }
    }
    public bool Accelerate(int dir)
    {
        if (dir == 1)
        {
            vel = Mathf.Min(vel + accelerate * Time.deltaTime, maxVelocity);
        } else if (dir == -1)
        {
            vel = Mathf.Max(vel - accelerate * Time.deltaTime, -maxVelocity); ;
        }

        return true;
    }
    public Vector2 getMomentum()
    {
        return new Vector2(vel, 0);
    }
    bool IsTouchingRightWall()
    {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();

        RaycastHit2D raycastHitRight = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.right, 0.03f, platformLayerMask);

        return raycastHitRight.collider != null;
    }

    bool IsTouchingLeftWall()
    {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();

        RaycastHit2D raycastHitLeft = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.left, 0.03f, platformLayerMask);

        return raycastHitLeft.collider != null;
    }
    public void ResetPosition()
    {
        transform.position = new Vector2(start_x, start_y);
    }
    public float getVelocity()
    {
        return vel;
    }
}
