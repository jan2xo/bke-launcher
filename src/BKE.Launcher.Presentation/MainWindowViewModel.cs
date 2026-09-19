using System.ComponentModel;
using System.Runtime.CompilerServices;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Presentation;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly LauncherAccountSessionController _accountSession;
    private string _sessionStatus = "SIGNED_OUT";
    private string _accountDisplay = "Not signed in";
    private string _userCode = string.Empty;
    private string _verificationUri = string.Empty;
    private string _message = "Connect this Launcher to the BKE Licensing Agent.";

    public MainWindowViewModel(LauncherAccountSessionController accountSession)
    {
        _accountSession = accountSession;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string SessionStatus
    {
        get => _sessionStatus;
        private set => SetField(ref _sessionStatus, value);
    }

    public string AccountDisplay
    {
        get => _accountDisplay;
        private set => SetField(ref _accountDisplay, value);
    }

    public string UserCode
    {
        get => _userCode;
        private set => SetField(ref _userCode, value);
    }

    public string VerificationUri
    {
        get => _verificationUri;
        private set => SetField(ref _verificationUri, value);
    }

    public string Message
    {
        get => _message;
        private set => SetField(ref _message, value);
    }

    public async Task StartSignInAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountSession.StartAsync(cancellationToken);
            SessionStatus = response.Status;
            UserCode = response.UserCode ?? string.Empty;
            VerificationUri = response.VerificationUri ?? string.Empty;
            Message = response.Status switch
            {
                "AUTHENTICATED" => "This machine already has an authenticated BKE account session.",
                "PENDING" => "Complete sign-in in your browser, then refresh account status.",
                _ => response.Error?.Message ?? "BKE account sign-in could not be started.",
            };
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or InvalidDataException)
        {
            SessionStatus = "AGENT_UNAVAILABLE";
            Message = "BKE Licensing Agent is unavailable or returned an invalid response.";
        }
    }

    public async Task RefreshStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountSession.StatusAsync(cancellationToken);
            ApplyStatus(response);
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or InvalidDataException)
        {
            SessionStatus = "AGENT_UNAVAILABLE";
            AccountDisplay = "Not available";
            Message = "BKE Licensing Agent is unavailable or returned an invalid response.";
        }
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountSession.LogoutAsync(cancellationToken);
            SessionStatus = response.Status;
            AccountDisplay = "Not signed in";
            UserCode = string.Empty;
            VerificationUri = string.Empty;
            Message = response.Error?.Message ?? "Signed out on this machine.";
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or InvalidDataException)
        {
            SessionStatus = "AGENT_UNAVAILABLE";
            Message = "The local session could not be changed because the BKE Licensing Agent is unavailable.";
        }
    }

    private void ApplyStatus(AccountSessionStatusResponse response)
    {
        SessionStatus = response.Status;

        if (response.Account is not null)
        {
            AccountDisplay = $"{response.Account.DisplayName} · {response.Account.Email}";
        }
        else if (response.Status is "SIGNED_OUT" or "DENIED" or "EXPIRED" or "FAILED")
        {
            AccountDisplay = "Not signed in";
        }

        Message = response.Error?.Message ?? response.Status switch
        {
            "AUTHENTICATED" => "Launcher and License Center share this Agent-owned account session.",
            "PENDING" => "Waiting for browser approval.",
            "SIGNED_OUT" => "No BKE account is authenticated on this machine.",
            "DENIED" => "BKE account authorization was denied.",
            "EXPIRED" => "BKE account authorization expired.",
            _ => "BKE account-session status updated.",
        };
    }

    private void SetField(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (string.Equals(field, value, StringComparison.Ordinal))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
