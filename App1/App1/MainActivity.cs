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

namespace App1
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        #region mTapScreenTextAnimRes
        private int[] mTapScreenTextAnimRes =
        {
            Resource.Drawable.kiss0000_01_min,
            Resource.Drawable.kiss0001_01_min,
            Resource.Drawable.kiss0002_01_min,
            Resource.Drawable.kiss0003_01_min,
            Resource.Drawable.kiss0004_01_min,
            Resource.Drawable.kiss0005_01_min,
            Resource.Drawable.kiss0006_01_min,
            Resource.Drawable.kiss0007_01_min,
            Resource.Drawable.kiss0008_01_min,
            Resource.Drawable.kiss0009_01_min,
            Resource.Drawable.kiss0010_01_min,
            Resource.Drawable.kiss0011_01_min,
            Resource.Drawable.kiss0012_01_min,
            Resource.Drawable.kiss0013_01_min,
            Resource.Drawable.kiss0014_01_min,
            Resource.Drawable.kiss0015_01_min
        };
        private int[] mTapScreenTextAnimRes1 =
        {
            66,
            66,
            66,
            66,
            66,
            66,
            66,
            66,
            66,
            66,
            66,
            66,
            66,
            66,
            66,
            66
        }; 
        #endregion

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // sample 1
            ImageView imageView = FindViewById<ImageView>(Resource.Id.animated_android);
            //new SceneAnimation(imageView, mTapScreenTextAnimRes, mTapScreenTextAnimRes1);

            // sample 2
            var animation = CreateLoadingAnimationDrawable();
            imageView.SetImageDrawable(animation);
            
            animation.Start();

            
            
        }

        protected AnimationDrawable CreateLoadingAnimationDrawable()
        {
            AnimationDrawable animation = new AnimationDrawable();
            animation.OneShot = true;

            for (int i = 0; i < 15; i++)
            {
                int index = i;
                string stringIndex = index.ToString("00");
                string bitmapStringId = "kiss00" + stringIndex + "_01_min";
                int resID = this.Resources.GetIdentifier(bitmapStringId, "drawable", this.PackageName);

                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InJustDecodeBounds = false;
                options.InPreferredConfig = Bitmap.Config.Rgb565;
                options.InDither = true;

                Bitmap bitmap = BitmapFactory.DecodeResource(this.Resources, resID, options);
                BitmapDrawable frame = new BitmapDrawable(bitmap);
                
                animation.AddFrame(frame, 66);
            }

            return animation;
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

        //public override void OnWindowFocusChanged(bool hasFocus)
        //{
        //    //if (hasFocus)
        //    //{
        //    //    ImageView imageView = FindViewById<ImageView>(Resource.Id.animated_android);
        //    //    AnimationDrawable animation = (AnimationDrawable)imageView.Drawable;
        //    //    animation.Start();
        //    //}
        //} 
        #endregion
    }
}

