using System.Threading.Tasks;
using Android.Media;
using Java.Lang;

namespace AppAngie
{
    /// <summary>
    /// 
    /// </summary>
    public class AudioRecorder : INotificationReceiver
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRecorder"/> class.
        /// </summary>
        public AudioRecorder()
        {
            // init media recorder
            _recorder = new MediaRecorder();
        } 
        #endregion

        public Task StartAsync()
        {
            StartRecorder();

            var tcs = new TaskCompletionSource<object>(/*TaskCreationOptions.LongRunning*/);
            tcs.SetResult(null);
            return tcs.Task;
        }

        public void Stop()
        {
            if (_recorder != null)
            {
                _recorder.Stop();
                _recorder.Release();
                _recorder = null;
            }
        }

        public double GetAmplitude()
        {
            if (_recorder != null)
                return _recorder.MaxAmplitude;

            return 0;
        }

        private void StartRecorder()
        {
            try
            {
                _recorder.SetAudioSource(AudioSource.Mic);
                _recorder.SetOutputFormat(OutputFormat.Mpeg4);
                _recorder.SetAudioEncoder(AudioEncoder.AmrNb); // Initialized state.
                _recorder.SetOutputFile("/dev/null");
                _recorder.Prepare();
                _recorder.Start();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void CalculateDecibels()
        {

        }

        #region Fields
        private MediaRecorder _recorder; 
        #endregion
    }
}