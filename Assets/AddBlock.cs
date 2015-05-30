using UnityEngine;
using System.Collections;

public class AddBlock : OperationBlock {

	public override int operation(int curValue) {
		return (curValue + 1);
	}
}
