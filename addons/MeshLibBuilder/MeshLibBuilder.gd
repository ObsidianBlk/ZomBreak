extends EditorPlugin
tool

var selection = null

onready var ei : EditorInterface = get_editor_interface()

func _ready() -> void:
	print("Hello from my plugin!")
	set_process_input(false)
	ei.get_selection().connect("selection_changed", self, "_on_selection_changed")


func _input(event) -> void:
	if Input.is_key_pressed(KEY_F12):
		var fd : FileDialog = FileDialog.new()
		fd.dialog_hide_on_ok = true
		fd.add_filter("*.meshlib ; MeshLib")
		fd.connect("file_selected", self, "_on_file_selected", [fd])
		add_child(fd)
		fd.show()


func _on_file_selected(filename : String, fd : FileDialog) -> void:
	if selection != null:
		var meshlib : MeshLibrary = _BuildMeshLib(selection)
		var _res = ResourceSaver.save(filename, meshlib)
	remove_child(fd)


func _on_selection_changed() -> void:
	var snl = ei.get_selection().get_selected_nodes()
	selection = null
	print("Checking selection change...")
	print("There are ", snl.size(), " selected nodes!")
	if snl.size() == 1:
		selection = snl[0]
		set_process_input(true)
	else:
		set_process_input(false)


func _BuildMeshLib(src_node : Spatial) -> MeshLibrary:
	var meshlib = MeshLibrary.new()
	var iid = 0
	for child in src_node:
		if child is Spatial:
			var mesh = child.get_node("StaticBody/mesh")
			var shape = child.get_node("StaticBody/CollisionShape")
			if mesh != null and shape != null:
				var preview = ei.make_mesh_previews([mesh.mesh], 128)
				meshlib.create_item(iid)
				meshlib.set_item_mesh(iid, mesh.mesh)
				meshlib.set_item_shapes(iid, [shape.shape, Transform.IDENTITY])
				meshlib.set_item_preview(iid, preview[0])
				meshlib.set_item_name(child.name)
	return meshlib
	#var _res = ResourceSaver.save("res://assets/mesh.meshlib", meshlib)



