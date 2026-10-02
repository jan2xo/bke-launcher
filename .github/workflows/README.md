# Active GitHub Actions

This directory contains only intentional certification workflows.

Entrypoint:
- `certify.yml` — explicit `/certify ...` or `workflow_dispatch`

Retained proof targets:
- contracts
- core
- parent

Rules:
- no automatic `pull_request` certification
- no ordinary branch-push certification
- parent certification remains explicitly callable and preserves exact Launcher + Agent SHA inputs

Legacy-only for now:
- `pr-guard.yml` — automatic PR trigger removed
- `preproduction-release.yml` — publication action, not certification; requires a separate explicit release intent before reactivation

Historical workflows are preserved verbatim under `.github/legacy-workflows/2026-10-02/`.
