using BKE.Launcher.Application;

namespace BKE.Launcher.PluginHost;

public sealed class LauncherPluginRuntime : ILauncherPluginRuntime
{
    private readonly IReadOnlyDictionary<string, PluginState> _plugins;
    private readonly ILauncherPluginAuthorizationPort _authorization;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public LauncherPluginRuntime(
        IEnumerable<IBkeLauncherPlugin> plugins,
        ILauncherPluginAuthorizationPort authorization)
    {
        ArgumentNullException.ThrowIfNull(plugins);
        _authorization = authorization ??
            throw new ArgumentNullException(nameof(authorization));

        var registered =
            new Dictionary<string, PluginState>(StringComparer.Ordinal);

        foreach (var plugin in plugins)
        {
            if (plugin is null)
            {
                throw new ArgumentException(
                    "Launcher plugin registration cannot contain null.",
                    nameof(plugins));
            }

            ValidateIdentity(plugin.Identity);

            if (!registered.TryAdd(
                    plugin.Identity.ProductId,
                    new PluginState(plugin)))
            {
                throw new InvalidDataException(
                    $"Duplicate Launcher plugin product id: {plugin.Identity.ProductId}");
            }
        }

        _plugins = registered;
    }

    public bool IsRegistered(
        string productId,
        string version)
    {
        if (!_plugins.TryGetValue(productId, out var state))
        {
            return false;
        }

        return string.Equals(
                   state.Plugin.Identity.PluginVersion,
                   version,
                   StringComparison.Ordinal) &&
               state.Plugin.Identity.MinimumHostContractVersion <=
                   LauncherPluginContract.Version;
    }

    public async Task<LauncherPluginOpenResult> OpenAsync(
        string productId,
        string version,
        CancellationToken cancellationToken)
    {
        if (!_plugins.TryGetValue(productId, out var state))
        {
            return new LauncherPluginOpenResult(
                "UNAVAILABLE",
                "plugin_not_bundled",
                "This Launcher build does not contain the selected plugin.");
        }

        if (!string.Equals(
                state.Plugin.Identity.PluginVersion,
                version,
                StringComparison.Ordinal))
        {
            return new LauncherPluginOpenResult(
                "UNAVAILABLE",
                "plugin_version_mismatch",
                "This Launcher build does not contain the catalog-authorized plugin version.");
        }

        if (state.Plugin.Identity.MinimumHostContractVersion >
            LauncherPluginContract.Version)
        {
            return new LauncherPluginOpenResult(
                "UNAVAILABLE",
                "host_contract_too_old",
                "This plugin requires a newer BKE Launcher.");
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var gateway = new ScopedAuthorizationGateway(
                state.Plugin.Identity.ProductId,
                state.Plugin.Identity.PluginVersion,
                _authorization);

            var decision = await gateway.AuthorizeDecisionAsync(
                cancellationToken);

            if (!decision.Authorized)
            {
                return new LauncherPluginOpenResult(
                    decision.Status,
                    decision.Reason,
                    decision.Message);
            }

            try
            {
                if (!state.Initialized)
                {
                    await state.Plugin.InitializeAsync(
                        new LauncherContext(gateway),
                        cancellationToken);
                    state.Initialized = true;
                }

                await state.Plugin.OpenAsync(cancellationToken);
                return new LauncherPluginOpenResult(
                    "OPENED",
                    "opened");
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                return new LauncherPluginOpenResult(
                    "FAILED",
                    "plugin_failed",
                    "The bundled Launcher plugin failed to open.");
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task ShutdownAsync(
        CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            foreach (var state in _plugins.Values
                         .Where(value => value.Initialized)
                         .Reverse())
            {
                try
                {
                    await state.Plugin.ShutdownAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch
                {
                    // One plugin must not prevent cleanup of the remaining
                    // Launcher-hosted plugins during application shutdown.
                }
                finally
                {
                    state.Initialized = false;
                }
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private static void ValidateIdentity(
        LauncherPluginIdentity identity)
    {
        if (string.IsNullOrWhiteSpace(identity.ProductId) ||
            identity.ProductId.Length > 128 ||
            !identity.ProductId.All(character =>
                character is >= 'a' and <= 'z' ||
                character is >= '0' and <= '9' ||
                character == '-'))
        {
            throw new InvalidDataException(
                "Launcher plugin product id is invalid.");
        }

        if (string.IsNullOrWhiteSpace(identity.PluginVersion) ||
            identity.PluginVersion.Length > 128)
        {
            throw new InvalidDataException(
                "Launcher plugin version is invalid.");
        }

        if (identity.MinimumHostContractVersion <= 0)
        {
            throw new InvalidDataException(
                "Launcher plugin minimum host contract version is invalid.");
        }
    }

    private sealed class PluginState(
        IBkeLauncherPlugin plugin)
    {
        public IBkeLauncherPlugin Plugin { get; } = plugin;
        public bool Initialized { get; set; }
    }

    private sealed class LauncherContext(
        IProductAuthorizationGateway authorization)
        : ILauncherContext
    {
        public IProductAuthorizationGateway Authorization { get; } =
            authorization;
    }

    private sealed class ScopedAuthorizationGateway(
        string productId,
        string version,
        ILauncherPluginAuthorizationPort authorization)
        : IProductAuthorizationGateway
    {
        public async Task<ProductAuthorizationResult> AuthorizeAsync(
            string requestedProductId,
            CancellationToken cancellationToken)
        {
            var decision = await AuthorizeDecisionAsync(
                requestedProductId,
                cancellationToken);
            return new ProductAuthorizationResult(
                decision.Authorized,
                decision.Reason);
        }

        public Task<LauncherPluginAuthorizationDecision> AuthorizeDecisionAsync(
            CancellationToken cancellationToken) =>
            AuthorizeDecisionAsync(
                productId,
                cancellationToken);

        private Task<LauncherPluginAuthorizationDecision> AuthorizeDecisionAsync(
            string requestedProductId,
            CancellationToken cancellationToken)
        {
            if (!string.Equals(
                    requestedProductId,
                    productId,
                    StringComparison.Ordinal))
            {
                return Task.FromResult(
                    new LauncherPluginAuthorizationDecision(
                        "DENIED",
                        false,
                        "product_scope_mismatch",
                        "A Launcher plugin may authorize only its own product identity."));
            }

            return authorization.AuthorizeAsync(
                productId,
                version,
                cancellationToken);
        }
    }
}
