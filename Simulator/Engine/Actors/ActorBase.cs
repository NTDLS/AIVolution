using Simulator.Engine.Types;

namespace Simulator.Engine.Actors
{
    public class ActorBase
    {
        #region Properties.
        public EngineCore Core;

        public delegate void PositionChanged(ActorBase obj);
        public event PositionChanged? OnPositionChanged;

        public delegate void Rotated(ActorBase obj);
        public event Rotated? OnRotated;

        public delegate void VisibilityChange(ActorBase obj);
        public event VisibilityChange? OnVisibilityChange;

        public Guid UID { get; private set; } = Guid.NewGuid();
        public VelocityD Velocity { get; set; } = new VelocityD();
        public RotationMode RotationMode { get; set; }
        public string Name { get; set; }

        private Image? _image;
        private PointD _location = new PointD();

        /// <summary>
        /// Do not modify this location, it will not have any affect.
        /// </summary>
        public PointD Location
        {
            get
            {
                return new PointD(_location);
            }
            set
            {
                if (_location != value)
                {
                    Invalidate();
                    _location = value;
                    Invalidate();
                }
            }
        }

        public double X
        {
            get
            {
                return _location.X;
            }
            set
            {
                if (_location.X != value)
                {
                    Invalidate();
                    _location.X = value;
                    OnPositionChanged?.Invoke(this);
                    Invalidate();
                }
            }
        }

        public double Y
        {
            get
            {
                return _location.Y;
            }
            set
            {
                if (_location.Y != value)
                {
                    Invalidate();
                    _location.Y = value;
                    OnPositionChanged?.Invoke(this);
                    Invalidate();
                }
            }
        }

        private Size _size;
        public virtual Size Size
        {
            get
            {
                return _size;
            }
        }

        public RectangleF Bounds
        {
            get
            {
                return new RectangleF((float)_location.X, (float)_location.Y, Size.Width, Size.Height);
            }
        }

        public Rectangle BoundsI
        {
            get
            {
                return new Rectangle((int)_location.X, (int)_location.Y, Size.Width, Size.Height);
            }
        }

        private bool _isDeleted;
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
        }

        public void Delete()
        {
            _isDeleted = true;
            Visable = false;
        }

        private bool _isVisible = true;
        public bool Visable
        {
            get
            {
                return _isVisible && !_isDeleted;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;

                    var invalidRect = new Rectangle(
                        (int)(_location.X - _size.Width / 2.0),
                        (int)(_location.Y - _size.Height / 2.0),
                        _size.Width, _size.Height);

                    Core.Display.DrawingSurface.Invalidate(invalidRect);

                    OnVisibilityChange?.Invoke(this);
                }
            }
        }

        public bool IsOnScreen
        {
            get
            {
                return Core.Display.VisibleBounds.IntersectsWith(Bounds);
            }
        }

        #endregion

        public ActorBase(EngineCore core, string name = "")
        {
            Name = name;
            Core = core;
            RotationMode = RotationMode.Upsize;
            Velocity.MaxSpeed = Constants.Limits.MaxPlayerSpeed;
            Velocity.MaxRotationSpeed = Constants.Limits.MaxPlayerSpeed;
            Velocity.ThrottlePercentage = 0;
        }

        public virtual void ApplyIntelligence()
        {
            //Dumb as a rock...
        }

        public void Invalidate()
        {
            var invalidRect = new Rectangle(
                (int)(_location.X - _size.Width / 2.0),
                (int)(_location.Y - _size.Height / 2.0),
                _size.Width, _size.Height);
            Core.Display.DrawingSurface.Invalidate(invalidRect);
        }

        public void SetImage(Image image, Size? size = null)
        {
            _image = image;

            if (size != null)
            {
                _image = Utility.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }
            _size = new Size(_image.Size.Width, _image.Size.Height);
            Invalidate();
        }

        public void SetImage(string imagePath, Size? size = null)
        {
            _image = Core.GetBitmapCached(imagePath);
            if (_image != null)
            {
                if (size != null)
                {
                    _image = Utility.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
                }

                _size = new Size(_image.Size.Width, _image.Size.Height);
            }
            else
            {
                _size = new Size(0, 0);
            }
        }

        public Image? GetImage() => _image;

        public void Render(Graphics dc)
        {
            if (_isVisible && _image != null)
            {
                DrawImage(dc, _image);
            }
        }

        private void DrawImage(Graphics dc, Image rawImage, double? angleInDegrees = null)
        {
            double angle = (double)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            Bitmap bitmap = new Bitmap(rawImage);

            if (angle != 0 && RotationMode != RotationMode.None)
            {
                if (RotationMode == RotationMode.Upsize) //Very expensize
                {
                    var image = Utility.RotateImageWithUpsize(bitmap, angle, Color.Transparent);
                    Rectangle rect = new Rectangle((int)(_location.X - image.Width / 2.0), (int)(_location.Y - image.Height / 2.0), image.Width, image.Height);
                    dc.DrawImage(image, rect);
                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
                else if (RotationMode == RotationMode.Clip) //Much less expensive.
                {
                    var image = Utility.RotateImageWithClipping(bitmap, angle, Color.Transparent);
                    Rectangle rect = new Rectangle((int)(_location.X - image.Width / 2.0), (int)(_location.Y - image.Height / 2.0), image.Width, image.Height);
                    dc.DrawImage(image, rect);

                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
            }
            else //Almost free.
            {
                Rectangle rect = new Rectangle((int)(_location.X - bitmap.Width / 2.0), (int)(_location.Y - bitmap.Height / 2.0), bitmap.Width, bitmap.Height);
                dc.DrawImage(bitmap, rect);
            }
        }

        public bool Intersects(ActorBase otherObject)
        {
            if (Visable && otherObject.Visable && !IsDeleted && !otherObject.IsDeleted)
            {
                return Bounds.IntersectsWith(otherObject.Bounds);
            }
            return false;
        }

        /// <summary>
        /// Allows for intersect detection with larger/smaller "hit box".
        /// </summary>
        /// <param name="otherObject"></param>
        /// <param name="sizeAdjust"></param>
        /// <returns></returns>
        public bool Intersects(ActorBase otherObject, PointD sizeAdjust)
        {
            if (Visable && otherObject.Visable && !IsDeleted && !otherObject.IsDeleted)
            {
                var alteredHitBox = new RectangleF(
                    otherObject.Bounds.X - (float)(sizeAdjust.X / 2),
                    otherObject.Bounds.Y - (float)(sizeAdjust.Y / 2),
                    otherObject.Bounds.Width + (float)(sizeAdjust.X / 2),
                    otherObject.Bounds.Height + (float)(sizeAdjust.Y / 2));

                return Bounds.IntersectsWith(alteredHitBox);
            }
            return false;
        }

        public List<ActorBase> Intersections()
        {
            var intersections = new List<ActorBase>();

            foreach (var intersection in Core.Actors.Collection)
            {
                if (intersection != this && intersection.Visable && intersection is not ActorTextBlock)
                {
                    if (Intersects(intersection))
                    {
                        intersections.Add(intersection);
                    }
                }
            }

            return intersections;
        }

        public void Rotate(double degrees)
        {
            if (degrees != 0)
            {
                Velocity.Angle.Degrees += degrees;
                Invalidate();
                OnRotated?.Invoke(this);
            }
        }

        public void MoveInDirectionOf(PointD location, double? speed = null)
        {
            Velocity.Angle.Degrees = PointD.AngleTo(Location, location);
            if (speed != null)
            {
                Velocity.MaxSpeed = (double)speed;
            }
        }

        public void MoveInDirectionOf(ActorBase obj, double? speed = null)
        {
            Velocity.Angle.Degrees = PointD.AngleTo(Location, obj.Location);

            if (speed != null)
            {
                Velocity.MaxSpeed = (double)speed;
            }
        }

        /// <summary>
        /// Calculated the difference in heading angle from one object to get to another.
        /// </summary>
        /// <param name="atObj"></param>
        /// <returns></returns>
        public double DeltaAngle(ActorBase atObj)
        {
            return Utility.DeltaAngle(this, atObj);
        }

        /// <summary>
        /// Calculates the angle in degrees of one objects location to another location.
        /// </summary>
        /// <param name="atObj"></param>
        /// <returns></returns>
        public double AngleTo(ActorBase atObj)
        {
            return Utility.AngleTo(this, atObj);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle)
        {
            return Utility.IsPointingAt(this, atObj, toleranceDegrees, maxDistance, offsetAngle);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees, double maxDistance)
        {
            return Utility.IsPointingAt(this, atObj, toleranceDegrees, maxDistance);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees)
        {
            return Utility.IsPointingAt(this, atObj, toleranceDegrees);
        }

        public double DistanceTo(ActorBase to)
        {
            return PointD.DistanceTo(Location, to.Location);
        }

        public double DistanceTo(PointD to)
        {
            return PointD.DistanceTo(Location, to);
        }
    }
}
