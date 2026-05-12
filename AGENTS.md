# AI assistant instructions

This repository ships team-shared playbooks for AI coding assistants under [`.claude/skills/`](.claude/skills/). Each skill is vendor-neutral Markdown — usable by any AI assistant (Claude Code, GitHub Copilot, Cursor, Codex, …) or human reader. Only Claude Code auto-loads them by frontmatter; other tools should read the relevant `SKILL.md` as a referenced runbook.

Start with [`.claude/skills/README.md`](.claude/skills/README.md) — it lists what's available and when each skill applies.

For build / run / test instructions see [`README.md`](README.md).
