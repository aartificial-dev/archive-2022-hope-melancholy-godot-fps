using Godot;
using System;

public class TreeAcessor : Node { 

    public override void _Ready() {
        Helper.SetTreeAcessor(this);
    }
}