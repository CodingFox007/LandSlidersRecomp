using System;
using GooglePlayGames.BasicApi;
using GooglePlayGames.Native.PInvoke;

namespace GooglePlayGames
{
	internal interface IClientImpl
	{
		PlatformConfiguration CreatePlatformConfiguration();

		TokenClient CreateTokenClient();

		void GetPlayerStats(IntPtr apiClient, Action<CommonStatusCodes, PlayGamesLocalUser.PlayerStats> callback);
	}
}
