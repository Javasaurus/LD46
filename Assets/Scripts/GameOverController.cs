using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public BGMController bgmController;
    public GameObject GameOverFieldUI;
    public void SetGameOver()
    {
        Debug.Log("Game Over");
        HintsController.INSTANCE.ShowText("");
        GameOverFieldUI.SetActive(false);
        bgmController.SetGameOver();
        CharacterController.INSTANCE.resting = true;
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        Image overlay = GameObject.FindGameObjectWithTag("FadingOverlay")?.GetComponent<Image>();
        float a = 0;
        //fade in
        while (a < 1)
        {
            a += 0.01f;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, a);
            yield return null;
        }
        GameOverFieldUI.SetActive(true);
        CharacterController.INSTANCE.BabyObject.enabled = false;
    }

    public void Reload()
    {
        SceneManager.LoadScene(0);
    }

}
