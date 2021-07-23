using Godot;
using System;

public class Player : KinematicBody
{

  // Car state properties
  private Vector3 acceleration = new Vector3(0f, 0f, 0f);  // current acceleration
  private Vector3 velocity = new Vector3(0f, 0f, 0f);  // current velocity
  
  private vehicle car = null;
  private CollisionShape body_shape = null;
  private CollisionShape front_wheel_shape = null;
  private CollisionShape rear_wheel_shape = null;

  private bool drifting = false;

  public float gravity = -9.8f; //-20.0f; 
   


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
    if (car == null){return;}
    if (IsOnFloor()){
      get_input();
      apply_friction(delta);
      calculate_steering(delta);
      acceleration.y = 0f;
    } else {
      acceleration.y = gravity;
    }

    velocity += acceleration * delta;
    velocity = MoveAndSlideWithSnap(velocity, -Transform.basis.y, new Vector3(0f, 1f, 0f), true);
  }

  private void get_input(){
    float lstrength = Input.GetActionStrength("left");
    float rstrength = Input.GetActionStrength("right");
    float turn = lstrength - rstrength;
    car.wheel_angle = turn * car.steering_limit;

    acceleration = new Vector3(0f,0f,0f);
    if (Input.IsActionPressed("accel"))
      acceleration = -Transform.basis.z * car.engine_power;
    if (Input.IsActionPressed("brake"))
      acceleration = -Transform.basis.z * car.braking;
  }


  private void apply_friction(float delta){
    if (velocity.Length() < 0.5f && acceleration.Length() == 0f){
        velocity.x = 0f;
        velocity.z = 0f;
    }
    Vector3 friction_force = velocity * car.friction * delta;
    Vector3 drag_force = velocity * velocity.Length() * car.drag * delta;
    //if (velocity.Length() > 0f)
      //GD.Print("Vel: ", velocity, " | Fric: ", friction_force, " | Drag: ", drag_force);
    acceleration += drag_force + friction_force;
    //GD.Print("Accel: ", acceleration);
  }

  private Vector3 lerp_v3(Vector3 A, Vector3 B, float by){
    return new Vector3(
      /*
      Mathf.Lerp(A.x, B.x, by),
      Mathf.Lerp(A.y, B.y, by),
      Mathf.Lerp(A.z, B.z, by)
      */
      (A.x * (1f - by)) + (B.x * by),
      (A.y * (1f - by)) + (B.y * by),
      (A.z * (1f - by)) + (B.z * by)
    );
  }

  private void calculate_steering(float delta){
    //GD.Print("Wheel Base: ", car.wheel_base);
    Vector3 rear_wheel = Transform.origin + Transform.basis.z * car.wheel_base / 2.0f;
    Vector3 front_wheel = Transform.origin - Transform.basis.z * car.wheel_base / 2.0f;
    rear_wheel += velocity * delta;
    front_wheel += velocity.Rotated(Transform.basis.y, car.wheel_angle_rad) * delta;
    Vector3 new_heading = rear_wheel.DirectionTo(front_wheel);

    if (!drifting && velocity.Length() > car.slip_speed){
      drifting = true;
    } else if (drifting && velocity.Length() < car.slip_speed && car.wheel_angle == 0f){
      drifting = false;
    }
    float traction = car.traction_slow;
    if (drifting)
      traction = car.traction_fast;

    float d = new_heading.Dot(velocity.Normalized());
    if (d > 0f)
      velocity = lerp_v3(velocity, new_heading * velocity.Length(), traction);
        //velocity = new_heading * velocity.Length();
    else if (d < 0f)
      velocity = -new_heading * Math.Min(velocity.Length(), car.max_speed_reverse);
    if (new_heading.Length() > 0f)
      LookAt(Transform.origin + new_heading, Transform.basis.y);
  }
}




