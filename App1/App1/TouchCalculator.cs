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
        }
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Determines whether head is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool IsHead(float x, float y)
        {
            // head x(38-62) ; y(15-28)
            return IsWithinRange(x, y, 38, 62, 15, 28);
        }

        /// <summary>
        /// Determines whether left hand is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool IsLeftHand(float x, float y)
        {
            // left hand x(60-70) ; y (45-60)
            return IsWithinRange(x, y, 60, 70, 45, 60);
        }

        /// <summary>
        /// Determines whether right hand is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool IsRightHand(float x, float y)
        {
            // right hand x(30-40) ; y (45-60)
            return IsWithinRange(x, y, 30, 40, 45, 60);
        }

        /// <summary>
        /// Determines whether belly is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool IsBelly(float x, float y)
        {
            // belly x(40-60) ; y (45-55)
            return IsWithinRange(x, y, 40, 60, 45, 55);
        }

        /// <summary>
        /// Determines whether feet is touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool IsFeet(float x, float y)
        {
            // feet x(35-65) ; y (80-92)
            return IsWithinRange(x, y, 35, 65, 80, 92);
        }

        #region IsTits
        /// <summary>
        /// Determines whether tits are touched.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool IsTits(float x, float y)
        {
            // tits x(40-60) ; y(30-40)
            return IsWithinRange(x, y, 40, 60, 30, 40);
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
        #endregion
    }
}