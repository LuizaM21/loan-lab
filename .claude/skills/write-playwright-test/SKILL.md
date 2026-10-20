---
name: write-playwright-test
description: Writes Playwright end-to-end UI tests for the Dose lab React app (taper plan pages). Use when asked to add, extend, or fix a UI, browser, or end-to-end test in frontend/e2e/. Not for C# unit or API tests
---

# Write a Playwright test

1. Find the rule. Read it in `CLAUDE.md` (Domain rules) or the acceptance criterion. Look at the UI only to find labels and roles. Write the rule in one sentence before you write code.
   
2. Work out the expected values **by hand** from the rule. Never copy them from running the code.
   - UI tests cover user flows (preview, save, error message). 
   - Boundary values belong in unit and API tests (the pyramid).
3. Write the test in the right file:
   - Add to the existing spec for that feature (e.g. `taper-plans.spec.ts`). Create a new `<feature>.spec.ts` only for a new page.
   - each test must have the layout 
   - ```//setup //execute //verify```
4. Use the page object:
    - Use and extend `TaperPlansPage.ts`
    - Put new locators and actions in the page object and keep the assertions in the test
5. Locators: 
   - use locator method `getByRole` for buttons, dropdowns
   - use locator method `getByLabel` for form fields with a label
   - No CSS or XPath selectors
6. Assertions and waiting:
   - no waitForTimeout
   - Web-first assertions (await expect(...))
   - `await expect(locator).toHaveCount(n)` only for deterministic lists like the schedule rows. Never exact counts on the shared plans list.
   - check that created unique row is visible
7. Use test data:
   - Synthetic data only. Give saved data a unique name, e.g. `Ana Pop ${Date.now()}`, so tests don't collide.
8. Run it: 
   - in `frontend` run the test with `npm run test:e2e`
   - the config starts the API and UI automatically
   - it runs on Chromium and Firefox. Both must pass
9. Prove it can fail:
   - Change one expected value in the assertion, run it and see it fail, then put it back
10. If it fails:
   - Read the trace and screenshot
   - Do not change product code or weaken the assertion
   - Classify the failure with the triage-test-failure skill

## Done when

- Every test links to a rule or user flow.
- All tests pass on Chromium and Firefox, and each new test was seen failing once.
- Role and label locators only, no `waitForTimeout`, page object used.
- No product code changed.
- ...