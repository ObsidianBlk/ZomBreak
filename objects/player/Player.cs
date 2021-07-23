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


  public override void _PhysicsProcess(float delta) {
    if (IsOnFloor()){
      get_input();
      apply_friction(delta);
      calculate_steering(delta);
    }
    acceleration.y = gravity;
    velocity += acceleration * delta;
    velocity = MoveAndSlideWithSnap(velocity, -Transform.basis.y, new Vector3(0f, 1f, 0f), true);
  }

  private void get_input(){
    float lstrength = Input.GetActionStrength("left");
    float rstrength = Input.GetActionStrength("right");
    float turn = lstrength - rstrength;
    //steer_angle = turn * steering_limit
    car.wheel_angle = turn * steering_limit;
    //GD.Print("Car Wheel Angle: ", car.wheel_angle);
    //$tmpParent/sedanSports/wheel_frontRight.rotation.y = steer_angle*2
    //$tmpParent/sedanSports/wheel_frontLeft.rotation.y = steer_angle*2
    acceleration = new Vector3(0f,0f,0f);
    if (Input.IsActionPressed("accel"))
      //GD.Print("Acceleration Pressed");
      acceleration = -Transform.basis.z * engine_power;
    if (Input.IsActionPressed("brake"))
      acceleration = -Transform.basis.z * braking;
  }


  private void apply_friction(float delta){
    if (velocity.Length() < 0.2f && acceleration.Length() == 0f){
        velocity.x = 0f;
        velocity.z = 0f;
    }
    Vector3 friction_force = velocity * friction * delta;
    Vector3 drag_force = velocity * velocity.Length() * drag * delta;
    acceleration += drag_force + friction_force;
  }

  private void calculate_steering(float delta){
    //GD.Print("Wheel Base: ", car.wheel_base);
    Vector3 rear_wheel = Transform.origin + Transform.basis.z * car.wheel_base / 2.0f;
    Vector3 front_wheel = Transform.origin - Transform.basis.z * car.wheel_base / 2.0f;
    rear_wheel += velocity * delta;
    front_wheel += velocity.Rotated(Transform.basis.y, car.wheel_angle_rad) * delta;
    Vector3 new_heading = rear_wheel.DirectionTo(front_wheel);
    //GD.Print("Rear: ", rear_wheel, " | Front: ", front_wheel, " | New Heading: ", new_heading);

    float d = new_heading.Dot(velocity.Normalized());
    if (d > 0f)
        velocity = new_heading * velocity.Length();
    else if (d < 0f)
        velocity = -new_heading * Math.Min(velocity.Length(), max_speed_reverse);
    if (new_heading.Length() > 0f)
      LookAt(Transform.origin + new_heading, Transform.basis.y);
  }
}




