using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueNPCSpawnNode : Node {
	public GameObject npcToSpawn;
	
	[Input] public Node prevNode;
	[Output] public Node nextNode;
}