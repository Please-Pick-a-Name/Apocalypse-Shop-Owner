using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueNPCSpawnNode : Node {
	public GameObject npcToSpawn;
	public float delay;
	
	[Input] public Node prevNode;
	[Output] public Node nextNode;
}