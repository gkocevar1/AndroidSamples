using System;
using System.IO;
using System.Xml.Linq;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AppAngie
{
    [Activity(Label = "Angie", MainLauncher = true, Icon = "@drawable/Angie", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        #region Fields
        private ImageView _mainImageView;
        private TextView _statusTextView;
        private int _headCounter = 0;
        private int _pullShirtDownPicture = 0;
        private float _startY = 0;
        bool _pullDownInProgress = false;
        private TouchCalculator _tc;

        // cache animations
        private JavaDictionary<string, AnimationDrawable> _animationsDrawable = new JavaDictionary<string, AnimationDrawable>(); 
        #endregion

        #region OnCreate
        /// <summary>
        /// Called when [create].
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            _statusTextView = FindViewById<TextView>(Resource.Id.statusTextView);
            _statusTextView.Text = "Status: init";

            _mainImageView = FindViewById<ImageView>(Resource.Id.animated_android);
            _mainImageView.Touch += _mainImageView_Touch;

            // cache all images
            CacheImages();

            // init touch calculator
            _tc = new TouchCalculator(Resources.DisplayMetrics);
        } 
        #endregion

        

        private void _mainImageView_Touch(object sender, View.TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    #region Down
                    {
                        _statusTextView.Text = "Status: action down.";
                        _startY = e.Event.GetY();
                    } 
                    #endregion
                    break;

                case MotionEventActions.Move:
                    #region Move
                    {
                        _statusTextView.Text = "Status: action move.";

                        _statusTextView.Text += " x=" + e.Event.GetX() + " y=" + e.Event.GetY();

                        MoveAction(e.Event.GetX(), e.Event.GetY());
                    } 
                    #endregion
                    break;

                case MotionEventActions.Up:
                    #region Up
                    {
                        _statusTextView.Text = "Status: action up.";
                        _statusTextView.Text += " x=" + e.Event.GetX() + " y=" + e.Event.GetY();// + " xdp=" + ConvertPixelsToDp(e.Event.GetX()) + " ydp=" + ConvertPixelsToDp(e.Event.GetY());

                        if (_pullDownInProgress)
                        {
                            _pullDownInProgress = false;
                            _startY = 0;
                            _mainImageView.SetImageResource(Resource.Drawable.background);
                        }
                        else
                        {
                            // touch only
                            var animation = AnimationType.None;
                            float x = e.Event.GetX();
                            float y = e.Event.GetY();

                            var metrics = Resources.DisplayMetrics;

                            var height = metrics.HeightPixels;
                            var width = metrics.WidthPixels;

                            var xPercents = (int)((x * 100) / width);
                            var yPercents = (int)((y * 100) / height);

                            _statusTextView.Text += " x=" + xPercents + "% y=" + yPercents + "%";

                            if (_tc.IsHead(x, y))
                            {
                                animation = _headCounter % 2 == 0 ? AnimationType.Kiss1 : AnimationType.Kiss2;
                                if (_headCounter++ == 3)
                                {
                                    animation = AnimationType.KissBack;
                                }
                            }
                            else if (_tc.IsRightHand(x, y))
                            {
                                animation = AnimationType.RightHandPoke;
                            }
                            else if (_tc.IsLeftHand(x, y))
                            {
                                animation = AnimationType.LeftHandPoke;
                            }
                            else if (_tc.IsBelly(x, y))
                            {
                                animation = AnimationType.PokeUnderBelly;
                            }
                            else if (_tc.IsFeet(x, y))
                            {
                                animation = AnimationType.Jump;
                            }

                            if (animation != AnimationType.Kiss1 && animation != AnimationType.Kiss2)
                            {
                                _headCounter = 0;
                            }

                            try
                            {
                                if (animation != AnimationType.None)
                                {
                                    ClearAnimation();

                                    var animationDrawable = _animationsDrawable[animation.ToString()];
                                    _mainImageView.SetImageDrawable(animationDrawable);
                                    animationDrawable.Start();
                                }
                            }
                            catch (Exception ex)
                            {
                                _statusTextView.Text = ex.ToString();
                            }
                        }
                    } 
                    #endregion
                    break;
            }
        }



        #region MoveAction
        /// <summary>
        /// Moves the action.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        private void MoveAction(float x, float y)
        {
            if (!_pullDownInProgress && _tc.IsTits(x, y))
            {
                _statusTextView.Text += " - inside the starting range";
                _pullDownInProgress = true;
            }
            else if (_pullDownInProgress)
            {
                _statusTextView.Text += " - in progress...";
                // calculate distance in percents between y start postion and current y position (user move)
                var diff = _tc.CalculateDistanceInPercents(_startY, y);
                if (diff > 0)
                {
                    _statusTextView.Text += " DOWN";
                    ChangePicturePullShirtDown(diff);
                }
                else if (diff < 0)
                {
                    _statusTextView.Text += " UP";
                    ChangePicturePullShirtDown(diff);
                }
            }
            else
            {
                _statusTextView.Text += " - out side from the valid range";
            }
        } 
        #endregion

        #region ChangePicturePullShirtDown
        /// <summary>
        /// Changes the picture pull shirt down.
        /// </summary>
        /// <param name="diff">The difference in percent between y start position and current y position.</param>
        private void ChangePicturePullShirtDown(int diff)
        {
            _statusTextView.Text = diff.ToString();

            // 8 pictures for pull down
            if (diff == _pullShirtDownPicture || diff < 0 || diff > 7)
            {
                return;
            }

            _pullShirtDownPicture = diff;

            string bitmapString = string.Format("pull_shirt_down_000{0}_min", diff);
            ChangePicture(bitmapString);
        } 
        #endregion

        #region ChangePicture
        /// <summary>
        /// Changes the picture.
        /// </summary>
        /// <param name="bitmapString">The bitmap string.</param>
        private void ChangePicture(string bitmapString)
        {
            var resourceId = Resources.GetIdentifier(bitmapString, "drawable", PackageName);
            _mainImageView.SetImageResource(resourceId);
        }
        #endregion

        #region ClearAnimation
        /// <summary>
        /// Clears the animation.
        /// </summary>
        private void ClearAnimation()
        {
            var animation = _mainImageView.Drawable as AnimationDrawable;
            if (animation == null)
            {
                return;
            }

            animation.Stop();

            _mainImageView.SetImageResource(Resource.Drawable.background);

            if (animation.NumberOfFrames > 0)
            {
                for (int i = 0; i < animation.NumberOfFrames; ++i)
                {
                    BitmapDrawable frame = animation.GetFrame(i) as BitmapDrawable;
                    if (frame != null)
                    {
                        //Bitmap bitmap = frame.Bitmap;
                        //bitmap.Recycle();
                        //bitmap.Dispose();
                        //bitmap = null;

                        frame.SetCallback(null);
                    }
                }

                animation.SetCallback(null);
                animation = null;
            }
        } 
        #endregion

        #region CacheImages
        /// <summary>
        /// Caches the images.
        /// </summary>
        private void CacheImages()
        {
            try
            {
                foreach (AnimationType animation in Enum.GetValues(typeof(AnimationType)))
                {
                    if (animation == AnimationType.None)
                    {
                        continue;
                    }
                    
                    var animationDrawable = new AnimationDrawable();

                    using (var stream = new StreamReader(Assets.Open(string.Format("{0}.xml", animation))))
                    {
                        var document = XDocument.Load(stream);
                        var root = document.Root;
                        var elements = root.Descendants();

                        foreach (var element in elements)
                        {
                            var drawable = ReadAttribute(element, "drawable");
                            var duration = ReadAttribute(element, "duration");

                            var bitmapStringId = drawable.Substring(drawable.IndexOf('/') + 1);
                            var resID = Resources.GetIdentifier(bitmapStringId, "drawable", PackageName);

                            BitmapFactory.Options options = new BitmapFactory.Options();
                            options.InJustDecodeBounds = false;
                            options.InPreferredConfig = Bitmap.Config.Rgb565;
                            options.InDither = true;
                            options.InSampleSize = 2;
                            options.InPurgeable = true;

                            using (Bitmap bitmap = BitmapFactory.DecodeResource(Resources, resID, options))
                            {
                                BitmapDrawable bitmapDrawable = new BitmapDrawable(bitmap);
                                animationDrawable.AddFrame(bitmapDrawable, int.Parse(duration));
                            }
                        }
                        
                        // cache animation
                        _animationsDrawable.Add(animation.ToString(), animationDrawable);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: log this
            }
        } 
        #endregion

        #region ReadAttribute
        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        private string ReadAttribute(XElement element, string attributeName)
        {
            const string nameSpace = "http://schemas.android.com/apk/res/android";

            var attribute = element.Attribute(XName.Get(attributeName, nameSpace));
            if (attribute == null)
            {
                return null;
            }

            return attribute.Value;
        } 
        #endregion
    }
    #region Comment
    //var metrics = Resources.DisplayMetrics;
    //var v = (int)metrics.DensityDpi;
    //var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
    //var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);

    //dp - Density - independent Pixels - an abstract unit that is based on the physical density of the screen.These units are relative to a 160 dpi screen, so one dp is one pixel on a 160 dpi screen. The ratio of dp-to-pixel will change with the screen density, but not necessarily in direct proportion. Note: The compiler accepts both "dip" and "dp", though "dp" is more consistent with "sp".

    // second option
    //new SceneAnimation(imageView, mTapScreenTextAnimRes, mTapScreenTextAnimRes1);

    //private int ConvertPixelsToDp(float pixelValue)
    //{
    //    var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
    //    return dp;
    //}

    //public static Bitmap DecodeSampledBitmapFromResource(Resources res, int resId, int reqWidth, int reqHeight)
    //{

    //    // First decode with inJustDecodeBounds=true to check dimensions
    //    BitmapFactory.Options options = new BitmapFactory.Options();
    //    options.InJustDecodeBounds = true;

    //    Bitmap bitmap = BitmapFactory.DecodeResource(res, resId, options);

    //    // Calculate inSampleSize
    //    options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

    //    // Decode bitmap with inSampleSize set
    //    options.InJustDecodeBounds = false;
    //    options.InPreferredConfig = Bitmap.Config.Rgb565;
    //    options.InDither = true;
    //    return BitmapFactory.DecodeResource(res, resId, options);
    //}

    //public static int CalculateInSampleSize(
    //    BitmapFactory.Options options, int reqWidth, int reqHeight)
    //{
    //    // Raw height and width of image
    //    int height = options.OutHeight;
    //    int width = options.OutWidth;
    //    int inSampleSize = 1;

    //    if (height > reqHeight || width > reqWidth)
    //    {

    //        int halfHeight = height / 2;
    //        int halfWidth = width / 2;

    //        // Calculate the largest inSampleSize value that is a power of 2 and keeps both
    //        // height and width larger than the requested height and width.
    //        while ((halfHeight / inSampleSize) > reqHeight
    //                && (halfWidth / inSampleSize) > reqWidth)
    //        {
    //            inSampleSize *= 2;
    //        }
    //    }

    //    return inSampleSize;
    //}
    #endregion
}

