using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(AudioSource))]
public class SampleManager3 : MonoBehaviour {

   #region Static Properties
   public static SampleManager3 Instance { get; set; }
   #endregion

   public enum OriginType {
      Default,
      Mix,             
      Mic,
      None
   }

   // TODO: change this to relate to intent of mixload
   // MixLoadIntent.Restore
   // MixLoadIntent.New
   public static bool inAsyncRestore = false;

   public int maxChannels = 16;
   public int numChannels = 4;
   public bool editMode;
   public AudioClip[] soundClips;
   public OriginType[] soundOrigins;
   public string lastMixToken = null;

   public AudioSource[] soundSources;
   private GameObject[] buttonGOs;
   private bool inRecord = false;
   private int channelInRecord = -1;
   private Image imageInRecord = null;
   private double recordStartTime = 0f;
   private double recordEndTime = 0f;
   private double recordMaxDuration = 6f;

    AudioListener _audioListener;

    // is there a connected microphone?
    private bool micConnected = false;

   // maximum and minimum available recording frequencies  
   private int minFreq;
   private int maxFreq;
   private int maxRecordSec = 6;
   private string[] defaultSoundAssetnames = new string[4] {
      "80sSynthwave/bass_G#5",
      "80sSynthwave/bass_B4",
      "80sSynthwave/bass_G#4",
      "80sSynthwave/bass_D#4"
   };
   private Dictionary<string, object> localState = new Dictionary<string, object>();
   private string stateKey = "soundState";

    public static float[] _samplesLeft = new float[512];
    public static float[] _samplesRight = new float[512];

    float[] _freqBand = new float[8];
    float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];

    float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    public static float _Amplitude, _AmplitudeBuffer;
    float _AmplitudeHighest;
    public float _audioProfile;

    public enum _channel { Stereo, Left, Right };
    public _channel channel = new _channel();

    void Awake() {
      if (SampleManager3.Instance == null)
         SampleManager3.Instance = this;
   }

   // Use this for initialization
   void Start() {

      soundClips = new AudioClip[numChannels];
      soundSources = new AudioSource[numChannels];
      soundOrigins = new OriginType[numChannels];
      buttonGOs = new GameObject[numChannels];
      
      // initialize default audio sources
      for (int i = 0; i < numChannels; i++) {
         GameObject child = new GameObject("Sound2" + i);
         child.transform.parent = gameObject.transform;
         soundSources[i] = child.AddComponent<AudioSource>() as AudioSource;
         
         if (defaultSoundAssetnames.Length >= (i + 1)) {
            soundSources[i].clip = Resources.Load(defaultSoundAssetnames[i], typeof(AudioClip)) as AudioClip;
            soundOrigins[i] = OriginType.Default;
         }
         else {
            soundOrigins[i] = OriginType.None;
         }
      }

      editMode = false;

      // get refs to the GOs for each sample button
      for (int i = 0; i < numChannels; i++) {
         GameObject go = GameObject.Find("SampleButton" + i);
         buttonGOs[i] = go;
      }

      if (!MicCheck()) {
         // TODO: handle the case where there is no mic
      }

      // restore prev state of samples
      RestoreState();

        _audioListener = GameObject.Find("CenterEyeAnchor").GetComponent<AudioListener>();
   }

   // public method to revert to default sample state
   public void ResetSamples() {
      SetStateFromDefault();
   }

   // initialize default audio sources for all
   private void SetStateFromDefault() {
      for (int i = 0; i < numChannels; i++) {
         SetSlotFromDefault(i);
      }
   }

   // initialize one default audio source
   private void SetSlotFromDefault(int i) {
      if (defaultSoundAssetnames.Length >= (i + 1)) {
         soundSources[i].clip = Resources.Load(defaultSoundAssetnames[i], typeof(AudioClip)) as AudioClip;
         soundOrigins[i] = OriginType.Default;
         SaveSlot(i, defaultSoundAssetnames[i], "default", defaultSoundAssetnames[i]);
      }
      else {
         // blank out the clip
         soundSources[i].clip = null;
         soundOrigins[i] = OriginType.None;
      }
   }

   // initialize one audio source from mic recording
   private void SetSlotFromMic(string filename, int i) {
      if (numChannels >= (i + 1)) {
         string url = "file://" + Application.persistentDataPath + "/" + filename + ".wav";
         StartCoroutine(SetClipFromUrl(url, i));
      }
      else {
         // blank out the clip
         soundSources[i].clip = null;
         soundOrigins[i] = OriginType.None;
      }
   }

   private void RestoreState() {
      if (String.IsNullOrEmpty(PlayerPrefs.GetString(stateKey))) {
         SetStateFromDefault();
         return;
      }

      RestoreStateSync();
   }

   // restore audio clips from available sources
   private void RestoreStateSync() {
      var stateInfo = MiniJSON.Json.Deserialize(PlayerPrefs.GetString(stateKey)) as Dictionary<string, object>;
      for (int i = 0; i < numChannels; i++) {
         if (stateInfo.ContainsKey(i.ToString()) == false) {
            SetSlotFromDefault(i);
            continue;
         }
         
         var tmpSlot = stateInfo[i.ToString()] as Dictionary<string, object>;
         var tmpType = tmpSlot["type"] as string;
         var tmpSrc = tmpSlot["src"] as string;
         
         localState[i.ToString()] = tmpSlot;
         
         if (tmpType == "default") {
            SetSlotFromDefault(i);
         }
         else if (tmpType == "mic") {
            SetSlotFromMic(tmpSrc, i);
         }
      }
      inAsyncRestore = false;
   }

   private void SaveSlot(int i, string name, string type, string src) {
      var slot = new Dictionary<string, object>();
      slot.Add("name", name);
      slot.Add("type", type);
      slot.Add("src", src);
      localState[i.ToString()] = slot;
      SaveState(localState);
   }
   
   private void SaveState(Dictionary<string, object> stateObj) {
      PlayerPrefs.SetString(stateKey, MiniJSON.Json.Serialize(stateObj));
   }

   public Dictionary<string, object> GetState() {
      var stateInfo = MiniJSON.Json.Deserialize(PlayerPrefs.GetString(stateKey)) as Dictionary<string, object>;
      return stateInfo;
   }

   // tidy up when it's time to stop recording
   public void FinishRecording() {
      if (!inRecord)
         return;

      Microphone.End(null); // stop the audio recording
      
      string tmpName = "s" + channelInRecord;
      AudioHelper.Save(tmpName, soundSources[channelInRecord].clip);

      SaveSlot(channelInRecord, tmpName, "mic", tmpName);

      // update visual state
      Component[] groups = buttonGOs[channelInRecord].GetComponentsInChildren<CanvasGroup>();
      foreach (CanvasGroup group in groups) {
         group.alpha = 0f;
      }
      imageInRecord.fillAmount = 0f;

      // update flags
      inRecord = false;
      channelInRecord = -1;
      imageInRecord = null;

      recordStartTime = 0f;
      recordEndTime = 0f;
   }

   // handles taps on all sample buttons
   public void PointerDown(int channel) {
      // if not in editing mode, play the sample
      if (!editMode) {
         PlaySample(channel, AudioSettings.dspTime);
         return;
      }

      // if currently recording, finish up
      if (inRecord) {
         FinishRecording();
         DisplayManager2.Instance.ToggleRecordingState("stopped");
         return;
      }

      // handle recording capabilities
      if (!micConnected) { // TODO: handle no mic case
         Debug.Log("Error: no mic!");  
         return;
      }

      // start recording; audio captured in AudioClip
      Debug.Log("starting to record channel " + channel + " ...");
      soundSources[channel].clip = Microphone.Start(null, true, maxRecordSec, maxFreq);
      DisplayManager2.Instance.ToggleRecordingState("started");

      // update visual state
      Transform circleTransform = buttonGOs[channel].transform.Find("ProgressCircle");
      imageInRecord = circleTransform.gameObject.GetComponent<Image>();
      imageInRecord.fillAmount = 0.23f;

      Component[] groups = buttonGOs[channel].GetComponentsInChildren<CanvasGroup>();
      foreach (CanvasGroup group in groups) {
         group.alpha = 0.7f;
      }

      // update flags
      inRecord = true;
      channelInRecord = channel;

      // record start and max end time
      recordStartTime = AudioSettings.dspTime;
      recordEndTime = recordStartTime + Convert.ToDouble(maxRecordSec);
   }

   public void PointerUp(int channel) {
      // Debug.Log("Sample " + channel + " Pointer is up");
   }

   public void AssignClipToChannel(AudioClip clip, int channel) {
      soundClips[channel] = clip; // save the clip
      soundSources[channel].clip = clip;
      // TODO: update state
   }

   // wrapper for putting url/file ref into a channel
   public void AssignUrlToChannel(string url, int channel) {
      // StartCoroutine(SetClipFromUrl(url, soundSources[channel]));
   }

   // get audio data from url and assign to source
   IEnumerator SetClipFromUrl(string url, int channel) {
      WWW localFile = new WWW(url);
      yield return localFile;
      
      if (localFile.error == null) {
         Debug.Log("Loaded from url: OK");
      }
      else {
         Debug.Log("Loaded from url: error: " + localFile.error);
         yield break; 
      }

      soundClips[channel] = localFile.GetAudioClip(false, false); // save the clip
      soundSources[channel].clip = soundClips[channel];
      Debug.Log("Set sample " + channel + " from mic recording");
   }

   public void PlaySample(int channel, double playTime) {
      soundSources[channel].PlayScheduled(playTime);
   }

   public void PlayDelayed(int channel, float delay) {
      soundSources[channel].PlayDelayed(delay);
   }

   public void StopSample(int channel) {
      soundSources[channel].Stop();
   }

   public void StopAll() {
      for (int i = 0; i < numChannels; i++) {
         soundSources[i].Stop();
      }
   }

   public bool MicCheck() {
      // check if there is at least one microphone connected  
      if (Microphone.devices.Length <= 0) {  
         // TODO: throw a warning message at the console if there isn't  
         Debug.Log("Microphone not connected!");  
      }
      else { // at least one microphone is present  
         // set 'micConnected' to true  
         micConnected = true;  
         
         // get the default microphone recording capabilities  
         Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);  
         
         // according to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
         if (minFreq == 0 && maxFreq == 0) {  
            // .. .meaning 44100 Hz can be used as the recording sampling rate  
            maxFreq = 44100;  
         }
         else if (maxFreq > 44100) {
            maxFreq = 44100;
         }
      }

      return micConnected;
   }

   // Update is called once per frame
   void Update() {

        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();

        if (!inRecord)
         return;

      // compute fill ratio
      double fillRatio = 1f - (recordEndTime - AudioSettings.dspTime) / recordMaxDuration;

      // check for timeout
      if (fillRatio > 1f) {
         FinishRecording();
         return;
      }

      // update any progress bars
      imageInRecord.fillAmount = (float)Math.Min(1f, Math.Max(fillRatio, 0f));

    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }

    void GetAmplitude()
    {
        float _CurrentAmplitude = 0;
        float _CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            _CurrentAmplitude += _audioBand[i];
            _CurrentAmplitudeBuffer += _audioBandBuffer[i];
        }
        if (_CurrentAmplitude > _AmplitudeHighest)
        {
            _AmplitudeHighest = _CurrentAmplitude;
        }
        //_Amplitude = _CurrentAmplitude / _AmplitudeHighest;
        _Amplitude = Mathf.Clamp01(_CurrentAmplitude / _AmplitudeHighest);
        _AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void GetSpectrumAudioSource()
    {
        //for (int i = 0; i < numChannels; i++)
        //{
        //    soundSources[i].GetSpectrumData(_samplesLeft, 0, FFTWindow.Blackman);
        //    soundSources[i].GetSpectrumData(_samplesRight, 1, FFTWindow.Blackman);
        //}

            AudioListener.GetSpectrumData(_samplesLeft, 0, FFTWindow.Blackman);
            AudioListener.GetSpectrumData(_samplesRight, 1, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; g++)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }

            if (_freqBand[g] < _bandBuffer[g])
            {
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        /*
         * 22050 / 512 = 43 hertz per sample
         * 
         * 20 - 60 hertz
         * 60 - 250 hertz
         * 250 - 500 hertz
         * 500 - 2000 hertz
         * 2000 - 4000 hertz
         * 6000 - 20000 hertz
         * 
         * 0 -2 = 86 hertz
         * 1 - 4 = 172 hertz - 87-258 
         * 2 - 8 = 344 hertz - 259-602
         * 3 - 16 = 688 hertz - 603-1290
         * 4 - 32 = 1376 hertz - 1291-2666
         * 5 - 64 = 2752 hertz - 2667-5418
         * 6 - 128 = 5504 hertz - 5419-10922
         * 7 - 256 = 11008 hertz - 10923-21930
         * 510
         */

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                if (channel == _channel.Stereo)
                {
                    average += (_samplesLeft[count] + _samplesRight[count]) * (count + 1);
                }
                if (channel == _channel.Left)
                {
                    average += _samplesLeft[count] * (count + 1);
                }
                if (channel == _channel.Right)
                {
                    average += _samplesRight[count] * (count + 1);
                }
                count++;
            }

            average /= count;
            _freqBand[i] = average * 10;
        }
    }
}
