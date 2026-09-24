#!/usr/bin/env bash
set -euo pipefail

mapfile -t owners < <(grep -El '^[[:space:]]*pull_request:[[:space:]]*$' .github/workflows/*.yml | sort)
if [ "${#owners[@]}" -ne 1 ] || [ "${owners[0]}" != ".github/workflows/pr-guard.yml" ]; then
  echo "PR workflow ownership must belong only to pr-guard.yml" >&2
  printf '%s\n' "${owners[@]}" >&2
  exit 1
fi

grep -q 'issue_comment:' .github/workflows/certify.yml
grep -q 'workflow_dispatch:' .github/workflows/certify.yml
grep -q 'required-certification:' .github/workflows/certify.yml
grep -q 'response.data.head.sha' .github/workflows/certify.yml
grep -q 'eng/licensing-agent-source.sha' .github/workflows/certify.yml
grep -q 'allowedTargets = \["contracts", "core", "parent"\]' .github/workflows/certify.yml

for workflow in _launcher-contracts.yml _intent-contracts.yml ci.yml windows-parent-installer.yml; do
  grep -q 'source_sha:' ".github/workflows/$workflow"
  if grep -Eq '^[[:space:]]*pull_request:[[:space:]]*$' ".github/workflows/$workflow"; then
    echo "$workflow must not auto-run on pull_request" >&2
    exit 1
  fi
done

grep -q 'workflow_call:' .github/workflows/ci.yml
grep -q 'workflow_dispatch:' .github/workflows/ci.yml
grep -Fq 'branches: [main]' .github/workflows/ci.yml
grep -q 'workflow_call:' .github/workflows/windows-parent-installer.yml
grep -q 'workflow_dispatch:' .github/workflows/windows-parent-installer.yml
grep -q 'agent_source_sha:' .github/workflows/windows-parent-installer.yml

for path in .github/workflows/*.yml; do
  [ "$path" = ".github/workflows/pr-guard.yml" ] && continue
  if grep -q 'github.event.pull_request' "$path"; then
    echo "stale pull_request event context in $path" >&2
    exit 1
  fi
done

grep -Eq '^[0-9a-fA-F]{40}$' eng/licensing-agent-source.sha

echo 'Launcher intent-driven CI ownership GREEN'
