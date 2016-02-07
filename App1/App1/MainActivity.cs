﻿using System;
using System.IO;
using System.Threading.Tasks;
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

/*
Audio:

https://developer.xamarin.com/guides/android/application_fundamentals/working_with_audio/
http://stackoverflow.com/questions/29534506/talking-app-like-talking-tom-audio-recording-didnt-work-on-all-devices
http://stackoverflow.com/questions/8043387/android-audiorecord-supported-sampling-rates
http://developer.android.com/training/managing-audio/audio-focus.html
http://android-er.blogspot.si/2014/04/audiorecord-and-audiotrack-and-to.html   AudioRecord and AudioTrack, and to implement voice changer
http://stackoverflow.com/questions/14181449/android-detect-sound-level
http://stackoverflow.com/questions/16129480/how-to-test-sound-level-rms-algorithm

*/

namespace AppAngie
{
    /// <summary>
    /// Angie main activity class.
    /// </summary>
    /// <seealso cref="Android.App.Activity" />
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        #region Fields
        private ImageView _mainImageView;
        private TextView _statusTextView;
        private TextView _recordStatusTextView;
        private Button _soundButton;
        private Button _videoButton;
        
        private AudioRecorder _audioRecord;
        private TouchCalculator _touchCalculator;

        private int _pullShirtDownPicture = 0;
        private float _startY = 0;
        bool _pullDownInProgress = false;
        private bool _useNotifications = true;

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

            // cache all images
            CacheImages();

            _mainImageView = FindViewById<ImageView>(Resource.Id.animated_android);
            _mainImageView.Touch += MainImageView_Touch;

            // helpers - temporary
            // status text
            _statusTextView = FindViewById<TextView>(Resource.Id.statusTextView);
            _statusTextView.Text = "Status: init";

            // record button
            _soundButton = FindViewById<Button>(Resource.Id.soundButton);
            _soundButton.Click += async delegate
            {
                await StartOperationAsync(_audioRecord);
            };
            _videoButton = FindViewById<Button>(Resource.Id.soundButton);
            //_videoButton.Click += async delegate
            //{
            //    await StartOperationAsync(_audioRecord);
            //};

            _recordStatusTextView = FindViewById<TextView>(Resource.Id.recordStatusTextView);

            // init touch calculator
            _touchCalculator = new TouchCalculator(Resources.DisplayMetrics);
            // init audio recorder
            _audioRecord = new AudioRecorder();
        }

        private async Task StartOperationAsync(INotificationReceiver notificationReceiver)
        {
            //if (_useNotifications)
            //{
            //    bool haveFocus = nMan.RequestAudioResources(nRec);
            //    if (haveFocus)
            //    {
            //        status.Text = "Granted";
            //        await nRec.StartAsync();
            //    }
            //    else {
            //        status.Text = "Denied";
            //    }
            //}
            //else {
            //    await nRec.StartAsync();
            //}

            await notificationReceiver.StartAsync();
            //await CheckAplitude(notificationReceiver);

            //notificationReceiver.GetAmplitude();
        }

        private async Task CheckAplitude(INotificationReceiver notificationReceiver)
        {
            await Task.Run(() =>
            {
                //var t = (double)0;
                //var t2 = DateTime.Now;
                while (true)
                {
                    //try
                    //{
                    //    if ((DateTime.Now - t2).TotalSeconds > 5)
                    //    {
                    //        t2 = DateTime.Now;
                    //        t = (double)0;
                    //    }

                    //    var amplitude = notificationReceiver.GetAmplitude();
                    //    if (amplitude > t)
                    //    {
                    //        t = amplitude;
                    //        RunOnUiThread(() =>
                    //        {
                    //            _recordStatusTextView.Text = amplitude.ToString();
                    //        });
                    //    }
                    //}
                    //catch (Exception ex)
                    //{

                    //}

                    Task.Delay(250);
                }
            });
        }



        #endregion

        #region Methods
        #region Private
        #region Events
        #region MainImageView_Touch
        /// <summary>
        /// Handles the Touch event of the MainImageView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="View.TouchEventArgs"/> instance containing the event data.</param>
        private void MainImageView_Touch(object sender, View.TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    #region Down
                    {
                        _startY = e.Event.GetY();
                    }
                    #endregion
                    break;

                case MotionEventActions.Move:
                    #region Move
                    {
                        // mode is detected
                        MoveAction(e.Event.GetX(), e.Event.GetY());
                    }
                    #endregion
                    break;

                case MotionEventActions.Up:
                    #region Up
                    {
                        // if move event ends
                        if (_pullDownInProgress)
                        {
                            // reset start position of move
                            _startY = 0;
                            _pullDownInProgress = false;
                            ChangePicture(Resource.Drawable.background);
                        }
                        else
                        {
                            // touch only
                            float x = e.Event.GetX();
                            float y = e.Event.GetY();
                            var animation = _touchCalculator.FindAnimation(x, y);

                            _statusTextView.Text = string.Format("Status: UP ({0})", animation);

                            try
                            {
                                if (animation == AnimationType.None)
                                {
                                    break;
                                }

                                ClearAnimation();

                                //runable
                                //https://forums.xamarin.com/discussion/14652/how-to-convert-javas-runnable-in-c
                                
                                // get animation drawable object from cache
                                var animationDrawable = _animationsDrawable[animation.ToString()];
                                _mainImageView.SetImageDrawable(animationDrawable);
                                animationDrawable.Start();
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
        #endregion 
        #endregion

        #region MoveAction
        /// <summary>
        /// Moves the action.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        private void MoveAction(float x, float y)
        {
            var animation = _touchCalculator.FindAnimation(x, y);
            _statusTextView.Text = "Status: MOVE - ";            

            if (!_pullDownInProgress && animation == AnimationType.ShirtPullDown)
            {
                _pullDownInProgress = true;
            }
            else if (_pullDownInProgress)
            {
                // calculate distance in percents between y start postion and current y position (user move)
                var diff = _touchCalculator.CalculateDistanceInPercents(_startY, y);
                if (diff > 0)
                {
                    // down
                    ChangePicturePullShirtDown(diff);
                    _statusTextView.Text += "DOWN";
                }
                else if (diff < 0)
                {
                    // up
                    ChangePicturePullShirtDown(diff);
                    _statusTextView.Text += "UP";
                }
            }

            _statusTextView.Text = "Status: MOVE - nothing";
            //out side from the valid range
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
            ChangePicture(resourceId);
        }

        /// <summary>
        /// Changes the picture.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        private void ChangePicture(int resourceId)
        {
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
                    if (animation == AnimationType.None || animation == AnimationType.ShirtPullDown)
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
                _statusTextView.Text = ex.ToString();
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
        #endregion
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

