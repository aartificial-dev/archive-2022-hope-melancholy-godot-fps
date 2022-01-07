using Godot;
using System;

public class OldPlayerStats {

    private float health;
    private float sanity;
    private float chi;

    private const float HEALTH_MAX = 100f;
    private const float SANITY_MAX = 100f;
    private const float CHI_MAX = 100f;

    public OldPlayerStats() {
        health = 100f;
        sanity = 100f;
        chi = 100f;
    }

}