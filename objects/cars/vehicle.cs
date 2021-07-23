using Godot;
using System;

public class vehicle : Spatial {

  private Spatial rear_axel = null;
  private Spatial forward_axel = null;
  private Spatial wheel_front_left = null;
  private Spatial wheel_front_right = null;
   

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

  private void UpdateWheelAngle(){
    if (wheel_front_left != null && wheel_front_right != null){
      Vector3 angle = new Vector3(0, wheel_angle, 0);
      wheel_front_left.RotationDegrees = angle;
      wheel_front_right.RotationDegrees = angle;
    }
  }

  public vehicle(){
    //rps = 0.0f;
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

  public override void _Ready() {
    forward_axel = GetNode<Spatial>("Forward_Axel");
    if (forward_axel != null){
      rear_axel = GetNode<Spatial>("Rear_Axel");
      wheel_front_left = forward_axel.GetNode<Spatial>("Wheel_Left");
      wheel_front_right = forward_axel.GetNode<Spatial>("Wheel_Right");
    }
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
