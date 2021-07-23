using Godot;
using System;
using System.Collections.Generic;

public class vehicle : Spatial {

  [Signal]
  public delegate void CamTargetChanged(Spatial target);

  protected Spatial rear_axel = null;
  protected Spatial forward_axel = null;
  protected Spatial wheel_front_left = null;
  protected Spatial wheel_front_right = null;

  private List<Spatial> camTargets = null;
  private int curCamTarget = 0;


  private float _steering_limit = 15f; // Front wheel max turning angle (deg)
  public float steering_limit {
    get {return _steering_limit;}
    set {
      if (value >= 0f)
        _steering_limit = value;
    }
  }

  private float _engine_power = 2.0f;
  public float engine_power {
    get {return _engine_power;}
    set {
      if (value >= 0f)
        _engine_power = value;
    }
  }

  private float _braking = -6.0f;
  public float braking {
    get {return _braking;}
    set {_braking = value;}
  }

  private float _friction = -5.0f;
  public float friction {
    get {return _friction;}
    set {_friction = value;}
  }

  private float _drag = -2.0f;
  public float drag {
    get {return _drag;}
    set {_drag = value;}
  }

  private float _max_speed_reverse = 3.0f;
  public float max_speed_reverse {
    get {return _max_speed_reverse;}
    set {
      if (value > 0f)
        _max_speed_reverse = value;
    }
  }

  private float _slip_speed = 4.0f;
  public float slip_speed {
    get {return _slip_speed;}
    set {
      if (value >= 0f)
        _slip_speed = value;
    }
  }

  private float _traction_slow = 0.75f;
  public float traction_slow {
    get {return _traction_slow;}
    set {
      _traction_slow = Math.Max(0f, Math.Min(1f, value));
    }
  }

  private float _traction_fast = 0.02f;
  public float traction_fast {
    get {return _traction_fast;}
    set {
      _traction_fast = Math.Max(0f, Math.Min(1f, value));
    }
  }

  public float wheel_base {
    get {
      if (rear_axel != null && forward_axel != null){
        return rear_axel.Translation.DistanceTo(forward_axel.Translation);
      } else {
        return 0f;
      }
    }
  }

  private float _rps = 0.0f;
  public float rps {
    get {return _rps;}
    set {
      if (value >= 0.0f)
        _rps = value;
    }
  }

  private float _wheel_angle = 0.0f;
  public float wheel_angle {
    get {return _wheel_angle;}
    set {
      _wheel_angle = Math.Max(-90.0f, Math.Min(90.0f, value));
      UpdateWheelAngle();
    }
  }

  public float wheel_angle_rad {
    get {return (float)(Math.PI/180)*_wheel_angle;}
  }


  protected void AddCamTarget(Spatial target){
    if (camTargets == null)
      camTargets = new List<Spatial>();
    if (!camTargets.Contains(target)){
      camTargets.Add(target);
      if (camTargets.Count == 1){
        EmitSignal(nameof(CamTargetChanged), target);
      }
    }
  }

  public void NextCamera(){
    if (camTargets != null){
      curCamTarget++;
      if (curCamTarget == camTargets.Count)
        curCamTarget = 0;
      EmitSignal(nameof(CamTargetChanged), camTargets[curCamTarget]);
    }
  }

  public void PrevCamera(){
    if (camTargets != null){
      curCamTarget--;
      if (curCamTarget < 0)
        curCamTarget = camTargets.Count - 1;
      EmitSignal(nameof(CamTargetChanged), camTargets[curCamTarget]);
    }
  }

  private void UpdateWheelAngle(){
    if (wheel_front_left != null && wheel_front_right != null){
      Vector3 angle = new Vector3(0, wheel_angle, 0);
      wheel_front_left.RotationDegrees = angle;
      wheel_front_right.RotationDegrees = angle;
    }
  }

  public Vector3 GetForwardAxelPosition(){
    Vector3 pos = new Vector3(0f, 0f, 0f);
    if (forward_axel != null)
      pos = forward_axel.Translation;
    return pos;
  }

  public Vector3 GetRearAxelPosition(){
    Vector3 pos = new Vector3(0f, 0f, 0f);
    if (rear_axel != null)
      pos = rear_axel.Translation;
    return pos;
  } 

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta) {
    if (rear_axel == null){return;}
    float rev = 360.0f * _rps * delta;
    Vector3 rot = rear_axel.RotationDegrees;
    rot.x = (rot.x + rev) % 360.0f;
    rear_axel.RotationDegrees = rot;

    MeshInstance wl = wheel_front_left.GetNode<MeshInstance>("wheel");
    MeshInstance wr = wheel_front_right.GetNode<MeshInstance>("wheel");
    rot = wl.RotationDegrees;
    rot.x = (rot.x + rev) % 360.0f;
    wl.RotationDegrees = rot;

    rot = wr.RotationDegrees;
    rot.x = (rot.x + rev) % 360.0f;
    wr.RotationDegrees = rot;
  }
}
