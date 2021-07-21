using Godot;
using System;

public class Player : KinematicBody
{

  // Car state properties
  private Vector3 acceleration = new Vector3(0f, 0f, 0f);  // current acceleration
  private Vector3 velocity = new Vector3(0f, 0f, 0f);  // current velocity
  private float steer_angle = 0.0f;  // current wheel angle
  
  private vehicle car = null;
  private CollisionShape body_shape = null;
  private CollisionShape front_wheel_shape = null;
  private CollisionShape rear_wheel_shape = null;


  // Car behavior parameters, adjust as needed
  public float gravity = -20.0f;
  //public float wheel_base = 0.6f;

  public float steering_limit = 10.0f;  // front wheel max turning angle (deg)

  public float engine_power = 6.0f;
  public float braking = -9.0f;
  public float friction = -2.0f;
  public float drag = -2.0f;
  public float max_speed_reverse = 3.0f;
   


  public override void _Ready(){
    car = GetNode<vehicle>("Car");
    front_wheel_shape = GetNode<CollisionShape>("FrontWheelShape");
    rear_wheel_shape = GetNode<CollisionShape>("RearWheelShape");
    if (car != null && front_wheel_shape != null && rear_wheel_shape != null){
      Vector3 pos = car.GetForwardAxelPosition();
      Vector3 sPos = front_wheel_shape.Translation;
      sPos.z = pos.z;
      front_wheel_shape.Translation = sPos;

      pos = car.GetRearAxelPosition();
      sPos = rear_wheel_shape.Translation;
      sPos.z = pos.z;
      rear_wheel_shape.Translation = sPos;
    }
  }


  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta) {
    
  }
}
