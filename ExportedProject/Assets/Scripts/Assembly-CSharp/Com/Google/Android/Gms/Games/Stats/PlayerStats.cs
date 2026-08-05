namespace Com.Google.Android.Gms.Games.Stats
{
	public interface PlayerStats
	{
		float getAverageSessionLength();

		int getDaysSinceLastPlayed();

		int getNumberOfPurchases();

		int getNumberOfSessions();

		float getSessionPercentile();

		float getSpendPercentile();
	}
}
