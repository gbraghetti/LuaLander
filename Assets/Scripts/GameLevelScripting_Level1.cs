using System;
using UnityEngine;

public class GameLevelScripting_Level1 : MonoBehaviour {

  [SerializeField] private CargoArea dropOffCargoArea;

  private void Start() {
    dropOffCargoArea.OnDroppedOff += DropOffCargoArea_OnDroppedOff;
  }

  private void DropOffCargoArea_OnDroppedOff(object sender, EventArgs e) {
    GameManager.Instance.AddScore(1000);
  }
}
