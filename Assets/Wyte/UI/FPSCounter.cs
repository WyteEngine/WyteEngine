using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

	int cnt;
	float time;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > 1)
		{
			GetComponent<Text>().text = $"fps{cnt.ToString()}";
			time = cnt = 0;
		}
		cnt++;
	}
}
