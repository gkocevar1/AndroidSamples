using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;

namespace App1
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += delegate 
            {
                ImageView imageView = FindViewById<ImageView>(Resource.Id.animated_android);
                imageView.SetImageResource(Resource.Animation.animate_android);
                AnimationDrawable animation = (AnimationDrawable)imageView.Drawable;
                animation.Start();
            };

            Button button2 = FindViewById<Button>(Resource.Id.MyButton2);
            button2.Click += delegate 
            {
                ImageView imageView = FindViewById<ImageView>(Resource.Id.animated_android);
                imageView.SetImageResource(Resource.Animation.animate_android_reverse);
                AnimationDrawable animation = (AnimationDrawable)imageView.Drawable;
                animation.Start();
            };
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            if (hasFocus)
            {
                ImageView imageView = FindViewById<ImageView>(Resource.Id.animated_android);
                AnimationDrawable animation = (AnimationDrawable)imageView.Drawable;
                animation.Start();
            }
        }
    }
}

