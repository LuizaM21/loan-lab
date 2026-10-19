# Exam 1 — Theory
## Part A — Multiple choice (1 point each)

Choose one answer.

**1.** Why should most automated tests sit at the bottom of the pyramid?
a) Unit tests find every bug.
b) They are fast and stable, and a failure points to the exact code.
c) UI tests are not allowed in CI.
d) They need no maintenance.
Answer: B

**2.** A rule says the starting dose must be from 1 mg to 20 mg, inclusive, with 0.01 mg precision. Which set uses boundary value analysis best?
a) 1, 20
b) 0.99, 1, 20, 20.01
c) 0, 10, 30
d) 1, 10, 20
Answer: B

**3.** A test passes, then fails, then passes again with no code change. It is:
a) a regression test
b) a flaky test
c) a smoke test
d) a confirmation test
Answer: B

**4.** In Playwright, the best way to wait for a new row in a table is:
a) `await page.waitForTimeout(2000)`
b) `await expect(rowLocator).toBeVisible()`
c) a loop with `sleep`
d) set `retries: 5`
Answer: B

**5.** Which locator is the most robust?
a) `page.locator('div > table tr:nth-child(3) td')`
b) an absolute XPath from `/html/body`
c) `page.getByRole('button', { name: 'Save' })`
d) `page.locator('.btn-primary')`
Answer: A

**6.** "p95 response time is 250 ms" means:
a) The average is 250 ms.
b) 95% of requests took 250 ms or less.
c) 95 requests took 250 ms.
d) The slowest request took 250 ms.
Answer: B

**7.** A stress test:
a) checks the system at expected traffic
b) pushes beyond expected load to find the breaking point and how the system fails
c) runs for 12 hours to find leaks
d) is one user checking that the app starts
Answer: B

**8.** A soak (endurance) test mainly finds:
a) layout bugs
b) memory leaks and slow degradation over time
c) wrong labels
d) boundary bugs
Answer: B

**9.** An endpoint gets slower as the number of rows grows, because each row triggers its own database call. This is:
a) a race condition
b) the N+1 query problem
c) a deadlock
d) a cache hit
Answer: B

**10.** The correct status code for a successful create is:
a) 200
b) 201
c) 204
d) 302
Answer: B

**11.** A request has no credentials, or wrong ones. The correct status code is:
a) 400
b) 401
c) 403
d) 404
Answer: B

**12.** `WebApplicationFactory<Program>` gives you:
a) a real browser
b) the real ASP.NET pipeline in memory, without a network port
c) a mock of every class
d) the production database
Answer: B

**13.** Which pipeline order gives the fastest useful feedback?
a) UI → API → unit
b) unit → API → UI → performance smoke
c) performance → UI → unit
d) all tests in one job, in random order
Answer: B

**14.** In ISTQB terms, confirmation testing is:
a) checking that a fixed defect is really fixed
b) re-running all tests after any change
c) testing done by the customer
d) load testing
Answer: C

**15.** "Exhaustive testing is impossible" means:
a) do not test at all
b) use risk and test-design techniques to choose tests
c) only test the UI
d) automate every possible input
Answer: B

**16.** Severity and priority:
a) are the same thing
b) severity is the impact; priority is how soon it must be fixed
c) priority is the impact; severity is how soon
d) are set only by the tester
Answer: B

**17.** IEC 62304 is about:
a) web accessibility
b) the life cycle of medical device software
c) password rules
d) cloud costs
Answer: B

**18.** ISO 14971 is about:
a) risk management for medical devices
b) user interface colors
c) the Java language
d) data protection law
Answer: A

**19.** An AI tool wrote 20 green tests for existing code. What is the best next step?
a) Merge them.
b) Review the assertions, and check that each test fails when the behavior is broken.
c) Delete them.
d) Ask for 20 more.
Answer: B

**20.** `CLAUDE.md` in a repo is:
a) the license
b) project context and rules that Claude Code reads at the start of a session
c) a test report
d) the CI configuration
Answer: B

## Part B — Short answers (2 points each)

Two or three sentences each.

**21.** In Dose lab, where would you test that 10 mg over 3 weeks gives 6.67 mg in week 2? And where would you test that the screen shows `6.67`? Why both?
Answer: In the backend, to ensure the calculation is correct (unit test). In the frontend to ensure that the value is displayed correctly (UI test).

**22.** Name three causes of flaky UI tests, and one fix for each.
1. Race Condition (happens when the result depends on which of two things finishes first and the order is not guaranteed)
2. Network latency (instead of waiting for the element to be displayed immediately start the process in an async manner)
3. Element locator issues (the element is not found because of the change of the DOM. Use a robust search element. A stable locator: GetByRole, GetByLabel) 
4. Tests that run in parallel and have the same shared data (use isolated test data that don't share the same state) 

**23.** Explain load, stress, spike, and soak tests, one sentence each.
Load test – apply a realistic load (TPS) to the system to measure the performance of the system under normal conditions (P99/P95 Response Time, Error rate, P99 Throughput).
Stress test – gradually increase the load until the system fails. This is done to identify the breaking point of the application and how it recovers.
Spike test – a sudden increase of the load that lasts for a short period of time then raises again, then decreases. This is done to identify the stability of the application and how it scales, load balancer behaves.
Soak test – a constant load that runs for a long period of time to spot any memory leaks or resource saturation, DB connection pool saturation, etc.

**24.** A k6 run shows p95 = 900 ms for `GET /api/taper-plans`, but only after 1,000 plans exist. With 10 plans it is 10 ms. What do you suspect, and how do you confirm it?
This reflects a database problem. The N+1 query problem. 
For 11 records, the database is queried 10 times, 
but for 1,000 records, the database is queried 1,001 times, increasing the response time.
To confirm it, run k6 load test with several data sizes to confirm the problem: 10 records, then 100 records, then 1,000 records.

**25.** Why must a Playwright test that saves data use a unique name? Give one other way to keep tests independent.
Because the test may fail if another test runs before it and uses the same name. Since the records are stored in a database, once the test runs, 
the record is saved in the database. If another test runs before it and uses the same name, 
the new record can't be saved because the record already exists. 
Another way is Cleanup. Delete the data the test in teardown. Fresh DB per run or per worker.

**26.** What is shift-left? Give two concrete practices.
Shift-left is moving testing earlier in the development process. 
Two concrete practices are unit testing and integration testing. 
TDD (Test Driven Development) is one strategy. Write test before the code.
Performance testing is another strategy. Test early, not just before release (a quick check in CI or nightly builds against a baseline). 

**27.** You find that `GET /api/taper-plans` returns patient names with no authentication. Give a bug title, severity, and the OWASP category, with one sentence on why.
Title: GET /api/taper-plans returns patient medication plans without authentication
Severity: Critical
OWASP category: A01 Broken Access Control
Why: The endpoint does not require authentication, allowing unauthorized access to the user to read medical records of a patient which is a data breach risk.

**28.** What is a test harness? Give one example from this repo.
A test harness is a framework that provides a consistent environment and tools for running tests. 
An example from this repo is the DoseLabApiFactory 

**29.** What is traceability in medical software testing, and why does an auditor care?
Represents the proof that the requirements are tested. Test cases, test results, and other metrics.
Traceability is a chain of links between requirements → test cases → tests results → defects → fixes → regression tests → release

**30.** Name two risks of AI-generated tests and how you control each one.
AI can generate false-positive tests (tests that pass but do not verify the requirement). Check each test is valid. Test it and validate how it should work. What is should verify. When it should run. When it should pass.  
Test cases that do not cover anything. Check each generated test covers a real case. 
AI can generate invalid test data. Check each generated test data is valid. 
Using AI can leak sensitive data. Use synthetic data to avoid leaking sensitive data. Use only AI company, approved tools.