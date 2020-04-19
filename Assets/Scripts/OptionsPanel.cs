using UnityEngine;

public class OptionsPanel : MonoBehaviour
{
    public void applyOptions()
    {
        OptionsManager.INSTANCE.Hide();
    }
}
