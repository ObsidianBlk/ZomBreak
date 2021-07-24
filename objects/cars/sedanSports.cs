using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class sedanSports : vehicle {
  public override void _Ready() {
    forward_axel = GetNode<Spatial>("Forward_Axel");
    if (forward_axel != null){
      rear_axel = GetNode<Spatial>("Rear_Axel");
      wheel_front_left = forward_axel.GetNode<Spatial>("Wheel_Left");
      wheel_front_right = forward_axel.GetNode<Spatial>("Wheel_Right");
    }
    foreray = GetNode<RayCast>("ForeRay");
    rearray = GetNode<RayCast>("RearRay");

    Spatial CamTargets = GetNode<Spatial>("CamTargets");
    if (CamTargets != null){
      var slist = CamTargets.GetChildren().OfType<Spatial>();
      /*var slist = CamTargets.GetChildren()
        .Where(child => child is Spatial)
        .Cast<Spatial>()
        .ToList();*/
      foreach(Spatial item in slist){
        //GD.Print("Adding target: ", item.Name);
        AddCamTarget(item);
      }
    }
  }
}
