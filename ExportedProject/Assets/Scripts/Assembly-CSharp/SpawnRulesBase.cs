using System.Collections.Generic;

public interface SpawnRulesBase
{
	bool CheckTile(HexTile tile);

	HexTile PickRandomTile(List<HexTile> tiles);

	void SpawnPartners();

	int CheckCount();
}
