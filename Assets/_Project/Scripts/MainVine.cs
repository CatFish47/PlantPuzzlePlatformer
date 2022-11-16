using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainVine : MonoBehaviour
{
    private float start_x;
    private float start_y;
    private float start_height;

    public GameObject vineTop;
    public GameObject vineMain;
    // Start is called before the first frame update
    void Start()
    {
        start_x = transform.position.x;
        start_y = transform.position.y;
        start_height = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Grow()
    {
        SpriteRenderer sprRend = gameObject.GetComponent<SpriteRenderer>();
        float increment = Time.deltaTime / 2;
        sprRend.size = new Vector2(sprRend.size.x, sprRend.size.y + increment);
        transform.position = new Vector2(transform.position.x, transform.position.y + increment / 2 * vineMain.transform.localScale.y);
        vineTop.transform.position = new Vector2(vineTop.transform.position.x, vineTop.transform.position.y + increment * vineMain.transform.localScale.y);
    }
    public bool Ungrow()
    {
        SpriteRenderer sprRend = gameObject.GetComponent<SpriteRenderer>();
        float increment = Time.deltaTime / 2;
        float limit = 0.32f;
        Debug.Log(sprRend.size.y - increment);
        if (sprRend.size.y - increment >= limit)
        {
            sprRend.size = new Vector2(sprRend.size.x, sprRend.size.y - increment);
            transform.position = new Vector2(transform.position.x, transform.position.y - increment / 2 * vineMain.transform.localScale.y);
            vineTop.transform.position = new Vector2(vineTop.transform.position.x, vineTop.transform.position.y - increment * vineMain.transform.localScale.y);

            return true;
        }
        return false;
    }
    public void ResetPosition()
    {
        transform.position = new Vector2(start_x, start_y);
        transform.localScale = new Vector2(transform.localScale.x, start_height);
    }
}
