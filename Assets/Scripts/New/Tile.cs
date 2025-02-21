using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class Tile : MonoBehaviour
{  
    #region Events
    public event EventHandler OnTileChanged;
    #endregion

    public Material[] materials;
    public Renderer rend;

    private int index = 0;

    private bool trigger;

    private float visualDelay = 0.1f;

   private bool _playState;
   private bool _visualState;
   public bool PlayState {
      get { return _playState; }
   }
   public bool VisualState {
      get { return _visualState; }
   }
   
   public int channel;
   public int slot;

   void Start() {
      SetVisualState(false);
      SetPlayState(false);

        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = materials[0];
    }
   
   public void SetPlayState(bool state) {
      _playState = state;
        if (_playState)
        {
            index += 1; 
        }
        //index += 1; 
        //gameObject.GetComponentInChildren<CanvasGroup>().alpha = 0.7f;
        else
        {
            index = 0;
        }
           // index = 1; 
                       // gameObject.GetComponentInChildren<CanvasGroup>().alpha = 0f;

        Debug.Log("playState: " + channel + ", " + slot + ": " + _playState);
   }
   public void SetVisualState(bool state) {
      _visualState = state;
      Debug.Log("visualState: " + channel + ", " + slot + ": " + _visualState);
   }

   public void OnPlayHit() {
      StartCoroutine(PlayHit());
   }
   
   private IEnumerator PlayHit() {
      _visualState = true;
      index += 1;
        //gameObject.GetComponentInChildren<CanvasGroup>().alpha = 1.0f;
      yield return new WaitForSeconds(visualDelay);
      _visualState = false;

      // handle case where tapped during hilite
      if (_playState)
      {
           index += 1;
      }
        //gameObject.GetComponentInChildren<CanvasGroup>().alpha = 0.7f;
        else
        {
            index = 0;
        }
        //gameObject.GetComponentInChildren<CanvasGroup>().alpha = 0f;
   }

    //public void OnPointerDown(PointerEventData data)
    //{
    //   SetPlayState(!_playState);
    //   if (OnTileChanged != null) {
    //      TileChangedEventArgs args = new TileChangedEventArgs();
    //      args.channel = channel;
    //      args.slot = slot;
    //      args.state = _playState;
    //      OnTileChanged(this, args);
    //   }
    //}

    //public bool Trigger()
    //{
    //        SetPlayState(!_playState);
    //        if (OnTileChanged != null)
    //        {
    //            TileChangedEventArgs args = new TileChangedEventArgs();
    //            args.channel = channel;
    //            args.slot = slot;
    //            args.state = _playState;
    //            OnTileChanged(this, args);
    //        }
    //    return trigger;
    //}

    public void OnTriggerEnter(Collider other) // I ADDED THIS
    {
        if (other.gameObject.tag == "Box")
        {
            SetPlayState(!_playState);
            if (OnTileChanged != null)
            {
                rend.sharedMaterial = materials[0];
                TileChangedEventArgs args = new TileChangedEventArgs();
                args.channel = channel;
                args.slot = slot;
                args.state = _playState;
                OnTileChanged(this, args);
            }
            other.GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", Color.cyan);
           // other.GetComponentInChildren<ParticleSystem>().Play();
        }
    }

    public void OnTriggerExit(Collider other) // I ADDED THIS
    {
        if (other.gameObject.tag == "Box")
        {
            SetVisualState(false);
            SetPlayState(false);
            other.GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", Color.blue);
            //other.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    //public void OnTriggerExit(Collision other) // I ADDED THIS
    //{
    //    if (other.gameObject.tag == "Box")
    //    {
    //        if (materials.Length == 0)
    //        {
    //            return;
    //        }

    //        SetPlayState(!_playState);
    //        if (OnTileChanged != null)
    //        {
    //            rend.sharedMaterial = materials[0];
    //            TileChangedEventArgs args = new TileChangedEventArgs();
    //            args.channel = channel;
    //            args.slot = slot;
    //            args.state = _playState;
    //            OnTileChanged(this, args);
    //        }

    //        if (index == materials.Length + 1)
    //        {
    //            rend.sharedMaterial = materials[0];
    //        }

    //        rend.sharedMaterial = materials[1];
    //    }
    //}

    //public void OnCollisionEnter(Collision col) // I ADDED THIS
    //{
    //    if(col.gameObject.tag == "Box")
    //    {
    //        if (materials.Length == 0)
    //        {
    //            return;
    //        }

    //        SetPlayState(!_playState);
    //        if (OnTileChanged != null)
    //        {
    //            rend.sharedMaterial = materials[0];
    //            TileChangedEventArgs args = new TileChangedEventArgs();
    //            args.channel = channel;
    //            args.slot = slot;
    //            args.state = _playState;
    //            OnTileChanged(this, args);
    //        }

    //        if (index == materials.Length + 1)
    //        {
    //            rend.sharedMaterial = materials[0];
    //        }

    //        rend.sharedMaterial = materials[1];
    //    }

    //}

    //public void OnMouseDown() // I ADDED THIS
    //{
    //    //SetPlayState(!_playState);
    //    //if (OnTileChanged != null)
    //    //{
    //    //    TileChangedEventArgs args = new TileChangedEventArgs();
    //    //    args.channel = channel;
    //    //    args.slot = slot;
    //    //    args.state = _playState;
    //    //    OnTileChanged(this, args);
    //    //}

    //    if (materials.Length == 0)
    //    {
    //        return;
    //    }

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        SetPlayState(!_playState);
    //        if (OnTileChanged != null)
    //        {
    //            rend.sharedMaterial = materials[0];
    //            TileChangedEventArgs args = new TileChangedEventArgs();
    //            args.channel = channel;
    //            args.slot = slot;
    //            args.state = _playState;
    //            OnTileChanged(this, args);
    //        }
    //        //index += 1;
    //        //rend.sharedMaterial = materials[1];
    //        if (index == materials.Length + 1)
    //        {
    //            rend.sharedMaterial = materials[0];
    //            //index = 1;
    //        }

    //        //rend.sharedMaterial = materials[index-1];
    //        rend.sharedMaterial = materials[1];

    //        //SetPlayState(!_playState);
    //        //if (OnTileChanged != null)
    //        //{
    //        //    TileChangedEventArgs args = new TileChangedEventArgs();
    //        //    args.channel = channel;
    //        //    args.slot = slot;
    //        //    args.state = _playState;
    //        //    OnTileChanged(this, args);
    //        //}
    //    }
    //}


    #region Public Support Classes
    public class TileChangedEventArgs : EventArgs {
      public int channel { get; set; }
      
      public int slot { get; set; }
      
      public bool state { get; set; }
   }
   #endregion
}
