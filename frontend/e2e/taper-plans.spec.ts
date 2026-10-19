import { expect, test } from '@playwright/test'
import { TaperPlansPage } from './pages/TaperPlansPage' /**Exercise 5*/



test('shows the seeded taper plans', async ({ page }) => {
  // setup

  // execute
  await page.goto('/')

  // verify
  await expect(page.getByRole('heading', { name: 'Taper plans' })).toBeVisible()
  const plans = page.getByRole('table', { name: 'Taper plans' })
  await expect(plans.getByRole('cell', { name: 'Maria Ionescu' })).toBeVisible()
  await expect(plans.getByRole('cell', { name: 'Andrei Stan' })).toBeVisible()
})

/**Exercise 2*/
test('previews an uneven taper with two decimals', async ({ page }) => {
  //setup
  const plansPage = new TaperPlansPage(page)
  await plansPage.open()
  await plansPage.openForm()
  //fill the Medication = CLM, dose = 10, weeks = 3 (Patient name isn't needed for a preview)
  await plansPage.fillForm('CLM', '10', '3')

  //execute
  await plansPage.preview()
  
  //verify exactly 3 body rows: schedule.locator('tbody tr') with toHaveCount
  const rows = plansPage.schedule().locator('tbody tr')
  await expect(rows).toHaveCount(3)
  //week 2 row shows 6.67
  await expect(rows.nth(1).getByRole('cell').nth(1)).toHaveText('6.67');
  //week 3 row shows 3.34
  await expect(rows.nth(2).getByRole('cell').nth(1)).toHaveText('3.34');
})

/**Exercise 3*/
test('shows a saved plan in the list without a reload', async ({ page }) => {
  //setup
  const plansPage = new TaperPlansPage(page)
  await plansPage.open()
  const patient = `Ana Pop ${Date.now()}`
  await plansPage.openForm()
  
  //execute
  await plansPage.fillForm('CLM','10','4',patient)
  await plansPage.save()
  
  //verify
  const plans = plansPage.plans()
  await expect(plans.getByRole('cell', {name: patient})).toBeVisible();
  //check the form closed
  await expect(page.getByRole('heading', { name: 'New taper plan' })).toBeHidden();
  
})

/**Exercise 4*/
test('shows the server message when the dose is above the maximum', async ({ page }) => {
  // setup
  const plansPage = new TaperPlansPage(page)
  await plansPage.open()
  await plansPage.openForm()

  // execute
  // Medication NRV(valid), dose 25(invalid), weeks 4 (valid)
  await plansPage.fillForm('NRV', '25','4')
  await plansPage.preview()
  
  // verify
  //the alert has exactly the expected text
  await expect(plansPage.alert()).toHaveText('Starting daily dose must not exceed 20 mg for Nervalin.');
  //the schedule table is not shown
  await expect(plansPage.schedule()).toBeHidden();
})

/**Exercise 6*/
test('shows an error when the plans cannot be loaded', async ({ page }) => {
  // setup: intercept BEFORE the page loads
  const plansPage = new TaperPlansPage(page)
  await plansPage.createServerError()

  // execute
  await page.goto('/')

  // verify the alert contains "Could not load taper plans"
  await expect(plansPage.alert()).toContainText('Could not load taper plans.');
  // the plan table is not shown
  await expect(plansPage.plans()).toBeHidden();
})

/** Exercise 7 */
test('shows a plan created through the API', async ({ page, request }) => {
  // setup: create the data directly, with no UI
  const patient = `Api Patient ${Date.now()}`
  const response = await request.post('http://localhost:5080/api/taper-plans', {
    headers: { 'X-Api-Key': 'lab-dev-key' },
    data: { patientName: patient, medicationCode: 'STR', startingDailyDoseMg: 40, weekCount: 4 },
  })
  // assert the setup worked
  await expect(response).toBeOK()
  
  // execute -> open the page with page object
  const plansPage = new TaperPlansPage(page)
  await plansPage.open()
  
  // verify the patient appears in the plan table
  await expect(plansPage.plans()).toContainText(patient);
})