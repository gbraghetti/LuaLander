exemplificando como o código com coroutine abaixo:

```c#
using System;
using System.Collections;
using UnityEngine;

public class LanderVisuals : MonoBehaviour {

  [Header("Partículas Normais")]
  [SerializeField] private ParticleSystem leftThrusterParticleSystem;
  [SerializeField] private ParticleSystem middleThrusterParticleSystem;
  [SerializeField] private ParticleSystem rightThrusterParticleSystem;

  [Header("Visual do Burst (Fogo Sólido)")]
  // Arraste o objeto "BurstFlame" que criamos para aqui
  [SerializeField] private Transform burstFlameTransform;

  [Header("Configuração da Animação")]
  [SerializeField] private float burstDuration = 0.4f; // Quanto tempo dura
  [SerializeField] private Vector3 targetScale = new Vector3(1.5f, 4f, 1f); // Tamanho máximo (X=Largura, Y=Comprimento)

  private Lander lander;
  private Coroutine currentBurstRoutine;

  private void Awake() {
    lander = GetComponent<Lander>();

    lander.OnUpForce += Lander_OnUpForce;
    lander.OnLeftForce += Lander_OnLeftForce;
    lander.OnRightForce += Lander_OnRightForce;
    lander.OnBeforeForce += Lander_OnBeforeForce;
    lander.OnBurstForce += Lander_OnBurstForce;

    SetEnabledThrustedParticleSystem(leftThrusterParticleSystem, false);
    SetEnabledThrustedParticleSystem(middleThrusterParticleSystem, false);
    SetEnabledThrustedParticleSystem(rightThrusterParticleSystem, false);

    // Garante que o fogo comece invisível (escala zero)
    if (burstFlameTransform != null) {
      burstFlameTransform.localScale = Vector3.zero;
      burstFlameTransform.gameObject.SetActive(false); // Desativa para economizar performance
    }
  }

  private void Lander_OnBurstForce(object sender, EventArgs e) {
    // 1. Solta as partículas (opcional, mas fica bonito junto)
    ParticleSystem.EmitParams burstParams = new ParticleSystem.EmitParams();
    burstParams.startSize = 0.6f;
    burstParams.startLifetime = 1.5f;
    burstParams.startColor = new Color(0.2f, 0.8f, 1f) * 3f; // Azul neon

    leftThrusterParticleSystem.Emit(burstParams, 10);
    middleThrusterParticleSystem.Emit(burstParams, 10);
    rightThrusterParticleSystem.Emit(burstParams, 10);

    // 2. Inicia a animação do Fogo Sólido
    if (burstFlameTransform != null) {
      if (currentBurstRoutine != null) StopCoroutine(currentBurstRoutine);
      currentBurstRoutine = StartCoroutine(HandleSolidFlameRoutine());
    }
  }

  private IEnumerator HandleSolidFlameRoutine() {
    burstFlameTransform.gameObject.SetActive(true);

    float timer = 0f;

    while (timer < burstDuration) {
      timer += Time.deltaTime;

      // Calcula a porcentagem do tempo (vai de 0 até 1)
      float percent = timer / burstDuration;

      // Inverte a porcentagem (vai de 1 até 0) porque queremos diminuir
      float scaleMultiplier = 1f - percent;

      // Aplica a escala: Começa no tamanho alvo e vai diminuindo até zero
      burstFlameTransform.localScale = targetScale * scaleMultiplier;

      // (Opcional) Adiciona um "tremido" na rotação para parecer instável
      float shakeAmount = 5f * scaleMultiplier; // Treme mais quando está grande
      burstFlameTransform.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-shakeAmount, shakeAmount) + 180);
      //   burstFlameTransform.localRotation = Quaternion.Euler(0, 0, 180);

      yield return null;
    }

    // Garante que suma no final
    burstFlameTransform.localScale = Vector3.zero;
    burstFlameTransform.gameObject.SetActive(false);
  }

  // --- Métodos padrões de movimento abaixo ---
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
}
```

virou um código pra ser usado com Time.deltTime no `Update()`

```c#
using System;
using UnityEngine;

public class LanderVisuals : MonoBehaviour {

    [Header("Partículas Normais")]
    [SerializeField] private ParticleSystem leftThrusterParticleSystem;
    [SerializeField] private ParticleSystem middleThrusterParticleSystem;
    [SerializeField] private ParticleSystem rightThrusterParticleSystem;

    [Header("Visual do Burst (Fogo Sólido)")]
    [SerializeField] private Transform burstFlameTransform;

    [Header("Configuração da Animação")]
    [SerializeField] private float burstDuration = 0.4f;
    [SerializeField] private Vector3 targetScale = new Vector3(1.5f, 4f, 1f);

    private Lander lander;

    // Variáveis de controle do Timer (Substituindo a Corrotina)
    private float burstTimer;
    private bool isBursting;

    private void Awake() {
        lander = GetComponent<Lander>();

        lander.OnUpForce += Lander_OnUpForce;
        lander.OnLeftForce += Lander_OnLeftForce;
        lander.OnRightForce += Lander_OnRightForce;
        lander.OnBeforeForce += Lander_OnBeforeForce;
        lander.OnBurstForce += Lander_OnBurstForce;

        SetEnabledThrustedParticleSystem(leftThrusterParticleSystem, false);
        SetEnabledThrustedParticleSystem(middleThrusterParticleSystem, false);
        SetEnabledThrustedParticleSystem(rightThrusterParticleSystem, false);

        if (burstFlameTransform != null) {
            burstFlameTransform.localScale = Vector3.zero;
            burstFlameTransform.gameObject.SetActive(false);
        }
    }

    // AQUI ESTÁ A MUDANÇA PRINCIPAL
    private void Update() {
        // Se não estiver em burst, não faz nada
        if (!isBursting) return;

        // Decrementa o tempo (Igual ao RagdollDeadTimer)
        burstTimer -= Time.deltaTime;

        if (burstTimer > 0f) {
            // --- CÓDIGO DE ANIMAÇÃO ENQUANTO O TEMPO RODA ---

            // Como o timer vai do Máximo até 0, podemos usar essa proporção direto
            // Ex: Se timer é 0.4 (total), scaleMultiplier é 1. Se timer é 0, scaleMultiplier é 0.
            float scaleMultiplier = burstTimer / burstDuration;

            burstFlameTransform.localScale = targetScale * scaleMultiplier;

            // Efeito de tremer (Shake)
            float shakeAmount = 5f * scaleMultiplier;
            burstFlameTransform.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-shakeAmount, shakeAmount) + 180);
        }
        else {
            // --- O TEMPO ACABOU ---
            isBursting = false;
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

        // 2. Ativa o estado de Burst no Update
        if (burstFlameTransform != null) {
            burstFlameTransform.gameObject.SetActive(true);
            isBursting = true;
            burstTimer = burstDuration; // Reseta o timer para o valor cheio
        }
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
}
```
