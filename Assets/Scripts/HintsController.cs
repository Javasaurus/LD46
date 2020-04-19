using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class HintsController : MonoBehaviour
{
    public static HintsController INSTANCE;
    private TextMeshProUGUI textField;

    private float textDisplayTimer = 0;
    private readonly float textDisplayInterval = 4f;



    public void Awake()
    {
        INSTANCE = this;
        textField = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void ShowText( string text )
    {
        textField.text = text;
        textDisplayTimer = Time.time + textDisplayInterval;
    }


    public void Update()
    {
        if (Time.time > textDisplayTimer)
        {
            textField.text = "";
        }
    }


}
