using System;
using System.Threading;
using UnityEngine;

namespace GooglePlayGames.Android
{
	internal class AndroidTokenClient : TokenClient
	{
		public static AndroidJavaObject GetActivity()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				return androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
		}

		public AndroidJavaObject GetApiClient(bool getServerAuthCode = false, string serverClientID = null)
		{
			Debug.Log("Calling GetApiClient....");
			using (AndroidJavaObject androidJavaObject = GetActivity())
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.android.gms.plus.Plus"))
				{
					using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("com.google.android.gms.common.api.GoogleApiClient$Builder", androidJavaObject))
					{
						androidJavaObject2.Call<AndroidJavaObject>("addApi", new object[1] { androidJavaClass.GetStatic<AndroidJavaObject>("API") });
						androidJavaObject2.Call<AndroidJavaObject>("addScope", new object[1] { androidJavaClass.GetStatic<AndroidJavaObject>("SCOPE_PLUS_LOGIN") });
						if (getServerAuthCode)
						{
							androidJavaObject2.Call<AndroidJavaObject>("requestServerAuthCode", new object[2] { serverClientID, androidJavaObject2 });
						}
						AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("build", new object[0]);
						androidJavaObject3.Call("connect");
						int num = 100;
						while (!androidJavaObject3.Call<bool>("isConnected", new object[0]) && num-- != 0)
						{
							Thread.Sleep(100);
						}
						Debug.Log("Done GetApiClient is " + androidJavaObject3);
						return androidJavaObject3;
					}
				}
			}
		}

		private string GetAccountName()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.android.gms.plus.Plus"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("AccountApi"))
				{
					using (AndroidJavaObject androidJavaObject2 = GetApiClient())
					{
						return androidJavaObject.Call<string>("getAccountName", new object[1] { androidJavaObject2 });
					}
				}
			}
		}

		public string GetEmail()
		{
			return GetAccountName();
		}

		public string GetAuthorizationCode(string serverClientID)
		{
			throw new NotImplementedException();
		}

		public string GetAccessToken()
		{
			string text = null;
			string text2 = GetAccountName() ?? "NULL";
			string text3 = "oauth2:https://www.googleapis.com/auth/plus.me";
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.android.gms.auth.GoogleAuthUtil"))
			{
				text = androidJavaClass.CallStatic<string>("getToken", new object[3]
				{
					GetActivity(),
					text2,
					text3
				});
			}
			Debug.Log("Access Token " + text);
			return text;
		}

		public string GetIdToken(string serverClientID)
		{
			string text = null;
			string text2 = GetAccountName() ?? "NULL";
			string text3 = "audience:server:client_id:" + serverClientID;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.google.android.gms.auth.GoogleAuthUtil"))
				{
					using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						text = androidJavaClass2.CallStatic<string>("getToken", new object[3] { androidJavaObject, text2, text3 });
					}
				}
			}
			Debug.Log("ID Token " + text);
			return text;
		}
	}
}
