# AI assistant instructions

This repository ships team-shared playbooks for AI coding assistants under [`.claude/skills/`](.claude/skills/). Each skill is vendor-neutral Markdown — usable by any AI assistant (Claude Code, GitHub Copilot, Cursor, Codex, …) or human reader. Only Claude Code auto-loads them by frontmatter; other tools should read the relevant `SKILL.md` as a referenced runbook.

Start with [`.claude/skills/README.md`](.claude/skills/README.md). The primary workflow in this repo is [`regen-dotnet-client`](.claude/skills/regen-dotnet-client/SKILL.md) — the NSwag regen sequence (`download_swagger.py` → `generate-client.ps1` → `patch_optional_multipart.py`), v1/v2 branch-protection rules, and the two consumption paths (`UseLocalClientLib` for fast iteration vs. preview NuGet via PR-to-`v2`).

For build / run / test instructions see [`README.md`](README.md).
