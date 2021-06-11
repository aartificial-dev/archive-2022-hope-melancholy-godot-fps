using Godot;
using System;

public static class SecurityAlarmStatus { 
    
    private static float alarmTime = 0f;
    public static float AlarmTime {get => alarmTime;}

    public static void SetAlarmTime(float time) {
        alarmTime = time;
    }
}