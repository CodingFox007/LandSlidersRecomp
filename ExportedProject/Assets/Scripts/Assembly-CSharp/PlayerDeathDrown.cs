public class PlayerDeathDrown : PlayerDeath
{
	public override void Die()
	{
		base.Die();
		Singleton<Game>.Instance.Player.UseGravity = true;
	}
}
