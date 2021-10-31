using Godot;
using System;

enum VisualType
{
    VisibleOnly = 1,
    Classic = 2,
    Actionnable = 3
}

public class VisualEntity
{
    // Visuals
    private Spatial _visible; public Spatial VisibleInstance { get { return _visible; } }
    private Spatial _ghost; public Spatial GhostInstance { get { return _ghost; } }
    private Spatial _visibleActionned; public Spatial VisibleActionnedInstance { get { return _visibleActionned; } }
    private Spatial _ghostActionned; public Spatial GhostActionnedInstance { get { return _ghostActionned; } }

    // Coordinates
    private int _x;
    private int _y;

    // State
    private VisualType _type;
    private bool _isVisible = false;
    private bool _isActionned = false;


    public VisualEntity(int x, int y, PackedScene visible)
    {
        _x = x;
        _y = y;

        _visible = CreateInstance(visible);

        _type = VisualType.VisibleOnly;
        _isVisible = true;

        Init();
    }

    public VisualEntity(int x, int y, PackedScene visible, PackedScene ghost)
    {
        _x = x;
        _y = y;

        _visible = CreateInstance(visible);
        _ghost = CreateInstance(ghost);

        _type = VisualType.Classic;

        Init();
    }

    public VisualEntity(int x, int y, PackedScene visible, PackedScene ghost, PackedScene visibleActionned, PackedScene ghostActionned)
    {
        _x = x;
        _y = y;

        _visible = CreateInstance(visible);
        _ghost = CreateInstance(ghost);
        _visibleActionned = CreateInstance(visibleActionned);
        _ghostActionned = CreateInstance(ghostActionned);

        _type = VisualType.Actionnable;

        Init();
    }

    private Spatial CreateInstance(PackedScene packedScene)
    {
        Spatial instance = (Spatial)packedScene.Instance();
        instance.Translate(new Vector3(_x, 0, _y));
        instance.Visible = false;
        return instance;
    }

    private void Init()
    {
        switch (_type)
        {
            case VisualType.VisibleOnly:
                _visible.Visible = true;
                break;
            case VisualType.Classic:
                _ghost.Visible = true;
                break;
            case VisualType.Actionnable:
                _ghost.Visible = true;
                break;
            default:
                break;
        }
    }

    public void ApplyRotation(float x, float y, float z)
    {
        Vector3 rotation = new Vector3(x, y, z);
        RotateInstance(_visible, rotation);
        RotateInstance(_ghost, rotation);
        RotateInstance(_visibleActionned, rotation);
        RotateInstance(_ghostActionned, rotation);
    }

    private void RotateInstance(Spatial instance, Vector3 rotation)
    {
        if (instance != null)
        {
            instance.RotateX(rotation.x);
            instance.RotateY(rotation.y);
            instance.RotateZ(rotation.z);
        }
    }

    public void SetVisible(bool visible)
    {
        switch (_type)
        {
            case VisualType.VisibleOnly:
                break;
            case VisualType.Classic:
                _visible.Visible = visible;
                _ghost.Visible = !visible;
                _isVisible = visible;
                break;
            case VisualType.Actionnable:
                if (_isActionned)
                {
                    _visibleActionned.Visible = visible;
                    _ghostActionned.Visible = !visible;
                }
                else
                {
                    _visible.Visible = visible;
                    _ghost.Visible = !visible;
                }
                _isVisible = visible;
                break;
            default:
                break;
        }
    }

    public void SetActionned(bool actionned)
    {
        if (_type == VisualType.Actionnable)
        {
            if (_isVisible)
            {
                _visibleActionned.Visible = actionned;
                _visible.Visible = !actionned;
            }
            else
            {
                _ghostActionned.Visible = actionned;
                _ghost.Visible = !actionned;
            }
            _isActionned = actionned;
        }
    }
}