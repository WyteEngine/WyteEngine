using UnityEngine;
using System.Collections;

public class PCGameViewBehaviour : SingletonBaseBehaviour<PCGameViewBehaviour>
{
	// Use this for initialization
	void Start()
	{
		if (IsSmartDevice)
			gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
			
	}
}
