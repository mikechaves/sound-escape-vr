using UnityEngine;
using System.Collections;

public class LocationBar : MonoBehaviour {

   #region Static Properties
   public static LocationBar Instance { get; set; }
   #endregion

   public float _rangeWidth = 0f;
   private float _origOffset = 0f;
   private float _anchoredY = 0f;
   private GameObject _locationBar = null;
   private GameObject _locationBar2 = null;
    private GameObject _locationBar3 = null;

    public float RangeWidth {
      get { return _rangeWidth; }
   }

   public void ToggleState(bool state) {
      if (state == true) {
         _locationBar.GetComponent<CanvasGroup>().alpha = 1f;
         _locationBar2.GetComponent<CanvasGroup>().alpha = 1f;
            _locationBar3.GetComponent<CanvasGroup>().alpha = 1f;
        }
      else {
         _locationBar.GetComponent<CanvasGroup>().alpha = 0f;
         _locationBar2.GetComponent<CanvasGroup>().alpha = 0f;
            _locationBar3.GetComponent<CanvasGroup>().alpha = 0f;
        }
   }

   public void Reset() {
      float currX = _origOffset;
      _locationBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, _anchoredY);
        _locationBar2.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, _anchoredY);
        _locationBar3.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, _anchoredY);
    }

   public void SetDisplayRange(float xOffset, float tileWidth, float totalWidth) {
      _origOffset = xOffset;
      _rangeWidth = totalWidth;

      if (_locationBar && _locationBar2 && _locationBar3)
         Reset();
   }

   void Awake() {
      if (LocationBar.Instance == null)
         LocationBar.Instance = this;
   }

   // Use this for initialization
   void Start() {
        // init our private vars
        _locationBar = GameObject.Find("LocationBar");
        _locationBar2 = GameObject.Find("LocationBar2");
        _locationBar3 = GameObject.Find("LocationBar3");
        _anchoredY = _locationBar.GetComponent<RectTransform>().anchoredPosition.y;
        _anchoredY = _locationBar2.GetComponent<RectTransform>().anchoredPosition.y;
        _anchoredY = _locationBar3.GetComponent<RectTransform>().anchoredPosition.y;

        // keep on top of render order
        _locationBar.transform.SetAsLastSibling();
        //_locationBar2.transform.SetAsLastSibling(); ///////////////////////////////////////////////////////

      ToggleState(false);
      Reset();
   }

   public void UpdatePosition(int slot) {
      float currX = _origOffset + _rangeWidth * (float)(slot*1f / 16f);
      _locationBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, _anchoredY);
        _locationBar2.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, _anchoredY);
        _locationBar3.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, _anchoredY);
    }
}
