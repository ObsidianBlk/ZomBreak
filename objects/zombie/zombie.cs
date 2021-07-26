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
  private float max_walk_distance = 4f;
  private float walk_speed = 4f;

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
      target_position = (new Vector3(
          (float)rand.NextDouble(),
          GlobalTransform.origin.y,
          (float)rand.NextDouble()
      )).Normalized() * walk_distance;
      state = STATE.WANDER;
    } else {
      GD.Print("I'm good right here.");
    }
  }

  private void ProcessState_Wander(float delta){
    if (anim.CurrentAnimation != "Walking")
      anim.Play("Walking");
    float target_angle = GlobalTransform.basis.z.AngleTo(target_position); 
    GlobalTransform = GlobalTransform.Rotated(GlobalTransform.basis.y, target_angle * delta);
    MoveAndSlide(GlobalTransform.basis.z * walk_speed * delta);
    if (GlobalTransform.origin.DistanceTo(target_position) < 0.5f){
      state = STATE.IDLE;
    }
  }

  private void ProcessState_Death(float delta){

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
}



