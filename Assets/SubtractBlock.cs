using UnityEngine;
using System.Collections;

public class SubtractBlock : OperationBlock {

	public override int operation(int curValue) {
		return (curValue - 1);
	}
}
