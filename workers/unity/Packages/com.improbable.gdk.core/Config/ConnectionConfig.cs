using Improbable.Worker;
using Improbable.Worker.Core;

namespace Improbable.Gdk.Core
{
    public abstract class ConnectionConfig
    {
        /// <summary>
        ///     The type of networking that should be used.
        /// </summary>
        /// <remarks>
        ///    Default is RakNet.
        /// </remarks>
        public NetworkConnectionType LinkProtocol = NetworkConnectionType.RakNet;

        /// <summary>
        ///     Denotes whether protocol logging should be enabled.
        /// </summary>
        /// <remarks>
        ///    Default is false.
        /// </remarks>
        public bool EnableProtocolLoggingAtStartup = false;

        /// <summary>
        ///     Denotes whether the worker will use the local IP to connect to the SpatiaLOS runtime.
        /// </summary>
        /// <remarks>
        ///    Default is false.
        ///    This should be true if connecting client-workers to cloud deployments or if connecting via the
        ///    locator flow.
        /// </remarks>
        public bool UseExternalIp = false;

        /// <summary>
        ///     The ID of the worker.
        /// </summary>
        public string WorkerId;

        /// <summary>
        ///     The type of the worker.
        /// </summary>
        public string WorkerType;

        /// <summary>
        ///    Validates that the connection configuration is sane.
        /// </summary>
        public abstract void Validate();

        /// <summary>
        ///     Creates the <see cref="Improbable.Worker.Core.ConnectionParameters"/> corresponding to this
        ///     <see cref="ConnectionConfig"/> instance.
        /// </summary>
        /// <returns></returns>
        public ConnectionParameters CreateConnectionParameters()
        {
            var connectionParameters = new ConnectionParameters
            {
                WorkerType = WorkerType,
                Network =
                {
                    ConnectionType = LinkProtocol,
                    UseExternalIp = UseExternalIp,
                },
                EnableProtocolLoggingAtStartup = EnableProtocolLoggingAtStartup,
                DefaultComponentVtable = new PassthroughComponentVtable()
            };
            return connectionParameters;
        }

        /// <summary>
        ///     Basic validation function which checks that a string value is non-null or empty.
        /// </summary>
        /// <param name="configValue">The value to validate.</param>
        /// <param name="configName">The name of the configuration.</param>
        /// <exception cref="ConnectionFailedException">
        ///     Thrown if configValue is null or empty.
        /// </exception>
        protected void ValidateConfig(string configValue, string configName)
        {
            if (string.IsNullOrEmpty(configValue))
            {
                throw new ConnectionFailedException(
                    $"Config validation failed with: No valid {configName} has been provided",
                    ConnectionErrorReason.InvalidConfig);
            }
        }
    }
}
