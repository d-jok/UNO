using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_Element : MonoBehaviour
{
	private void OnMouseOver()
	{
		this.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
	}

	private void OnMouseExit()
	{
		this.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
	}
}
