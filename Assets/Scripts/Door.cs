using System;
using UnityEngine;

public class Door : MonoBehaviour {

  private const string IS_OPEN = "IsOpen";

  [SerializeField] private CargoArea dropOffCargoArea;

  private Animator animator;

  private void Awake() {
    animator = GetComponent<Animator>();
  }

  private void Start() {
    dropOffCargoArea.OnDroppedOff += DropOffCargoArea_OnDroppedOff;
  }

  private void DropOffCargoArea_OnDroppedOff(object sender, EventArgs e) {
    OpenDoor();
  }

  public void OpenDoor() {
    animator.SetBool(IS_OPEN, true);
  }

  public void CloseDoor() {
    animator.SetBool(IS_OPEN, false);
  }
}
