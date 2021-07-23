using Godot;
using System;
using System.Collections.Generic;

public class sedanSports : vehicle {
  public override void _Ready() {
    forward_axel = GetNode<Spatial>("Forward_Axel");
    if (forward_axel != null){
      rear_axel = GetNode<Spatial>("Rear_Axel");
      wheel_front_left = forward_axel.GetNode<Spatial>("Wheel_Left");
      wheel_front_right = forward_axel.GetNode<Spatial>("Wheel_Right");
    }

    Spatial CamTargets = GetNode<Spatial>("CamTargets");
    /*if (CamTargets != null){
      List<Spatial> slist = CamTargets.GetChildren()
        .Where(child => child is Spatial)
        .Cast<Spatial>()
        .ToList();
      foreach(Spatial item in slist)
        AddCamTarget(item);
    }*/
  }
}
