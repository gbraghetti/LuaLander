using UnityEngine;

public class TurretBullet : MonoBehaviour {

  private Rigidbody2D bulletRigidbody2D;
  private bool destroySelf;

  private void Awake() {
    bulletRigidbody2D = GetComponent<Rigidbody2D>();
    Destroy(gameObject, 4f);
  }

  private void LateUpdate() {
    if (destroySelf) {
      DestroySelf();
    }
  }

  public void Setup(Vector2 moveDirection) {
    float speed = 7f;
    bulletRigidbody2D.linearVelocity = moveDirection * speed;
  }

  private void OnCollisionEnter2D(Collision2D collision2D) {
    destroySelf = true;
  }

  private void DestroySelf() {
    Destroy(gameObject);
  }

}
