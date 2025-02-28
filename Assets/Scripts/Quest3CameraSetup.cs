using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

namespace BNG {
    /// <summary>
    /// Configures camera and tracking settings for Meta Quest 3
    /// </summary>
    public class Quest3CameraSetup : MonoBehaviour {
        
        [Header("Camera Setup")]
        [Tooltip("The main camera transform used for VR")]
        public Transform XRCameraTransform;
        
        [Tooltip("Set camera tracking origin to floor or device")]
        public TrackingOriginModeFlags TrackingOrigin = TrackingOriginModeFlags.Floor;
        
        [Header("Quest 3 Settings")]
        [Tooltip("Enable passthrough on Quest 3 devices")]
        public bool EnablePassthrough = false;
        
        [Tooltip("Enable increased hand tracking field of view on Quest 3")]
        public bool EnableWideFOVHandTracking = true;
        
        [Tooltip("Enable this to force Quest 3 settings even on other devices. Use for testing in editor.")]
        public bool ForceQuestSettings = false;

        void Start() {
            SetupCamera();
            
            // Configure Quest 3 specific settings
            ConfigureQuestDevice();
        }
        
        void SetupCamera() {
            if (XRCameraTransform == null) {
                if (Camera.main != null) {
                    XRCameraTransform = Camera.main.transform;
                }
                else {
                    Debug.LogWarning("No main camera found for XR setup");
                    return;
                }
            }
            
            // Set tracking origin mode
            SetTrackingOriginMode(TrackingOrigin);
        }
        
        void ConfigureQuestDevice() {
            bool isQuestDevice = IsQuestDevice();
            
            if (isQuestDevice || ForceQuestSettings) {
                if (EnablePassthrough) {
                    EnableQuestPassthrough();
                }
                
                if (EnableWideFOVHandTracking) {
                    EnableQuestHandTracking();
                }
            }
        }
        
        bool IsQuestDevice() {
            #if UNITY_ANDROID && !UNITY_EDITOR
                // Check for Quest device name
                string deviceName = SystemInfo.deviceName;
                if (deviceName.Contains("Quest")) {
                    return true;
                }
                
                // Check for Oculus / Meta device model
                string deviceModel = SystemInfo.deviceModel;
                if (deviceModel.Contains("Quest") || deviceModel.Contains("Oculus") || deviceModel.Contains("Meta")) {
                    return true;
                }
                
                // Check if XR device is Oculus
                return XRSettings.loadedDeviceName != null && 
                   (XRSettings.loadedDeviceName.ToLower().Contains("oculus") || 
                    XRSettings.loadedDeviceName.ToLower().Contains("meta"));
            #else
                // In editor, rely on XR settings
                return XRSettings.enabled && XRSettings.loadedDeviceName != null &&
                    (XRSettings.loadedDeviceName.ToLower().Contains("oculus") || 
                     XRSettings.loadedDeviceName.ToLower().Contains("meta"));
            #endif
        }
        
        void EnableQuestPassthrough() {
            #if OCULUS_XR_PLUGIN || UNITY_ANDROID
                // In a real implementation, use OVRManager to enable passthrough
                Debug.Log("Quest Passthrough feature enabled");
                
                /* Uncomment when using Oculus Integration package
                if (OVRManager.instance != null) {
                    // Enable passthrough
                    OVRManager.instance.isInsightPassthroughEnabled = true;
                    
                    // For Quest 3 color passthrough
                    if (OVRManager.instance.IsOVRManagerInstance) {
                        // Configure color settings as needed
                    }
                }
                */
                
                // If using Unity XR SDK directly without Oculus Integration, 
                // you would use other methods to enable passthrough
                
            #endif
        }
        
        void EnableQuestHandTracking() {
            #if OCULUS_XR_PLUGIN || UNITY_ANDROID
                // Enable hand tracking with wide FOV
                Debug.Log("Quest Hand Tracking with Wide FOV enabled");
                
                /* Uncomment when using Oculus Integration package
                if (OVRHand.HandTracking) {
                    OVRHand.HandTracking = true;
                    
                    // If using Quest 3, enable wide FOV tracking
                    OVRInput.Controller controllerType = OVRInput.GetActiveController();
                    // Check if Quest 3 and enable enhanced tracking
                }
                */
                
                // If using Unity XR SDK directly, you would configure this differently
                
            #endif
        }
        
        // Set tracking origin mode (floor or device)
        void SetTrackingOriginMode(TrackingOriginModeFlags trackingOrigin) {
            StartCoroutine(ChangeOriginModeRoutine(trackingOrigin));
        }
        
        System.Collections.IEnumerator ChangeOriginModeRoutine(TrackingOriginModeFlags trackingOrigin) {
            // Wait a frame for XR to initialize properly
            yield return null;
            
            List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(subsystems);
            
            if (subsystems.Count > 0) {
                for (int i = 0; i < subsystems.Count; i++) {
                    var supportedModes = subsystems[i].GetSupportedTrackingOriginModes();
                    bool supportsMode = (supportedModes & trackingOrigin) != 0;
                    
                    if (supportsMode) {
                        // Wait until subsystem is running
                        int attempts = 0;
                        while (!subsystems[i].running && attempts < 50) {
                            yield return new WaitForSeconds(0.1f);
                            attempts++;
                        }
                        
                        // Set tracking origin mode
                        if (subsystems[i].running) {
                            if (subsystems[i].TrySetTrackingOriginMode(trackingOrigin)) {
                                Debug.Log("Successfully set TrackingOriginMode to " + trackingOrigin);
                            }
                            else {
                                Debug.LogWarning("Failed to set TrackingOriginMode to " + trackingOrigin);
                            }
                        }
                    }
                }
            }
            else {
                Debug.LogWarning("No XR subsystems detected. Unable to set Tracking Origin.");
            }
        }
    }
}