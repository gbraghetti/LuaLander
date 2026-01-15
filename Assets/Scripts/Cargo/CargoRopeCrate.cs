using UnityEngine;

public class CargoRopeCrate : MonoBehaviour {
  private void OnCollisionEnter2D(Collision2D collision2D) {
    if (!collision2D.gameObject.TryGetComponent(out Lander lander)) {
      Lander.Instance.CargoCrashed();
    }
  }
}
