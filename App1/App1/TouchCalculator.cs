using Android.Util;

namespace AppAngie
{
    /// <summary>
    /// Calculates touch positions
    /// </summary>
    public class TouchCalculator
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchCalculator"/> class.
        /// </summary>
        /// <param name="displayMetrics">The display metrics.</param>
        public TouchCalculator(DisplayMetrics displayMetrics)
        {
            _displayMetrics = displayMetrics;
            _headCounter = 0;
        }
        #endregion

        #region Methods
        #region Public
        #region FindAnimation
        /// <summary>
        /// Finds the animation.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public AnimationType FindAnimation(float x, float y)
        {
            var animation = AnimationType.None;

            if (IsBelly(x, y))
            {
                animation = AnimationType.PokeUnderBelly;
            }
            else if (IsFeet(x, y))
            {
                animation = AnimationType.Jump;
            }
            else if (IsHead(x, y))
            {
                animation = _headCounter % 2 == 0 ? AnimationType.Kiss1 : AnimationType.Kiss2;
                if (_headCounter++ == 3)
                {
                    animation = AnimationType.KissBack;
                }
            }
            else if (IsLeftHand(x, y))
            {
                animation = AnimationType.LeftHandPoke;
            }
            else if (IsRightHand(x, y))
            {
                animation = AnimationType.RightHandPoke;
            }
            else if (IsTits(x, y))
            {
                animation = AnimationType.ShirtPullDown;
            }
            
            if (animation != AnimationType.Kiss1 && 
                animation != AnimationType.Kiss2)
            {
                _headCounter = 0;
            }

            return animation;
        } 
        #endregion
        
        #region CalculateDistanceInPercents
        /// <summary>
        /// Calculates the distance in percents.
        /// </summary>
        /// <param name="startY">The start y.</param>
        /// <param name="endY">The end y.</param>
        /// <returns></returns>
        public int CalculateDistanceInPercents(float startY, float endY)
        {
            var height = _displayMetrics.HeightPixels;

            var yEndPercents = (int)((endY * 100) / height);
            var yStartPercents = (int)((startY * 100) / height);

            return yEndPercents - yStartPercents;
        }
        #endregion
        #endregion

        #region Private
        #region IsHead
        /// <summary>
        /// Determines whether head is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private bool IsHead(float x, float y)
        {
            // head x(38-62) ; y(15-28)
            return IsWithinRange(x, y, 38, 62, 15, 28);
        }
        #endregion

        #region IsLeftHand
        /// <summary>
        /// Determines whether left hand is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private bool IsLeftHand(float x, float y)
        {
            // left hand x(60-70) ; y (45-60)
            return IsWithinRange(x, y, 60, 70, 45, 60);
        }
        #endregion

        #region IsRightHand
        /// <summary>
        /// Determines whether right hand is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private bool IsRightHand(float x, float y)
        {
            // right hand x(30-40) ; y (45-60)
            return IsWithinRange(x, y, 30, 40, 45, 60);
        }
        #endregion

        #region IsBelly
        /// <summary>
        /// Determines whether belly is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private bool IsBelly(float x, float y)
        {
            // belly x(40-60) ; y (45-55)
            return IsWithinRange(x, y, 40, 60, 45, 55);
        } 
        #endregion

        #region IsFeet
        /// <summary>
        /// Determines whether feet is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private bool IsFeet(float x, float y)
        {
            // feet x(35-65) ; y (80-92)
            return IsWithinRange(x, y, 35, 65, 80, 92);
        }
        #endregion

        #region IsTits
        /// <summary>
        /// Determines whether tits are touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private bool IsTits(float x, float y)
        {
            // tits x(40-60) ; y(30-40)
            return IsWithinRange(x, y, 40, 60, 30, 40);
        }
        #endregion

        #region IsWithinRange
        /// <summary>
        /// Determines whether [is within range].
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="xFrom">The x from.</param>
        /// <param name="xTo">The x to.</param>
        /// <param name="yFrom">The y from.</param>
        /// <param name="yTo">The y to.</param>
        /// <returns></returns>
        private bool IsWithinRange(float x, float y, int xFrom, int xTo, int yFrom, int yTo)
        {
            var metrics = _displayMetrics;

            var height = metrics.HeightPixels;
            var width = metrics.WidthPixels;

            var xPercents = (int)((x * 100) / width);
            var yPercents = (int)((y * 100) / height);

            return xPercents >= xFrom && xPercents <= xTo && yPercents >= yFrom && yPercents <= yTo;
        }
        #endregion
        #endregion
        #endregion

        #region Fields
        private DisplayMetrics _displayMetrics;
        private int _headCounter = 0;
        #endregion
    }
}