using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BNG {
    /// <summary>
    /// Updates input profiles and settings specifically for Meta Quest 3 controllers
    /// </summary>
    public class Quest3ControllerProfileUpdater : MonoBehaviour {
        
        [Header("Input References")]
        [Tooltip("Reference to the Input Bridge for configuration")]
        public InputBridge InputBridgeRef;
        
        [Tooltip("Auto-detect and fix input issues on startup")]
        public bool AutoFixOnStart = true;
        
        [Header("Controller Settings")]
        [Tooltip("Controller haptic frequency - adjusted for Quest 3")]
        public float HapticFrequency = 0.3f;
        
        [Tooltip("Controller haptic amplitude - adjusted for Quest 3")]
        public float HapticAmplitude = 0.7f;
        
        [Header("Input Deadzone Settings")]
        [Tooltip("Thumbstick X deadzone for Quest 3")]
        [Range(0.001f, 0.2f)]
        public float ThumbstickDeadzoneX = 0.1f;
        
        [Tooltip("Thumbstick Y deadzone for Quest 3")]
        [Range(0.001f, 0.2f)]
        public float ThumbstickDeadzoneY = 0.1f;

        void Start() {
            if (AutoFixOnStart) {
                DetectAndFixInputIssues();
            }
        }
        
        public void DetectAndFixInputIssues() {
            // Find InputBridge if not already assigned
            if (InputBridgeRef == null) {
                InputBridgeRef = FindObjectOfType<InputBridge>();
                
                if (InputBridgeRef == null) {
                    Debug.LogWarning("No InputBridge found in scene. Some Quest 3 optimizations cannot be applied.");
                    return;
                }
            }
            
            // Check if we're on a Quest device
            if (IsQuestDevice()) {
                UpdateQuestControllerSettings();
            }
        }
        
        bool IsQuestDevice() {
            if (InputBridgeRef != null) {
                // Try to use InputBridge's detection methods
                if (InputBridgeRef.GetIsOculusDevice()) {
                    return true;
                }
                
                // Check if we can detect Quest 3 specifically
                try {
                    if (InputBridgeRef.GetIsMetaQuest3()) {
                        return true;
                    }
                }
                catch (System.Exception) {
                    // Method might not exist yet in older versions
                }
            }
            
            // Fallback to generic device checks
            #if UNITY_ANDROID && !UNITY_EDITOR
                string deviceName = SystemInfo.deviceName;
                if (deviceName.Contains("Quest")) {
                    return true;
                }
                
                return UnityEngine.XR.XRSettings.loadedDeviceName != null && 
                       (UnityEngine.XR.XRSettings.loadedDeviceName.ToLower().Contains("oculus") || 
                        UnityEngine.XR.XRSettings.loadedDeviceName.ToLower().Contains("meta"));
            #else
                // In editor, we can test based on XR settings
                #if UNITY_EDITOR
                return UnityEngine.XR.XRSettings.enabled && 
                       UnityEngine.XR.XRSettings.loadedDeviceName != null &&
                       (UnityEngine.XR.XRSettings.loadedDeviceName.ToLower().Contains("oculus") || 
                        UnityEngine.XR.XRSettings.loadedDeviceName.ToLower().Contains("meta"));
                #else
                return false;
                #endif
            #endif
        }
        
        void UpdateQuestControllerSettings() {
            if (InputBridgeRef == null) {
                return;
            }
            
            // Update input settings for Quest 3
            Debug.Log("Applying Quest 3 controller optimizations");
            
            // Set the right input source
            #if OCULUS_XR_PLUGIN
            InputBridgeRef.InputSource = XRInputSource.OVRInput;
            #else
            InputBridgeRef.InputSource = XRInputSource.XRInput;
            #endif
            
            // Update deadzones
            InputBridgeRef.ThumbstickDeadzoneX = ThumbstickDeadzoneX;
            InputBridgeRef.ThumbstickDeadzoneY = ThumbstickDeadzoneY;
            
            // Fix tracking origin to floor by default for Quest 3
            InputBridgeRef.TrackingOrigin = UnityEngine.XR.TrackingOriginModeFlags.Floor;
            InputBridgeRef.SetTrackingOriginMode(UnityEngine.XR.TrackingOriginModeFlags.Floor);
            
            // Update input events and features
            UpdateInputEvents();
        }
        
        void UpdateInputEvents() {
            // Find all components that might need updating for Quest 3 compatibility
            
            // Update grabbers for better Quest 3 response
            Grabber[] grabbers = FindObjectsOfType<Grabber>();
            foreach (Grabber grabber in grabbers) {
                // Update the grabber's DefaultGrabButton setting for Quest 3
                // Many users find that GripOrTrigger works better on Quest 3
                grabber.DefaultGrabButton = GrabButton.GripOrTrigger;
            }
            
            // Find and update GrabbablesInTrigger components
            GrabbablesInTrigger[] grabTriggers = FindObjectsOfType<GrabbablesInTrigger>();
            foreach (GrabbablesInTrigger trigger in grabTriggers) {
                // Enable Quest optimizations if they exist
                var questField = trigger.GetType().GetField("UseQuestOptimizations");
                if (questField != null) {
                    questField.SetValue(trigger, true);
                }
                
                // Improve remote grabbing for Quest 3
                trigger.CheckRemoteGrabbables = true;
                trigger.RaycastRemoteGrabbables = true;
                
                // More frequent updates are affordable on Quest 3
                trigger.UpdateGrabbableFrequency = 0.05f;
            }
            
            // Update RemoteGrabber settings
            RemoteGrabber[] remoteGrabbers = FindObjectsOfType<RemoteGrabber>();
            foreach (RemoteGrabber grabber in remoteGrabbers) {
                // Quest 3 controllers work well with raycasts
                grabber.PhysicsCheckType = RemoteGrabType.Raycast;
                grabber.TriggerRequiresRaycast = true;
                
                // Slightly increase raycast length for Quest 3 ergonomics
                grabber.RaycastLength = 20f;
            }
            
            // Update any HandCollision components
            HandCollision[] handCollisions = FindObjectsOfType<HandCollision>();
            foreach (HandCollision hand in handCollisions) {
                // Hand collision settings optimal for Quest 3
                hand.EnableHandCollision = true;
                hand.EnableCollisionOnPoint = true;
                
                // Disable collisions during grab to prevent physics glitches
                hand.EnableCollisionDuringGrab = false;
            }
        }
        
        // Method to apply haptic feedback optimized for Quest 3
        public void ApplyHapticFeedback(ControllerHand hand, float duration) {
            if (InputBridgeRef != null) {
                InputBridgeRef.VibrateController(HapticFrequency, HapticAmplitude, duration, hand);
            }
        }
        
        #if UNITY_EDITOR
        // Helper method to analyze scenes and fix issues
        [ContextMenu("Analyze Scene for Quest 3 Compatibility")]
        public void AnalyzeSceneForQuestCompatibility() {
            Debug.Log("Analyzing scene for Quest 3 compatibility...");
            
            // Check for InputBridge
            InputBridge input = FindObjectOfType<InputBridge>();
            if (input == null) {
                Debug.LogError("No InputBridge found in scene! This is required for Quest 3 input handling.");
            }
            else {
                Debug.Log("InputBridge found. Checking settings...");
                
                // Check input source
                if (input.InputSource != XRInputSource.XRInput && input.InputSource != XRInputSource.OVRInput) {
                    Debug.LogWarning("InputBridge is not set to XRInput or OVRInput. For Quest 3, use XRInput with Unity XR SDK or OVRInput with Oculus Integration.");
                }
                
                // Check tracking origin
                if (input.TrackingOrigin != UnityEngine.XR.TrackingOriginModeFlags.Floor) {
                    Debug.LogWarning("Tracking origin is not set to Floor. Floor tracking is recommended for Quest 3.");
                }
            }
            
            // Check for XR Camera Rig
            var cameraRigs = FindObjectsOfType<Transform>().Where(t => 
                t.name.Contains("XRRig") || 
                t.name.Contains("CameraRig") || 
                t.name.Contains("OVRCameraRig")).ToList();
                
            if (cameraRigs.Count == 0) {
                Debug.LogError("No XR Camera Rig found in scene! This is required for Quest 3 VR.");
            }
            else {
                Debug.Log($"Found {cameraRigs.Count} potential XR camera rig(s).");
            }
            
            // Check for potential performance issues
            var particleSystems = FindObjectsOfType<ParticleSystem>();
            if (particleSystems.Length > 10) {
                Debug.LogWarning($"Found {particleSystems.Length} particle systems. Consider optimizing for Quest 3 performance.");
            }
            
            // Check for lights
            var lights = FindObjectsOfType<Light>();
            if (lights.Length > 4) {
                Debug.LogWarning($"Found {lights.Length} lights. Quest 3 performs best with 4 or fewer dynamic lights.");
            }
            
            Debug.Log("Analysis complete. See console for details.");
        }
        #endif
    }
}