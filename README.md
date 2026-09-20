# BKE Launcher

Customer-facing software hub for the BKE ecosystem.

## Current development state

Active branch: `feat/launcher-core`

This first wave establishes the .NET 10 Launcher shell, the Agent-owned account-session boundary, owner-controlled product execution types, and Launcher Plugin Contract v1.

No merge is implied by this branch.

## Ownership boundaries

```text
Digital Solutions
  identity / accounts / organizations / entitlements / catalog authority
                         │
                         ▼
                 BKE Licensing Agent
          machine identity + account session
       signed leases + install/update authority
                         │
          ┌──────────────┴──────────────┐
          ▼                             ▼
     BKE Launcher                 Standalone apps
          │                       Render Dock / Air Stack
      Plugin Host                        │
          │                              │
   Launcher products                    │
          └──────────────┬───────────────┘
                         ▼
                    SAME AGENT
```

### BKE Launcher owns

- account UX
- catalog presentation
- install/open/update UX
- Launcher plugin hosting
- presentation of machine installation state

### BKE Licensing Agent owns

- machine identity
- reusable BKE account-session secrets
- access/refresh tokens
- local licensing authority
- signed-lease validation
- privileged install/update/repair/rollback work

Launcher never receives the Agent refresh token.

## Product execution policy

Execution type is explicit owner policy:

```text
LAUNCHER_PLUGIN
STANDALONE
```

It is never inferred from product size or complexity.

### LAUNCHER_PLUGIN

Runs inside the BKE Launcher plugin host. The plugin receives narrow Launcher capabilities, not cloud credentials.

### STANDALONE

Installed through the Agent-managed path, then independently executable. The Launcher may be closed; the standalone application still authorizes through the Licensing Agent.

## Account sharing

Launcher and future Licensing Center account UX share the same machine-local session because both talk to the Licensing Agent.

Across PCs, the same BKE cloud account can expose the same organizations, entitlements, and catalog while each PC keeps its own device identity and token family.

## Current local Agent surface

```text
POST /v1/account-session/start
POST /v1/account-session/status
POST /v1/account-session/logout
```

Default endpoint:

```text
http://127.0.0.1:43873
```

The Launcher Agent client rejects non-loopback base addresses.

## Projects

```text
src/
  BKE.Launcher.Contracts
  BKE.Launcher.Application
  BKE.Launcher.AgentClient
  BKE.Launcher.PluginHost
  BKE.Launcher.Infrastructure
  BKE.Launcher.Presentation
  BKE.Launcher.Desktop

tests/
  BKE.Launcher.ContractCertification
  BKE.Launcher.PluginCertification
```

The desktop shell uses .NET 10 and Avalonia 12.1.1, matching the known-good current BKE Licensing Agent desktop baseline.

The final Launcher product ID and release version are deliberately not locked by this scaffold.
