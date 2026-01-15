using UnityEngine;

public class WindZone : MonoBehaviour {

  [SerializeField] private Vector2 windForce;

  private void OnTriggerStay2D(Collider2D collider2D) {
    if (collider2D.gameObject.TryGetComponent(out Lander lander)) {
      lander.AddForce(windForce);
    }
  }

  public Vector2 GetWindForce() {
    return windForce;
  }
}
