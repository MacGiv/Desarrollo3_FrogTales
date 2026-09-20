using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ProjectVersionDisplay : MonoBehaviour
{
    private void Start()
    {
        TMP_Text textComponent = GetComponent<TMP_Text>();
        textComponent.text = Application.version;
    }
}