using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class ToggleSwitch : Control
{
    private bool _checked = false;
    private float _knobPosition = 0f;
    private Timer _animTimer;
    public event EventHandler CheckedChanged;

    public bool Checked
    {
        get => _checked;
        set
        {
            _checked = value;
            _animTimer.Start();
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public ToggleSwitch()
    {
        Size = new Size(46, 22);
        DoubleBuffered = true;
        Cursor = Cursors.Hand;

        _animTimer = new Timer();
        _animTimer.Interval = 10;
        _animTimer.Tick += AnimTimer_Tick;
    }

    private void AnimTimer_Tick(object sender, EventArgs e)
    {
        float target = _checked ? 1f : 0f;
        float speed = 0.15f;

        if (Math.Abs(_knobPosition - target) < 0.01f)
        {
            _knobPosition = target;
            _animTimer.Stop();
        }
        else
        {
            _knobPosition += (target - _knobPosition) * speed;
        }
        Invalidate();
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        Checked = !Checked;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        Color off = Color.FromArgb(90, 90, 90);
        Color on = Color.FromArgb(76, 175, 80);
        Color bg = Lerp(off, on, _knobPosition);

        using (var brush = new SolidBrush(bg))
        {
            var path = RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), Height / 2);
            g.FillPath(brush, path);
        }

        int knobDiameter = Height - 4;
        int minX = 2;
        int maxX = Width - knobDiameter - 2;
        int knobX = (int)(minX + (maxX - minX) * _knobPosition);

        using (var knobBrush = new SolidBrush(Color.White))
        {
            g.FillEllipse(knobBrush, knobX, 2, knobDiameter, knobDiameter);
        }
    }

    private Color Lerp(Color a, Color b, float t)
    {
        int r = (int)(a.R + (b.R - a.R) * t);
        int gr = (int)(a.G + (b.G - a.G) * t);
        int bl = (int)(a.B + (b.B - a.B) * t);
        return Color.FromArgb(r, gr, bl);
    }

    private GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}