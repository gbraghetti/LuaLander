using UnityEngine;

public class Turret : MonoBehaviour {

  [SerializeField] private Transform headTransform;
  [SerializeField] private TurretBullet turretBulletPrefab;
  [SerializeField] private Transform shootPointTransform;

  private float shootTimer;
  private float waitToShoot;

  private void Awake() {
    waitToShoot = 3f;
  }

  private void Update() {
    float isReady = waitToShoot -= Time.deltaTime;
    if (isReady > 0) {
      return;
    }
    if (!IsPlayerInRange()) {
      return;
    }
    Vector2 aimDirection = (Lander.Instance.transform.position - headTransform.position).normalized;
    headTransform.right = aimDirection;

    shootTimer -= Time.deltaTime;
    if (shootTimer < 0f) {
      float shootTimerMax = 1.5f;
      shootTimer = shootTimerMax;

      TurretBullet turretBullet = Instantiate(turretBulletPrefab, shootPointTransform.position, Quaternion.identity);
      turretBullet.Setup(aimDirection);

    }
  }

  private bool IsPlayerInRange() {
    float range = 14f;
    return Vector2.Distance(transform.position, Lander.Instance.transform.position) < range;
  }
}
