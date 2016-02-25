using System;
using System.IO;
using System.Threading.Tasks;
using Android.Media;
using Android.Util;

namespace AppAngie
{
    /// <summary>
    /// This class use the low level class AudioTrack in order to play Audio
    /// </summary>
    public class AudioPlayer : INotificationReceiver
    {
        #region Methods
        #region Public
        #region StartAsync
        /// <summary>
        /// Starts playing audio.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            await PlaybackAsync();
        }
        #endregion

        #region Stop
        /// <summary>
        /// Stops playing the audio.
        /// </summary>
        public void Stop()
        {
            if (_audioTrack != null)
            {
                _audioTrack.Stop();
                _audioTrack.Release();
                _audioTrack = null;
            }
        }  
        #endregion
        #endregion

        #region Private
        #region PlaybackAsync
        /// <summary>
        /// Playbacks the asynchronous.
        /// </summary>
        /// <returns></returns>
        private async Task PlaybackAsync()
        {
            try
            {
                using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        var totalBytes = new FileInfo(_filePath).Length;
                        var buffer = binaryReader.ReadBytes((Int32)totalBytes);

                        binaryReader.Close();
                        await PlayAudioTrackAsync(buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Verbose(TAG, "Error playing audio", ex);
            }
        }
        #endregion
        #region PlayAudioTrackAsync
        /// <summary>
        /// Plays the audio track asynchronous.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        private async Task PlayAudioTrackAsync(byte[] buffer)
        {
            _audioTrack = new AudioTrack(
                // Stream type
                Android.Media.Stream.Music,
                // Frequency
                Constants.SampleRate,
                // Mono or stereo
                ChannelOut.Mono,
                // Audio encoding
                Encoding.Pcm16bit,
                // Length of the audio clip.
                buffer.Length,
                // Mode. Stream or static.
                AudioTrackMode.Stream);

            _audioTrack.Play();

            await _audioTrack.WriteAsync(buffer, 0, buffer.Length);
        }
        #endregion
        #endregion
        #endregion

        #region Fields
        private static string _filePath = "/data/data/App.Angie/files/audio.mp4";
        //private static string _filePath = "/storage/sdcard0/test.mp4";
        private AudioTrack _audioTrack = null;
        private const string TAG = "AudioPlayer";
        #endregion
    }
}