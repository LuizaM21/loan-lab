import { expect, type Page } from '@playwright/test'

export class TaperPlansPage {
    constructor(private readonly page: Page){}

    // locators: methods that return a Locator (lazy, so they're safe to call anytime)
    plans() { return this.page.getByRole('table', { name: 'Taper plans' }) }
    schedule() { return this.page.getByRole('table', { name: 'Taper schedule' }) }
    alert() { return this.page.getByRole('alert') }
    
    //actions
    async open(){
        await this.page.goto('/')
        await expect(this.plans()).toBeVisible()
    }
    
    // click "Add taper plan"
    async openForm(){
        await this.page.getByRole('button', { name: 'Add taper plan' }).click()
        await expect(this.page.getByRole('heading', { name: 'New taper plan' })).toBeVisible()
    }

    // fill patient only if given, then medication, dose, weeks
    async fillForm(medicationCode: string, dose: string, weeks: string, patient?: string) {
        if(patient) {
            await this.page.getByLabel('Patient name').fill(patient)
        } 
        await this.page.getByLabel('Medication').selectOption(medicationCode)
        await this.page.getByLabel('Starting daily dose (mg)').fill(dose)
        await this.page.getByLabel('Number of weeks').fill(weeks)
    }
    
    async preview() {
        await this.page.getByRole('button',{ name: 'Preview schedule' }).click()
    }

    async save() {
        await this.page.getByRole('button', { name: 'Save' }).click()
    }
    
    async createServerError() {
        await this.page.route
        (/\/api\/(taper-plans|medications)$/,
            (route) => 
                route.fulfill({ status: 500, body: '{}' }))
    }

}