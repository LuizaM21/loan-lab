# 08 — AI agents in QA (Claude Code)

Time: 50 minutes.

Goal: use an AI agent to write, review, and triage tests, keep the output trustworthy, and build a reusable skill.

The job ad asks for this directly: "leverage AI agents and Claude Code to generate, maintain and triage tests, and to build automation skills and workflows". This is a strong point for you if you can show real files.

## Learn

### Claude Code in one minute

Claude Code is an AI coding agent that runs in the terminal (and in IDEs). It reads the repo, runs commands (like `dotnet test`), and edits files. You give it a task. It plans, acts, and shows you the changes.

| Piece | File | What it is |
| --- | --- | --- |
| Project memory | `CLAUDE.md` | Read at the start of every session. Project facts, commands, rules. |
| Skill | `.claude/skills/<name>/SKILL.md` | A reusable procedure. Loaded only when the task matches its description. |
| Subagent | `.claude/agents/<name>.md` | A specialist with its own instructions, tools, and clean context. Example: a test reviewer. |
| Slash command | `/review`, or your own | A saved prompt you start by name. |
| Hook | `.claude/settings.json` | A script that runs on an event, for example "run tests after every edit". |
| MCP server | config | Connects tools: a browser (Playwright MCP), Jira, a database. |
| Headless mode | `claude -p "..."` | Runs without chat. Use it in CI pipelines, for example to triage a failed run. |

Cursor, GitHub Copilot, and others have the same ideas (rules files, agents, MCP).

### Context engineering

The agent is only as good as the context you give it. **Give the right context, not all context.**

- Facts it cannot guess: commands, folder layout, naming rules, test style.
- Boundaries: what not to touch (`Do not read instructor/`. `Never change src/ to make a test pass.`).
- One good example to copy.
- Keep `CLAUDE.md` short. Move long procedures into skills, loaded on demand.

### Where AI helps in QA

- Turn acceptance criteria into test cases and code.
- Find missing cases: "list boundary values for this rule".
- Review tests: weak assertions, missing negative cases, hidden shared state.
- Triage failures: product bug, test bug, environment, or flaky?
- Read a Playwright trace or a long log and summarize it.
- Fix locators after a UI change.
- Generate test data and bug report drafts.
- Turn manual test cases into automated tests.

### Risks, and how you control them

| Risk | Control |
| --- | --- |
| It writes tests that pass on buggy code (it copies current behavior) | Give the **rule** and the expected values, not "test this code". Check each test fails when the behavior breaks (red/green, mutation, the bug kit). |
| It invents APIs or wrong facts | Run the tests. Review every diff. |
| It changes product code to make a test pass | A rule in `CLAUDE.md`, and review |
| Weak or no assertions | A review checklist (the `test-reviewer` agent) |
| Patient data sent to an outside service | Never paste real patient data. Use synthetic data. Follow company policy. |
| Audit trail in regulated work | A human reviews and owns every test. Keep tests traceable to requirements. |

**AI writes tests that pass. Your job is tests that find bugs.**

## In this repo

- `CLAUDE.md` — project memory for agents.
- `.claude/skills/write-unit-test/SKILL.md` — how to write a unit test here.
- `.claude/skills/triage-test-failure/SKILL.md` — how to classify a failure and draft a bug report.
- `.claude/agents/test-reviewer.md` — a reviewer subagent with a checklist.

## Do

If you have Claude Code (or Cursor), use it. If not, do steps 1, 4, and 5 by hand. They still prepare you for the questions.

1. **Read** the four files above. For each, say: who reads it, when, and why.
2. **Generate.** Ask the agent: "Use the write-unit-test skill. Add boundary tests for the Calmafen dose limit (1 to 40 mg)." Review the output with the test-reviewer checklist. Did it pick 0.99, 1, 40, 40.01? Did it run `dotnet test`?
3. **Trust check.** Run `instructor/bug.sh on 02`. Do the AI tests fail? (They should: bug 02 is a boundary bug.) Then `instructor/bug.sh reset`.
4. **Triage.** Take a failing test output (break a test on purpose). Ask the agent to use the triage skill. Is the classification right?
5. **Build a skill.** Write `.claude/skills/write-playwright-test/SKILL.md` for this repo. Include: where tests go, the page object, locator rules, unique data, no sleeps, how to run, a done check. This shows "building automation skills the QA community can reuse".

## Check yourself

1. What is the difference between `CLAUDE.md`, a skill, and a subagent?
Claude.md is a file with rules and facts that the agent must obey. It can be used to guide the agent's behavior and decision-making. The file with rules is always loaded. 
A skill is a reusable procedure, a written instruction for the agent to follow, not automation by itself. The agent reads and follows the instructions. The skill is loaded only when the task matches the description.
A subagent is a separate agent with its own instructions, tools, and fresh context. The subagent is independent and also keeps the main conversation's context clean. 

2. What is context engineering? Give two rules you would put in `CLAUDE.md`.
Context engineering is guidance not to give the entire project context but to give the right context. 
Too little and it guesses; too much and the important parts get diluted.
Rule one: Do not create test cases if you don't have requirements/specification/expected results. 
Rule two: Use synthetic data only. Never real patient data. 
Rule three: Always ask users for input to create test cases.

3. Why can AI-generated tests be dangerous? How do you check them?
AI can create tests that pass buggy code. -> Check: Give a rule and expected values mot the code.
AI can create tests that don't cover any cases. -> Make a review on the cases that cover any edge cases.
AI can invent APIs that don't exist.  -> Run the test and check the API exists.
AI can use the wrong APIs in a test. -> Create a rule in Cluade.md plus review the used API.
AI can modify tests to pass when it shouldn't. -> Check the test fails when the behavior is broken.

4. How could you use an AI agent inside a CI pipeline?
In CI I’d run the agent in headless mode as a pipeline step. 
When tests fail, it uses a triage skill to classify the failure and post a draft bug report on the pull request, 
so the team starts from a summary instead of a raw log. It could also review new tests or summarise performance results. 
But it only advises: a human makes the decision and owns the outcome.

5. What data must never go into an AI prompt in a medical company?
Real patient data, passwords, encrypted keys, API keys, code structure, Health data is a special category under GDPR, production data.

## Say it in the interview

- "I use Claude Code with a short CLAUDE.md for project facts and boundaries, and skills for repeated QA work: writing tests in our style, triaging failures."
- "I never trust a green AI test. I give it the rule and the expected values, and I check the test fails when the behavior is broken."
- "I'd build shared skills and a reviewer agent for the QA community, so everyone gets the same quality bar."
- "No real patient data in prompts. Synthetic data only, and a human owns every test."
