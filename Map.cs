using Godot;
using System;
using System.Collections.Generic;


public struct Block {
    public double X1 { get; set; } // X coordinate
    public double X2 { get; set; } // X coordinate
    public double Y1 { get; set; } // Y coordinate
    public double Y2 { get; set; } // Y coordinate
    public List<Unit> units { get; set; } = new List<Unit>();  
    public Block(double x, double y, int size)
    {
        X1 = x;
        X2 = x + size;
        Y1 = y;
        Y2 = y + size;
    }
    public override string ToString()
    {
        return $"({X1}, {Y1})";
    }
}

public partial class Map : Node2D
{

    List<Block> blocks;
    int mapWidth = 8192;
    int mapHeight = 8192;
    int blockSize = 512;

    Player player1;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        blocks = new List<Block>();
        for (int i = 0; i < mapWidth; i += blockSize) {
            for (int j = 0; j < mapHeight; j += blockSize) {
                blocks.Add(new Block(i, j, blockSize));
            }
        }

        player1 = GetNode<Player>("Player1");
        player1.setBlocks(blocks);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
