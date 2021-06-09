using System;
using Godot;

public class IAlarmSetter : Spatial {
    [Signal]
    public delegate void AlarmSignal();   
}
