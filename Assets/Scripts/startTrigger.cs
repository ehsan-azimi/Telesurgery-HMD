using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startTrigger : MonoBehaviour
{
	void ChangeAlpha(Material mat, float alphaVal)
	{
		Color oldColor = mat.color;
		Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
		mat.SetColor("_Color", newColor);

	}
	void OnTriggerEnter(Collider collision)
	{
		Material mat = gameObject.GetComponent<Renderer>().material;
		ChangeAlpha(mat, 0.5f);
		timerclass.timeStart = true;
		timerclass.timeReset = true;
		Counter.pegcounter = 0;
		Counter.wirecounter = 0;
		Counter.isNewTry = true;
		// start data recording at start
		Counter.recordData = true;
	}
	void OnTriggerExit(Collider collision)
	{
		Material mat = gameObject.GetComponent<Renderer>().material;
		ChangeAlpha(mat, 1.0f);

	}

}

public class timerclass
{
	public static bool timeStart = false;
	public static bool timeReset = false;
	public static bool timeRecord = false;
}
