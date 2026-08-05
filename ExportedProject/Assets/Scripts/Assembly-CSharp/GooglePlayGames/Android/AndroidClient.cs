using System;
using Com.Google.Android.Gms.Common.Api;
using Com.Google.Android.Gms.Games;
using Com.Google.Android.Gms.Games.Stats;
using GooglePlayGames.BasicApi;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace GooglePlayGames.Android
{
	internal class AndroidClient : IClientImpl
	{
		private class StatsResultCallback : ResultCallbackProxy<Stats_LoadPlayerStatsResultObject>
		{
			private Action<int, PlayerStats> callback;

			public StatsResultCallback(Action<int, PlayerStats> callback)
			{
				this.callback = callback;
			}

			public override void OnResult(Stats_LoadPlayerStatsResultObject arg_Result_1)
			{
				callback(arg_Result_1.getStatus().getStatusCode(), arg_Result_1.getPlayerStats());
			}
		}

		private const string BridgeActivityClass = "com.google.games.bridge.NativeBridgeActivity";

		private const string LaunchBridgeMethod = "launchBridgeIntent";

		private const string LaunchBridgeSignature = "(Landroid/app/Activity;Landroid/content/Intent;)V";

		private AndroidTokenClient tokenClient;

		public PlatformConfiguration CreatePlatformConfiguration()
		{
			AndroidPlatformConfiguration androidPlatformConfiguration = AndroidPlatformConfiguration.Create();
			androidPlatformConfiguration.EnableAppState();
			using (AndroidJavaObject androidJavaObject = AndroidTokenClient.GetActivity())
			{
				androidPlatformConfiguration.SetActivity(androidJavaObject.GetRawObject());
				androidPlatformConfiguration.SetOptionalIntentHandlerForUI(delegate(IntPtr intent)
				{
					IntPtr intentRef = AndroidJNI.NewGlobalRef(intent);
					PlayGamesHelperObject.RunOnGameThread(delegate
					{
						try
						{
							LaunchBridgeIntent(intentRef);
						}
						finally
						{
							AndroidJNI.DeleteGlobalRef(intentRef);
						}
					});
				});
				return androidPlatformConfiguration;
			}
		}

		public TokenClient CreateTokenClient()
		{
			if (tokenClient == null)
			{
				tokenClient = new AndroidTokenClient();
			}
			return tokenClient;
		}

		private static void LaunchBridgeIntent(IntPtr bridgedIntent)
		{
			object[] args = new object[2];
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.games.bridge.NativeBridgeActivity"))
				{
					using (AndroidJavaObject androidJavaObject = AndroidTokenClient.GetActivity())
					{
						IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(androidJavaClass.GetRawClass(), "launchBridgeIntent", "(Landroid/app/Activity;Landroid/content/Intent;)V");
						array[0].l = androidJavaObject.GetRawObject();
						array[1].l = bridgedIntent;
						AndroidJNI.CallStaticVoidMethod(androidJavaClass.GetRawClass(), staticMethodID, array);
					}
				}
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		public void GetPlayerStats(IntPtr apiClient, Action<CommonStatusCodes, PlayGamesLocalUser.PlayerStats> callback)
		{
			GoogleApiClient arg_GoogleApiClient_ = new GoogleApiClient(apiClient);
			StatsResultCallback resultCallback;
			try
			{
				resultCallback = new StatsResultCallback(delegate(int result, PlayerStats stats)
				{
					Debug.Log("Result for getStats: " + result);
					PlayGamesLocalUser.PlayerStats arg = null;
					if (stats != null)
					{
						arg = new PlayGamesLocalUser.PlayerStats
						{
							AvgSessonLength = stats.getAverageSessionLength(),
							DaysSinceLastPlayed = stats.getDaysSinceLastPlayed(),
							NumberOfPurchases = stats.getNumberOfPurchases(),
							NumOfSessions = stats.getNumberOfSessions(),
							SessPercentile = stats.getSessionPercentile(),
							SpendPercentile = stats.getSpendPercentile()
						};
					}
					callback((CommonStatusCodes)result, arg);
				});
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				callback(CommonStatusCodes.DeveloperError, null);
				return;
			}
			PendingResult<Stats_LoadPlayerStatsResultObject> pendingResult = Games.Stats.loadPlayerStats(arg_GoogleApiClient_, true);
			pendingResult.setResultCallback(resultCallback);
		}
	}
}
