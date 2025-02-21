using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class EventsMainMenuMix : MonoBehaviour, IPointerDownHandler {
   
   private static Canvas _canvas = null;
   
   void Start() {
      if (!_canvas)
         _canvas = FindObjectOfType(typeof(Canvas)) as Canvas;
   }
   
   public void OnPointerDown(PointerEventData data) {
      // handle tap events for this segment of ux
      switch (this.gameObject.name) {
      case "BtnStart":
         StartGame();
         break;
      case "BtnQuit":
         QuitGame();
         break;
      default:
         Debug.Log("Found unknown mix header button name");
         break;
      }
   }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    //public IEnumerator StartGame()
    //{
    //    yield return new WaitForSeconds(3.0f);
    //    SceneManager.LoadScene(1);
    //}

    public void QuitGame()
    {
        Application.Quit();
    }
}
