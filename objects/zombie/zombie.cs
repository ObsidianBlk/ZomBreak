using Godot;
using System;

public class zombie : KinematicBody
{

  /* ------------------------------------------------------------
   * Private/Protected properties
   * ----------------------------------------------------------*/
  private AnimationPlayer anim = null;
  
  private enum STATE : byte {
    IDLE = 0,
    WANDER = 1,
    DEATH = 2
  }
  private STATE state = STATE.IDLE;

  private float min_walk_distance = 1f;
  private float max_walk_distance = 2f;
  private float walk_speed = 4f;
  private float turn_rate = 90f; // Degrees per second

  private Vector3 target_position = new Vector3(0f,0f,0f); 

  /* ------------------------------------------------------------
   * Public Properties
   * ----------------------------------------------------------*/
  

  /* ------------------------------------------------------------
   * Override Methods
   * ----------------------------------------------------------*/
  public override void _Ready(){
    anim = GetNode<AnimationPlayer>("Anim");
  }

  public override void _PhysicsProcess(float delta){
    switch(state){
      case STATE.IDLE:
        ProcessState_Idle(delta); 
        break;
      case STATE.WANDER:
        ProcessState_Wander(delta);
        break;
      case STATE.DEATH:
        ProcessState_Death(delta);
        break;
    }
  }

  /* ------------------------------------------------------------
   * Private Methods
   * ----------------------------------------------------------*/ 

  private void ProcessState_Idle(float delta){
    if (anim.CurrentAnimation != "Idle")
      anim.Play("Idle");
    var rand = new System.Random();
    if (rand.Next(100) > 75){
      GD.Print("I'm gonna walk");
      float walk_distance = min_walk_distance + ((max_walk_distance - min_walk_distance) * (float)rand.NextDouble());
      target_position = GlobalTransform.origin + ((new Vector3(
          (float)rand.NextDouble() - 0.5f,
          0f,
          (float)rand.NextDouble() - 0.5f
      )).Normalized() * walk_distance);
      state = STATE.WANDER;
    } else {
      GD.Print("I'm good right here.");
    }
  }

  private float D2R(float d){
    return ((float)(Math.PI/180.0))*d;
  }

  private float Sign(float v){
    if (v >= 0f){return 1f;}
    return -1f;
  }

  private void ProcessState_Wander(float delta){
    if (anim.CurrentAnimation != "Walking")
      anim.Play("Walking");
    Vector3 dir = GlobalTransform.origin.DirectionTo(target_position);
    float d = (GlobalTransform.basis.z).Dot(dir);
    float target_angle = D2R(180f) * d;
    float aps = D2R(Sign(d) * turn_rate) * delta;
    if (Math.Abs(aps) > Math.Abs(target_angle)){
      aps = 0f;
    }
    //GD.Print("APS: ", aps, " | Angle To: ", target_angle, " | Rot: ", Rotation.y);


    Rotate(new Vector3(0f, 1f, 0f), aps);
    //GD.Print("Rot: ", Rotation);
    //GlobalTransform = GlobalTransform.Rotated(GlobalTransform.basis.y, target_angle * delta);
    MoveAndSlide(GlobalTransform.basis.z * walk_speed * delta);
    float dist = DistanceTo2D();
    //GD.Print("From: ", GlobalTransform.origin, " | To: ", target_position);
    //GD.Print("Distance To: ", dist);
    if (dist < 0.1f){
      state = STATE.IDLE;
    }
  }

  private void ProcessState_Death(float delta){
    if (anim.CurrentAnimation != "Death")
      anim.Play("Death");
  }

  private float DistanceTo2D(){
    Vector2 from = new Vector2(
      GlobalTransform.origin.x,
      GlobalTransform.origin.z
    );
    Vector2 to = new Vector2(
      target_position.x,
      target_position.z
    );
    //GD.Print("From: ", from, " | To: ", to);
    return from.DistanceTo(to);
  }

  private float AngleOf(Vector3 v){
    Vector2 _v = new Vector2(v.x, v.z);
    return _v.Angle();
  }

  /* ------------------------------------------------------------
   * Public Methods
   * ----------------------------------------------------------*/

  /* ------------------------------------------------------------
   * Event Handlers
   * ----------------------------------------------------------*/
  public void _on_animation_finished(string anim_name){
    if (anim_name == "Death"){
      Spatial parent = GetParent<Spatial>();
      parent.RemoveChild(this);
      QueueFree();
    }
  }


  public void _on_body_entered(Node body){
    if (body.IsInGroup("player"))
      state = STATE.DEATH;
  }
}



