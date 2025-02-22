extends TileMapLayer

# Map dimensions
const MAP_WIDTH = 100
const MAP_HEIGHT = 100
const SCALE = 0.05  # Controls the smoothness of biome transitions

# Biome and Tile Type Mapping
var biome_noise = FastNoiseLite.new()
var tile_noise = FastNoiseLite.new()

# Biome Thresholds (the chance of a biome being selected is from its threshold to the last biome's threshold)
# one biome can appear in multiple thresholds
# from -1 to 1
# var biome_thresholds: = {
# 	-0.8: "Ocean",
# 	-0.6: "Sea",      
# 	-0.4: "Plain", 
# 	-0.2: "Forest",   
# 	0.0: "Desert",
# 	0.3: "Badlands",
# 	0.5: "Plain",
# 	0.7: "Snow"
# }

var biome_thresholds = {
	-0.6: "Ocean",
	-0.2: "Sea",      
	0.2: "Plain", 
	0.5: "Forest",   
	0.6: "Plain",
	0.7: "Snow"
}

var tileType_thresholds = {	
	0.0 : "Grass",
	0.4 : "Forest",
	0.45 : "Water",
	0.7 : "Stone",
	0.9 : "Mountain"
}

# dict of tile atlas coordinates for each biome and tile type


# Runs at startup
func _ready():
	configure_noise()

	tile_set.get_source(3)


	var source:TileSetAtlasSource = tile_set.get_source(3)

	print(source.get_tiles_count())

	map_all_tiles()
	generate_map()

	for i in 3:
		print(i)

var tile_map:Dictionary = {}  # Dictionary to store (Biome, TileType) → Array [Tile Position]

func map_all_tiles():
	for a in 1:	
		var source = tile_set.get_source(3)

		if source is TileSetAtlasSource:
			var grid_size = source.get_atlas_grid_size()

			for i in grid_size.x:
				for j in grid_size.y:
					var tile_pos = Vector2i(i, j)

					if source.has_tile(tile_pos):
						var tile_data:TileData = source.get_tile_data(tile_pos, 0)
						if tile_data:
							var biome = tile_data.get_custom_data("Biome")
							var tile_type = tile_data.get_custom_data("TileType")
							var key = biome + "_" + tile_type  # Unique key

							if not tile_map.has(key):
								tile_map[key] = []

							for k in tile_data.probability:
								tile_map[key].append(tile_pos)
	print("Tile Lookup Table:", tile_map)  # Debug check


# Configure Perlin Noise settings
func configure_noise():
	biome_noise.seed = randi()
	biome_noise.frequency = SCALE
	biome_noise.noise_type = FastNoiseLite.TYPE_SIMPLEX

	tile_noise.seed = randi()
	tile_noise.frequency = SCALE * 2.5  # Smaller-scale variations for tile types

# Generate the world using noise
func generate_map():
	var center:Vector2 = Vector2(MAP_WIDTH / 2.0, MAP_HEIGHT / 2.0)
	var max_distance:float = center.length()  # Max possible distance	

	for x in range(MAP_WIDTH):
		for y in range(MAP_HEIGHT):
			var distance:float = center.distance_to(Vector2(x, y))/max_distance
			var noise_value = biome_noise.get_noise_2d(x, y)

			var falloff:float = pow(distance, 2)  
			noise_value = noise_value-falloff  # Smooth out the noise

			var biome = determine_biome(noise_value)

			# Get a valid tile that matches this biome
			noise_value = tile_noise.get_noise_2d(x, y)
			var tile_coords = get_tile_for_biome(biome,noise_value)
			
			set_cell(Vector2i(x, y), 3, tile_coords)

# Determine biome based on noise value
func determine_biome(noise_value: float) -> String:
	for threshold in biome_thresholds.keys():
		if noise_value <= threshold:
			return biome_thresholds[threshold]
	return "Plain"  # Default fallback

func determine_tile_type(noise_value: float) -> String:
	for threshold in tileType_thresholds.keys():
		if noise_value <= threshold:
			return tileType_thresholds[threshold]
	return "Grass"  # Default fallback

# Get a valid tile that matches a given biome
func get_tile_for_biome(biome: String,noise_value: float) -> Vector2i:
	var tileType = determine_tile_type(noise_value)
	#print(" TileType: ",tileType)
	var tiles = get_used_cells_by_biome(biome, tileType)
    
	if tiles.is_empty():
		return Vector2i(0, 0)  # Fallback tile
	else:
		return tiles.pick_random()  # Pick a random matching tile

# Retrieve all tiles matching a given biome from the TileSet
func get_used_cells_by_biome(biome: String,tileType:String) -> Array:

	var matching_tiles = []	

	var key = biome + "_" + tileType
	if biome=="Sea":
		key = "Sea"

	if tile_map.has(key):
		matching_tiles = tile_map[key]

	if matching_tiles.is_empty():
		key = biome + "_"
		if tile_map.has(key):
			matching_tiles = tile_map[key]
		return matching_tiles

	return matching_tiles
