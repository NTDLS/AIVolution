using Simulator.Engine;
using Simulator.Engine.Types;

namespace Simulator
{
    public partial class FormMain : Form
    {
        private EngineCore _core;

        protected override CreateParams CreateParams
        {
            get
            {
                //Paints all descendants of a window in bottom-to-top painting order using double-buffering.
                // For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000; //WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        public FormMain()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            BackColor = Color.FromArgb(1, 1, 10);

            _core = new EngineCore(this, new Size(Width, Height));

            _core.Start();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void FormMain_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            _core.Stop();
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            _core.Render(e.Graphics);
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(PlayerKey.SpeedBoost, KeyPressState.Down);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(PlayerKey.Forward, KeyPressState.Down);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(PlayerKey.RotateCounterClockwise, KeyPressState.Down);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(PlayerKey.Reverse, KeyPressState.Down);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(PlayerKey.RotateClockwise, KeyPressState.Down);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(PlayerKey.Fire, KeyPressState.Down);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(PlayerKey.Escape, KeyPressState.Down);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(PlayerKey.Left, KeyPressState.Down);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(PlayerKey.Right, KeyPressState.Down);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(PlayerKey.Up, KeyPressState.Down);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(PlayerKey.Down, KeyPressState.Down);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(PlayerKey.Enter, KeyPressState.Down);

            _core.Input.HandleSingleKeyPress(e.KeyCode);
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(PlayerKey.SpeedBoost, KeyPressState.Up);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(PlayerKey.Forward, KeyPressState.Up);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(PlayerKey.RotateCounterClockwise, KeyPressState.Up);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(PlayerKey.Reverse, KeyPressState.Up);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(PlayerKey.RotateClockwise, KeyPressState.Up);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(PlayerKey.Fire, KeyPressState.Up);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(PlayerKey.Escape, KeyPressState.Up);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(PlayerKey.Left, KeyPressState.Up);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(PlayerKey.Right, KeyPressState.Up);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(PlayerKey.Up, KeyPressState.Up);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(PlayerKey.Down, KeyPressState.Up);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(PlayerKey.Enter, KeyPressState.Up);
        }
    }
}
