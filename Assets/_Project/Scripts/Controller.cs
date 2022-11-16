using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;
    public float speed = 1f;
    public float gravity = 1f;
    public float jumpPower = 1f;

    public WaterBar waterBar;   
    public float maxWater = 100;
    public float currentWater;
    public float waterSpeed = 0.1f; // Speed of watering and sucking

    public GameObject winText;

    private SpriteRenderer spriteRenderer;
    
    public Animator animator; 

    private int facing; // 0 is left, 1 is right

    public float min_y = -1;
    private float start_x;
    private float start_y;

    private float restartTimer;

    private float boostTimer;
    private float boost;
    private bool boosting;
    public float maxBoostTimer = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        currentWater = maxWater;
        waterBar.SetMaxWater(maxWater);

        facing = 1;

        start_x = transform.position.x;
        start_y = transform.position.y;

        restartTimer = 0f;

        boostTimer = 0;
        boost = 0;
        boosting = false;
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        bool grounded = IsGrounded();
        bool jumping = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool use = Input.GetKey(KeyCode.Mouse0);
        bool suck = Input.GetKey(KeyCode.Mouse1);
        bool manualRestart = Input.GetKey(KeyCode.R);
        float j = jumping && grounded ? jumpPower : rb.velocity.y / speed;
        bool sink = CheckSink();
        bool win = CheckWin();
        Vector2 move;

        UpdateBoost();

        if (win)
        {
            x = 0f;
            winText.SetActive(true);
        }

        if (manualRestart)
        {
            restartTimer += Time.deltaTime;
        } else
        {
            restartTimer = 0;
        }

        if (GetOutOfBounds() || restartTimer > 3) // Code that restarts the game
        {
            restartTimer = 0;
            ResetLevel();
        }

        if (sink)
        {
            x = 0f;
        }

        if (y < 0 && CheckNearbyColliders("VineTop", 1.5f, true).Length > 0)
        {
            CheckNearbyColliders("VineTop", 1.5f, true)[0].GetComponent<Collider2D>().isTrigger = true;
        }
        else
        {
            foreach (Collider2D tops in CheckCollidersInRange("VineTop", 1f, 5f, false))
            {
                tops.GetComponent<Collider2D>().isTrigger = false;
            }
        }

        if (y != 0 && OnVine())
        {
            move = new Vector2(x, y);
            rb.gravityScale = 0.0f;
        }
        else if (OnVine())
        {
            move = new Vector2(x, 0);
            rb.gravityScale = 0.0f;
        }
        else if (boosting)
        {
            move = new Vector2(x + boost, j);
            rb.gravityScale = gravity;
        }
        else
        {
            move = new Vector2(x, j);
            rb.gravityScale = gravity;
        }

        rb.velocity = move * speed;

        bool flipSprite = (spriteRenderer.flipX ? (rb.velocity.x > 0) : (rb.velocity.x < 0));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            facing = -facing; // flips between -1 and 1
        }

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (use)
        {
            WaterVines();
            MoveLilypads();
        } else if (suck)
        {
            SuckPlants();
        }
    }
    void UpdateBoost()
    {
        if (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
        } else
        {
            boostTimer = 0;
            boost = 0;
            boosting = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Lilypad")
        {
            GetComponent<BoxCollider2D>().transform.SetParent(collision.transform);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Lilypad")
        {
            //GetComponent<Rigidbody2D>().AddForce(new Vector2(1000f, 0f));
            GetComponent<BoxCollider2D>().transform.SetParent(null);
            boosting = true;
            boost = collision.gameObject.GetComponent<MainLily>().getVelocity();
            boostTimer = maxBoostTimer;
        }
    }
    bool IsGrounded()
    {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        RaycastHit2D raycastHit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, 0.05f, platformLayerMask);
        return raycastHit.collider != null;
    }

    private bool GetOutOfBounds()
    {
        return transform.position.y < min_y;
    }

    Collider2D[] CheckNearbyColliders(string tag, float radius, bool stopAtFirst)
    {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(bc.bounds.center, bc.size.x * radius);

        var results = new List<Collider2D>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == tag)
            {
                //colliders[i].gameObject.GetComponent<GrowVine>().Grow();
                results.Add(colliders[i]);

                if (stopAtFirst)
                {
                    return results.ToArray();
                }
            }
        }

        return results.ToArray();
    }
    Collider2D[] CheckCollidersInRange(string tag, float inner, float outer, bool stopAtFirst)
    {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(bc.bounds.center, bc.size.x * outer);
        Collider2D[] exclusions = Physics2D.OverlapCircleAll(bc.bounds.center, bc.size.x * inner);

        var results = new List<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            bool skip = false;
            foreach (Collider2D exclude in exclusions)
            {
                if (collider == exclude)
                {
                    skip = true;
                    break;
                }
            }

            if (!skip && collider.tag == tag)
            {
                //colliders[i].gameObject.GetComponent<GrowVine>().Grow();
                results.Add(collider);

                if (stopAtFirst)
                {
                    return results.ToArray();
                }
            }
        }

        return results.ToArray();
    }
    void ResetLevel()
    {
        transform.position = new Vector3(start_x, start_y, 0);

        Collider2D[] vines = CheckNearbyColliders("Vine", 1000f, false);
        Collider2D[] lilypads = CheckNearbyColliders("Lilypad", 1000f, false);
        
        foreach (Collider2D vine in vines)
        {
            vine.GetComponent<MainVine>().ResetPosition();
        }

        foreach (Collider2D lily in lilypads)
        {
            lily.GetComponent<MainLily>().ResetPosition();
        }

        currentWater = maxWater;
        waterBar.SetWater(currentWater);
    }
    void WaterVines()
    {
        Collider2D[] vines = CheckNearbyColliders("Vine", 1.5f, true);
        if (vines.Length > 0 && currentWater > 0)
        {
            vines[0].gameObject.GetComponent<MainVine>().Grow();

            currentWater -= waterSpeed * Time.deltaTime;
            waterBar.SetWater(currentWater);
        }
    }
    void MoveLilypads()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D[] lilypads = CheckNearbyColliders("Lilypad", 1.5f, true);
        if (lilypads.Length > 0 && currentWater > 0)
        {
            if (lilypads[0].gameObject.GetComponent<MainLily>().Accelerate(-facing)) {
                currentWater -= waterSpeed * Time.deltaTime;
                waterBar.SetWater(currentWater);
            }
        }
    }
    void SuckPlants()
    {
        Collider2D[] vines = CheckNearbyColliders("Vine", 1.5f, true);
        if (vines.Length > 0 && currentWater < maxWater)
        {
            if (vines[0].gameObject.GetComponent<MainVine>().Ungrow())
            {
                currentWater += waterSpeed * Time.deltaTime;
                waterBar.SetWater(currentWater);
            };
        }
    }
    bool OnVine()
    {
        return CheckNearbyColliders("Vine", 0.5f, true).Length > 0;
    }
    bool CheckSink()
    {
        Collider2D[] liquids = CheckNearbyColliders("Liquid", 0.5f, true);
        if (liquids.Length > 0)
        {
            return !!liquids[0];
        }

        return false;
    }
    bool CheckWin()
    {
        Collider2D[] flag = CheckNearbyColliders("Flag", 1f, true);
        if (flag.Length > 0)
        {
            return true;
        }

        return false;
    }
}