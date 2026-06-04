using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Control {

    [Export] int scrollSpeed = 10;
    List<Unit> units = new List<Unit>();
    List<Unit> selected = new List<Unit>();
    bool selecting = false;
    Vector2 cameraMovement = new Vector2(0, 0);
    Vector2 start = new Vector2(0, 0);
    Vector2 end = new Vector2(0, 0);
    Camera2D camera;
    private PackedScene _unit = GD.Load<PackedScene>("res://unit.tscn");
    List<Block> blocks;
    int blockSize = 512;


    public void setBlocks(List<Block> blocks)
    {
        this.blocks = blocks;
    }

    /*
    private void selectUnits(float X1, float X2, float Y1, float Y2) {
        if (X2 < X1) { float temp = X2; X2 = X1; X1 = temp; }
        if (Y2 < Y1) { float temp = Y2; Y2 = Y1; Y1 = temp; }
        selected.Clear();
        if (units.Count == 0) { return; }
        foreach (Unit u in units) {
            if (X1 <= u.Position[0] && X2 >= u.Position[0] && Y1 <= u.Position[1] && Y2 >= u.Position[1]) {
                selected.Add(u);
            }
        }
        foreach (Unit u in selected) {
            GD.Print(u);
        }
    }
    */

    private void selectUnits(int X1, int X2, int Y1, int Y2) {
        selected.Clear();
        if (units.Count == 0) { return; }
        if (X2 < X1) { int temp = X2; X2 = X1; X1 = temp; }
        if (Y2 < Y1) { int temp = Y2; Y2 = Y1; Y1 = temp; }

        Vector2 pos;
        foreach (Unit u in units) { 
            pos = u.Position;
            if (X1 < pos[0] && X2 > pos[0] && Y1 < pos[1] && Y2 > pos[1])
                selected.Add(u);
        }
    }

    /*
    public void registerUnit(Unit u) {
        Vector2 pos = u.Position;
        foreach (Block b in blocks) { 
            if (b.X1 < pos[0] && b.X2 > pos[0] && b.Y1 < pos[1] && b.Y2 > pos[1]) { 
                b.units.Add(u);
            }
        }
        units.Add(u);
        GD.Print(u);
    }
    */
    public void registerUnit(Unit u)
    {
        units.Add(u);
        GD.Print(u);
    }




    public override void _Input(InputEvent @event)
    {
        if      (@event.IsAction("up")) { if (@event.IsPressed() && !@event.IsEcho()) { cameraMovement += new Vector2(0, -(scrollSpeed)); } if (@event.IsReleased()) { cameraMovement -= new Vector2(0, -(scrollSpeed)); } }
        else if (@event.IsAction("left")) { if (@event.IsPressed() && !@event.IsEcho()) { cameraMovement += new Vector2(-(scrollSpeed), 0); } if (@event.IsReleased()) { cameraMovement -= new Vector2(-(scrollSpeed), 0); } }
        else if (@event.IsAction("down")) { if (@event.IsPressed() && !@event.IsEcho()) { cameraMovement += new Vector2(0, (scrollSpeed)); } if (@event.IsReleased()) { cameraMovement -= new Vector2(0, (scrollSpeed)); } }
        else if (@event.IsAction("right")) { if (@event.IsPressed() && !@event.IsEcho()) { cameraMovement += new Vector2((scrollSpeed), 0); } if (@event.IsReleased()) { cameraMovement -= new Vector2((scrollSpeed), 0); } }

        else if (@event.IsAction("select")) {
            if (@event.IsPressed() && !@event.IsEcho()){ 
                selecting = true;
                start = GetGlobalMousePosition();
                
                GD.Print(start);
            }
            if (@event.IsReleased()) {
                selecting = false;
                end = GetGlobalMousePosition();
                GD.Print(end);
                selectUnits((int)start[0], (int)end[0], (int)start[1], (int)end[1]);
                start = new Vector2(0, 0);
                end = new Vector2(0, 0);
                QueueRedraw();

            }
        }
        else if (@event.IsActionPressed("order"))
        {
            GD.Print("order");
            foreach (Unit u in selected){
                u.move_towards_location(GetGlobalMousePosition());
            }
        }
        else if (@event is InputEventMouseMotion eventMouseMotion)
        {
            if (selecting)
            {
                end = GetGlobalMousePosition();
                QueueRedraw();

                //draw selection box
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        camera = GetNode<Camera2D>("camera");
        Unit unit1 = GetNode<Unit>("unit");
        Unit unit2 = GetNode<Unit>("unit2");
        registerUnit(unit1);
        registerUnit(unit2);

        var playerInstance = _unit.Instantiate<Unit>();

        // Set position or other properties before adding
        playerInstance.Position = new Vector2(70, 70);

        // Add to the current scene
        AddChild(playerInstance);
        registerUnit(playerInstance);

        camera.Zoom = new Vector2(2, 2);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        
    }
    public override void _PhysicsProcess(double delta)
    {
        camera.Translate(cameraMovement);
        Vector2 pos;
        foreach (Block b in blocks)
        {
            foreach (Unit u in b.units)
            {
                pos = u.Position;
                if (b.X1 > pos[0] && b.X2 < pos[0] && b.Y1 > pos[1] && b.Y2 < pos[1])
                {
                    b.units.Remove(u);
                    registerUnit(u);
                }

            }
        }
    }

    public override void _Draw()
    {
        DrawLine(new Vector2(start[0], start[1]), new Vector2(end[0], start[1]), Colors.Green, 1.0f);
        DrawLine(new Vector2(end[0], start[1]), new Vector2(end[0], end[1]), Colors.Green, 1.0f);
        DrawLine(new Vector2(end[0], end[1]), new Vector2(start[0], end[1]), Colors.Green, 1.0f);
        DrawLine(new Vector2(start[0], end[1]), new Vector2(start[0], start[1]), Colors.Green, 1.0f);
    }
}





