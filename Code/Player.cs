using Sandbox;
using Sandbox.VR;
using System;

public sealed class Player : Component
{
	[Property]
	public PlayerController controller;

	[Property]
	public SpringJoint spring = null;

	[Property]
	public LineRenderer lineRenderer;

	[Property]
	public GameObject grapplePhysiscs;

	[Property]
	public Rigidbody grapplePhysiscsRigidbody;

	private bool grappled = false;
	protected override void OnFixedUpdate()
	{
		grapplePhysiscsRigidbody.Velocity += Input.AnalogMove * 2.5f;

		if ( Input.Pressed( "attack1" ) )
		{
			if ( controller != null )
			{
				SceneTraceResult tr = Scene.Trace.Ray( Scene.Camera.WorldPosition, Scene.Camera.WorldPosition + Scene.Camera.WorldRotation.Forward * 1000f )
				.WithAnyTags( "grapple" )
				.Run();
				Log.Info( Scene.Camera.WorldPosition + Scene.Camera.WorldRotation.Forward * 1000f );
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
}
