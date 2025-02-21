using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DisplayManager : MonoBehaviour {

   #region Static Properties
   public static DisplayManager Instance { get; set; }
   #endregion

   public enum PanelType {
      Sample,
      Mix,             
      Share,
      Settings
   }
   
   private PanelType _panelType = PanelType.Mix;
   private GameObject _samplePanel = null;
   private GameObject _mixPanel = null;
   private GameObject _mixTempoField = null;
   private InputField _mixTempoInput = null;
   private GameObject _imageMixPlay = null;
   private GameObject _imageMixPause = null;
   
   void Awake() {
      if (DisplayManager.Instance == null)
         DisplayManager.Instance = this;
   }

   // Use this for initialization
   void Start() {
      // main panels
      _samplePanel = GameObject.Find("SamplesPanel");
      _mixPanel = GameObject.Find("MixPanel");

      // text fields to manipulate
      _mixTempoField = GameObject.Find("MixTempoField");
      _mixTempoInput = _mixTempoField.GetComponent<InputField>();

      _imageMixPlay = GameObject.Find("ImageMixPlay");
      _imageMixPause = GameObject.Find("ImageMixPause");

      // now set the current
      SetPanels(_panelType);
   }
   
   // Update is called once per frame
   void Update() {
   
   }

   public void SetTempo(int tempo) {
      string tempoString = tempo.ToString();
      _mixTempoInput.text = tempoString;
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
   
   public void TogglePlayPause(string stateOn) {
      if (stateOn == "play") {
         _imageMixPlay.GetComponent<CanvasGroup>().alpha = 1f;
         _imageMixPlay.GetComponent<CanvasGroup>().interactable = true;
         _imageMixPlay.GetComponent<CanvasGroup>().blocksRaycasts = true;
         _imageMixPause.GetComponent<CanvasGroup>().alpha = 0f;
         _imageMixPause.GetComponent<CanvasGroup>().interactable = false;
         _imageMixPause.GetComponent<CanvasGroup>().blocksRaycasts = false;
      }
      else {
         _imageMixPause.GetComponent<CanvasGroup>().alpha = 1f;
         _imageMixPause.GetComponent<CanvasGroup>().interactable = true;
         _imageMixPause.GetComponent<CanvasGroup>().blocksRaycasts = true;
         _imageMixPlay.GetComponent<CanvasGroup>().alpha = 0f;
         _imageMixPlay.GetComponent<CanvasGroup>().interactable = false;
         _imageMixPlay.GetComponent<CanvasGroup>().blocksRaycasts = false;
      }
   }
   
   public void SetPanels(PanelType panelType) {
      
      // blank all panels
      TogglePanel(_mixPanel, false);
      TogglePanel(_samplePanel, false);
      
      // turn on the current one
      switch (panelType) {
      case PanelType.Mix:
         TogglePanel(_mixPanel, true);
         //ToggleMixMode("mix");
         // HACK: flaky if done from TileManager Start()
         TileManager.Instance.RestoreState();
         break;
      case PanelType.Sample:
          TogglePanel(_samplePanel, true);
          break;
      default:
         Debug.Log("Found unknown panel type");
         break;
      }
      
      // record current
      _panelType = panelType; 
   }
}
