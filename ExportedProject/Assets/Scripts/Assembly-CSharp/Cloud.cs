using UnityEngine;

public class Cloud : MonoBehaviour
{
	[SerializeField]
	public Vector3 m_bounds;

	public bool ClipsLevel()
	{
		m_bounds.x *= base.transform.localScale.x;
		m_bounds.y *= base.transform.localScale.y;
		m_bounds.z *= base.transform.localScale.z;
		Rect rect = new Rect(base.transform.position.x - m_bounds.x * 0.5f, base.transform.position.z - m_bounds.z * 0.5f, m_bounds.x, m_bounds.z);
		HexMap hexMap = Singleton<Game>.Instance.LevelManager.CurrentLevel.HexMap;
		HexGrid[] hexGrids = hexMap.HexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile[,] tiles = hexGrid.Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					HexTile hexTile = tiles[j, k];
					if (hexTile.Occupied)
					{
						Vector3 vector = hexMap.HexWorldPosition(hexTile);
						Rect other = new Rect(vector.x - 5f, vector.z - 4.33f, 10f, 8.66f);
						if (rect.Overlaps(other))
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}
}
