using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueItemSpawnNode : Node {
	public GameObject gameObjectToSpawn;
	public float delay;
	public DialogueManager.SpawnLocation spawnLocation;
	
	[Input] public Node prevNode;
	[Output] public Node nextNode;
}