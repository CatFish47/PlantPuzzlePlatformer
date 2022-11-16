using UnityEngine;

public class LevelSelector : MonoBehaviour
{ 

    public void Select(string leveName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(leveName);
    }
}
