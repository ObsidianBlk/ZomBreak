using Godot;
using System;

public class ChaseCamera : Camera
{
  [Export] private float _lerp_speed = 10f;
  public float lerp_speed {
    get {return _lerp_speed;}
    set {_lerp_speed = value;}
  }

  private Spatial _target = null;

  public override void _PhysicsProcess(float delta){
    if (_target == null){return;}
    GlobalTransform.InterpolateWith(_target.GlobalTransform, _lerp_speed * delta);
  }

  public void _on_target_change(Spatial target){
    _target = target;
  }
}
