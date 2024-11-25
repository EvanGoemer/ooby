using Sandbox;

public sealed class DashCrystal : Component, Component.ITriggerListener
{
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		var player = other.GameObject.GetComponent<Player>;
		if ( player().IsValid() )
		{
			var sound = Sound.Play( "glass_break", Transform.World.Position);
			player().dashes = player().maxDashes;
		}
	}
}
