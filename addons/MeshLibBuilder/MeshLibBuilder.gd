extends EditorPlugin
tool

var selection = null
var meshlib = null

var mesh_scale = 1

onready var ei : EditorInterface = get_editor_interface()

func _ready() -> void:
	ei.get_selection().connect("selection_changed", self, "_on_selection_changed")
	_on_selection_changed()
	set_process_input(true)
	print("MeshLibBuilder is initialized.")
	print("Press F12 to run the MeshLib Builder process.")
	print("Press F11 for more detailed information.")


func _input(event) -> void:
	if selection != null:
		if Input.is_key_pressed(KEY_F12):
			var fd : FileDialog = FileDialog.new()
			fd.dialog_hide_on_ok = true
			fd.add_filter("*.meshlib ; MeshLib")
			fd.rect_min_size = Vector2(800,600)
			fd.connect("file_selected", self, "_on_file_selected", [fd])
			add_child(fd)
			fd.popup_centered()
	if Input.is_key_pressed(KEY_F11):
		_MoreInformation()


func _on_file_selected(filename : String, fd : FileDialog) -> void:
	if selection != null:
		_BuildMeshLib()
		if meshlib != null and meshlib.get_item_list().size() > 0:
			var _res = ResourceSaver.save(filename, meshlib)
	remove_child(fd)
	fd.queue_free()


func _on_selection_changed() -> void:
	var snl = ei.get_selection().get_selected_nodes()
	selection = null
	if snl.size() == 1:
		selection = snl[0]


func _ScanForMesh(node : Spatial):
	for child in node.get_children():
		if child is MeshInstance:
			return child
		if child is StaticBody:
			var res = _ScanForMesh(child)
			if res:
				return res
	return null

func _ScanForShape(node : Spatial):
	for child in node.get_children():
		if child is CollisionShape:
			return child
		if child is MeshInstance:
			var res = _ScanForShape(child)
			if res:
				return res
		if child is StaticBody:
			var res = _ScanForShape(child)
			if res:
				return res
	return null


func _ScanForNavMesh(node : Spatial):
	for child in node.get_children():
		if child is NavigationMeshInstance:
			return child
		if child is Navigation:
			var res = _ScanForNavMesh(child)
			if res:
				return res
		if child is StaticBody:
			var res = _ScanForNavMesh(child)
			if res:
				return res
		if child is MeshInstance:
			var res = _ScanForNavMesh(child)
			if res:
				return res
	return null


func _ScanForMeshGroups(node : Spatial, depth : int = 4):
	for child in node.get_children():
		var mesh = _ScanForMesh(child)
		if mesh != null:
			var shape = _ScanForShape(child)
			var nav = _ScanForNavMesh(child)
			_StoreNewMesh(child.name, mesh, shape, nav)
		elif depth > 0:
			_ScanForMeshGroups(child, depth - 1)

func _ScaledMesh(omesh : Mesh, scale : float) -> ArrayMesh:
	var mdt = MeshDataTool.new()
	var nmesh : ArrayMesh = ArrayMesh.new()
	for s in range(omesh.get_surface_count()):
		mdt.create_from_surface(omesh, s)
		for i in range(mdt.get_vertex_count()):
			var vert = mdt.get_vertex(i)
			vert *= scale
			mdt.set_vertex(i, vert)
		mdt.commit_to_surface(nmesh)
	return nmesh

	
	

func _StoreNewMesh(item_name : String, mesh : MeshInstance, shape, nav) -> void:
	print("Storing a mesh named ", item_name)
	var iid = meshlib.get_item_list().size()
	var meshobj : Mesh = mesh.mesh
	if mesh_scale > 1:
		print("Trying to scale by: ", mesh_scale)
		meshobj = _ScaledMesh(mesh.mesh, mesh_scale);	
	var preview = ei.make_mesh_previews([meshobj], 128)
	meshlib.create_item(iid)
	meshlib.set_item_name(iid, item_name)
	meshlib.set_item_mesh(iid, meshobj)
	meshlib.set_item_preview(iid, preview[0])
	if shape is CollisionShape:
		var shapeobj = shape.shape
		var shapetrans = shape.transform
		if mesh_scale > 1:
			shapeobj = meshobj.create_trimesh_shape()
			shapetrans = Transform.IDENTITY
		meshlib.set_item_shapes(iid, [shapeobj, shapetrans])
	if nav is NavigationMeshInstance:
		var navmesh = nav.navmesh
		if mesh_scale > 1:
			navmesh = NavigationMesh.new()
			navmesh.create_from_mesh(meshobj)
		meshlib.set_item_navmesh(iid, navmesh)

func _BuildMeshLib() -> MeshLibrary:
	meshlib = MeshLibrary.new()
	_ScanForMeshGroups(selection)
	return meshlib


func _MoreInformation() -> void:
	print("MeshLib Builder scans through the children of the currently selected node.")
	print("For each child that contains a MeshInstance node, an entry is added to")
	print("the MeshLib named for the child in which the MeshInstance node was found.")
	print("")
	print("In addition to the MeshInstance node, the system will additionally look for")
	print("a CollisionShape node as well as a NevigationMeshInstance node (both are")
	print("optional).")
	print("The system will dive into StaticBody nodes for all of the above mentioned nodes.")
	print("The system will also look into Navigation nodes for NavigationMeshInstance")
	print("nodes.")
	print("")
	print("Layout Example 1:")
	print("-----------------------")
	print("+ Cool Mesh Object (Spatial)")
	print("|")
	print("+-+ Navigation")
	print("| |")
	print("| +-+ NavigationMeshInstance")
	print("|")
	print("+-+ StaticBody")
	print("  |")
	print("  +-+ MeshInstance")
	print("  |")
	print("  +-+ CollisionShape")





