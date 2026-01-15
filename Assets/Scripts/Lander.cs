using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour {

  private const float GRAVITY_NORMAL = 0.7f;
  public static Lander Instance { get; private set; }

  // note: Aqui ele dispara os eventos e "não se importa com quem tá ouvindo"
  public event EventHandler OnUpForce;
  public event EventHandler OnRightForce;
  public event EventHandler OnLeftForce;
  public event EventHandler OnBeforeForce;
  public event EventHandler OnCoinPickup;
  public event EventHandler OnFuelPickup;
  public event EventHandler OnBurstForce; // Evento novo para o burst (útil para tocar som ou partículas)
  public event EventHandler OnBurstFlameForce;
  public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
  public class OnStateChangedEventArgs : EventArgs {
    public State state;
  }

  public event EventHandler<OnLandedEventArgs> OnLanded;
  public class OnLandedEventArgs : EventArgs {
    public LandingType landingType;
    public int score;
    public float dotVector;
    public float landingSpeed;
    public float scoreMultiplier;
  }

  public enum LandingType {
    Success,
    WrongLandingArea,
    TooSteepAngle,
    TooFastLanding,
    CargoCrashed,
  }

  public enum State {
    WaitingToStart,
    Normal,
    GameOver,
  }

  [SerializeField] private Transform cargoRopePrefab; // cargoRopePrefab > lidam com o mesmo objeto

  [SerializeField] private float burstForce = 10f; // Burst: Força do impulso
  [SerializeField] private float burstFuelCost = 3f; // Burst: Custo alto de combustível
  private Rigidbody2D landerRigidbody2D;
  private float fuelAmount;
  private float fuelAmountMax = 100f; // 10f
  private State state;
  private LanderVisuals landerVisuals;
  private Transform cargoRopeTransform; // cargoRopeTransform > lidam com o mesmo objeto
  private CargoSO cargoSO;

  private void Awake() {
    Instance = this;

    fuelAmount = fuelAmountMax;
    state = State.WaitingToStart;

    landerRigidbody2D = GetComponent<Rigidbody2D>();
    landerVisuals = GetComponent<LanderVisuals>();

    landerRigidbody2D.gravityScale = 0f;

  }

  private void FixedUpdate() {
    // note:
    // if (Input.GetKey(KeyCode.UpArrow)) { // old method
    //   Debug.Log("Up Old");
    // }

    OnBeforeForce?.Invoke(this, EventArgs.Empty);

    switch (state) {
      default:
      case State.WaitingToStart:
        if (GameInput.Instance.IsUpActionPressed() ||
            GameInput.Instance.IsLeftActionPressed() ||
            GameInput.Instance.IsRightActionPressed() ||
            GameInput.Instance.GetMovementInputVector2() != Vector2.zero) {
          // Pressing any input
          landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
          SetState(State.Normal);
        }
        break;
      case State.Normal:
        // Debug.Log(fuelAmount);
        if (fuelAmount <= 0f) {
          // No fuel
          return;
        }

        if (GameInput.Instance.IsUpActionPressed() ||
            GameInput.Instance.IsLeftActionPressed() ||
            GameInput.Instance.IsRightActionPressed() ||
            GameInput.Instance.GetMovementInputVector2() != Vector2.zero) {
          // Pressing any input
          ConsumeFuel(1f);
        }

        if (Keyboard.current.spaceKey.isPressed) {
          if (landerVisuals.CanUseBurstPressing()) {
            // Verifica se tem combustível suficiente para o burst
            if (fuelAmount >= burstFuelCost) {
              // ForceMode2D.Impulse é a chave aqui: aplica força instantânea
              landerRigidbody2D.AddForce(transform.up * burstForce, ForceMode2D.Impulse);

              ConsumeFuel(burstFuelCost); // Gasta mais combustível
              OnBurstForce?.Invoke(this, EventArgs.Empty); // Dispara evento
              OnBurstFlameForce?.Invoke(this, EventArgs.Empty); // Dispara evento
              Debug.Log("Burst Ativado!");
            }
          }
        }

        float gamepeadDeadzone = .4f;
        if (GameInput.Instance.IsUpActionPressed() ||
          GameInput.Instance.GetMovementInputVector2().y > gamepeadDeadzone) { // new version method
          float force = 700f;
          landerRigidbody2D.AddForce(force * transform.up * Time.deltaTime);
          OnUpForce?.Invoke(this, EventArgs.Empty);
        }
        if (GameInput.Instance.IsLeftActionPressed() ||
          GameInput.Instance.GetMovementInputVector2().x < -gamepeadDeadzone) {
          float turnSpeed = +100f;
          landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
          OnLeftForce?.Invoke(this, EventArgs.Empty);
        }
        if (GameInput.Instance.IsRightActionPressed() ||
          GameInput.Instance.GetMovementInputVector2().x > gamepeadDeadzone) {
          float turnSpeed = -100f;
          landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
          OnRightForce?.Invoke(this, EventArgs.Empty);
        }
        break;
      case State.GameOver:
        break;
    }
  }

  private void OnCollisionEnter2D(Collision2D collision2D) {
    if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad)) {
      Debug.Log("Crashed on the Terrain");
      OnLanded?.Invoke(this, new OnLandedEventArgs {
        landingType = LandingType.WrongLandingArea,
        score = 0,
        dotVector = 0f,
        landingSpeed = 0f,
        scoreMultiplier = 0,
      });
      SetState(State.GameOver);
      return;
    }

    float softLandingVelocityMagnitude = 4f;
    float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
    if (relativeVelocityMagnitude > softLandingVelocityMagnitude) {
      // Landed too hard!
      Debug.Log("Landed too hard!");
      OnLanded?.Invoke(this, new OnLandedEventArgs {
        landingType = LandingType.TooFastLanding,
        score = 0,
        dotVector = 0f,
        landingSpeed = relativeVelocityMagnitude,
        scoreMultiplier = 0,
      });
      SetState(State.GameOver);
      return;
    }

    float dotVector = Vector2.Dot(Vector2.up, transform.up);
    float minDotVector = .90f;
    if (dotVector < minDotVector) {
      // Landed on a too steep angle!      
      Debug.Log("Landed on a too steep angle!");
      OnLanded?.Invoke(this, new OnLandedEventArgs {
        landingType = LandingType.TooSteepAngle,
        score = 0,
        dotVector = dotVector,
        landingSpeed = relativeVelocityMagnitude,
        scoreMultiplier = 0,
      });
      SetState(State.GameOver);
      return;
    }
    Debug.Log("Sucessful landing!");

    float maxScoreAmountLandingAngle = 100;
    float scoreDotVectorMultiplier = 10f;
    float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

    float maxScoreAmountLandingSpeed = 100;
    float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

    Debug.Log("landingAngleScore: " + landingAngleScore);
    Debug.Log("landingSpeedScore: " + landingSpeedScore);

    int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());

    Debug.Log("score: " + score);
    OnLanded?.Invoke(this, new OnLandedEventArgs {
      landingType = LandingType.Success,
      score = score,
      dotVector = dotVector,
      landingSpeed = relativeVelocityMagnitude,
      scoreMultiplier = landingPad.GetScoreMultiplier(),
    });
    SetState(State.GameOver);

  }

  private void OnTriggerEnter2D(Collider2D collider2D) {
    if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup)) {
      float addFuelAmount = 10f;
      fuelAmount += addFuelAmount;
      if (fuelAmount > fuelAmountMax) {
        fuelAmount = fuelAmountMax;
      }
      OnFuelPickup?.Invoke(this, EventArgs.Empty);
      fuelPickup.DestroySelf();
    }

    if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup)) {
      OnCoinPickup?.Invoke(this, EventArgs.Empty);
      coinPickup.DestroySelf();
    }
  }

  private void SetState(State state) {
    this.state = state;
    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
      state = state
    });
  }

  private void ConsumeFuel(float amountMultiplier) {
    // Se for burst, não multiplica por Time.deltaTime pois é instantâneo, 
    // mas aqui mantive simples. Para o burst, passe um valor alto no parametro.

    // Correção para o burst gastar um valor fixo (sem Time.deltaTime) vs contínuo:
    if (amountMultiplier > 1f) {
      fuelAmount -= amountMultiplier; // Gasto fixo (Burst)
    } else {
      fuelAmount -= amountMultiplier * Time.deltaTime; // Gasto por tempo (Normal)
    }
  }

  public float GetFuel() {
    return fuelAmount;
  }

  public float GetFuelAmountNormalized() {
    return fuelAmount / fuelAmountMax;
  }

  public float GetSpeedX() {
    return landerRigidbody2D.linearVelocityX;
  }

  public float GetSpeedY() {
    return landerRigidbody2D.linearVelocityY;
  }

  public void PickUpCargo(CargoSO cargoSO) {
    this.cargoSO = cargoSO;
    cargoRopeTransform = Instantiate(cargoRopePrefab, transform);
  }

  public void DropCargo() {
    if (cargoRopeTransform != null) {
      cargoSO = null;
      Destroy(cargoRopeTransform.gameObject);
    }
  }

  public CargoSO GetCargoSO() {
    return cargoSO;
  }

  public void CargoCrashed() {
    OnLanded?.Invoke(this, new OnLandedEventArgs {
      landingType = LandingType.CargoCrashed,
      score = 0,
      dotVector = 0f,
      landingSpeed = 0f,
      scoreMultiplier = 0,
    });
    SetState(State.GameOver);
  }

  public void AddForce(Vector2 force) {
    landerRigidbody2D.AddForce(force * Time.deltaTime);
  }

}
