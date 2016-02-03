//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.Graphics.Drawables;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Java.Lang;

//namespace AppAngie
//{
//    /**
// * Provides a callback when a non-looping {@link AnimationDrawable} completes its animation sequence. More precisely,
// * {@link #onAnimationComplete()} is triggered when {@link View#invalidateDrawable(Drawable)} has been called on the
// * last frame.
// * 
// * @author Benedict Lau
// */
//    public class AnimationDrawableCallback : Drawable.ICallback
//    {
//        /**
//     * The total number of frames in the {@link AnimationDrawable}.
//     */
//        private int mTotalFrames;

//        /**
//         * The last frame of {@link Drawable} in the {@link AnimationDrawable}.
//         */
//        private Drawable mLastFrame;

//    /**
//     * The current frame of {@link Drawable} in the {@link AnimationDrawable}.
//     */
//    private int mCurrentFrame = 0;

//        /**
//         * The client's {@link Callback} implementation. All calls are proxied to this wrapped {@link Callback}
//         * implementation after intercepting the events we need.
//         */
//        private Drawable.ICallback mWrappedCallback;

//        /**
//         * Flag to ensure that {@link #onAnimationCompleted()} is called only once, since
//         * {@link #invalidateDrawable(Drawable)} may be called multiple times.
//         */
//        private bool mIsCallbackTriggered = false;

//        public IntPtr Handle
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public AnimationDrawableCallback(AnimationDrawable animationDrawable, Drawable.ICallback callback)
//        {
//            mTotalFrames = animationDrawable.NumberOfFrames;
//            mLastFrame = animationDrawable.GetFrame(mTotalFrames - 1);
//            mWrappedCallback = callback;
//        }

//        public void Dispose()
//        {

//        }

//        public void InvalidateDrawable(Drawable who)
//        {
//            if (mWrappedCallback != null)
//            {
//                mWrappedCallback.InvalidateDrawable(who);
//            }

//            if (!mIsCallbackTriggered && mLastFrame != null && mLastFrame.Equals(who.Current))
//            {
//                mIsCallbackTriggered = true;
//                onAnimationCompleted();
//            }
//        }

//        public void ScheduleDrawable(Drawable who, IRunnable what, long when)
//        {
//            if (mWrappedCallback != null)
//            {
//                mWrappedCallback.ScheduleDrawable(who, what, when);
//            }

//            onAnimationAdvanced(mCurrentFrame, mTotalFrames);
//            mCurrentFrame++;
//        }

//        public void UnscheduleDrawable(Drawable who, IRunnable what)
//        {
//            if (mWrappedCallback != null)
//            {
//                mWrappedCallback.UnscheduleDrawable(who, what);
//            }
//        }

//        /**
//     * Callback triggered when a new frame of {@link Drawable} has been scheduled.
//     *
//     * @param currentFrame the current frame number.
//     * @param totalFrames  the total number of frames in the {@link AnimationDrawable}.
//     */
//        public abstract void onAnimationAdvanced(int currentFrame, int totalFrames);

//        /**
//         * Callback triggered when {@link View#invalidateDrawable(Drawable)} has been called on the last frame, which marks
//         * the end of a non-looping animation sequence.
//         */
//        public abstract void onAnimationCompleted();
//    }
//}