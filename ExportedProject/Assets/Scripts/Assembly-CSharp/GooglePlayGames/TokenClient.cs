namespace GooglePlayGames
{
	internal interface TokenClient
	{
		string GetEmail();

		string GetAccessToken();

		string GetAuthorizationCode(string serverClientId);

		string GetIdToken(string serverClientId);
	}
}
