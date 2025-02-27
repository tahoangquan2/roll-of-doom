using Godot;
using System.Collections.Generic;

public partial class BaseWorld : TileMapLayer
{
    // Map dimensions
    private const int MAP_WIDTH = 100;
    private const int MAP_HEIGHT = 100;
    private const float SCALE = 0.07f; // Controls the smoothness of biome transitions

    // Biome noise generator
    private FastNoiseLite biomeNoise = new FastNoiseLite();

    // Biome Thresholds
    private Dictionary<float, List<string>> biomeThresholds = new Dictionary<float, List<string>>
    {
        {-0.98f, new List<string> { "Ocean" }},
        {-0.6f, new List<string> { "Sea" }},
        {-0.5f, new List<string> { "Plain" }},
        {-0.45f, new List<string> { "Swamp" }},
        {-0.3f, new List<string> { "Desert" }},
        {-0.26f, new List<string> { "Badlands" }},
		{-0.2f, new List<string> { "Desert" }},
        {0.3f, new List<string> { "Plain" }},
        {1.0f, new List<string> { "Taiga" }}
    };

    private Dictionary<string, List<Vector2I>> tileMap = new Dictionary<string, List<Vector2I>>();

    public override void _Ready()
    {
        ConfigureNoise();
        TileSet.GetSource(3);
        MapAllTiles();
        MakeThresholdsTransitions();
        GenerateMap();
    }

    private void MapAllTiles()
    {
        TileSetAtlasSource source = TileSet.GetSource(3) as TileSetAtlasSource;
        if (source != null)
        {
            Vector2I gridSize = source.GetAtlasGridSize();

            for (int i = 0; i < gridSize.X; i++)
            {
                for (int j = 0; j < gridSize.Y; j++)
                {
                    Vector2I tilePos = new Vector2I(i, j);

                    if (source.HasTile(tilePos))
                    {
                        TileData tileData = source.GetTileData(tilePos, 0);
                        if (tileData != null)
                        {
                            string biome = tileData.GetCustomData("Biome").ToString();
                            if (!tileMap.ContainsKey(biome))
                                tileMap[biome] = new List<Vector2I>();

							for (int p=0; p<tileData.Probability; p++)
                            	tileMap[biome].Add(tilePos);
                        }
                    }
                }
            }
        }
    }

    private void MakeThresholdsTransitions()
    {
        Dictionary<string, List<string>> transitions = new Dictionary<string, List<string>>
        {
            { "Desert", new List<string> { "Badlands", "Plain" } },
            { "Plain", new List<string> { "Desert", "Taiga" } },
            { "Badlands", new List<string> { "Taiga", "Desert" } },
            { "Taiga", new List<string> { "Plain" } }
        };

        List<float> currentThresholds = new List<float>(biomeThresholds.Keys);
        for (int i = 0; i < currentThresholds.Count - 1; i++)
        {
            string currentBiome = biomeThresholds[currentThresholds[i]][0];
            string nextBiome = biomeThresholds[currentThresholds[i + 1]][0];

            if (transitions.ContainsKey(currentBiome) && transitions[currentBiome].Contains(nextBiome))
            {
                float newThreshold = currentThresholds[i] + 0.05f;
                string newBiome = $"{currentBiome}-{nextBiome}";

                if (!tileMap.ContainsKey(newBiome))
                    newBiome = $"{nextBiome}-{currentBiome}";

                biomeThresholds[newThreshold] = new List<string> { newBiome };
            }
        }
    }

    private void ConfigureNoise()
    {
        biomeNoise.Seed = (int) GD.Randi();
        biomeNoise.Frequency = SCALE;
        biomeNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Value;
		 biomeNoise.FractalOctaves = 3;
		 biomeNoise.FractalLacunarity = 2.0f;
		biomeNoise.FractalGain = 0.5f;
    }

    private void GenerateMap()
    {
        Vector2 center = new Vector2(0, 0);
        float maxDistance = new Vector2(MAP_WIDTH / 2.0f, MAP_HEIGHT / 2.0f).Length();

        for (int x = -MAP_WIDTH / 2; x < MAP_WIDTH / 2; x++)
        {
            for (int y = -MAP_HEIGHT / 2; y < MAP_HEIGHT / 2; y++)
            {
                float distance = center.DistanceTo(new Vector2(x, y)) / maxDistance;
                float noiseValue = biomeNoise.GetNoise2D(x, y);

                float falloff = distance * 1.2f;
                noiseValue -= falloff;

                string biome = DetermineBiome(noiseValue);
                Vector2I tileCoords = GetTileForBiome(biome);

                SetCell(new Vector2I(x, y), 3, tileCoords);
            }
        }
    }

    private string DetermineBiome(float noiseValue)
    {
        foreach (var threshold in biomeThresholds.Keys)
        {
            if (noiseValue <= threshold)
                return biomeThresholds[threshold][0];
        }
        return "Plain";
    }

    private Vector2I GetTileForBiome(string biome)
    {
        List<Vector2I> tiles = GetUsedCellsByBiome(biome);
        return tiles.Count > 0 ? tiles[(int)(GD.Randi() % (uint)tiles.Count)] : new Vector2I(0, 0);
    }

    private List<Vector2I> GetUsedCellsByBiome(string biome)
    {
        if (tileMap.ContainsKey(biome))
            return tileMap[biome];

        return tileMap[biomeThresholds[0.3f][0]];
    }
}
