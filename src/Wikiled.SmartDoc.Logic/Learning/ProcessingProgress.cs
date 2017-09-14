namespace Wikiled.SmartDoc.Logic.Learning
{
    public class ProcessingProgress
    {
        public ProcessingProgress(int current, int total)
        {
            Total = total;
            Current = current;
        }

        public int Total { get; }

        public int Current { get; }
    }
}
