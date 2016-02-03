using Android.Widget;
using Java.Lang;

namespace AppAngie
{
    public class SceneAnimation
    {
        private ImageView mImageView;
        private int[] mFrameRess;
        private int[] mDurations;
        private int mDuration;

        private int mLastFrameNo;
        private long mBreakDelay;

        public SceneAnimation(ImageView pImageView, int[] pFrameRess, int[] pDurations)
        {
            mImageView = pImageView;
            mFrameRess = pFrameRess;
            mDurations = pDurations;
            mLastFrameNo = pFrameRess.Length - 1;

            mImageView.SetImageResource(mFrameRess[0]);
            play(1);
        }

        public SceneAnimation(ImageView pImageView, int[] pFrameRess, int pDuration)
        {
            mImageView = pImageView;
            mFrameRess = pFrameRess;
            mDuration = pDuration;
            mLastFrameNo = pFrameRess.Length - 1;

            mImageView.SetImageResource(mFrameRess[0]);
            playConstant(1);
        }

        public SceneAnimation(ImageView pImageView, int[] pFrameRess, int pDuration, long pBreakDelay)
        {
            mImageView = pImageView;
            mFrameRess = pFrameRess;
            mDuration = pDuration;
            mLastFrameNo = pFrameRess.Length - 1;
            mBreakDelay = pBreakDelay;

            mImageView.SetImageResource(mFrameRess[0]);
            playConstant(1);
        }


        private void play(int pFrameNo)
        {
            var r = new Runnable(() => {
                mImageView.SetImageResource(mFrameRess[pFrameNo]);
                if (pFrameNo == mLastFrameNo)
                    play(0);
                else
                    play(pFrameNo + 1);
            });

            mImageView.PostDelayed(r, mDurations[pFrameNo]);
        }


        private void playConstant(int pFrameNo)
        {
            var r = new Runnable(() => {
                mImageView.SetImageResource(mFrameRess[pFrameNo]);
                if (pFrameNo == mLastFrameNo)
                    playConstant(0);
                else
                    playConstant(pFrameNo + 1);
            });


            mImageView.PostDelayed(r, pFrameNo==mLastFrameNo && mBreakDelay>0 ? mBreakDelay : mDuration);
        }        
    }
}