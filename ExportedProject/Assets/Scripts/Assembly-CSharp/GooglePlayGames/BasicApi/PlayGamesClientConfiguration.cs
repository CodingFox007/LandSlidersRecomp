using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi
{
	public struct PlayGamesClientConfiguration
	{
		public class Builder
		{
			private bool mEnableSaveGames;

			private InvitationReceivedDelegate mInvitationDelegate = delegate
			{
			};

			private MatchDelegate mMatchDelegate = delegate
			{
			};

			public Builder EnableSavedGames()
			{
				mEnableSaveGames = true;
				return this;
			}

			public Builder WithInvitationDelegate(InvitationReceivedDelegate invitationDelegate)
			{
				mInvitationDelegate = Misc.CheckNotNull(invitationDelegate);
				return this;
			}

			public Builder WithMatchDelegate(MatchDelegate matchDelegate)
			{
				mMatchDelegate = Misc.CheckNotNull(matchDelegate);
				return this;
			}

			internal bool HasEnableSaveGames()
			{
				return mEnableSaveGames;
			}

			internal MatchDelegate GetMatchDelegate()
			{
				return mMatchDelegate;
			}

			internal InvitationReceivedDelegate GetInvitationDelegate()
			{
				return mInvitationDelegate;
			}

			public PlayGamesClientConfiguration Build()
			{
				return new PlayGamesClientConfiguration(this);
			}
		}

		public static readonly PlayGamesClientConfiguration DefaultConfiguration = new Builder().Build();

		private readonly bool mEnableSavedGames;

		private readonly InvitationReceivedDelegate mInvitationDelegate;

		private readonly MatchDelegate mMatchDelegate;

		public bool EnableSavedGames
		{
			get
			{
				return mEnableSavedGames;
			}
		}

		public InvitationReceivedDelegate InvitationDelegate
		{
			get
			{
				return mInvitationDelegate;
			}
		}

		public MatchDelegate MatchDelegate
		{
			get
			{
				return mMatchDelegate;
			}
		}

		private PlayGamesClientConfiguration(Builder builder)
		{
			mEnableSavedGames = builder.HasEnableSaveGames();
			mInvitationDelegate = builder.GetInvitationDelegate();
			mMatchDelegate = builder.GetMatchDelegate();
		}
	}
}
