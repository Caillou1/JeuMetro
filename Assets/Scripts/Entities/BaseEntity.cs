using System;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class BaseEntity : MonoBehaviour
{
	public float moveSpeed = 2.0f;

	private Path path = new Path (new Dictionary<TileID, float>(), 0);
	private Rigidbody rigidbody;

	void Start()
	{
		rigidbody = GetComponent<Rigidbody> ();
	}

	void Update()
	{
		if (path.finished ())
			CreatePath ();

		var target = path.next (transform.position);
		var dir = target - transform.position;
		if (dir.magnitude < 0.1f)
			dir = new Vector3();
		else
			dir = dir.normalized * moveSpeed;

		rigidbody.velocity = dir;
	}

	void CreatePath()
	{
		var offset = new UniformVector2CircleDistribution (10).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
		var nextPos = transform.position + new Vector3(offset.x, 0, offset.y);

		path.create (transform.position, nextPos);
	}
}
