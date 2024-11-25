using System;

public class Player : Component
{
	[Property, Group("Components")]
	public PlayerController controller;

	[Property, Group( "Components" )]
	public SpringJoint spring = null;

	[Property, Group( "Components" )]
	public LineRenderer lineRenderer;

	[Property, Group( "Components" )]
	public GameObject grapplePhysiscs;

	[Property, Group( "Components" )]
	public Rigidbody grapplePhysiscsRigidbody;

	[Property, Range(1, 5), Group( "Stats" )]
	public int maxDashes = 1;

	public int dashes = 0;

	public Vector3 respawnPoint;

	private bool grappled = false;

	protected override void OnAwake()
	{
		respawnPoint = controller.WorldPosition;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		doDeath();

		doDash();
		doGrapple();
	}

	private void doGrapple()
	{
		grapplePhysiscsRigidbody.Velocity += Input.AnalogMove * 2.5f;

		if ( Input.Pressed( "attack1" ) )
		{
			if ( controller != null )
			{
				SceneTraceResult tr = Scene.Trace.Ray( Scene.Camera.WorldPosition, Scene.Camera.WorldPosition + Scene.Camera.WorldRotation.Forward * 1000f )
				.WithAnyTags( "grapple" )
				.Run();
				if ( tr.Hit )
				{
					grappled = true;
					spring.Enabled = true;
					spring.Body = tr.GameObject;
					spring.MaxLength = Vector3.DistanceBetween( controller.GameObject.WorldPosition, tr.GameObject.WorldPosition );
					lineRenderer.Points.Clear();
					lineRenderer.Points.Add( GameObject );
					lineRenderer.Points.Add( tr.GameObject );
				}
			}
		}

		if ( Input.Released( "attack1" ) )
		{
			grappled = false;
			spring.Enabled = false;
			lineRenderer.Points.Clear();
		}
		
		grapplePhysiscs.WorldRotation = controller.GameObject.WorldRotation;
		if ( grappled )
		{
			controller.GameObject.WorldPosition = grapplePhysiscs.WorldPosition;
			controller.Body.Velocity = grapplePhysiscsRigidbody.Velocity;
			controller.Body.AngularVelocity = grapplePhysiscsRigidbody.AngularVelocity;
		}
		else
		{
			grapplePhysiscs.WorldPosition = controller.GameObject.WorldPosition;
			grapplePhysiscsRigidbody.Velocity = controller.Body.Velocity;
			grapplePhysiscsRigidbody.AngularVelocity = controller.Body.AngularVelocity;
		}
	}

	private void doDash()
	{
		if ( controller.IsOnGround )
		{
			dashes = maxDashes;
		}

		if ( Input.Pressed( "Run" ) && !controller.IsOnGround && !grappled && dashes > 0 )
		{
			dashes--;
			controller.Body.Velocity = 0;
			controller.Body.ApplyImpulse( Scene.Camera.WorldRotation.Forward * 500000f );
		}
	}

	private void doDeath()
	{
		if ( controller.WorldPosition.z <= -2048 )
		{
			controller.WorldPosition = respawnPoint;
			controller.Body.Velocity = 0;
			dashes = maxDashes;
		}
	}
}
