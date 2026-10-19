# 10 — Interview playbook

Time: 30 minutes, plus practice.

Goal: tell your story, answer the likely questions, and show your work.

## Your story (60 seconds)

Write it in `notes/my-story.md`, then say it out loud 5 times.

1. Who you are: years in testing, the domains you know.
I’m a performance test engineer with 10 years in testing. For the last six and a half years I’ve been at Mambu, 
a core banking platform, where I load-test the system for large client migrations: 
I identified the critical APIs, build the load model in a Gatling framework with SCALA, and use Grafana, AWS metrics, 
and Dynatrace to size the infrastructure so a migration finishes on time. 
Before that, at Endava, I did functional and UI testing on an insurance platform, 
with JMeter for REST and SOAP services and Selenium WebDriver in Java.

2. What you do best as a tester: finding the risky cases, clear bug reports, working with developers.
What I do best is noticing what others miss. When performance drops after a build or infrastructure change, 
I don’t stop at "it’s slower". I dig until I find the cause and can show it with data. 
I’m also careful about trustworthy results: I run tests from a CI pipeline instead of my laptop, 
so local network noise doesn’t skew the numbers and anyone can reproduce them.

3. What you built recently: "I built a full test suite for a .NET and React medical-style app: unit, API, Playwright, k6 load and stress tests, a CI pipeline, and AI skills for test work." Show your fork.
At Mambu I built a performance regression framework for the core banking engine, in Scala with Gradle and Docker. 
It became software as a service SaS tool: at my guidance, other teams run it on their own builds and see the performance impact before anything reaches production. 
To prepare for this role, I built a full test suite for a .NET and React medical-style app: 
xUnit unit tests, API integration tests with WebApplicationFactory, Playwright UI tests on Chromium and Firefox, 
k6 load tests, all running in a GitHub Actions pipeline.

4. Why this role: medical software, where quality protects patients.
Banking taught me that a bug costs money. In medical software, a bug can hurt a patient, 
so quality isn’t a phase, it’s the product. After years focused on performance, 
I want to own the whole test strategy, from unit tests to the pipeline, 
and the last few weeks building DoseLab showed me how quickly I can grow into that.

Be honest about experience. Show that you learn fast and think in risks.

## STAR stories

Prepare 5 stories. **S**ituation, **T**ask, **A**ction, **R**esult (with a number if possible).
Situation: Our services ran on AWS ECS, which was supposed to scale up automatically when latency or CPU went above 50% in a short time.
Task: Make sure the system could handle a sudden load increase.
Action: I ran a stress test, and the service degraded without scaling up. I looked at how the scaling rules were defined. They checked latency and CPU only once every 15 minutes, so a spike was over, or had already hurt users, before the rule even noticed it. I changed the rule to evaluate 3 samples within 5 minutes, then reran the same stress test to confirm.
Result: The infrastructure scaled up in time and handled the load (from 7 tasks to 14 tasks). 

1. A bug you found that really mattered.
The bug was described in the STAR story 
2. A disagreement with a developer or product owner, and how you solved it.
   S: After a release, several APIs got slower, and the developer whose change was in that build didn’t think his code was the cause.
   T: Find out objectively whether the change was responsible.
   A: Instead of arguing, I ran the same load test on the build before and after his change, in the same environment. The results showed the degradation came with his build. In the monitoring we saw the DB connection pool fully used, so other APIs were waiting for a free connection. I shared the results with him and we went through them together.
   R: He reverted, fixed the connection handling, and performance went back to the baseline.
3. A process you improved (faster tests, better bug reports, fewer escaped bugs).
   S: Every performance test needed a large volume of realistic data, and each person created it from scratch.
   T: Cut the setup time for everyone.
   A: I populated a database through the API, so the data passed the same validations as real data, then saved it as a snapshot that any developer could restore before a test.
   R: Setup went from [7 working days] to [30 minutes]. [All the development teams use it].
4. A time you learned something new fast (this lab is one).
   S: I’m applying for a C# test automation role, but my daily stack is Scala and Gatling.
   T: Get productive in .NET testing quickly.
   A: In about two weeks I built a full suite on a .NET and React app: xUnit unit tests, API tests with WebApplicationFactory, Playwright UI tests, k6 load tests, and a GitHub Actions pipeline. I learned C# by mapping it to what I knew from Scala, such as records and case classes, or LINQ and collections.
   R: The whole suite runs in CI on Chromium and Firefox. [Today I even found a bug. The firefox test could not run on CI because of a missing firefox installer. Read the error: understand the problem and introduce the missing image for firefox. Ran again and the pipeline was green again]
5. A time requirements were unclear, and what you did.
   S: A client was migrating to our core banking platform. The request was “size the infrastructure so the migration finishes in max 4 hours,” but we had no details on their data.
   T: Find the right infrastructure size so the migration fits the client’s time window.
   A: I asked for the inputs that drive the load: their product configuration, the number of active clients and accounts, and historical transactions per account. 
      These told me which APIs would be called and how many times. [If something was missing: “Where a number wasn’t available, I wrote down my assumption and got the team to confirm it.”] 
      I turned that into a load model, ran short tests to measure resource use, and extrapolated the size, then confirmed it with a test at the target size.
   R: The migration finished in under [3] hours, within the [4]-hour window, on the infrastructure I recommended.


## Bug report template

```text
Title:     Saving a plan without an API key is accepted
Area:      API, POST /api/taper-plans
Severity:  High (unauthorized changes to prescriptions)   Priority: P1
Steps:     1. curl -X POST http://localhost:5080/api/taper-plans -H "Content-Type: application/json"
              -d '{"patientName":"X","medicationCode":"STR","startingDailyDoseMg":40,"weekCount":4}'
Expected:  401 Unauthorized, nothing saved
Actual:    201 Created, plan saved
Evidence:  failing test TaperPlansApiShould.RejectASaveWithoutAnApiKey, response body
Notes:     A wrong key is still rejected. Only a missing header gets through.
```

## Likely questions, and where you learned the answer

| Question | Lesson |
| --- | --- |
| How would you design a test strategy for our product? | 01 |
Test strategy: 
    Define the scope: What needs to be tested and how much.
    Define the risk: what are the most vulnerable and important areas to cover. What could harm a patient. 
    Define the test level: design unitTest for each new core function, design integration tests for each component (API test), design UI test for the most important functionality (e2e tests).
    Choose the tool: for unit tests: Xunit, for integration test (mainly on APIs) WebApplicationFactory, for UI tests: Playwright with TypeScript.
    Choose the test environment and CI: local, staging, production with CI/CD pipelines.
    Choose the test data: fake data (stubs), random data, edge cases.
    Choose the test coverage: I’d set a floor, say 80% on critical modules like dose calculation, and use mutation testing to check that the tests actually catch bugs.
    Choose the test frequency: 
            unit tests + integration tests (APIs) for every change/commit,
            UI smoke test for each MR!/PR!,
            the full UI suite and performance tests nightly, 
            stress tests before a release.
    Choose the test priority: critical, high, medium, low.
    Choose the metrics: code coverage, test duration, flaky test rate, test failure rate.
    Choose the reporting tool: Jira, GitHub, Slack, email.

| Explain the testing pyramid. When do you break it? | 01 |
    Test pyramid consists of the base level represented by:
    many unit tests at the bottom -> (fast, cheap, verbose, stable, point straight to the breaking code line)
    fewer integration tests in the middle, 
    and a small number of UI end-to-end tests on top -> (Flaky, slow, more fragile)
    The reason is cost and speed. Unit tests run in milliseconds, rarely break by accident, 
    and tell you exactly what’s wrong. UI tests are slow and more fragile, 
    so you keep them for the critical user journeys.

| How do you choose test cases for a numeric limit? | 02 |
    For a starting dose that has a start value limit of 1 and max limit of 20 I would choose boundary value testing technique  
    the test cases would be:
    - Test case 1: Starting dose = 0.99m (invalid)
    - Test case 2: Starting dose = 1m (valid)
    - Test case 3: Starting dose = 20m (valid)
    - Test case 4: Starting dose = 20.01m (invalid)

| How do you test an API? What do you check? | 03 |
    Check: Status Code, Headers, and Response Body and the state (perform a GET after a POST was done) to confirm what was actually stored  
    I would also check the response time and ensure that the API is returning the expected data.
    Negative testing: invalid inputs will give a 400: Bad Request with a clear error message, 
                        missing or wrong API key should retrieve 401: Unauthorized,
                        unknown API endpoint should retrieve 404: Not Found
    In .NET, I use WebApplicationFactory It runs the real API in memory, with routing, 
    validation and filters, but no network, so tests are fast and stable.

| How do you structure a large Playwright suite? How do you stop flaky tests? | 04 |
I use the POM technique (Page Object Model) to structure separate actions live open(), previewForm(), close(), SaveForm() from the tests. 
Each page has its own actions definition inside a POM structure.
To avoid flaky tests, I use Playwright methods like is toHaveText() toBeVisible() instead of isVisible() or contains().
The difference is that these methods are more reliable and wait for the element to load. They check the element many times until it reaches the maximum wait of 5 seconds.
Another method is identifying the UI elements using locator like getByRole() -> for buttons/dropdown lists or getByLabel() -> for fields.
These locator methods survive the CSS changes in case of any. Always avoid using an absolute path for locator methods.
I also use fixtures to share setup and teardown code between tests. 
Usage of specific tags like @smoke @regression @featureName. This way the CI can a fast subset on each pull request and the full suite nightly, across browser projects.

| Walk me through a performance test. What do you report? | 05 |
For a load test type:
1. Emulate a production like environment and data (use synthetic data at production volume/setup the same properties/enable features)
2. Apply an expected load (concurrent users/sec) for 20 ~ 30 minutes to follow if the system can sustain a normal load and what is the resource consumption for that load. 
3. Run a baseline load test to identify the current performance of the system.
4. Run a second test that will be compared with the baseline test to identify the performance impact of the changes. 
5. In a 20-min test duration (created in Gatling with open-model injection profile), the first step is to apply the load with a ramp up -> hold the steady state -> then ramp down. 
6. Identify the infra usage for that period of time and check if the system can sustain the load. Check the CPU, Memory, DB connections, Network usage. 
7. Compare with Expected SLA's only for the steady load period: Response time. Ex: P95 < 100ms, Error Rate < 1%, Throughput > 1000 req/sec
Passed: P95 < 100ms: 95% of the total requests have Response time under 100ms
Passed: Error Rate < 1%: Fewer than 1% of all requests failed.
Passed: Throughput > 1000 req/sec: During the steady state, the system completed on average 1,150 requests per second. 
8. Analyze the report and identify the bottlenecks and recommend improvements 
9. Create a report with the results and share it with the team

| How do you find a bottleneck? | 05 |
I start from the symptom and work down. In the load test results I find where the p95 spikes or where throughput stops growing 
as load increases. Then I take that time window into the APM, Dynatrace in my current job, 
and check what saturated at the same moment: CPU, memory and garbage collection, the DB connection pool, slow queries.
From there I drill down: which API, which service behind it, how many calls per request, 
how long it waits for a DB connection. A typical finding is high-connection acquisition time: the service holds connections 
too long, the pool runs out, and every other API waits. I also make sure the load generator itself isn’t the limit. 
Once I have a confirmed cause, I raise a ticket with the evidence, and after the fix I rerun the same test against the baseline to prove it’s resolved.

| Design a pipeline for us. What runs when? | 06 |
After each change/commit -> runs the Build, Unit + API tests, security scan, small E2E smoke, small perf smoke (~10 min)
After each merge/pull request -> trigger a run for integration tests, Full E2E suite on all browsers
At each nightly pipeline -> (the longest runs) Full regression, load and endurance tests against the baseline
Release validation candidate -> Stress test, full security scan, sign-off on the evidence
After each release -> Production smoke test + monitoring

| How do you test security? | 07 |
I test to see if I can access sensitive data without authorization (API-key). Broken Access Control.
I test to see if the url manipulation can bypass the authentication
I test to see if different fields allow me to inject a script or a query.
I insert a huge input that must give a 400, never a 500 with a stack trace.

| How do you use AI agents in QA? How do you trust the output? | 08 |
I create rules and context for the AI tool. I will try to give the right context, not the whole context. 
For longer procedures I create skills such as converting acceptance criteria into test cases, 
verifying if the assertion is weak or not. Always verify and validate the out made by AI. It can create invalid scenarios, invent test data.
I never trust a green (passed) AI test. The biggest risk is a test that passes on buggy code, because the AI copied current 
behavior. So I give it the rule and the expected values, I check every new test fails when I break the behavior, and I review every diff. 
A reviewer agent with a checklist helps catch weak assertions.

| What is different about medical software? | 09 |
| TDD and shift-left: how do you work with developers? | 09 |

### Topics outside this lab

**C++ desktop apps.** The pyramid is the same.

- Unit tests: GoogleTest or Catch2.
- UI automation for Windows desktop: Ranorex (named in the ad), FlaUI (C#, Windows UI Automation), WinAppDriver / Appium.
- Same rules as web: stable element ids (automation ids), no fixed waits, page objects ("screen objects").

**Ranorex** is a commercial tool for desktop, web, and mobile UI tests, with a recorder and C# code. **Cypress** is a JavaScript web test tool, similar to Playwright. It runs inside the browser, while Playwright drives the browser from outside.

If you do not know something: say so, then say how you would find out. "I have not used Ranorex yet. I'd start with its C# API, because I write my tests in C# already."

## Questions to ask them

1. What does your test pyramid look like today? Where is it weakest?
2. How long does the pipeline take, and how flaky is it?
3. Which safety class is the software, and how do you handle traceability?
4. How does the QA team use AI agents today? What would you like this role to build?
5. Who owns the test strategy: QA, developers, or both?
6. What would success look like in my first 6 months?

## The day before

- Pass all three exams (see `exams/`).
- Push your fork. Clean up `notes/`. Make the README of your fork say what you added.
- Practice your story and 5 STAR stories out loud.
- Prepare a 3-minute demo: run `dotnet test`, run one Playwright test in UI mode, show a k6 summary, show a skill file.
