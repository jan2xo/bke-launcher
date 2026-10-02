#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

PIN_FILE="eng/demo-app-source.sha"
PLUGIN_PATH="plugins/bke-demo-app"

[[ -f "$PIN_FILE" ]] || {
  echo "BKE Demo App source pin is missing." >&2
  exit 1
}

pin="$(tr -d '[:space:]' < "$PIN_FILE")"
[[ "$pin" =~ ^[0-9a-f]{40}$ ]] || {
  echo "BKE Demo App source pin must be an exact lowercase SHA." >&2
  exit 1
}

gitlink="$(git rev-parse "HEAD:$PLUGIN_PATH")"
[[ "$gitlink" == "$pin" ]] || {
  echo "BKE Demo App gitlink drifted: pin=$pin gitlink=$gitlink" >&2
  exit 1
}

[[ -d "$PLUGIN_PATH/.git" || -f "$PLUGIN_PATH/.git" ]] || {
  echo "BKE Demo App submodule is not hydrated." >&2
  exit 1
}

checkout="$(git -C "$PLUGIN_PATH" rev-parse HEAD)"
[[ "$checkout" == "$pin" ]] || {
  echo "BKE Demo App checkout drifted: pin=$pin checkout=$checkout" >&2
  exit 1
}

launcher_pin_file="$PLUGIN_PATH/eng/launcher-source.sha"
[[ -f "$launcher_pin_file" ]] || {
  echo "BKE Demo App Launcher authority pin is missing." >&2
  exit 1
}

launcher_pin="$(tr -d '[:space:]' < "$launcher_pin_file")"
[[ "$launcher_pin" =~ ^[0-9a-f]{40}$ ]] || {
  echo "BKE Demo App Launcher authority must be an exact lowercase SHA." >&2
  exit 1
}

git cat-file -e "$launcher_pin^{commit}" 2>/dev/null || {
  echo "Pinned Demo App Launcher authority is unavailable in Launcher history." >&2
  exit 1
}

git merge-base --is-ancestor "$launcher_pin" HEAD || {
  echo "Demo App was certified against a Launcher contract that is not an ancestor of this source." >&2
  exit 1
}

echo "BKE bundled plugin provenance: PASS"
echo "demo_source_sha=$pin"
echo "demo_launcher_contract_sha=$launcher_pin"
