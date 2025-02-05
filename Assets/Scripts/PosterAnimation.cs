using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class manages the poster animation when selecting a movie from the search results

public class PosterAnimation : MonoBehaviour
{

    [Header("References")]
    private RectTransform startPoster;
    public RectTransform endPoster;
    public RectTransform endPosterL;
    private GameObject posterGO;
    private RectTransform posterTransform;
    private Image posterImage;

    [Header("Resources")]
    public AnimationCurve movementCurve;

    [Header("Variables")]
    public bool isMoving = false;
    float elapsedTime = 0;
    float duration = 0.25f;

    void Awake()
    {
        posterGO = transform.GetChild(0).gameObject;
        posterTransform = posterGO.GetComponent<RectTransform>();
        posterImage = posterGO.GetComponent<Image>();
    }

    // If isMoving and elapsedTime didn't reach the max duration, it means that the animation started and is currently playing
    // To make the animation, I lerp the values of origin/target position and origin/target size
    // Also, a spin animation for the poster is played
    void Update()
    {
        if (isMoving && startPoster != null)
        {
            elapsedTime += Time.deltaTime;

            // Apply a custom curve
            float t = elapsedTime / duration;
            t = movementCurve.Evaluate(t);

            // I get the global position of origin/target and move between them
            Vector2 startPosition = RectTransformUtility.WorldToScreenPoint(null, startPoster.position);
            Vector2 endPosition;

            if(Screen.height >= Screen.width){
                endPosition = RectTransformUtility.WorldToScreenPoint(null, endPoster.position);
            }else{
                endPosition = RectTransformUtility.WorldToScreenPoint(null, endPosterL.position);
            }

            Vector2 newPosition = Vector2.Lerp(startPosition, endPosition, t);

            // I needed to use world based coordinates, because the animated poster wasn't a sibling of the origin/target poster
            Vector3 worldPosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(posterTransform.parent as RectTransform, newPosition, null, out worldPosition);
            posterTransform.position = worldPosition;

            // Interpolate poster sizes too
            if(Screen.height >= Screen.width){
                posterTransform.sizeDelta = Vector2.Lerp(startPoster.sizeDelta, endPoster.sizeDelta, t);
            }else{
                posterTransform.sizeDelta = Vector2.Lerp(startPoster.sizeDelta, endPosterL.sizeDelta, t);
            }

            // Stop the animation once elapsedTime reaches duration
            if (elapsedTime >= duration)
            {
                isMoving = false;
                startPoster = null;
                posterGO.SetActive(false);
                if(Screen.height >= Screen.width){
                    endPoster.GetComponent<Image>().enabled = true;
                }else{
                    endPosterL.GetComponent<Image>().enabled = true;
                }
            }
        }
    }

    // This method triggers the animation and defines the starting position
    public void PlayAnimation(RectTransform originPosition)
    {
        // Play the animation if it's not playing already
        if (!isMoving)
        {
            // Decide the target poster based on device orientation
            if(Screen.height >= Screen.width){
                endPoster.GetComponent<Image>().enabled = false;
            }else{
                endPosterL.GetComponent<Image>().enabled = false;
            }
            
            // The animated poster uses the starting poster sprite
            posterImage.sprite = originPosition.GetComponent<Image>().sprite;

            posterGO.SetActive(true);
            
            // Set starting parameters
            startPoster = originPosition;
            elapsedTime = 0f;
            isMoving = true;

            // Set the position and size of the animated poster at the beginning point (using world space again)
            Vector2 startPosition = RectTransformUtility.WorldToScreenPoint(null, startPoster.position);
            Vector3 worldPosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(posterTransform.parent as RectTransform, startPosition, null, out worldPosition);
            posterTransform.position = worldPosition;
            posterTransform.sizeDelta = startPoster.sizeDelta;
        }
    }
}
