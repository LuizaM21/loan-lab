using DoseLab.Application;
using Xunit;

namespace DoseLab.UnitTests;

public class TaperPlanServiceShould
{
    [Fact]
    public void SaveAPlanThatAppearsInTheList()
    {
        // setup

        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        var catalog = new InMemoryMedicationCatalog(SimulatedLatency.None);
        var service = new TaperPlanService(repository, catalog, TimeProvider.System);

        // execute

        service.Create("Ana Pop", "STR", 40m, 4);

        // verify

        var saved = Assert.Single(service.List());
        Assert.Equal("Ana Pop", saved.Plan.PatientName);
        Assert.Equal("STR", saved.Plan.MedicationCode);
        Assert.Equal("Steriva", saved.MedicationName);
        Assert.Equal(40m, saved.Plan.StartingDailyDoseMg);
        Assert.Equal(4, saved.Plan.WeekCount);
        Assert.Equal("TP-0001", saved.Plan.ReferenceNumber);
    }

    // Add your own [Fact] or [Theory] methods below this line.
    // boundary test for a week interval
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(53)]
    public void RejectAPlanWithAnInvalidWeekCount(int weekCount)
    {   
        //setup
        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        var catalog = new InMemoryMedicationCatalog(SimulatedLatency.None);
        var service = new TaperPlanService(repository, catalog, TimeProvider.System);
        
        //execute
        var exception = Assert.Throws<TaperValidationException>(
            () => service.Create("Ana Pop", "STR", 40m, weekCount));
        
        //verify    
        Assert.Equal("Number of weeks must be between 1 and 52.", exception.Message);
        Assert.Empty(service.List()); //a rejected plan must not be saved
    }

    public static TheoryData <string, decimal, string> InvalidStartingDoses => new ()
    {
        //bellow the minimum daily dose
        {"STR",0.99m, "Starting daily dose must be at least 1 mg."},
        {"STR",0m, "Starting daily dose must be at least 1 mg."},
        {"STR",-1m, "Starting daily dose must be at least 1 mg."},
        
        //just above the medication maximum daily dose
        {"STR", 80.01m, "Starting daily dose must not exceed 80 mg for Steriva."},
        {"CLM", 40.01m, "Starting daily dose must not exceed 40 mg for Calmafen."},
        {"NRV", 20.01m, "Starting daily dose must not exceed 20 mg for Nervalin."},
    };

    [Theory]
    [MemberData(nameof(InvalidStartingDoses))]    
    public void RejectAPlanWithInvalidDailyDosageMedication(
        string medicationCode, decimal dailyDosage, string expectedMessage)
    {
        //setup
        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        var catalog = new InMemoryMedicationCatalog(SimulatedLatency.None);
        var service = new TaperPlanService(repository, catalog, TimeProvider.System);
        //execute
        var exception  = Assert.Throws<TaperValidationException>(() => 
            service.Create("Ana Pop", medicationCode, dailyDosage, 4));
        //verify
        Assert.Equal(expectedMessage, exception.Message); 
        Assert.Empty(service.List()); //a rejected plan must not be saved
            
    }
    

}
