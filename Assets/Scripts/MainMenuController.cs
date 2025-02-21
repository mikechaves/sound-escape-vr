using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance;

    // Reference to the generated Input Actions
    private MenuControls controls;

    void Awake()
    {
        // Singleton pattern (optional)
        Instance = this;
        
        // Initialize the input controls in Awake so that they're ready
        // by the time OnEnable is called
        controls = new MenuControls();
    }

    void OnEnable()
    {
        // If controls is null, something went wrong with initialization
        if (controls == null)
        {
            Debug.LogError("MenuControls is null in OnEnable(). Check if the Input Action Asset is set up correctly.");
            return;
        }

        // Bind the Quit action
        if (controls.UI.Quit != null)
        {
            controls.UI.Quit.performed += ctx => QuitGame();
            controls.UI.Quit.Enable();
            Debug.Log("Quit action bound successfully.");
        }
        else
        {
            Debug.LogError("Quit action not found in MenuControls.");
        }

        // Bind the Start action
        if (controls.UI.StartGame != null)
        {
            controls.UI.StartGame.performed += ctx => StartGame();
            controls.UI.StartGame.Enable();
            Debug.Log("StartGame action bound successfully.");
        }
        else
        {
            Debug.LogError("StartGame action not found in MenuControls.");
        }
    }

    void OnDisable()
    {
        // Disable actions when the object is disabled
        if (controls != null)
        {
            if (controls.UI.Quit != null) controls.UI.Quit.Disable();
            if (controls.UI.StartGame != null) controls.UI.StartGame.Disable();
        }
    }

    void Update()
    {
        // If VR device is not active, allow keyboard shortcuts (for testing in Editor)
        if (!XRSettings.isDeviceActive)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                QuitGame();
            }
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                StartGame();
            }
        }
    }

    public void StartGame()
    {
        // Assumes scene index 1 is your game level
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        // Quits the application; will do nothing in the Editor
        Application.Quit();
    }
}
