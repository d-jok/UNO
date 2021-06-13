using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingArrows : MonoBehaviour
{
	private int spinSpeed;
	private bool m_IsClockwise;

	void Start()
    {
		m_IsClockwise = true;
		spinSpeed = 30;
    }

    void Update()
    {
		if (m_IsClockwise == true)
		{
			transform.Rotate(0, 0, -(spinSpeed * Time.deltaTime));
		}
		else
		{
			transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
		}
	}
}
