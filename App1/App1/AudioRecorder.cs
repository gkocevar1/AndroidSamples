using System;
using System.Threading.Tasks;
using Android.Media;
using Java.Lang;
using System.IO;
using Android.Util;
using Java.Text;
using System.Linq;
using Java.Nio;

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
        public AudioRecorder(IProcessListener listener)
        {
            _listener = listener;
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #region Public
        #region StartAsync
        /// <summary>
        /// Starts the asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            await StartRecorder();
        } 
        #endregion

        #region Stop
        /// <summary>
        /// Stops audio recording.
        /// </summary>
        public void Stop()
        {
            _isRecording = false;
            // Give it time to drop out.
            Thread.Sleep(250); 
        }
        #endregion
        #endregion

        #region Private
        #region StartRecorder
        /// <summary>
        /// Starts the recorder.
        /// </summary>
        /// <returns></returns>
        private async Task StartRecorder()
        {
            try
            {
                //_endRecording = false;
                //_isRecording = true;

                //RaiseRecordingStateChangedEvent();

                // Buffer for 20 milliseconds of data, e.g. 160 samples at 8kHz.
                var buffer20ms = new short[Constants.SampleRate / 50];
                var audioBuffer = new byte[buffer20ms.Length];
                
                var bufferSize = AudioRecord.GetMinBufferSize(Constants.SampleRate, ChannelIn.Mono, Encoding.Pcm16bit);
                
                // TODO: check if I need that
                //if (bufferSize < Constants.SampleRate)
                //{
                //    bufferSize = Constants.SampleRate;
                //}

                var audioRecord = new AudioRecord(
                    // Hardware source of recording.
                    AudioSource.Mic,
                    // Frequency
                    Constants.SampleRate,
                    // Channel (Mono or Stereo)
                    ChannelIn.Mono,
                    // Audio encoding
                    Encoding.Pcm16bit,
                    // Length of the audio clip.
                    bufferSize);

                if (audioRecord.State != State.Initialized)
                {
                    // return error
                }

                _isRecording = true;
                audioRecord.StartRecording();

                while (_isRecording)
                {
                    //var numSamples = audioRecord.Read(buffer20ms, 0, buffer20ms.Length);
                    //_totalNumberOfSamples += numSamples;

                    // TODO: what if I will use bufferSize field (convert to short)


                    //var samplesNumber = await audioRecord.ReadAsync(buffer20ms, 0, buffer20ms.Length);
                    // or
                    var samplesNumber = await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);


                    //var pressure = ProcessAudio(audioBuffer.Select(x => (short)x).ToArray());

                    var buffer20ms2 = new short[audioBuffer.Length / 2];
                    ByteBuffer.Wrap(audioBuffer).Order(ByteOrder.LittleEndian).AsShortBuffer().Get(buffer20ms2);

                    var pressure = ProcessAudio(buffer20ms2);
                    if (pressure > 40 || _isRecording2)
                    {
                        // start recording
                        
                        if (!_isRecording2)
                        {
                            _fs = new FileStream(_filePath, FileMode.Create, FileAccess.ReadWrite);
                            _isRecording2 = true;
                        }


                        //byte[] bytes2 = new byte[buffer20ms.Length * 2];
                        //ByteBuffer.Wrap(bytes2).Order(ByteOrder.LittleEndian).AsShortBuffer().Put(buffer20ms);
                        await _fs.WriteAsync(audioBuffer, 0, samplesNumber);
                    }
                }
                _isRecording2 = false;
                _fs.Close();
                audioRecord.Stop();
                audioRecord.Release();
            }
            catch (System.Exception ex)
            {
                Log.Verbose(TAG, "Error reading audio", ex);
            }
        }

        private FileStream _fs;
        #endregion

        #region ReadAudioAsync
        /// <summary>
        /// Reads the audio asynchronous.
        /// </summary>
        /// <returns></returns>
        private async Task ReadAudioAsync()
        {
            //using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            //{
           

            //    //fileStream.Close();
            //}

            //_audioRecord.Stop();
            //_audioRecord.Release();
            //_isRecording = false;

            RaiseRecordingStateChangedEvent();
        }
        #endregion

        #region BufferSize
        /// <summary>
        /// Buffers the size.
        /// </summary>
        /// <param name="sampleRateInHz">The sample rate in hz.</param>
        /// <param name="channelConfig">The channel configuration.</param>
        /// <param name="audioFormat">The audio format.</param>
        /// <returns></returns>
        private int BufferSize(int sampleRateInHz, int channelConfig, int audioFormat)
        {
            int buffSize = AudioRecord.GetMinBufferSize(sampleRateInHz, ChannelIn.Mono, Encoding.Pcm16bit);
            if (buffSize < sampleRateInHz)
            {
                buffSize = sampleRateInHz;
            }
            return buffSize;
        } 
        #endregion

        #region RaiseRecordingStateChangedEvent
        /// <summary>
        /// Raises the recording state changed event.
        /// </summary>
        private void RaiseRecordingStateChangedEvent()
        {
            if (RecordingStateChanged != null)
                RecordingStateChanged(_isRecording);
        }
        #endregion

        private int ProcessAudio(short[] audioFrame)
        {
            // Compute the RMS value. (Note that this does not remove DC).
            double rms = 0;
            for (int i = 0; i < audioFrame.Length; i++)
            {
                rms += audioFrame[i] * audioFrame[i];
            }
            rms = Java.Lang.Math.Sqrt(rms / audioFrame.Length);

            // Compute a smoothed version for less flickering of the display.
            _rmsSmoothed = _rmsSmoothed * _alpha + (1 - _alpha) * rms;
            var rmsdB = 20.0 * Java.Lang.Math.Log10(_gain * _rmsSmoothed);

            DecimalFormat df = new DecimalFormat("##");
            

            DecimalFormat df_fraction = new DecimalFormat("#");
            var oneDecimal = (int)(Java.Lang.Math.Round(Java.Lang.Math.Abs(rmsdB * 10))) % 10;
            

            var result = string.Format("{0}.{1} dB", df.Format(20 + rmsdB), oneDecimal);

            var c = Java.Lang.Math.Round((float)(20 + rmsdB));
            _listener.ProcessContent(result, c > 45);

            return c;
        }
        #endregion
        #endregion

        #region Fields
        //private AudioRecord _audioRecord;

        //private byte[] _audioBuffer;

        public Action<bool> RecordingStateChanged;


        //private bool _endRecording = false;
        double _gain = 2500.0 / Java.Lang.Math.Pow(10.0, 90.0 / 20.0);
        // For displaying error in calibration.
        double _differenceFromNominal = 0.0;
        double _rmsSmoothed;  // Temporally filtered version of RMS.
        double _alpha = 0.9;  // Coefficient of IIR smoothing filter for RMS.
        // TODO: change to _isListening
        private bool _isRecording = false;
        // TODO: change to _isListening
        private bool _isRecording2 = false;
        private int _totalNumberOfSamples = 0;
        private IProcessListener _listener;
        private static string _filePath = "/storage/sdcard0/test.mp4";
        private const string TAG = "AudioRecord";
        #endregion
    }
}