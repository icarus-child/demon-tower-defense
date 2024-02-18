extends TextureProgressBar

# To attach to portal later
# @export var portal:Portal 
# Called when the node enters the scene tree for the first time.

var time = 0
func _ready():
	# To attach to portal later
	# portal.healthChanged.connected(update)	
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	# value = portal.currentHealth * 100 / portal.maxHealth

	time += delta
	
	if time >= 2:
		value -= 10
		time = 0

func update():
	pass
	# for future use
