using UnityEngine;
using System.Collections;

public class NeutralizerPowerUp : PowerUp {

	public override void setEffect ()
	{
		base.setEffect();

		// Set neutralizer as on in the power up controller
		PowerUpController.controller.setNeutralizer(true);

		OperationBlock[] operationBlocks = (OperationBlock[]) GameObject.FindObjectsOfType(typeof(OperationBlock));
		foreach (OperationBlock block in operationBlocks) {
			block.setValue(0);
			block.renderer.material.color = new Color(0, 1, 0, 0.6f);
		}
	}

	public override void passEffect ()
	{
		// Set neutralizer as off in the power up controller
		PowerUpController.controller.setNeutralizer(false);

		OperationBlock[] operationBlocks = (OperationBlock[]) GameObject.FindObjectsOfType(typeof(OperationBlock));
		foreach (OperationBlock block in operationBlocks) {
			block.setValue(1);
			block.renderer.material.color = new Color(1, 1, 1, 1);
		}
	}
}
