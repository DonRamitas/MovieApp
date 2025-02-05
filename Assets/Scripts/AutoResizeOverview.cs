using UnityEngine;
using TMPro;

// This class makes sure to resize TMP_Text height, based on its content
// I decided to use it because some movie overviews were too long

public class AutoResizeOverview : MonoBehaviour
{
    [Header("References")]
    private TMP_Text tmpText;
    private RectTransform rectTransform;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Change TMP_Text transform's size delta according to the overview lines
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, tmpText.preferredHeight);
    }
}