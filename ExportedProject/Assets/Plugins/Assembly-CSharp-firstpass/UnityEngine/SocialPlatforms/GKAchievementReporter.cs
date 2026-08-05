using System.Runtime.InteropServices;
using UnityEngine.SocialPlatforms.GameCenter;

namespace UnityEngine.SocialPlatforms
{
	public class GKAchievementReporter
	{
		public static void ReportAchievement(string achievementID, float progress, bool showsCompletionBanner)
		{
			if (Social.Active is GameCenterPlatform && Application.platform == RuntimePlatform.IPhonePlayer)
			{
				_ReportAchievement(achievementID, progress, showsCompletionBanner);
			}
			else
			{
				Social.ReportProgress(achievementID, progress, null);
			}
		}

		[DllImport("__Internal")]
		private static extern void _ReportAchievement(string achievementID, float progress, bool showsCompletionBanner);
	}
}
