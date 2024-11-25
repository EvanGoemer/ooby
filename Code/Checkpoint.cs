using Sandbox;

public sealed class Checkpoint : Component, Component.ITriggerListener
{
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		var player = other.GameObject.GetComponent<Player>;
		if ( player().IsValid())
		{
			player().respawnPoint = WorldPosition;
		}
	}
}
