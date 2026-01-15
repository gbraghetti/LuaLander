using System;
using UnityEngine;
using UnityEngine.Audio;

public class LanderAudio : MonoBehaviour {

  [SerializeField] private AudioSource thrusterAudioSource;

  [SerializeField] private AudioMixer mixer;
  private Lander lander;
  private const float BOOST_TIMER = 1.0f; // O "X" segundos
  private const float BOOSTED_VOLUME = 20.0f; // Volume alto desejado

  private const string AUDIO_MIXER_PARAMETER = "ThrusterVolume";
  private float boostCounterTimer = 0f; // Controla o tempo restante

  private void Awake() {
    lander = GetComponent<Lander>();
  }

  private void Start() {
    lander.OnBeforeForce += Lander_OnBeforeForce;
    lander.OnUpForce += Lander_OnUpForce;
    lander.OnRightForce += Lander_OnRightForce;
    lander.OnLeftForce += Lander_OnLeftForce;
    lander.OnBurstFlameForce += Lander_OnBurstFlameForce;

    SoundManager.Instance.OnSoundVolumeChanged += SoundManager_OnSoundVolumeChanged;

    thrusterAudioSource.Pause();
  }

  private void SoundManager_OnSoundVolumeChanged(object sender, EventArgs e) {
    thrusterAudioSource.volume = SoundManager.Instance.GetSoundVolumeNormalized();
  }

  private void Update() {
    // Se o timer for maior que zero, significa que o boost está ativo
    if (boostCounterTimer > 0) {
      boostCounterTimer -= Time.deltaTime; // Decrementa o tempo baseada no frame rate

      // Se o tempo acabou neste frame
      if (boostCounterTimer <= 0) {
        // thrusterAudioSource.volume = defaultVolume; // Restaura o volume
        mixer.ClearFloat(AUDIO_MIXER_PARAMETER);
        boostCounterTimer = 0; // Garante que fique zerado
      }
    }
  }

  private void Lander_OnBurstFlameForce(object sender, EventArgs e) {
    if (!thrusterAudioSource.isPlaying) {
      thrusterAudioSource.Play();
    }
    // Aplica o volume alto e reinicia o timer
    mixer.SetFloat(AUDIO_MIXER_PARAMETER, BOOSTED_VOLUME);
    boostCounterTimer = BOOST_TIMER;
  }

  private void Lander_OnLeftForce(object sender, EventArgs e) {
    if (!thrusterAudioSource.isPlaying) {
      thrusterAudioSource.Play();
    }
  }

  private void Lander_OnRightForce(object sender, EventArgs e) {
    if (!thrusterAudioSource.isPlaying) {
      thrusterAudioSource.Play();
    }
  }

  private void Lander_OnUpForce(object sender, EventArgs e) {
    if (!thrusterAudioSource.isPlaying) {
      thrusterAudioSource.Play();
    }
  }

  private void Lander_OnBeforeForce(object sender, EventArgs e) {
    thrusterAudioSource.Pause();
  }
}
