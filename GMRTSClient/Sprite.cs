using System.Collections.Generic;
using System.Linq;
using GMRTSClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public abstract class Sprite
{
    private Texture2D texture;
    private bool enabled;

    /// <summary>
    /// Whether to draw and update
    /// </summary>
    public bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            enabled = value;
            Rect.Enabled = value;
        }
    }

    /// <summary>
    /// The texture used for drawing
    /// </summary>
    public Texture2D Texture
    {
        get { return texture; }
        set
        {
            texture = value;
            Rect = new TransformRect(Transform, new Vector2(texture.Width, texture.Height));
        }
    }

    /// <summary>
    /// The position, rotation and scale of an object.
    /// </summary>
    public Transform Transform { get; set; }

    /// <summary>
    /// The rect used for hit detection
    /// </summary>
    public TransformRect Rect { get; set; }


    /// <summary>
    /// The color mask used when drawing
    /// </summary>
    public Color Color { get; set; }

    public Sprite(Texture2D texture, float scale)
        : this(texture, Vector2.Zero, scale) { }
    public Sprite(Texture2D texture, Vector2 position, float scale)
    {
        Texture = texture;
        Transform = new Transform(position, Vector2.Zero, new Vector2(scale));
        Enabled = true;
    }
    public void Update()
    {
        if (Enabled)
        {
            update();
        }
    }
    protected virtual void update()
    {
        //override me
    }
    public void Draw(SpriteBatch sb)
    {
        if (Enabled)
        {
            draw(sb);
        }
    }
    protected virtual void draw(SpriteBatch sb)
    {
        sb.Draw(Texture, Transform.WorldPosition, null, Color);
    }
}