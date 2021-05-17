﻿using Simulator.Engine.Types;
using System;
using System.Drawing;

namespace Simulator.Engine
{
    public class ActorTextBlock : BaseGraphicObject
    {
        private Rectangle? _prevRegion = null;
        private Font _font;
        private Graphics _genericDC; //Not used for drawing, only measuring.
        private Brush _color;
        public bool IsPositionStatic { get; set; }

        #region Properties.

        double _height = 0;
        public Double Height
        {
            get
            {
                if (_height == 0)
                {
                    _height = _genericDC.MeasureString("void", _font).Height;
                }
                return _height;
            }
        }

        private string _lastTextSizeCheck;
        private Size _size = Size.Empty;
        public override Size Size
        {
            get
            {
                if (_size.IsEmpty || _text != _lastTextSizeCheck)
                {
                    var fSize = _genericDC.MeasureString(_text, _font);

                    _size = new Size((int)Math.Ceiling(fSize.Width), (int)Math.Ceiling(fSize.Height));
                    _lastTextSizeCheck = _text;
                }
                return _size;
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                //If we have previously drawn text, then we need to invalidate the entire region which it occupied.
                if (_prevRegion != null)
                {
                    Core.Display.DrawingSurface.Invalidate((Rectangle)_prevRegion);
                }

                //Now that we have used _prevRegion to invaldate the previous region, set it to the new region coords.
                //And invalidate them for the new text.
                var stringSize = _genericDC.MeasureString(_text, _font);
                _prevRegion = new Rectangle((int)X, (int)Y, (int)stringSize.Width, (int)stringSize.Height);
                Core.Display.DrawingSurface.Invalidate((Rectangle)_prevRegion);
            }
        }

        #endregion

        public ActorTextBlock(Core core, string font, Brush color, double size, PointD location, bool isPositionStatic, string name = "")
            : base(core, name)
        {
            IsPositionStatic = isPositionStatic;
            Location = new PointD(location);
            _color = color;
            _font = new Font(font, (float)size);
            _genericDC = core.Display.DrawingSurface.CreateGraphics();
        }

        public new void Render(Graphics dc)
        {
            if (Visable)
            {
                dc.DrawString(_text, _font, _color, (float)X, (float)Y);

                //TODO: Rotate text is required.
            }
        }
    }
}
