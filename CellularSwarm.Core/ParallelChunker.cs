namespace CellularSwarm.Core;

public static class ParallelChunker
{
    public static void Run<T>(IList<T> items, Action<int, int> processChunk)
    {
        if (items.Count == 0) return;

        // Prevent over-provisioning threads for small arrays
        int threadCount = Math.Min(Environment.ProcessorCount, items.Count);
        int chunkSize = (int)Math.Ceiling((double)items.Count / threadCount);

        using var countdown = new CountdownEvent(threadCount);

        for (int i = 0; i < threadCount; i++)
        {
            int start = i * chunkSize;
            int end = Math.Min(start + chunkSize, items.Count);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    processChunk(start, end);
                }
                finally
                {
                    countdown.Signal(); // Decrement the counter when done
                }
            });
        }

        countdown.Wait(); // Wait for all ThreadPool workers to finish this frame
    }
}