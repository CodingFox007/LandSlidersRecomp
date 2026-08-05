using System;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	public class PlayGamesLocalUser : PlayGamesUserProfile, ILocalUser, IUserProfile
	{
		public class PlayerStats
		{
			private int numberOfPurchases;

			private float avgSessonLength;

			private int daysSinceLastPlayed;

			private int numOfSessions;

			private float sessPercentile;

			private float spendPercentile;

			public int NumberOfPurchases { get; set; }

			public float AvgSessonLength { get; set; }

			public int DaysSinceLastPlayed { get; set; }

			public int NumOfSessions { get; set; }

			public float SessPercentile { get; set; }

			public float SpendPercentile { get; set; }
		}

		internal PlayGamesPlatform mPlatform;

		private string emailAddress;

		private PlayerStats mStats;

		public IUserProfile[] friends
		{
			get
			{
				return mPlatform.GetFriends();
			}
		}

		public bool authenticated
		{
			get
			{
				return mPlatform.IsAuthenticated();
			}
		}

		public bool underage
		{
			get
			{
				return true;
			}
		}

		public new string userName
		{
			get
			{
				string text = string.Empty;
				if (authenticated)
				{
					text = mPlatform.GetUserDisplayName();
					if (!base.userName.Equals(text))
					{
						ResetIdentity(text, mPlatform.GetUserId(), mPlatform.GetUserImageUrl());
					}
				}
				return text;
			}
		}

		public new string id
		{
			get
			{
				string text = string.Empty;
				if (authenticated)
				{
					text = mPlatform.GetUserId();
					if (!base.id.Equals(text))
					{
						ResetIdentity(mPlatform.GetUserDisplayName(), text, mPlatform.GetUserImageUrl());
					}
				}
				return text;
			}
		}

		public string idToken
		{
			get
			{
				return (!authenticated) ? string.Empty : mPlatform.GetIdToken();
			}
		}

		public string accessToken
		{
			get
			{
				return (!authenticated) ? string.Empty : mPlatform.GetAccessToken();
			}
		}

		public new bool isFriend
		{
			get
			{
				return true;
			}
		}

		public new UserState state
		{
			get
			{
				return UserState.Online;
			}
		}

		public new string AvatarURL
		{
			get
			{
				string text = string.Empty;
				if (authenticated)
				{
					text = mPlatform.GetUserImageUrl();
					if (!base.id.Equals(text))
					{
						ResetIdentity(mPlatform.GetUserDisplayName(), mPlatform.GetUserId(), text);
					}
				}
				return text;
			}
		}

		public string Email
		{
			get
			{
				if (authenticated && emailAddress == null)
				{
					emailAddress = mPlatform.GetUserEmail();
					emailAddress = ((emailAddress == null) ? string.Empty : emailAddress);
				}
				return (!authenticated) ? string.Empty : emailAddress;
			}
		}

		internal PlayGamesLocalUser(PlayGamesPlatform plaf)
			: base("localUser", string.Empty, string.Empty)
		{
			mPlatform = plaf;
			emailAddress = null;
			mStats = null;
		}

		public void Authenticate(Action<bool> callback)
		{
			mPlatform.Authenticate(callback);
		}

		public void Authenticate(Action<bool> callback, bool silent)
		{
			mPlatform.Authenticate(callback, silent);
		}

		public void Authenticate(Action<bool, string> callback)
		{
			mPlatform.Authenticate(delegate(bool x)
			{
				callback(x, string.Empty);
			});
		}

		public void LoadFriends(Action<bool> callback)
		{
			mPlatform.LoadFriends(this, callback);
		}

		public void GetStats(Action<CommonStatusCodes, PlayerStats> callback)
		{
			if (mStats == null)
			{
				mPlatform.GetPlayerStats(delegate(CommonStatusCodes rc, PlayerStats stats)
				{
					mStats = stats;
					callback(rc, stats);
				});
			}
			else
			{
				callback(CommonStatusCodes.Success, mStats);
			}
		}
	}
}
