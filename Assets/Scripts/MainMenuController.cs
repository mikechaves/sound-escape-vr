using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using System;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance;

    // Reference to the generated Input Actions
    private MenuControls controls;
    
    // Backup controls in case the primary input fails
    private bool useBackupControls = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize the input controls in Awake so that they're ready
        // by the time OnEnable is called
        try {
            controls = new MenuControls();
        }
        catch (Exception e) {
            Debug.LogError("Failed to initialize MenuControls: " + e.Message);
            useBackupControls = true;
        }
    }

    void OnEnable()
    {
        if (!useBackupControls && controls != null) {
            try {
                // Bind the Quit action
                if (controls.UI.Quit != null)
                {
                    controls.UI.Quit.performed += ctx => QuitGame();
                    controls.UI.Quit.Enable();
                    Debug.Log("Quit action bound successfully.");
                }
                else
                {
                    Debug.LogWarning("Quit action not found in MenuControls.");
                    useBackupControls = true;
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
                    Debug.LogWarning("StartGame action not found in MenuControls.");
                    useBackupControls = true;
                }
            }
            catch (Exception e) {
                Debug.LogError("Failed to bind input actions: " + e.Message);
                useBackupControls = true;
            }
        }
    }

    void OnDisable()
    {
        // Disable actions when the object is disabled
        if (!useBackupControls && controls != null)
        {
            try {
                if (controls.UI.Quit != null) controls.UI.Quit.Disable();
                if (controls.UI.StartGame != null) controls.UI.StartGame.Disable();
            }
            catch (Exception e) {
                Debug.LogError("Error disabling controls: " + e.Message);
            }
        }
    }

    void Update()
    {
        // If using backup controls due to input system failure, check for Meta Quest controller buttons
        if (useBackupControls)
        {
            CheckBackupControls();
        }
        // If not in VR or for testing in Editor
        else if (!XRSettings.isDeviceActive)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                QuitGame();
            }
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                StartGame();
            }
        }
    }

    private void CheckBackupControls()
    {
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevices(inputDevices);
        
        foreach (var device in inputDevices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                bool primaryButtonPressed = false;
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out primaryButtonPressed) && primaryButtonPressed)
                {
                    StartGame();
                    return;
                }
                
                bool menuButtonPressed = false;
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out menuButtonPressed) && menuButtonPressed)
                {
                    QuitGame();
                    return;
                }
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        // In editor, we can just stop play mode
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}