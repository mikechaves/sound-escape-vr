using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DisplayManager2 : MonoBehaviour {

   #region Static Properties
   public static DisplayManager2 Instance { get; set; }
   #endregion

   public enum PanelType {
      Sample2,
      Mix2,             
      Share2,
      Settings2
   }
   
   private PanelType _panelType2 = PanelType.Mix2;
   private GameObject _samplePanel2 = null;
   private GameObject _mixPanel2 = null;
   //private GameObject _samplePlayPanel = null;
   //private GameObject _sampleRecordPanel = null;
   private GameObject _mixTempoField2 = null;
   private InputField _mixTempoInput2 = null;
   private GameObject _imageMixPlay2 = null;
   private GameObject _imageMixPause2 = null;
  // private GameObject _ftueSampleField = null;
   //private InputField _ftueSampleInput = null;
  // private GameObject _ftueMixField = null;
   //private InputField _ftueMixInput = null;

   //private string _ftueSampleRecordText = "In Record Mode: tap a square to start and stop recording from mic";
   //private string _ftueSamplePlayText = "In Play Mode: tap squares to play your sounds";
   //private string _ftueSampleStartedRecordingText = "Recording: tap the square to stop recording this sound";
  // private string _ftueSampleStoppedRecordingText = "Done: switch to Play Mode to hear your new sound";

   //private string _ftueMixText = "Tap squares where you want sounds to play";

   //private string _ftueResetText = "All sounds have been reset";
   
   void Awake() {
      if(DisplayManager2.Instance == null)
        {
            DisplayManager2.Instance = this;
        }
   }

   // Use this for initialization
   void Start() {
      // main panels
      _samplePanel2 = GameObject.Find("SamplesPanel2");
      _mixPanel2 = GameObject.Find("MixPanel2");

      // bg tabs in sample panel
      //_samplePlayPanel = GameObject.Find("SamplePlayBg");
      //_sampleRecordPanel = GameObject.Find("SampleRecordBg");

      // text fields to manipulate
      _mixTempoField2 = GameObject.Find("MixTempoField2");
      _mixTempoInput2 = _mixTempoField2.GetComponent<InputField>();

        //_ftueSampleField = GameObject.Find("FtueSampleField");
        //_ftueSampleInput = _ftueSampleField.GetComponent<InputField>();

        //_ftueMixField = GameObject.Find("FtueMixField");
        // _ftueMixInput = _ftueMixField.GetComponent<InputField>();

      _imageMixPlay2 = GameObject.Find("ImageMixPlay2");
      _imageMixPause2 = GameObject.Find("ImageMixPause2");

      // now set the current
      SetPanels(_panelType2);

      // TODO: bind any event listeners here
   }
   
   // Update is called once per frame
   void Update() {
   
   }

   public void SetTempo(int tempo) {
      string tempoString = tempo.ToString();
      _mixTempoInput2.text = tempoString;
    }

    public void TogglePanel(GameObject panel, bool state)
    {
        if (state == true)
        {
            panel.GetComponent<CanvasGroup>().alpha = 1f;
            panel.GetComponent<CanvasGroup>().interactable = true;
            panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            panel.GetComponent<CanvasGroup>().alpha = 0f;
            panel.GetComponent<CanvasGroup>().interactable = false;
            panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void ToggleRecordingState(string stateOn) {
      if (stateOn == "started") {
         //_ftueSampleInput.text = _ftueSampleStartedRecordingText;
      }
      else {
        // _ftueSampleInput.text = _ftueSampleStoppedRecordingText;
      }
   }

   public void ToggleRecordMode(string stateOn) {
      if (stateOn == "record") {
         //_ftueSampleInput.text = _ftueSampleRecordText;
      }
      else {
        // _ftueSampleInput.text = _ftueSamplePlayText;
      }
   }

   //public void ToggleMixMode(string stateOn) {
   //   if (stateOn == "mix") {
   //      _ftueMixInput.text = _ftueMixText;
   //   }
   //}

   public void ConveyReset() {
      //_ftueSampleInput.text = _ftueResetText;
     // _ftueMixInput.text = _ftueResetText;
   }
   
   public void TogglePlayPause(string stateOn) {
      if (stateOn == "play") {
         _imageMixPlay2.GetComponent<CanvasGroup>().alpha = 1f;
         _imageMixPlay2.GetComponent<CanvasGroup>().interactable = true;
         _imageMixPlay2.GetComponent<CanvasGroup>().blocksRaycasts = true;
         _imageMixPause2.GetComponent<CanvasGroup>().alpha = 0f;
         _imageMixPause2.GetComponent<CanvasGroup>().interactable = false;
         _imageMixPause2.GetComponent<CanvasGroup>().blocksRaycasts = false;
      }
      else {
         _imageMixPause2.GetComponent<CanvasGroup>().alpha = 1f;
         _imageMixPause2.GetComponent<CanvasGroup>().interactable = true;
         _imageMixPause2.GetComponent<CanvasGroup>().blocksRaycasts = true;
         _imageMixPlay2.GetComponent<CanvasGroup>().alpha = 0f;
         _imageMixPlay2.GetComponent<CanvasGroup>().interactable = false;
         _imageMixPlay2.GetComponent<CanvasGroup>().blocksRaycasts = false;
      }
   }
   
   public void SetPanels(PanelType panelType) {
      
      // blank all panels
      TogglePanel(_mixPanel2, false);
      TogglePanel(_samplePanel2, false);
      
      // turn on the current one
      switch (panelType) {
      case PanelType.Mix2:
         TogglePanel(_mixPanel2, true);
         //ToggleMixMode("mix");
         // HACK: flaky if done from TileManager Start()
         TileManager2.Instance.RestoreState();
         break;
            case PanelType.Sample2:
                TogglePanel(_samplePanel2, true);

                // set sub panels
                if (SampleManager2.Instance.editMode)
                {
                    //TogglePanel(_samplePlayPanel, false);
                   // TogglePanel(_sampleRecordPanel, true);
                    //ToggleRecordMode("record");
                }
                else
                {
                    //TogglePanel(_samplePlayPanel, true);
                    //TogglePanel(_sampleRecordPanel, false);
                    //ToggleRecordMode("play");
                }
                break;
            default:
         Debug.Log("Found unknown panel type");
         break;
      }
      
      // record current
      _panelType2 = panelType; 
   }
}
