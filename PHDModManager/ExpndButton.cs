using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class ExpandButton : Control
{
    private bool _expanded = false;
    private float _angle = 0f;
    private Timer _animTimer;
    public event EventHandler ExpandedChanged;

    public bool Expanded
    {
        get => _expanded;
        set
        {
            _expanded = value;
            _animTimer.Start();
            ExpandedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public ExpandButton()
    {
        Size = new Size(24, 24);
        DoubleBuffered = true;
        Cursor = Cursors.Hand;

        // Habilitar fondo transparente real
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;

        _animTimer = new Timer();
        _animTimer.Interval = 10;
        _animTimer.Tick += AnimTimer_Tick;
    }

    private void AnimTimer_Tick(object sender, EventArgs e)
    {
        float target = _expanded ? 180f : 0f;
        float speed = 0.2f;

        if (Math.Abs(_angle - target) < 1f)
        {
            _angle = target;
            _animTimer.Stop();
        }
        else
        {
            _angle += (target - _angle) * speed;
        }
        Invalidate();
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        Expanded = !Expanded;
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        // No pintar ningún fondo propio - dejar que se vea lo que hay detrás
        base.OnPaintBackground(pevent);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        float cx = Width / 2f;
        float cy = Height / 2f;

        g.TranslateTransform(cx, cy);
        g.RotateTransform(_angle);

        using (var brush = new SolidBrush(Color.FromArgb(200, 200, 200)))
        {
            PointF[] arrow = new PointF[]
            {
                new PointF(-5, -3),
                new PointF(5, -3),
                new PointF(0, 4)
            };
            g.FillPolygon(brush, arrow);
        }

        g.ResetTransform();
    }
}