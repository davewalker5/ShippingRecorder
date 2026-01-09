using ShippingRecorder.Entities.Jobs;

namespace ShippingRecorder.Api.Interfaces
{
    public interface IBackgroundQueue<T> where T : BackgroundWorkItem
    {
        T Dequeue();
        void Enqueue(T item);
    }
}