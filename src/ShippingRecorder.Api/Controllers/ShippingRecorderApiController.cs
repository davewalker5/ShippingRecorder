using System.Runtime.CompilerServices;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;
using Microsoft.AspNetCore.Mvc;

namespace ShippingRecorder.Api.Controllers
{
    public class ShippingRecorderApiController : Controller
    {
        protected ShippingRecorderFactory Factory { get; private set; }
        protected IShippingRecorderLogger Logger { get; private set; }

        public ShippingRecorderApiController(ShippingRecorderFactory factory, IShippingRecorderLogger logger)
        {
            Factory = factory;
            Logger = logger;
        }

        /// <summary>
        /// Write a message to the log file, including the caller method name
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="caller"></param>
        protected void LogMessage(Severity severity, string message, [CallerMemberName] string caller = null)
            => Logger.LogMessage(severity, $"{caller}: {message}");
    }
}