using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    [Header("Resources")]
    public Sprite noImage; // Image that shows up when no image is available

    [Header("Constants")]
    private const string IMAGE_URL = "https://image.tmdb.org/t/p/w500/";

    // Method that loads an specific network image into an specific Image component
    public void LoadImage(string path, Image targetImage)
    {
        StartCoroutine(LoadImageCoroutine(path, targetImage));
    }

    // A coroutine is needed because of the async nature of loading a network image
    private IEnumerator LoadImageCoroutine(string path, Image targetImage)
    {
        // If the path given isn't valid, load the fallback image
        if (string.IsNullOrEmpty(path))
        {
            targetImage.sprite = noImage;
            yield break;
        }

        // If the path was valid, show a loading icon while the image is being loaded
        GameObject loadingGO = targetImage.transform.GetChild(0).gameObject;
        loadingGO.SetActive(true);

        // To replace the current target Image component sprite
        targetImage.sprite = null;

        string fullUrl = IMAGE_URL + path;

        // This verifies if there's a cached image based on the requested image url, if it exists, load from cache
        if (ImageCache.TryGetImage(fullUrl, out Texture2D cachedTexture))
        {
            targetImage.sprite = TextureToSprite(cachedTexture);
            targetImage.preserveAspect = true; // I make sure to enable preserve aspect for the posters to look good
            loadingGO.SetActive(false); // Stop loading
            yield break;
        }

        // I decided to use UnityWebRequest this time because it's better for realtime image downloading
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullUrl);
        yield return request.SendWebRequest();

        // If the request was successful, and the target Image still exists, load the image into the poster
        // I verify if the target image still exists because navigating pages too fast could cause some issues
        if (request.result == UnityWebRequest.Result.Success && targetImage != null)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            ImageCache.AddImage(fullUrl, texture); // Guarda en caché
            targetImage.sprite = TextureToSprite(texture);
            targetImage.preserveAspect = true;
        }
        else // If there was an error loading the image, just show the fallback image
        {
            if (targetImage != null)
            {
                targetImage.sprite = noImage;
            }
        }

        loadingGO.SetActive(false); // Stop loading
    }

    // A small method that turns a texture into a sprite
    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}

// Class that stores images cache in a dictionary, based on their url
public static class ImageCache
{
    private static Dictionary<string, Texture2D> cachedImages = new Dictionary<string, Texture2D>();

    // Search in the cache dictionary if a cached image that matches the url key exists, return that if it does
    public static bool TryGetImage(string url, out Texture2D texture)
    {
        return cachedImages.TryGetValue(url, out texture);
    }

    // Stores an image (in this case through a texture) to the image cache
    public static void AddImage(string url, Texture2D texture)
    {
        if (!cachedImages.ContainsKey(url))
        {
            cachedImages[url] = texture;
        }
    }

    // Clear the images cache, in this case is not used, but it's important to have this method in larger apps
    public static void ClearCache()
    {
        cachedImages.Clear();
    }
}
