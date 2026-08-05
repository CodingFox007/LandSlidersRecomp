using System;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

public class CloudAndroid : MonoBehaviour
{
	private static bool m_loaded;

	public static void LoadFromCloud()
	{
		OpenSavedGame("Game", OpenFileCallback);
	}

	public static bool HasLoaded()
	{
		return m_loaded;
	}

	public static void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
	{
		m_loaded = true;
		Debug.Log("On saved game data read!");
		if (status == SavedGameRequestStatus.Success)
		{
			if (data != null && data.Length != 0)
			{
				Debug.Log("Load wrapper: " + Encoding.UTF8.GetString(data));
				JSONObject jSONObject = new JSONObject(Encoding.UTF8.GetString(data));
				{
					foreach (string key in jSONObject.keys)
					{
						Debug.Log("Registering " + key + ": " + jSONObject.GetField(key).ToString());
						SaveManager.RegisterGoogleData(key, jSONObject.GetField(key).ToString());
					}
					return;
				}
			}
			Debug.Log("empty data!");
		}
		else
		{
			Debug.Log("Read error");
		}
	}

	private static void OpenFileCallback(ISavedGameMetadata meta)
	{
		Debug.Log("Open file callback!");
		if (PlayGamesPlatform.Instance.IsAuthenticated())
		{
			ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
			savedGame.ReadBinaryData(meta, OnSavedGameDataRead);
		}
	}

	private static void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			Debug.Log("Save OK");
		}
		else
		{
			Debug.Log("Save error " + status);
		}
	}

	private static void ConflictCallback(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
	{
		if (PlayGamesPlatform.Instance.IsAuthenticated())
		{
			ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
			SavedGameMetadataUpdate.Builder builder = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(original.Filename);
			Debug.Log("Old name: " + original.Filename + ", new name: " + unmerged.Filename);
			byte[] updatedBinaryData = SaveManager.MergeAndroid(original.Filename, originalData, unmergedData);
			Debug.Log("Conflict - committing update...right?");
			SavedGameMetadataUpdate updateForMetadata = builder.Build();
			Debug.Log("original open?" + original.IsOpen);
			Debug.Log("unmerged open?" + unmerged.IsOpen);
			resolver.ChooseMetadata(original);
			savedGame.CommitUpdate(original, updateForMetadata, updatedBinaryData, OnSavedGameWritten);
		}
	}

	public static void OpenSavedGame(string name, Action<ISavedGameMetadata> callback)
	{
		if (!PlayGamesPlatform.Instance.IsAuthenticated())
		{
			return;
		}
		ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
		savedGame.OpenWithManualConflictResolution(name, DataSource.ReadCacheOrNetwork, true, ConflictCallback, delegate(SavedGameRequestStatus reqStatus, ISavedGameMetadata openedGame)
		{
			if (reqStatus == SavedGameRequestStatus.Success)
			{
				Debug.Log("Opened OK");
				if (callback != null)
				{
					callback(openedGame);
				}
			}
		});
	}

	public static void CommitSaveToCloud(string fileName, byte[] data, Action<bool> callback)
	{
		if (!m_loaded || !PlayGamesPlatform.Instance.IsAuthenticated())
		{
			return;
		}
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithManualConflictResolution(fileName, DataSource.ReadCacheOrNetwork, true, ConflictCallback, delegate(SavedGameRequestStatus reqStatus, ISavedGameMetadata openedGame)
		{
			if (reqStatus == SavedGameRequestStatus.Success)
			{
				SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedDescription(openedGame.Filename).Build();
				savedGameClient.CommitUpdate(openedGame, updateForMetadata, data, delegate(SavedGameRequestStatus status, ISavedGameMetadata game)
				{
					if (callback != null)
					{
						callback(status == SavedGameRequestStatus.Success);
					}
				});
			}
		});
	}
}
