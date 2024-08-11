using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuChangeButtonScript : MonoBehaviour
{
    private Button myButton;
    public TransitionHandler transitioner;
    public string sceneName;

    void Start()
    {
        // Get the Button component attached to this GameObject
        myButton = GetComponent<Button>();

        // Make sure the Button component is found
        if (myButton != null)
        {
            // Add a listener to the button's OnClick event
            myButton.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Button component not found on this GameObject.");
        }
    }

    // Method to handle the button click
    public void OnButtonClick()
    {
        StartCoroutine(LoadLevelCoroutine());
        // Add your button click handling code here
    }

    private IEnumerator LoadLevelCoroutine()
    {
        yield return transitioner.ExitTransition();
        Debug.Log("Loading level: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
