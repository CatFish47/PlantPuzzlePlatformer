using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotBarController : MonoBehaviour
{

    public int hotbarSize => gameObject.transform.childCount;

    KeyCode[] hotbarKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < hotbarKeys.Length; ++i)
        {
            if (Input.GetKeyDown(hotbarKeys[i]))
            {
                Debug.Log("Use Item: " + i);
                // Use Item
                return;
            }
        }
    }
}
