using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript
{
	public bool mIsMoveDone;
	public bool mIsRotateDone;
	public bool mIsScalingDone;

	void Start()
    {
		mIsMoveDone = false;
		mIsRotateDone = false;
		mIsScalingDone = false;
    }

    /*void Update()
    {
        
    }*/

	/*void FixedUpdate()
	{

	}*/

	public IEnumerator Move(GameObject obj, Vector3 targetPoint, float speed)
	{
		mIsMoveDone = false;
		//float step = (speed / (obj.transform.position - targetPoint).magnitude) * Time.fixedDeltaTime;
		//float t = 0;
		////while (t <= 1.0f)
		//while (obj.transform.position != targetPoint)
		//{
		//	t += step; // Goes from 0 to 1, incrementing by step each time
		//	obj.transform.position = Vector3.Lerp(obj.transform.position, targetPoint, t); // Move objectToMove closer to b
		//	yield return null;         // Leave the routine and return here in the next frame
		//}
		//obj.transform.position = targetPoint;

		float totalMovementTime = 0.1f; //the amount of time you want the movement to take
		float currentMovementTime = 0f;//The amount of time that has passed
		Vector3 startPosition = obj.transform.position;
		while (Vector3.Distance(obj.transform.localPosition, targetPoint) > 0)
		{
			currentMovementTime += Time.deltaTime;
			obj.transform.localPosition = Vector3.Lerp(startPosition, targetPoint, currentMovementTime / totalMovementTime);
			yield return null;
		}

		mIsMoveDone = true;
	}

	public IEnumerator Rotation(GameObject obj, Vector3 rotation, float speed)
	{
		mIsRotateDone = false;
		Quaternion start = obj.transform.rotation;
		Quaternion destination = start * Quaternion.Euler(rotation);
		float startTime = Time.time;
		float percentComplete = 0f;
		while (percentComplete <= 1.0f)
		{
			percentComplete = (Time.time - startTime) / speed; // 0.3f;
			obj.transform.rotation = Quaternion.Slerp(start, destination, percentComplete);
			yield return null;
		}

		obj.transform.rotation = destination;
		mIsRotateDone = true;
	}

	public IEnumerator Scaling(GameObject obj, float scale)
	{
		mIsScalingDone = false;
		float speed = 0.9f; // the more the value, the slower the animation.
		float endScale = obj.transform.localScale.x * scale;

		float startTime = Time.time;
		float percentComplete = 0f;

		if (endScale < obj.transform.localScale.x)
		{
			while (endScale < obj.transform.localScale.x)
			{
				percentComplete = (Time.time - startTime) / speed;
				obj.transform.localScale -= new Vector3(percentComplete, percentComplete, percentComplete);
				yield return null;
			}
		}
		else
		{
			while (endScale > obj.transform.localScale.x)
			{
				percentComplete = (Time.time - startTime) / speed;
				obj.transform.localScale += new Vector3(percentComplete, percentComplete, percentComplete);
				yield return null;
			}
		}

		mIsScalingDone = true;
	}
}
