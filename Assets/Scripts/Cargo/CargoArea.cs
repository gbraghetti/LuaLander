using System;
using UnityEngine;

public class CargoArea : MonoBehaviour {

  public event EventHandler OnInteractTimerChanged;
  public event EventHandler OnDroppedOff;
  public event EventHandler OnPickedUp;

  public enum InteractType {
    PickUp,
    Drop,
  }

  [SerializeField] private InteractType interactType;
  [SerializeField] private float interactTimerMax = 2f;
  [SerializeField] private CargoSO cargoSO;

  private float interactTimer;

  private void OnTriggerStay2D(Collider2D collider2D) {
    if (collider2D.gameObject.TryGetComponent(out Lander lander)) {
      switch (interactType) {
        case InteractType.PickUp:
          if (lander.GetCargoSO() != null) {
            // Lander is already carrying something, cannot carry more
            return;
          }
          break;
        case InteractType.Drop:
          if (lander.GetCargoSO() == null) {
            // Lander is not carrying anything so it cannot drop anything
            return;
          } else {
            // Lander is carrying something, check if that something is valid here
            if (lander.GetCargoSO() != cargoSO) {
              // Lander is carrying something that is not valid to drop here
              return;
            }
          }
          break;
      }

      interactTimer += Time.deltaTime;
      OnInteractTimerChanged?.Invoke(this, EventArgs.Empty);
      if (interactTimer > interactTimerMax) {
        switch (interactType) {
          case InteractType.PickUp:
            lander.PickUpCargo(cargoSO);
            OnPickedUp?.Invoke(this, EventArgs.Empty);
            break;
          case InteractType.Drop:
            lander.DropCargo();
            OnDroppedOff?.Invoke(this, EventArgs.Empty);
            break;
        }
        DestroySelf();
      }
    }
  }

  private void OnTriggerExit2D(Collider2D collider2D) {
    interactTimer = 0f;
    OnInteractTimerChanged?.Invoke(this, EventArgs.Empty);
  }

  public float GetInteractTimerNormalized() {
    return interactTimer / interactTimerMax;
  }

  public CargoSO GetCargoSO() {
    return cargoSO;
  }

  private void DestroySelf() {
    Destroy(gameObject);
  }

}
