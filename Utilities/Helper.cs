using Godot;
using System;

public static class Helper { 
    
    private static TreeAcessor treeAcessor;

    public static float startingHeight = 1920;
    public static float startingWidth = 1080;

    public static void SetTreeAcessor(TreeAcessor treeAcessor) {
        Helper.treeAcessor = treeAcessor;
        startingHeight = OS.WindowSize.y;
        startingWidth = OS.WindowSize.x;
    }

    public static Node GetLevel() {
        return GetNodeFromGroup<Node>("Level");
    }  

    public static Player GetPlayer() {
        return GetNodeFromGroup<Player>("Player");
    }

    public static PlayerCamera GetCamera() {
        return GetNodeFromGroup<PlayerCamera>("PlayerCamera");
    }

    public static HUDLog GetHUDLog() {
        return GetNodeFromGroup<HUDLog>("HUDLog");
    }

    public static Godot.Collections.Array GetNodesInGroup(String group) {
        Godot.Collections.Array arr = treeAcessor.GetTree().GetNodesInGroup(group);
        if (arr.Count > 0) {
            return arr;
        }
        return null;
    }

    public static T GetNodeFromGroup<T>(String group) {
        Godot.Collections.Array arr = treeAcessor.GetTree().GetNodesInGroup(group);
        foreach (Node node in arr) {
            if (node is T t) {
                return t;
            }
        }
        return default(T);
    }

    

    public static Vector2 GetFixedGlobalMousePosition(Control caller) {
        if ((startingWidth / OS.WindowSize.x) >= 1) {
            return caller.GetViewport().GetMousePosition() * (startingWidth / OS.WindowSize.x);
        } else {
            return caller.GetViewport().GetMousePosition() * (startingHeight / OS.WindowSize.y);
        }
    }

    public static Vector2 GetFixedLocalMousePosition(Control caller) {
        return GetFixedGlobalMousePosition(caller) - caller.RectGlobalPosition;
    }

}