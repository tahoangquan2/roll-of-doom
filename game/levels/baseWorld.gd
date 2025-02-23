extends TileMapLayer

# Map dimensions
const MAP_WIDTH = 100
const MAP_HEIGHT = 100
const SCALE = 0.07  # Controls the smoothness of biome transitions

# Biome and Tile Type Mapping
var biome_noise = FastNoiseLite.new()

# Biome Thresholds (the chance of a biome being selected is from its threshold to the last biome's threshold)
# one biome can appear in multiple thresholds
# from -1 to 1
# Ocean, Sea, Plain, Swamp, Forest, Desert, Badlands,Plain, Taiga, Tundra, 
# transitions tile: Desert-Badlands, Plain-Desert, Plain-Taiga, Badlands-Taiga


var biome_thresholds = { # put in array to allow multiple biomes in the same threshold
	-0.95: ["Ocean"],
	-0.6: ["Sea"],
	-0.5: ["Plain"],
	-0.45: ["Swamp"],
	-0.3: ["Desert"],
	-0.2: ["Badlands"],
	0.3: ["Plain"],
	1.0: ["Taiga"]
}

# dict of tile atlas coordinates for each biome and tile type


# Runs at startup
func _ready():
	configure_noise()

	tile_set.get_source(3)


	var source:TileSetAtlasSource = tile_set.get_source(3)

	print(source.get_tiles_count())

	map_all_tiles()
	make_thresholds_transitions()
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
							var key = biome   # Unique key

							if not tile_map.has(key):
								tile_map[key] = []

							for k in tile_data.probability:
								tile_map[key].append(tile_pos)
	#print("Tile Lookup Table:", tile_map)  # Debug check

func make_thresholds_transitions():
	# look for transitions in to add to biome_thresholds
	# transitions tile: Desert-Badlands, Plain-Desert, Plain-Taiga, Badlands-Taiga, 
	# transitions should be 0.05+this biome biome if the next has a transition with it
	pass

	var transitions = {
		"Desert": ["Badlands","Plain"],
		"Plain": ["Desert","Taiga"],
		"Badlands": ["Taiga","Desert"],
		"Taiga": ["Plain"]
	}

	# go linearly through the thresholds from the start
	# if the next biome has a transition with the current biome, add a threshold in the middle of the two

	var current_thresholds = biome_thresholds.keys()
	
	for i in range(current_thresholds.size()-1):
		var current_biome = biome_thresholds[current_thresholds[i]]
		var next_biome = biome_thresholds[current_thresholds[i+1]]

		if transitions.has(current_biome) and transitions[current_biome].has(next_biome):
			var new_threshold = current_thresholds[i]+0.05
			var new_biome = current_biome+"-"+next_biome

			if new_biome not in tile_map.keys():
				new_biome = next_biome+"-"+current_biome

			biome_thresholds[new_threshold] = new_biome

	# check if the next biome has a transition with the current biome
	




# Configure Perlin Noise settings
func configure_noise():
	biome_noise.seed = randi()
	biome_noise.frequency = SCALE
	biome_noise.noise_type = FastNoiseLite.TYPE_VALUE

# Generate the world using noise
func generate_map():
	var center:Vector2 = Vector2(MAP_WIDTH / 2.0, MAP_HEIGHT / 2.0)
	var max_distance:float = center.length()  # Max possible distance	

	for x in range(MAP_WIDTH):
		for y in range(MAP_HEIGHT):
			var distance:float = center.distance_to(Vector2(x, y))/max_distance # Normalize distance
			var noise_value = biome_noise.get_noise_2d(x, y) 

			var falloff:float = distance*1.2
			  # 1 or -1 depend on is the x> or < center
			noise_value = noise_value-falloff  # Smooth out the noise

			var biome = determine_biome(noise_value)
			var tile_coords = get_tile_for_biome(biome)
			
			set_cell(Vector2i(x, y), 3, tile_coords)

# Determine biome based on noise value
func determine_biome(noise_value: float) -> String:
	for threshold in biome_thresholds.keys():
		if noise_value <= threshold:
			return biome_thresholds[threshold][0]#.pick_random()
	return "Plain"  # Default fallback

# Get a valid tile that matches a given biome
func get_tile_for_biome(biome: String) -> Vector2i:
	var tiles = get_used_cells_by_biome(biome)
    
	if tiles.is_empty():
		return Vector2i(0, 0)  # Fallback tile
	else:
		return tiles.pick_random()  # Pick a random matching tile

# Retrieve all tiles matching a given biome from the TileSet
func get_used_cells_by_biome(biome: String) -> Array:

	var matching_tiles = []	

	var key = biome 

	if tile_map.has(key):
		matching_tiles = tile_map[key]

	if matching_tiles.is_empty():
		key = "Sea"
		matching_tiles = tile_map[key]

	return matching_tiles
