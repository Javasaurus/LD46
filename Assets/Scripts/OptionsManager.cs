using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager INSTANCE;

    public GameObject optionsPanel;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
     
        }
    }

    public void Hide()
    {
        optionsPanel.SetActive(false);
    }

    public void SHow()
    {
        optionsPanel.SetActive(true);
    }

}
