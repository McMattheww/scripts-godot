using Godot;
using System;

public partial class Unit : CharacterBody2D
{
    [Export] int health = 100;
    [Export] int damage = 100;
    [Export] double move_speed = 20.0;
    [Export] double attack_range = 50.0;
    [Export] double attack_rate = 0.5;

    double last_attack_time;
    Unit target;
    NavigationAgent2D agent;
    Sprite2D sprite;

    public override void _Ready() {
        agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        sprite = GetNode<Sprite2D>("Sprite2D");
        move_towards_location(new Vector2(50, 50));
    }

    void take_damage(int amount)
    {
        health -= amount;
        if (health <= 0) {
            QueueFree();
        }
    }

    void try_attack()
    {
        double cur_time = Time.GetUnixTimeFromSystem();
        if (cur_time - last_attack_time > attack_rate)
        {
            target.take_damage(damage);
            last_attack_time = cur_time;
        }
    }

    void set_target(Unit new_target)
    {
        target = new_target;
    }

    public void move_towards_location(Vector2 location)
    {
        target = null;
        agent.TargetPosition = location;
    }
    void target_check()
    {
        if (target != null)
        {
            double distance = GlobalPosition.DistanceTo(target.GlobalPosition);
            if (distance <= attack_range)
            {
                try_attack();
            }
            else
            {
                agent.TargetPosition = target.GlobalPosition;
            }
        }
    }

    public override void _Process(double delta)
    {
        target_check();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (agent.IsNavigationFinished())
        {
            //GD.Print("Navigation finished, stopping movement.");
            return;
        }
        Vector2 direction = GlobalPosition.DirectionTo(agent.GetNextPathPosition());
        //GD.Print($"Current position: {GlobalPosition}, Next path position: {agent.GetNextPathPosition()}, Direction: {direction}, Target location: {agent.TargetPosition}");
        Velocity = direction * (float)move_speed;
        MoveAndSlide();
    }
    
}




