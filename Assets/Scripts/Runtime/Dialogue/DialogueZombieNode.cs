using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueZombieNode : Node {

	public float spawnInterval = -1f;
	public int spawnCount = -1;
	public int enemiesUnlocked = -1;

	[Input] public Node prevNode;
	[Output] public Node nextNode;
}