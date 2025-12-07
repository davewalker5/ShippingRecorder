using ShippingRecorder.BusinessLogic.Config;
using ShippingRecorder.Data;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.Manager.Logic
{
    internal abstract class CommandHandlerBase
    {
        protected ShippingRecorderApplicationSettings Settings { get; private set; }
        protected ManagerCommandLineParser Parser { get; private set; }
        protected IShippingRecorderFactory Factory { get; private set; }
        protected ShippingRecorderDbContext Context { get { return Factory.GetContext<ShippingRecorderDbContext>(); }}

        public CommandHandlerBase(
            ShippingRecorderApplicationSettings settings,
            ManagerCommandLineParser parser,
            IShippingRecorderFactory factory)
        {
            Settings = settings;
            Parser = parser;
            Factory = factory;
        }
    }
}