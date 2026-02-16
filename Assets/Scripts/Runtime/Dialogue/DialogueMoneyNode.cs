using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueMoneyNode : Node {
	public int amountToAdd;
	
	[Input] public Node prevNode;
	[Output] public Node nextNode;
}