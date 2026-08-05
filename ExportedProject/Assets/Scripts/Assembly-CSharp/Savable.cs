public interface Savable
{
	JSONObject Save();

	void Load(JSONObject data);

	void Merge(JSONObject dataA, JSONObject dataB);

	JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB);

	JSONObject Reset();

	JSONObject UpdateVersion(JSONObject data);

	string SaveId();

	string Version();

	string GetVersion(JSONObject data);

	bool UseCloud();

	bool Verify(JSONObject data);
}
