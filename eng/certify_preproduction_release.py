#!/usr/bin/env python3
import json
import pathlib
import re
import sys

root = pathlib.Path(__file__).resolve().parents[1]
request_path = root / "eng" / "preproduction-parent-release.json"
workflow_path = root / ".github" / "workflows" / "preproduction-release.yml"

def require(condition: bool, message: str) -> None:
    if not condition:
        raise SystemExit(message)

request = json.loads(request_path.read_text(encoding="utf-8"))
require(request.get("schema") == "bke.preproduction-release-request.v1", "preproduction release request schema drifted")
require(request.get("status") == "PREPRODUCTION", "preproduction release request status drifted")
require(request.get("production_ready") is False, "preproduction release request crossed production boundary")
require(re.fullmatch(r"bke-v[0-9A-Za-z.+-]+-preproduction\.[0-9A-Za-z.+-]+", request.get("tag", "")) is not None, "preproduction release tag is invalid")
require(re.fullmatch(r"[0-9a-f]{40}", request.get("source_sha", "")) is not None, "preproduction release source SHA is invalid")
require(re.fullmatch(r"[0-9a-f]{40}", request.get("agent_source_sha", "")) is not None, "preproduction release Agent SHA is invalid")
require(isinstance(request.get("certification_run_id"), int) and request["certification_run_id"] > 0, "preproduction certification run id is invalid")
require(request.get("certification_artifact_name") == "BKE-Windows-Parent-Installer-PREPRODUCTION", "preproduction artifact authority drifted")

installer = request.get("installer") or {}
require(re.fullmatch(r"BKE-[^/]+-PREPRODUCTION-Windows\.exe", installer.get("file", "")) is not None, "preproduction installer filename is invalid")
require(re.fullmatch(r"[0-9a-f]{64}", installer.get("sha256", "")) is not None, "preproduction installer SHA-256 is invalid")
require(isinstance(installer.get("bytes"), int) and installer["bytes"] > 0, "preproduction installer size is invalid")

workflow = workflow_path.read_text(encoding="utf-8")
required_markers = [
    'branches:',
    '"preproduction-release/**"',
    'actions: read',
    'contents: write',
    'github.event.created == true',
    'eng/preproduction-parent-release.json',
    'BKE-Windows-Parent-Installer-PREPRODUCTION',
    'gh run download',
    'bke.parent-package-boundary.v2',
    'production_ready',
    'PREPRODUCTION',
    'sha256sum',
    'gh release create',
    '--prerelease',
    'gh release download',
    'git merge-base --is-ancestor',
]
for marker in required_markers:
    require(marker in workflow, f"preproduction publication workflow missing guard: {marker}")

for forbidden in [
    '--latest',
    'production_ready = true',
    'production_ready: true',
]:
    require(forbidden not in workflow, f"preproduction publication workflow contains forbidden production marker: {forbidden}")

print("BKE preproduction release publication contract: PASS")
