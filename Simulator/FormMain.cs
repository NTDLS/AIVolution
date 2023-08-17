using Simulator.Engine;
using Simulator.Engine.Actors;
using Simulator.Engine.Types;
using System.Text;

namespace Simulator
{
    public partial class FormMain : Form
    {
        private EngineCore _core;

        private readonly Control _drawingSurface = new Control();

        //This really shouldn't be necessary! :(
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

            _drawingSurface.BackColor = Color.FromArgb(1, 1, 10);
            Controls.Add(_drawingSurface);
            _drawingSurface.Dock = DockStyle.Fill;

            _drawingSurface.Paint += _drawingSurface_Paint;
            _drawingSurface.KeyUp += _drawingSurface_KeyUp;
            _drawingSurface.KeyDown += _drawingSurface_KeyDown;
            _drawingSurface.MouseDown += _drawingSurface_MouseDown;

            var doubleBuffered = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (doubleBuffered != null)
            {
                System.Reflection.PropertyInfo controlProperty = doubleBuffered;
                controlProperty.SetValue(_drawingSurface, true, null);
            }

            _core = new EngineCore(_drawingSurface, new Size(_drawingSurface.Width, _drawingSurface.Height));

            _core.Start();
        }

        private void FormMain_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            _core.Stop();
        }

        private void _drawingSurface_Paint(object? sender, PaintEventArgs e)
        {
            lock (this)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                _core.Render(e.Graphics);
            }
        }

        private void _drawingSurface_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(PlayerKey.SpeedBoost, KeyPressState.Down);
            else if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(PlayerKey.Forward, KeyPressState.Down);
            else if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(PlayerKey.RotateCounterClockwise, KeyPressState.Down);
            else if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(PlayerKey.Reverse, KeyPressState.Down);
            else if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(PlayerKey.RotateClockwise, KeyPressState.Down);
            else if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(PlayerKey.Fire, KeyPressState.Down);
            else if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(PlayerKey.Escape, KeyPressState.Down);
            else if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(PlayerKey.Left, KeyPressState.Down);
            else if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(PlayerKey.Right, KeyPressState.Down);
            else if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(PlayerKey.Up, KeyPressState.Down);
            else if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(PlayerKey.Down, KeyPressState.Down);
            else if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(PlayerKey.Enter, KeyPressState.Down);
            else if (e.KeyCode == Keys.P) _core.Input.KeyStateChanged(PlayerKey.Pause, KeyPressState.Down);

            _core.Input.HandleSingleKeyPress(e.KeyCode);
        }

        private void _drawingSurface_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(PlayerKey.SpeedBoost, KeyPressState.Up);
            else if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(PlayerKey.Forward, KeyPressState.Up);
            else if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(PlayerKey.RotateCounterClockwise, KeyPressState.Up);
            else if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(PlayerKey.Reverse, KeyPressState.Up);
            else if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(PlayerKey.RotateClockwise, KeyPressState.Up);
            else if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(PlayerKey.Fire, KeyPressState.Up);
            else if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(PlayerKey.Escape, KeyPressState.Up);
            else if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(PlayerKey.Left, KeyPressState.Up);
            else if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(PlayerKey.Right, KeyPressState.Up);
            else if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(PlayerKey.Up, KeyPressState.Up);
            else if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(PlayerKey.Down, KeyPressState.Up);
            else if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(PlayerKey.Enter, KeyPressState.Up);
            else if (e.KeyCode == Keys.P) _core.Input.KeyStateChanged(PlayerKey.Pause, KeyPressState.Down);
        }

        private void openBrainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool wasPaused = _core.IsPaused;
            if (wasPaused == false)
            {
                _core.TogglePause();
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text File (*.txt)|*.txt|All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _core.Stop();
                    _core.Start(ofd.FileName);
                }
            }

            if (wasPaused == false)
            {
                _core.TogglePause();
            }
        }

        private void saveFitestBrainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool wasPaused = _core.IsPaused;
            if (wasPaused == false)
            {
                _core.TogglePause();
            }

            var oldestBug = _core.Actors.Collection.OfType<ActorBug>().Where(o => o.Visible == true)
                .OrderByDescending(o => o.Brain.Fitness).Take(1).FirstOrDefault();

            if (oldestBug != null)
            {
                using var sfd = new SaveFileDialog();
                sfd.Filter = "Text File (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.FileName = "bugbrain.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    oldestBug.Brain.Save(sfd.FileName);
                }
            }

            if (wasPaused == false)
            {
                _core.TogglePause();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private ToolTip _interrogationTip = new ToolTip();

        private void _drawingSurface_MouseDown(object? sender, MouseEventArgs e)
        {
            lock (this)
            {
                ActorBase? tile = null;

                try
                {
                    tile = _core.Actors.Intersections(new Point<double>(e.X, e.Y), new Point<double>(1, 1)).LastOrDefault();
                }
                catch
                {
                    return;
                }

                if (tile != null)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        var menu = new ContextMenuStrip();

                        menu.ItemClicked += Menu_ItemClicked;
                        if (tile is ActorBug)
                        {
                            menu.Items.Add("Save Brain").Tag = tile;
                            menu.Items.Add("View Brain").Tag = tile;
                        }
                        menu.Items.Add("Delete").Tag = tile;

                        var location = new Point((int)e.X + 10, (int)e.Y);
                        menu.Show(_drawingSurface, location);
                    }
                    else if (e.Button == MouseButtons.Left)
                    {
                        var text = new StringBuilder();

                        text.AppendLine($"Type: {tile.GetType().Name}");
                        text.AppendLine($"UID: {tile.UID}");
                        text.AppendLine($"X,Y: {tile.X},{tile.X}");

                        if (text.Length > 0)
                        {
                            var location = new Point((int)e.X, (int)e.Y - tile.Size.Height);
                            _interrogationTip.Show(text.ToString(), _drawingSurface, location, 5000);
                        }
                    }
                }
            }
        }

        private void Menu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            if (sender == null) return;
            var menu = (ContextMenuStrip)sender;

            menu.Close();

            var tile = e.ClickedItem?.Tag as ActorBase;
            if (tile == null) return;

            if (e.ClickedItem?.Text == "Delete")
            {
                tile.Delete();
            }
            else if (e.ClickedItem?.Text == "Save Brain")
            {
                if (tile is ActorBug)
                {
                    var bug = (ActorBug)tile;

                    bool wasPaused = _core.IsPaused;
                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }

                    using var sfd = new SaveFileDialog();
                    sfd.Filter = "Text File (*.txt)|*.txt|All files (*.*)|*.*";
                    sfd.FileName = $"bugbrain {bug.UID}.txt";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        bug.Brain.Save(sfd.FileName);
                    }

                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }
                }
            }
            else if (e.ClickedItem?.Text == "View Brain")
            {
                if (tile is ActorBug)
                {
                    var bug = (ActorBug)tile;

                    bool wasPaused = _core.IsPaused;
                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }

                    var text = bug.Brain.Serialize();

                    using (var form = new FormViewBrain(text))
                    {
                        form.ShowDialog();
                    }

                    if (wasPaused == false)
                    {
                        _core.TogglePause();
                    }
                }
            }
        }
    }
}
