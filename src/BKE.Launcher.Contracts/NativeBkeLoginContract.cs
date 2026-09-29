using System.Text.Json.Serialization;

namespace BKE.Launcher.Contracts;

public static class BkePlatformContract
{
    public const string AccountSessionProtocolVersion = "bke.account-session.v1";
    public const string NativeLoginPath = "/api/agent-sessions/native/login";
    public const string NativeMfaVerifyPath = "/api/agent-sessions/native/mfa/verify";
    public const string NativePasswordResetRequestPath = "/api/agent-sessions/native/password-reset/request";
    public const string NativeRegistrationPreflightPath = "/api/agent-sessions/native/registration";
    public const string NativeRegistrationPath = "/api/agent-sessions/native/register";
    public const string NativeEmailVerifyPath = "/api/agent-sessions/native/verify-email";
    public const string NativeVerificationResendPath = "/api/agent-sessions/native/verification/resend";
}

public sealed record NativeBkeLoginRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("customer_account_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? CustomerAccountId,
    [property: JsonPropertyName("device_id")] string DeviceId,
    [property: JsonPropertyName("device_name")] string DeviceName,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("architecture")] string Architecture);

public sealed record NativeBkeMfaVerifyRequest(
    [property: JsonPropertyName("challenge_token")] string ChallengeToken,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("customer_account_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? CustomerAccountId,
    [property: JsonPropertyName("device_id")] string DeviceId,
    [property: JsonPropertyName("device_name")] string DeviceName,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("architecture")] string Architecture);

public sealed record NativeBkeAccountChoice(
    [property: JsonPropertyName("account_id")] string AccountId,
    [property: JsonPropertyName("account_type")] string AccountType,
    [property: JsonPropertyName("account_display_name")] string AccountDisplayName);

public sealed record NativeBkeLoginAccount(
    [property: JsonPropertyName("account_id")] string AccountId,
    [property: JsonPropertyName("account_type")] string AccountType,
    [property: JsonPropertyName("account_display_name")] string AccountDisplayName);

public sealed record NativeBkeLoginResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("accounts"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IReadOnlyList<NativeBkeAccountChoice>? Accounts = null,
    [property: JsonPropertyName("handoff_code"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? HandoffCode = null,
    [property: JsonPropertyName("expires_in"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? ExpiresIn = null,
    [property: JsonPropertyName("account"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] NativeBkeLoginAccount? Account = null,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error = null,
    [property: JsonPropertyName("challenge_token"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ChallengeToken = null,
    [property: JsonPropertyName("expires_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ExpiresAt = null,
    [property: JsonPropertyName("email_sent"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] bool? EmailSent = null,
    [property: JsonPropertyName("mfa_reference"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? MfaReference = null);

public sealed record NativeBkeMfaVerifyResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("accounts"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IReadOnlyList<NativeBkeAccountChoice>? Accounts = null,
    [property: JsonPropertyName("handoff_code"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? HandoffCode = null,
    [property: JsonPropertyName("expires_in"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? ExpiresIn = null,
    [property: JsonPropertyName("authentication_method"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? AuthenticationMethod = null,
    [property: JsonPropertyName("account"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] NativeBkeLoginAccount? Account = null,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error = null);


public sealed record NativeBkeRegistrationLegalDocument(
    [property: JsonPropertyName("document_type")] string DocumentType,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("slug")] string Slug,
    [property: JsonPropertyName("version_id")] string VersionId,
    [property: JsonPropertyName("version_number")] int VersionNumber,
    [property: JsonPropertyName("effective_at")] string? EffectiveAt,
    [property: JsonPropertyName("content_markdown")] string ContentMarkdown);

public sealed record NativeBkeRegistrationPreflightResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("legal_documents")] IReadOnlyList<NativeBkeRegistrationLegalDocument> LegalDocuments,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error = null);

public sealed record NativeBkeRegistrationRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("legal_version_ids")] IReadOnlyList<string> LegalVersionIds);

public sealed record NativeBkeRegistrationResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error = null);

public sealed record NativeBkeEmailVerificationRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("code")] string Code);

public sealed record NativeBkeEmailVerificationResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error = null);

public sealed record NativeBkeVerificationResendRequest(
    [property: JsonPropertyName("email")] string Email);

public sealed record NativeBkeVerificationResendResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error = null);
