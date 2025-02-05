using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

// This class initializes and prompts essential parameters for the app functionality, it's used in the first scene

public class Init : MonoBehaviour{

    [Header("References")]
    public TMP_InputField apiKeyInputField;
    public Button submitButton;
    public GameObject messageGO;
    public Image fadeOutImage;

    void Start(){

        // Makes sure that the fade out image starts with 0 alpha
        fadeOutImage.CrossFadeAlpha(0,0,false);

        // If the user used a valid API key before, the input field fills itself automatically with that key
        if(PlayerPrefs.HasKey("apiKey")){
            apiKeyInputField.text = PlayerPrefs.GetString("apiKey");
        }

        // This allows the user to use the Enter button to submit their key
        apiKeyInputField.onEndEdit.AddListener(OnInputSubmit);
        
        // These lines removes the 60 framerate limit in some (the majority) mobile devices
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;
    }

    // Enables the submit button only when something is written on the input field
    void Update(){
        if(apiKeyInputField.text.Trim() == ""){
            submitButton.interactable = false;
        }else{
            submitButton.interactable = true;
        }
    }

    // This allows the user to use the Enter button to submit their key
    private void OnInputSubmit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            EnterApiKey();
        }
    }

    // Submit and validates the entered API key, access the next scene if valid, show an error message if not
    public async void EnterApiKey(){
        messageGO.SetActive(false);
        string input = apiKeyInputField.text.Trim();
        bool valid = await APIHelper.TestAPIKey(input);
        if(valid){
            // If the API key was valid, it's stored in PlayerPrefs for future use
            PlayerPrefs.SetString("apiKey",input);
            PlayerPrefs.Save();
            FadeOut(); // Smooth transition to next scene
            
        }else{
            messageGO.SetActive(true);
        }
    }

    // Allows the user to use the developer's (Axel Neumann) API key
    public void UseDevApiKey(){
        apiKeyInputField.text = "666d00beb8a3dc5d87ddf5259ab016b0";
    }

    // Starts the fade transition
    void FadeOut(){
        fadeOutImage.CrossFadeAlpha(1f,1f,false);
        StartCoroutine(FadeOutDelay());
    }

    IEnumerator FadeOutDelay(){
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("SearchScene");
    }
}