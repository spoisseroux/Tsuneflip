using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverToggleRawImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RawImage targetImage;

    // This method is called when the pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.enabled = true;
        }
    }

    // This method is called when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.enabled = false;
        }
    }
}