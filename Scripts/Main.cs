using Godot;
using System;
using System.Drawing;

public class Main : Spatial
{
	Spatial visibleMap;
	Spatial invisibleMap;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		visibleMap = (Spatial)GetNode("VisibleMap");
		invisibleMap = (Spatial)GetNode("InvisibleMap");

		// Load prefabs
		PackedScene wallObject = (PackedScene)ResourceLoader.Load("res://Objects/Wall 2.tscn");
		PackedScene wallObjectTransparent = (PackedScene)ResourceLoader.Load("res://Objects/Wall (not seen) 2.tscn");
		PackedScene groundTileObject = (PackedScene)ResourceLoader.Load("res://Objects/GroundTile 2.tscn");
		PackedScene groundTileObjectTransparent = (PackedScene)ResourceLoader.Load("res://Objects/GroundTile (not seen) 2.tscn");

		// Load bitmap
		Bitmap bitmap = new Bitmap("./Bitmaps/Map 4.png");

		// Create terrain
		for (int i = 0; i < bitmap.Width; i++)
		{
			for (int j = 0; j < bitmap.Height; j++)
			{
				Spatial instance = null;

				System.Drawing.Color pixel = bitmap.GetPixel(i, j);
				string pixelHexValue = "#" + pixel.R.ToString("X2") + pixel.G.ToString("X2") + pixel.B.ToString("X2");

				bool isVisible = false;
				switch (pixelHexValue)
				{
					case "#000000": // Wall
						if (i < bitmap.Width / 2.0f && j < bitmap.Height / 2.0f)
						{
							instance = (Spatial)wallObject.Instance();
							isVisible = true;
						}
						else
						{
							instance = (Spatial)wallObjectTransparent.Instance();
						}
						break;

					case "#FFFFFF": // Ground tile
						if (i < bitmap.Width / 2.0f && j < bitmap.Height / 2.0f)
						{
							instance = (Spatial)groundTileObject.Instance();
							isVisible = true;
						}
						else
						{
							instance = (Spatial)groundTileObjectTransparent.Instance();
						}
						break;

					default:
						break;
				}

				instance.Translate(new Vector3(i, 0, j));
				if (isVisible)
					visibleMap.AddChild(instance);
				else
					invisibleMap.AddChild(instance);
			}
			GD.Print(i);
		}

		// Move camera
		Camera camera = (Camera)GetNode("./Camera");
		Transform transform = camera.Transform;

		// CENTERED CAMERA
		// transform.origin = new Vector3(bitmap.Width / 2.0f, (bitmap.Width + bitmap.Height) / 2.0f, bitmap.Height / 2.0f);
		// PERSPECTIVE CAMERA
		// transform.origin = new Vector3(bitmap.Width, (bitmap.Width + bitmap.Height) / 3.0f, bitmap.Height);
		// transform = transform.LookingAt(new Vector3(2 * bitmap.Width / 3.0f, 0, 2 * bitmap.Height / 3.0f), new Vector3(0, 1, 0));
		// ORTHOGONAL CAMERA
		int w = bitmap.Width;
		int h = bitmap.Height;
		int maxWH = Math.Max(w, h);

		transform.origin = new Vector3(w / 2.0f, maxWH, h / 2.0f);
		transform.origin += new Vector3(maxWH, maxWH, maxWH);
		transform = transform.LookingAt(new Vector3(w / 2.0f, 0, h / 2.0f), new Vector3(0, 1, 0));
		// transform.origin += new Vector3(w / 2.0f, maxWH, h / 2.0f);
		camera.Size = 3 + maxWH * 1.2f;

		// transform.origin = new Vector3(1.5f * maxWH, maxWH, 1.5f * maxWH);
		// transform = transform.LookingAt(new Vector3(maxWH / 2.0f, 0, maxWH / 2.0f), new Vector3(0, 1, 0));
		// camera.Size = 5 + maxWH;

		// transform.origin = new Vector3(bitmap.Width, (bitmap.Width + bitmap.Height) / 3.0f, bitmap.Height);
		// transform = transform.LookingAt(new Vector3(bitmap.Width / 2.0f, 0, bitmap.Height / 2.0f), new Vector3(0, 1, 0));
		// camera.Size = 5 + (bitmap.Width + bitmap.Height) / 2.0f;

		camera.Transform = transform;

		GD.Print("Done");
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
