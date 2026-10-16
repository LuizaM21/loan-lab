using DoseLab.Application;
using Xunit;

namespace DoseLab.UnitTests;

/// <summary>
/// Tests for TaperScheduleCalculator, grouped by equivalence partition:
/// divides evenly, doesn't divide evenly, and invalid/valid week count.
/// </summary>
public class TaperScheduleCalculatorShould
{

    [Fact]
    public void LowerTheDailyDoseByTheSameStepEveryWeek()
    {
        // setup
        var startingDailyDoseMg = 40m;
        var weekCount = 4;

        // execute
        var schedule = TaperScheduleCalculator.Calculate(startingDailyDoseMg, weekCount);

        // verify
        Assert.Equal(40m, schedule.Weeks[0].DailyDoseMg);
        Assert.Equal(10m, schedule.Weeks[3].DailyDoseMg);
        
        Assert.Equal(280m, schedule.Weeks[0].WeeklyTotalMg);
        Assert.Equal(70m, schedule.Weeks[3].WeeklyTotalMg);
        
        Assert.Equal(280m, schedule.Weeks[0].CumulativeTotalMg);
        Assert.Equal(700m, schedule.Weeks[3].CumulativeTotalMg);

        Assert.Equal(4, schedule.Weeks.Count);
        Assert.Equal(700m, schedule.TotalMg);
    }
    
    [Fact]
    public void KeepTheRoundingRemainderInTheLastWeek() {
        // setup
        var startingDailyDoseMg = 10m;
        var weekCount = 3;
        
        //execute
        var schedule = TaperScheduleCalculator.Calculate(startingDailyDoseMg, weekCount);
        
        //verify
        Assert.Equal(weekCount, schedule.Weeks.Count);
        Assert.Equal(10m, schedule.Weeks[0].DailyDoseMg);
        // verify the remainder dosage is included in the final week
        Assert.Equal(3.34m, schedule.Weeks[2].DailyDoseMg);
        
        // verify the weekly total dosage is correct
        Assert.Equal(70m, schedule.Weeks[0].WeeklyTotalMg);
        Assert.Equal(23.38m, schedule.Weeks[2].WeeklyTotalMg);
        
        // verify the cumulative total dosage is correct
        Assert.Equal(70m, schedule.Weeks[0].CumulativeTotalMg);
        Assert.Equal(140.07m, schedule.TotalMg);
    }
    
    [Fact]
    public void ReturnErrorWhenWeekCountIsLessThanOne() {
        //setup
        var dailyDose = 10m;
        var weekCount = 0;
        
        //execute & verify 
        Assert.Throws<ArgumentOutOfRangeException>(() => TaperScheduleCalculator.Calculate(dailyDose, weekCount));
        
    }

}
