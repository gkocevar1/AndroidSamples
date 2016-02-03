using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Content.Res;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using Android.Content.PM;

namespace App1
{
    [Activity(Label = "Angie", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        #region mTapScreenTextAnimRes
        //private int[] mTapScreenTextAnimRes =
        //{
        //    Resource.Drawable.kiss0000_01_min,
        //    Resource.Drawable.kiss0001_01_min,
        //    Resource.Drawable.kiss0002_01_min,
        //    Resource.Drawable.kiss0003_01_min,
        //    Resource.Drawable.kiss0004_01_min,
        //    Resource.Drawable.kiss0005_01_min,
        //    Resource.Drawable.kiss0006_01_min,
        //    Resource.Drawable.kiss0007_01_min,
        //    Resource.Drawable.kiss0008_01_min,
        //    Resource.Drawable.kiss0009_01_min,
        //    Resource.Drawable.kiss0010_01_min,
        //    Resource.Drawable.kiss0011_01_min,
        //    Resource.Drawable.kiss0012_01_min,
        //    Resource.Drawable.kiss0013_01_min,
        //    Resource.Drawable.kiss0014_01_min,
        //    Resource.Drawable.kiss0015_01_min
        //};
        //private int[] mTapScreenTextAnimRes1 =
        //{
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66,
        //    66
        //};
        #endregion

        private ImageView _mainImageView;
        private TextView _statusTextView;
        private int _headCounter = 0;
        private int _pullShirtDownPicture = 0;
        private float _startX = 0;
        private float _startY = 0;
        private float _tempY = 0;
        bool _pullDownInProgress = false;
        private JavaList<AnimationDetails> _animations = new JavaList<AnimationDetails>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //ActionBar.Hide();
            //NavigationPage.SetHasNavigationBar(this, false);

            _statusTextView = FindViewById<TextView>(Resource.Id.statusTextView);
            _statusTextView.Text = "Status: init";

            _mainImageView = FindViewById<ImageView>(Resource.Id.animated_android);
            _mainImageView.Touch += _mainImageView_Touch;

            //CacheImages();

            //var metrics = Resources.DisplayMetrics;
            //var v = (int)metrics.DensityDpi;
            //var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
            //var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);

            //dp - Density - independent Pixels - an abstract unit that is based on the physical density of the screen.These units are relative to a 160 dpi screen, so one dp is one pixel on a 160 dpi screen. The ratio of dp-to-pixel will change with the screen density, but not necessarily in direct proportion. Note: The compiler accepts both "dip" and "dp", though "dp" is more consistent with "sp".

            // second option
            //new SceneAnimation(imageView, mTapScreenTextAnimRes, mTapScreenTextAnimRes1);
        }

        //private int ConvertPixelsToDp(float pixelValue)
        //{
        //    var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
        //    return dp;
        //}

        private void _mainImageView_Touch(object sender, View.TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    _statusTextView.Text = "Status: action down.";

                    _startX = e.Event.GetX();
                    _startY = e.Event.GetY();
                    break;

                case MotionEventActions.Move:
                    _statusTextView.Text = "Status: action move.";

                    _statusTextView.Text += " x=" + e.Event.GetX() + " y=" + e.Event.GetY();

                    MoveAction(e.Event.GetX(), e.Event.GetY());

                    _tempY = e.Event.GetY();

                    break;

                case MotionEventActions.Up:
                    {
                        _statusTextView.Text = "Status: action up.";
                        _statusTextView.Text += " x=" + e.Event.GetX() + " y=" + e.Event.GetY();// + " xdp=" + ConvertPixelsToDp(e.Event.GetX()) + " ydp=" + ConvertPixelsToDp(e.Event.GetY());
                       

                        if (_pullDownInProgress)
                        {
                            _pullDownInProgress = false;
                            _mainImageView.SetImageResource(Resource.Drawable.background);
                            _startX = 0;
                            _startY = 0;
                        }
                        else
                        {
                            // touch only
                            Animation anim = Animation.None;
                            float x = e.Event.GetX();
                            float y = e.Event.GetY();

                            var metrics = Resources.DisplayMetrics;

                            var height = metrics.HeightPixels;
                            var width = metrics.WidthPixels;

                            var xPercents = (int)((x * 100) / width);
                            var yPercents = (int)((y * 100) / height);

                            _statusTextView.Text += " x=" + xPercents + "% y=" + yPercents + "%";

                            // head - x: 360 - 700 ; y: 150 - 460
                            if (IsHead(x, y))
                            {

                                anim = _headCounter % 2 == 0 ? Animation.Kiss1 : Animation.Kiss2;
                                if (_headCounter++ == 3)
                                {
                                    anim = Animation.KissBack;
                                }
                            }
                            // right  hand poke - x: 320 - 430 ; y: 800 - 970
                            else if (IsRightHand(x, y))
                            {
                                anim = Animation.RightHandPoke;
                            }
                            // left hand poke - x: 630 - 740 ; y: 800 - 970
                            else if (IsLeftHand(x, y))
                            {
                                anim = Animation.LeftHandPoke;
                            }
                            // poke under belly - x: 480 - 580 ; y: 770 - 870
                            else if (IsBelly(x, y))
                            {
                                anim = Animation.PokeUnderBelly;
                            }
                            // jump - x: 370 - 700 ; y: 1380 - 1500
                            else if (IsFeet(x, y))
                            {
                                anim = Animation.Jump;
                            }

                            if (anim != Animation.Kiss1 && anim != Animation.Kiss2)
                            {
                                _headCounter = 0;
                            }

                            try
                            {
                                if (anim != Animation.None)
                                {
                                    var animation = CreateLoadingAnimationDrawable2(anim);
                                    _mainImageView.SetImageDrawable(animation);
                                    animation.Start();
                                }
                            }
                            catch (Exception ex)
                            {
                                _statusTextView.Text = ex.ToString();
                            }
                        }
                    }
                    break;
            }
        }

        private bool IsHead(float x, float y)
        {
            // head x(38-62) ; y(15-28)
            return IsWithinRange(x, y, 38, 62, 15, 28);
        }

        private bool IsLeftHand(float x, float y)
        {
            // left hand x(60-70) ; y (45-60)
            return IsWithinRange(x, y, 60, 70, 45, 60);
        }

        private bool IsRightHand(float x, float y)
        {
            // right hand x(30-40) ; y (45-60)
            return IsWithinRange(x, y, 30, 40, 45, 60);
        }

        private bool IsBelly(float x, float y)
        {
            // belly x(40-60) ; y (45-55)
            return IsWithinRange(x, y, 40, 60, 45, 55);
        }

        private bool IsFeet(float x, float y)
        {
            // feet x(35-65) ; y (80-92)
            return IsWithinRange(x, y, 35, 65, 80, 92);
        }

        private bool IsWithinRange(float x, float y, int xFrom, int xTo, int yFrom, int yTo)
        {
            var metrics = Resources.DisplayMetrics;

            var height = metrics.HeightPixels;
            var width = metrics.WidthPixels;

            var xPercents = (int)((x * 100) / width);
            var yPercents = (int)((y * 100) / height);

            return xPercents >= xFrom && xPercents <= xTo && yPercents >= yFrom && yPercents <= yTo;
        }

        private void MoveAction(float x, float y)
        {
            // tits x(40-60) ; y(30-40)
            // change picture every 2%
            
            if (!_pullDownInProgress && IsWithinRange(x, y, 40, 60, 30, 40))
            {
                _statusTextView.Text += " - inside the starting range";

                _pullDownInProgress = true;
            }
            else if (_pullDownInProgress)
            {
                _statusTextView.Text += " - in progress...";
                var diff = Diff(y);
                if (diff > 0)
                {
                    //direction = Direction.Down;
                    _statusTextView.Text += " DOWN";
                    
                    ChangePicture(diff);
                }
                else if (diff < 0)
                {
                    //direction = Direction.Up;
                    _statusTextView.Text += " UP";

                    ChangePicture(diff);
                }

                // none
            }
            else
            {
                _statusTextView.Text += " - out side from the valid range";
            }
        }

        private int Diff(float y)
        {
            var metrics = Resources.DisplayMetrics;

            var height = metrics.HeightPixels;
            
            var yPercents = (int)((y * 100) / height);

            var yTempPercents = (int)((_startY * 100) / height);

            return yPercents - yTempPercents;
        }
        
        private void ChangePicture(int diff)
        {
            _statusTextView.Text = diff.ToString();

            //int pullShirtDownPicture = (int)(Math.Floor((double)(diff / 2)));
            int pullShirtDownPicture = diff;

            if (pullShirtDownPicture == _pullShirtDownPicture || pullShirtDownPicture < 0 || pullShirtDownPicture > 7)
            {
                return;
            }

            _pullShirtDownPicture = pullShirtDownPicture;

            string bitmapStringId = string.Format("pull_shirt_down_000{0}_min", pullShirtDownPicture);
            int resID = Resources.GetIdentifier(bitmapStringId, "drawable", PackageName);

            _mainImageView.SetImageResource(resID);
        }

        private JavaDictionary<string, JavaList<Bitmap>> _animations2 = new JavaDictionary<string, JavaList<Bitmap>>();
        private AnimationDrawable CreateLoadingAnimationDrawable2(Animation animation)
        {
            ClearAnimation();

            var animationDrawable = new AnimationDrawable();
            JavaList<Bitmap> bitmaps = new JavaList<Bitmap>();

            if (_animations2.ContainsKey(animation.ToString()))
            {
                bitmaps = _animations2[animation.ToString()];

                foreach (var bitmap in bitmaps)
                {
                    

                    BitmapDrawable bd = new BitmapDrawable(bitmap);
                    animationDrawable.AddFrame(bd, 66);
                }

                return animationDrawable;
            }

            using (var stream = new StreamReader(Assets.Open(string.Format("{0}.xml", animation))))
            {
                var document = XDocument.Load(stream);
                var root = document.Root;

                // set oneshot flag
                animationDrawable.OneShot = bool.Parse(ReadAttribute(root, "oneshot"));
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
                    //options.InDensity = true;

                    using (Bitmap bitmap = BitmapFactory.DecodeResource(Resources, resID, options))
                    {
                        bitmaps.Add(bitmap);

                        BitmapDrawable bd = new BitmapDrawable(bitmap);
                        animationDrawable.AddFrame(bd, int.Parse(duration));
                    }
                }

                _animations2.Add(animation.ToString(), bitmaps);
            }

            return animationDrawable;
        }

        private AnimationDrawable CreateLoadingAnimationDrawable(Animation animation)
        {
            ClearAnimation();
            
            var animationDetails = _animations.FirstOrDefault(x => x.Animation == animation);

            var animationDrawable = new AnimationDrawable
            {
                OneShot = animationDetails.OneShot
            };

            foreach (var ad in animationDetails.Frames)
            {
                BitmapDrawable frame = new BitmapDrawable(ad.Bitmap);
                animationDrawable.AddFrame(frame, ad.Duration);
            }
               
            return animationDrawable;
        }

        

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

        private void CacheImages()
        {
            try
            {
                foreach (Animation item in Enum.GetValues(typeof(Animation)))
                {
                    //var animationDetails = BuildAnimationBitmaps(item);
                    //_animations.Add(animationDetails);
                }
            }
            catch (Exception ex)
            {
                // TODO: log somewhere
            }
        }

        private AnimationDetails BuildAnimationBitmaps(Animation animation)
        {
            var animationDetails = new AnimationDetails
            {
                Animation = animation
            };

            using (var stream = new StreamReader(Assets.Open(string.Format("{0}.xml", animation))))
            {
                var document = XDocument.Load(stream);
                var root = document.Root;

                // set oneshot flag
                animationDetails.OneShot = bool.Parse(ReadAttribute(root, "oneshot"));
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

                    using (Bitmap bitmap = BitmapFactory.DecodeResource(Resources, resID, options))
                    {
                        var frame = new FrameInfo
                        {
                            Bitmap = bitmap,
                            Duration = int.Parse(duration)
                        };
                        animationDetails.Frames.Add(frame);
                    }
                }
            }

            return animationDetails;
        }

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

        #region Comment
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

    /// <summary>
    /// Animation enumeration
    /// </summary>
    public enum Animation
    {
        /// <summary>
        /// The none{CC2D43FA-BBC4-448A-9D0B-7B57ADF2655C}
        /// </summary>
        None,

        /// <summary>
        /// The Kiss1
        /// </summary>
        Kiss1,

        /// <summary>
        /// The Kiss2
        /// </summary>
        Kiss2,

        /// <summary>
        /// The kiss back
        /// </summary>
        KissBack,

        Jump,

        /// <summary>
        /// The left hand poke
        /// </summary>
        LeftHandPoke,

        /// <summary>
        /// The right hand poke
        /// </summary>
        RightHandPoke,

        /// <summary>
        /// The poke under belly
        /// </summary>
        PokeUnderBelly
    }

    /// <summary>
    /// 
    /// </summary>
    public class AnimationDetails
    {
        /// <summary>
        /// Gets or sets the animation.
        /// </summary>
        /// <value>
        /// The animation.
        /// </value>
        public Animation Animation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [one shot].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [one shot]; otherwise, <c>false</c>.
        /// </value>
        public bool OneShot { get; set; }

        private JavaList<FrameInfo> _frames;
        /// <summary>
        /// Gets the frames.
        /// </summary>
        /// <value>
        /// The frames.
        /// </value>
        public JavaList<FrameInfo> Frames
        {
            get
            {
                if (_frames == null)
                {
                    _frames = new JavaList<FrameInfo>();
                }

                return _frames;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FrameInfo
    {
        /// <summary>
        /// Gets or sets the bitmap.
        /// </summary>
        /// <value>
        /// The bitmap.
        /// </value>
        public Bitmap Bitmap { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public int Duration { get; set; }
    }
}

