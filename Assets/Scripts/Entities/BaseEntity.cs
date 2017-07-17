using System;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class BaseEntity : MonoBehaviour
{
	public float moveSpeed = 2.0f;
	public float rotationSpeed = 10.0f;
	public float avoidPower = 1.0f;

	private Path path = new Path (new Dictionary<TileID, float>());
	private new Rigidbody rigidbody;
	private Vector3 pathTarget;
	private Vector3 avoidDir;

	void Start()
	{
		rigidbody = GetComponent<Rigidbody> ();
	}

	void Update()
	{
		if (path.finished ())
			CreatePath ();

		var target = path.next (transform.position);
		if ((transform.position - target).magnitude > 1.5f)
			RecreatePath ();
		target += avoidDir;
		var dir = Vector3.Slerp (transform.forward, target - transform.position, Time.deltaTime * rotationSpeed).normalized;
		transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (dir, Vector3.up).eulerAngles.y, 0);
		
		Debug.DrawRay (transform.position, transform.forward, Color.blue);

		rigidbody.velocity = transform.forward.normalized * moveSpeed;
		avoidDir = new Vector3 ();
	}

	void CreatePath()
	{
		var offset = new UniformVector2CircleDistribution (10).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
		pathTarget = transform.position + new Vector3(offset.x, 0, offset.y);

		path.create (transform.position, pathTarget);
	}

	void RecreatePath()
	{
		path.create (transform.position, pathTarget);
	}

	void OnTriggerStay(Collider c)
	{
		if (c.gameObject.tag != "Entity")
			return;
		var dir = c.transform.position - transform.position;
		float dist = dir.magnitude;

		if (Vector3.Dot (transform.forward, dir) < 0)
			return;

		//float isleft = Mathf.Atan2 (dir.z, dir.x) - Mathf.Atan2 (transform.forward.z, transform.forward.x) > 0 ? -1 : 1;
		var direction = new Vector3 (-dir.z, 0, dir.x).normalized;

		avoidDir += direction * (2 - dist) / 2 * avoidPower;
	} 
}
