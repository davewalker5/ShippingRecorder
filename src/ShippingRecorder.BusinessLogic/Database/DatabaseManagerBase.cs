using ShippingRecorder.Data;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.BusinessLogic.Database
{
    internal abstract class DatabaseManagerBase
    {
        protected readonly IShippingRecorderFactory _factory;

        internal DatabaseManagerBase(IShippingRecorderFactory factory)
            => _factory = factory;

        protected ShippingRecorderDbContext Context
            => _factory.GetContext<ShippingRecorderDbContext>() as ShippingRecorderDbContext;
    }
}