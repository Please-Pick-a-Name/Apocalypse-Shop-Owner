using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class OptionDialogueNode : Node
{
	public Dialogue speaker;
	public Dialogue responses;
	
	[Input] public Node prevNode;
	
	[Output(dynamicPortList = true)] public Dialogue[] options;

	public string[] optionsRequireItems;
}
