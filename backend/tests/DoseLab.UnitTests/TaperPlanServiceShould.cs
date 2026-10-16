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
    
    // Boundary value analysis for week count
    [Theory]
    // week count outside the range
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
    
    [Theory]
    // week count inside the range
    [InlineData(1)]
    [InlineData(52)]
    public void SaveAPlanWithValidWeekCount(int weekCount)
    {   
        //setup
        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        var catalog = new InMemoryMedicationCatalog(SimulatedLatency.None);
        var service = new TaperPlanService(repository, catalog, TimeProvider.System);
        
        //execute
        service.Create("Ana Pop", "STR", 40m, weekCount);
        var storedPlans = service.List();
        
        //verify
        var actualPlan = Assert.Single(storedPlans);
        Assert.NotEmpty(actualPlan.Plan.ReferenceNumber);
        Assert.Equal(weekCount,actualPlan.Plan.WeekCount);
 
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

    //  Boundary value analysis 
    [Theory]
    // maximum start daily dosage medication
    [InlineData("STR", 80)]
    [InlineData("CLM", 40)]
    [InlineData("NRV", 20)]
    
    // minimum start daily dosage medication
    [InlineData("STR", 1)]
    [InlineData("CLM", 1)]
    [InlineData("NRV", 1)]
    public void SavePlanWithValidDailyDosageMedication( string medicationCode, decimal dailyDosage)
    {
        // setup
        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        var catalog = new InMemoryMedicationCatalog(SimulatedLatency.None);
        var service = new TaperPlanService(repository, catalog, TimeProvider.System);
        
        //execute
        service.Create("Ana Pop", medicationCode, dailyDosage, 4);
        var actualPans = service.List();
        
        //verify
        var storedPlan = Assert.Single(actualPans);
        Assert.NotEmpty(storedPlan.Plan.ReferenceNumber);
        Assert.Equal("Ana Pop", storedPlan.Plan.PatientName);
        Assert.Equal(medicationCode, storedPlan.Plan.MedicationCode);
        Assert.Equal(dailyDosage, storedPlan.Plan.StartingDailyDoseMg);
        Assert.Equal(4, storedPlan.Plan.WeekCount);

    }

    [Fact]
    public void NotSaveAPlanInPreview() 
    {
        //setup
        var medicationCode = "STR";
        var startingDailyDosage = 40m;
        var weekCount = 4;
        
        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        var catalog = new InMemoryMedicationCatalog(SimulatedLatency.None);
        var service = new TaperPlanService(repository, catalog, TimeProvider.System);
        
        //execute
        var schedule = service.Preview(medicationCode,  startingDailyDosage, weekCount);
        
        //verify
        Assert.Empty(service.List());
        Assert.NotNull(schedule);
        Assert.Equal(weekCount, schedule.Weeks.Count);

    }

    
    public static TheoryData<string?, string> InvalidPatientNames => new()
    {
        { "   ", "Patient name is required."},
        { "", "Patient name is required."},
        { new string('a', 101), "Patient name must be 100 characters or fewer."},
        { null, "Patient name is required."},
    };

    [Theory]
    [MemberData(nameof(InvalidPatientNames))]
    public void NotSaveAPlanWithInvalidName(string? patientName, string errorMessage)
    {
        //setup
        var medicationCode = "STR";
        var dailyDosage = 40m;
        var weekCount = 4;
        
        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        var catalog = new InMemoryMedicationCatalog(SimulatedLatency.None);
        var service = new TaperPlanService(repository, catalog, TimeProvider.System);
        
        //execute 
       var exception = Assert.Throws<TaperValidationException>( () => service.Create(patientName, medicationCode, dailyDosage, weekCount));
       
       //verify
       Assert.Equal(errorMessage, exception.Message);
       
    }


    [Fact]
    public void SaveAPlanWithCustomDateTime()
    {
        //setup
        var fixedClock = new DateTimeOffset(2026, 08, 15,23,59,59, TimeSpan.Zero);
        var patientName = "Ana Pop";
        var medicationCode = "STR";
        var dailyDosage = 40m;
        var weekCount = 4;

        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        var catalog = new InMemoryMedicationCatalog(SimulatedLatency.None);
        var service = new TaperPlanService(repository, catalog, new FixedClock(fixedClock));
        
        //execute
        service.Create(patientName, medicationCode, dailyDosage, weekCount);
        
        //verify
        var plan = Assert.Single(service.List());
        Assert.Equal(new DateOnly(2026, 08, 15).ToString(), plan.Plan.CreatedOn.ToString());
        
    }

    private sealed class FixedClock : TimeProvider
    {
        private readonly DateTimeOffset _now;
        public FixedClock(DateTimeOffset now) { _now = now; }
        public override DateTimeOffset GetUtcNow() => _now;
    }
    
    
}


