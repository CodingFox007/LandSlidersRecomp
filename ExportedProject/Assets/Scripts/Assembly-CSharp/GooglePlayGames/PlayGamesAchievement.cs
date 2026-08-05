using System;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	internal class PlayGamesAchievement : IAchievement, IAchievementDescription
	{
		private readonly ReportProgress mProgressCallback;

		private string mId = string.Empty;

		private double mPercentComplete;

		private bool mCompleted;

		private bool mHidden;

		private DateTime mLastModifiedTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		private string mTitle = string.Empty;

		private string mRevealedImageUrl = string.Empty;

		private string mUnlockedImageUrl = string.Empty;

		private WWW mImageFetcher;

		private Texture2D mImage;

		private string mDescription = string.Empty;

		private ulong mPoints;

		public string id
		{
			get
			{
				return mId;
			}
			set
			{
				mId = value;
			}
		}

		public double percentCompleted
		{
			get
			{
				return mPercentComplete;
			}
			set
			{
				mPercentComplete = value;
			}
		}

		public bool completed
		{
			get
			{
				return mCompleted;
			}
		}

		public bool hidden
		{
			get
			{
				return mHidden;
			}
		}

		public DateTime lastReportedDate
		{
			get
			{
				return mLastModifiedTime;
			}
		}

		public string title
		{
			get
			{
				return mTitle;
			}
		}

		public Texture2D image
		{
			get
			{
				return LoadImage();
			}
		}

		public string achievedDescription
		{
			get
			{
				return mDescription;
			}
		}

		public string unachievedDescription
		{
			get
			{
				return mDescription;
			}
		}

		public int points
		{
			get
			{
				return (int)mPoints;
			}
		}

		internal PlayGamesAchievement()
			: this(PlayGamesPlatform.Instance.ReportProgress)
		{
		}

		internal PlayGamesAchievement(ReportProgress progressCallback)
		{
			mProgressCallback = progressCallback;
		}

		internal PlayGamesAchievement(GooglePlayGames.BasicApi.Achievement ach)
			: this()
		{
			mId = ach.Id;
			mPercentComplete = (double)ach.CurrentSteps / (double)ach.TotalSteps;
			mCompleted = ach.IsUnlocked;
			mHidden = !ach.IsRevealed;
			mLastModifiedTime = ach.LastModifiedTime;
			mTitle = ach.Name;
			mDescription = ach.Description;
			mPoints = ach.Points;
			mRevealedImageUrl = ach.RevealedImageUrl;
			mUnlockedImageUrl = ach.UnlockedImageUrl;
		}

		public void ReportProgress(Action<bool> callback)
		{
			mProgressCallback(mId, mPercentComplete, callback);
		}

		private Texture2D LoadImage()
		{
			if (hidden)
			{
				return null;
			}
			string text = ((!completed) ? mRevealedImageUrl : mUnlockedImageUrl);
			if (!string.IsNullOrEmpty(text))
			{
				if (mImageFetcher == null || mImageFetcher.url != text)
				{
					mImageFetcher = new WWW(text);
					mImage = null;
				}
				if (mImage != null)
				{
					return mImage;
				}
				if (mImageFetcher.isDone)
				{
					mImage = mImageFetcher.texture;
					return mImage;
				}
			}
			return null;
		}
	}
}
