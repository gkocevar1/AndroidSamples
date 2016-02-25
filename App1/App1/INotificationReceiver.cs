using System.Threading.Tasks;

namespace AppAngie
{
    /// <summary>
    /// The best way to see audio notifications in action is to call the test phone, keep the call open for a while and then end the call while there is an ongoing operation. 
    /// Audio-focus will be taken away and then granted again when the call is complete.
    /// </summary>
    public interface INotificationReceiver
    {
        /// <summary>
        /// Starts the asynchronous.
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }
}