using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneLoader : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene(2);
    }
}
