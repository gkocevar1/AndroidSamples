namespace AppAngie
{
    public interface IProcessListener
    {
        void ProcessContent(string content, AnimationType animationType);
    }
}