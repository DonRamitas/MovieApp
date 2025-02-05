using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This class allows the user to use the device back button to get out of the movie data screen

public class BackButton : MonoBehaviour
{
    [Header("References")]
    public PosterAnimation posterAnimation;

    void Update()
    {
        // If the user press the device back button and there's no poster animation playing, go back
        if(Input.GetKeyDown(KeyCode.Escape) && this.gameObject.activeSelf && !posterAnimation.isMoving){
            EmulateClick();
        }
    }

    // I use this method to emulate all events entered in the Button/EventTrigger component of the Back Button
    // EventTrigger is useful to give functionality to Image UI components, but in this case it wasn't used
    void EmulateClick(){

        if(this.gameObject.GetComponent<Button>()!=null){
            this.gameObject.GetComponent<Button>().onClick.Invoke();
            return;
        }

        
        if (!this.gameObject.TryGetComponent<EventTrigger>(out var eventTrigger))
        {
            return;
        }

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);

        foreach (EventTrigger.Entry entry in eventTrigger.triggers)
        {
            if (entry.eventID == EventTriggerType.PointerClick || entry.eventID == EventTriggerType.PointerDown)
            {
                entry.callback.Invoke(pointerEventData);
            }
        }
    }
}
