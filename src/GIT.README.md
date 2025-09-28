# CI/CD Quick Ref

- Smoke build: push to any branch (restore + build only).
- Full build + tests: push to `feature/*`, `integration`, `release/*`, any PR, or include `[ci full]` in the commit message.
- Force full on any branch:
  `git commit -m "Your message [ci full]" && git push`
- Start a feature branch:
  `git checkout -b feature/my-change && git push -u origin feature/my-change`
- Manual full run: GitHub → Actions → CI → Run workflow → set `full=true`.
