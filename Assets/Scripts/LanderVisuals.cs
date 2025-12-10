using System;
using Unity.VisualScripting;
using UnityEngine;

public class LanderVisuals : MonoBehaviour {

  [SerializeField] private ParticleSystem leftThrusterParticleSystem;
  [SerializeField] private ParticleSystem middleThrusterParticleSystem;
  [SerializeField] private ParticleSystem rightThrusterParticleSystem;
  [SerializeField] private GameObject landerExplosionVfx;

  [SerializeField] private Transform burstFlameTransform; // Visual do Burst

  [SerializeField] private float burstDuration = 0.4f;
  [SerializeField] private Vector3 targetScale = new Vector3(1.5f, 4f, 1f);

  private Lander lander;

  private float burstTimer;
  private static float BURST_TIMER_USAGE_VALUE = 5f;
  private float burstTimerUsage = BURST_TIMER_USAGE_VALUE;
  private bool canUseBurstPressing = true;


  private void Awake() {
    lander = GetComponent<Lander>();

    lander.OnUpForce += Lander_OnUpForce;
    lander.OnLeftForce += Lander_OnLeftForce;
    lander.OnRightForce += Lander_OnRightForce;
    lander.OnBeforeForce += Lander_OnBeforeForce;
    lander.OnBurstForce += Lander_OnBurstForce;
    lander.OnBurstFlameForce += Lander_OnBurstFlameForce;

    SetEnabledThrustedParticleSystem(leftThrusterParticleSystem, false);
    SetEnabledThrustedParticleSystem(middleThrusterParticleSystem, false);
    SetEnabledThrustedParticleSystem(rightThrusterParticleSystem, false);

    burstFlameTransform.localScale = Vector3.zero;
    burstFlameTransform.gameObject.SetActive(false);
  }

  private void Lander_OnBurstFlameForce(object sender, EventArgs e) {
    if (canUseBurstPressing) {
      // 2. Ativa o estado de Burst no Update
      burstFlameTransform.gameObject.SetActive(true);
      burstTimer = burstDuration; // Reseta o timer para o valor cheio
    }

    canUseBurstPressing = false;

  }

  private void Start() {
    lander.OnLanded += Lander_OnLanded;
  }

  private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e) {
    switch (e.landingType) {
      case Lander.LandingType.TooFastLanding:
      case Lander.LandingType.TooSteepAngle:
      case Lander.LandingType.WrongLandingArea:
        // Crash!
        Instantiate(landerExplosionVfx, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        break;
    }
  }

  private void Update() {
    UsageFrameBurst();
  }

  private void UsageFrameBurst() {
    if (burstTimerUsage > 0f) {
      burstTimerUsage -= Time.deltaTime;
      //   Debug.Log("burstTimerUsage: " + burstTimerUsage);
    }
    if (burstTimerUsage <= 0) {
      burstTimerUsage = BURST_TIMER_USAGE_VALUE;
      canUseBurstPressing = true;
    }
    burstTimer -= Time.deltaTime;
    if (burstTimer > 0f) {
      // Como o timer vai do Máximo até 0, podemos usar essa proporção direto
      // Ex: Se timer é 0.4 (total), scaleMultiplier é 1. Se timer é 0, scaleMultiplier é 0.
      float scaleMultiplier = burstTimer / burstDuration;
      burstFlameTransform.localScale = targetScale * scaleMultiplier;
      // Efeito de tremer (Shake)
      float shakeAmount = 5f * scaleMultiplier;
      burstFlameTransform.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-shakeAmount, shakeAmount) + 180);
    } else {
      burstTimer = 0f;
      if (burstFlameTransform != null) {
        burstFlameTransform.localScale = Vector3.zero;
        burstFlameTransform.gameObject.SetActive(false);
      }
    }
  }

  private void Lander_OnBurstForce(object sender, EventArgs e) {
    // 1. Emite as partículas (igual antes)
    ParticleSystem.EmitParams burstParams = new ParticleSystem.EmitParams();
    burstParams.startSize = 0.6f;
    burstParams.startLifetime = 1.5f;
    burstParams.startColor = new Color(0.2f, 0.8f, 1f) * 3f;

    leftThrusterParticleSystem.Emit(burstParams, 10);
    middleThrusterParticleSystem.Emit(burstParams, 10);
    rightThrusterParticleSystem.Emit(burstParams, 10);
  }

  // --- Métodos padrões de movimento abaixo (sem alterações) ---
  private void Lander_OnBeforeForce(object sender, EventArgs e) {
    SetEnabledThrustedParticleSystem(leftThrusterParticleSystem, false);
    SetEnabledThrustedParticleSystem(middleThrusterParticleSystem, false);
    SetEnabledThrustedParticleSystem(rightThrusterParticleSystem, false);
  }

  private void Lander_OnUpForce(object sender, EventArgs e) {
    SetEnabledThrustedParticleSystem(leftThrusterParticleSystem, true);
    SetEnabledThrustedParticleSystem(middleThrusterParticleSystem, true);
    SetEnabledThrustedParticleSystem(rightThrusterParticleSystem, true);
  }

  private void Lander_OnLeftForce(object sender, EventArgs e) {
    SetEnabledThrustedParticleSystem(rightThrusterParticleSystem, true);
  }

  private void Lander_OnRightForce(object sender, EventArgs e) {
    SetEnabledThrustedParticleSystem(leftThrusterParticleSystem, true);
  }

  private void SetEnabledThrustedParticleSystem(ParticleSystem particleSystem, bool enabled) {
    ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
    emissionModule.enabled = enabled;
  }

  public bool CanUseBurstPressing() {
    return canUseBurstPressing;
  }
}