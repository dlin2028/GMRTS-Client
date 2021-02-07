using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public abstract class Sprite
{

    //this class looks oddly similar to the Unit class...

    public Vector2 position;
    private Vector2 origin;
    private float rotation;
    private float scale;
    private List<Matrix> children;

    public Color Color { get; set; }

    public bool Enabled { get; set; }
    public Texture2D Texture { get; set; }
    public Matrix Transform { get; set; }

    public Vector2 Origin
    {
        get { return origin; }
        set { origin = value; updateTransform(); }
    }

    public float Rotation
    {
        get { return rotation; }
        set { rotation = value; updateTransform(); }
    }

    public float Scale
    {
        get { return scale; }
        set { scale = value; updateTransform(); }
    }
    public Vector2 Position
    {
        get { return position; }
        set { position = value; updateTransform(); }
    }

    public List<Matrix> Children
    {
        get
        {
            if (children == null) children = new List<Matrix>();

            return children;
        }
        set { children = value; }
    }

    public Sprite(Texture2D texture, float scale)
    {
        Texture = texture;
        this.position = Vector2.Zero;
        this.scale = scale;
        updateTransform();
    }
    public Sprite(Texture2D texture, Vector2 position, float scale)
    {
        Texture = texture;
        this.position = position;
        this.scale = scale;
        updateTransform();
    }
    protected void updateTransform()
    {
        Transform = Matrix.CreateTranslation(new Vector3(-origin, 0))
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateRotationZ(rotation)
                        * Matrix.CreateTranslation(new Vector3(position, 0));
    }

    public void SetParent(Matrix trans)
    {
        Children.Add(trans);
    }

    public bool Intersecting(Rectangle rect)
    {
        //may the separating axis theorem gods be with me

        var size = new Vector2(Texture.Width, Texture.Height) * scale;
        var rotationMatrix = Matrix.CreateRotationZ(rotation);

        Vector2 topLeft = Vector2.Transform(Vector2.Zero, Transform);
        Vector2 topRight = Vector2.Transform(new Vector2(Texture.Width, 0), Transform);
        Vector2 bottomLeft = Vector2.Transform(new Vector2(0, Texture.Height), Transform);
        Vector2 bottomRight = Vector2.Transform(new Vector2(Texture.Width, Texture.Height), Transform);

        Vector2[] verticies = { topLeft, topRight, bottomLeft, bottomRight };
        Vector2[] rectVerticies = { new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y), new Vector2(rect.X, rect.Y + rect.Height), new Vector2(rect.X + rect.Width, rect.Y + rect.Height) };
        Vector2[] projections = { topLeft - topRight, topLeft - bottomLeft, Vector2.UnitX, Vector2.UnitY };

        foreach (var proj in projections)
        {
            var projs = verticies.Select(x => Vector2.Dot(x, proj));
            var rectProjs = rectVerticies.Select(x => Vector2.Dot(x, proj));

            if (!(rectProjs.Min() <= projs.Max() && projs.Min() <= rectProjs.Max()))
            {
                return false;
            }
        }

        return true;
    }

    public bool Intersecting(Vector2 vector)
    {
        Vector2 topLeft = Vector2.Transform(Vector2.Zero, Transform);
        Vector2 topRight = Vector2.Transform(new Vector2(Texture.Width, 0), Transform);
        Vector2 bottomRight = Vector2.Transform(new Vector2(Texture.Width, Texture.Height), Transform);

        #region bad code don't look
        var x = vector.X;
        var y = vector.Y;
        var ax = topLeft.X;
        var ay = topLeft.Y;
        var bx = topRight.X;
        var by = topRight.Y;
        var dx = bottomRight.X;
        var dy = bottomRight.Y;

        var bax = bx - ax;
        var bay = by - ay;
        var dax = dx - ax;
        var day = dy - ay;

        if ((x - ax) * bax + (y - ay) * bay < 0.0) return false;
        if ((x - bx) * bax + (y - by) * bay > 0.0) return false;
        if ((x - ax) * dax + (y - ay) * day < 0.0) return false;
        if ((x - dx) * dax + (y - dy) * day > 0.0) return false;

        // "if we connect the point to three vertexes of the rectangle then the angles between those segments and sides should be acute" - some smart guy
        // https://stackoverflow.com/a/2752754

        //yes this can be sped up using matrix math
        //no i don't want to do it

        return true;
        #endregion
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
        updateTransform();
        for (int i = 0; i < Children.Count; i++)
        {
            children[i] *= Transform;
        }
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
        sb.Draw(Texture, Vector2.Transform(Position, Transform), null, Color);
    }
}