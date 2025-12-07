using System;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Config;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.Manager.Logic
{
    internal class UserManagementHandler : CommandHandlerBase
    {
        public UserManagementHandler(
            ShippingRecorderApplicationSettings settings,
            ManagerCommandLineParser parser,
            IShippingRecorderFactory factory) : base (settings, parser, factory)
        {

        }

        /// <summary>
        /// Handle the user addition command
        /// </summary>
        /// <returns></returns>
        public async Task HandleAddUserAsync()
        {
            var userName = Parser.GetValues(CommandLineOptionType.AddUser)[0];
            var password = Parser.GetValues(CommandLineOptionType.AddUser)[1];
            await Factory.Users.AddUserAsync(userName, password);
            Console.WriteLine($"Added user {userName}");
        }

        /// <summary>
        /// Handle the user deletion command
        /// </summary>
        /// <returns></returns>
        public async Task HandleDeleteUserAsync()
        {
            var userName = Parser.GetValues(CommandLineOptionType.DeleteUser)[0];
            await Factory.Users.DeleteUserAsync(userName);
            Console.WriteLine($"Deleted user {userName}");
        }

        /// <summary>
        /// Handle the set password command
        /// </summary>
        /// <returns></returns>
        public async Task HandleSetPasswordAsync()
        {
            var userName = Parser.GetValues(CommandLineOptionType.SetPassword)[0];
            var password = Parser.GetValues(CommandLineOptionType.SetPassword)[1];
            await Factory.Users.SetPasswordAsync(userName, password);
            Console.WriteLine($"Set the password for user {userName}");
        }
    }
}