using Android.App;
using Android.Media;

namespace AppAngie
{
    /// <summary>
    /// Class used to manage audio notifications.
    /// </summary>
    public class NotificationManager
    {
        private static AudioManager _audioManager = null;
        public static AudioManager AudioManager
        {
            get
            {
                return _audioManager;
            }

            set
            {
                _audioManager = value;
            }
        }

        private static Activity _mainActivity = null;
        public static Activity MainActivity
        {
            set { _mainActivity = value; }
        }

        private AudioManager.IOnAudioFocusChangeListener _listener = null;
        
        /// <summary>
        /// Requests the audio resources.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public bool RequestAudioResources(INotificationReceiver parent)
        {
            _listener = new FocusChangeListener(parent);

            var ret = _audioManager.RequestAudioFocus(_listener, Stream.Music, AudioFocus.Gain);
            if (ret == AudioFocusRequest.Granted)
            {
                return true;
            }
            else if (ret == AudioFocusRequest.Failed)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Releases the audio resources.
        /// </summary>
        public void ReleaseAudioResources()
        {
            if (_listener != null)
            {
                _audioManager.AbandonAudioFocus(_listener);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <seealso cref="Java.Lang.Object" />
        /// <seealso cref="Android.Media.AudioManager.IOnAudioFocusChangeListener" />
        private class FocusChangeListener : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener
        {
            INotificationReceiver parent = null;

            public FocusChangeListener(INotificationReceiver parent)
            {
                this.parent = parent;
            }

            void SetStatus(string message)
            {
                //if (mainActivity is WorkingWithAudioActivity)
                //{
                //    WorkingWithAudioActivity wact = mainActivity as WorkingWithAudioActivity;
                //    wact.setStatus(message);
                //}
                if (_mainActivity is MainActivity)
                {
                    MainActivity ma = _mainActivity as MainActivity;
                    //ma.setStatus(message);
                }
            }

            /// <summary>
            /// Called when [audio focus change].
            /// </summary>
            /// <param name="focusChange">The focus change.</param>
            public void OnAudioFocusChange(AudioFocus focusChange)
            {
                switch (focusChange)
                {
                    // We will take any flavor of AudioFocusGain that the system gives us and use it.
                    case AudioFocus.GainTransient:
                    case AudioFocus.GainTransientMayDuck:
                    case AudioFocus.Gain:
                        parent.StartAsync();
                        SetStatus("Granted");
                        break;

                    // If we get any notificationthat removes focus - just terminate what we were doing.
                    case AudioFocus.LossTransientCanDuck:
                    case AudioFocus.LossTransient:
                    case AudioFocus.Loss:
                        parent.Stop();
                        SetStatus("Removed");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}