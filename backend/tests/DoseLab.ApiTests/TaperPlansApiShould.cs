using System.Net.Http.Json;
using Xunit;
using System.Net;

namespace DoseLab.ApiTests;

/**Exercise 7*/
public class TaperPlansApiShould(DoseLabApiFactory factory) : IClassFixture<DoseLabApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ReturnTheSeededTaperPlans()
    {
        // execute
        var response = await _client.GetAsync("/api/taper-plans");

        // verify
        response.EnsureSuccessStatusCode();
        var plans = await response.Content.ReadFromJsonAsync<List<TaperPlanBody>>();

        Assert.NotNull(plans);
        Assert.Contains(plans, plan => plan.PatientName == "Maria Ionescu" && plan.ReferenceNumber == "TP-0001");
        Assert.Contains(plans, plan => plan.PatientName == "Andrei Stan" && plan.ReferenceNumber == "TP-0002");
    }

    // Add your own [Fact] methods below this line.

    /**Exercise 1*/
    [Fact]
    public async Task ReturnCreatedWithALocationThatFindsThePlan()
    {
        //setup
        var patient = $"Ana Pop {Guid.NewGuid()}";
        var body = new { patientName = patient, medicationCode = "STR", startingDailyDoseMg = 40m, weekcount = 4 };
        
        //execute
        var response = await _client.SendAsync(CreateRequest(body, "lab-dev-key"));
        
        //verify
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var location = response.Headers.Location;
        Assert.NotNull(location);
        var created = await response.Content.ReadFromJsonAsync<TaperPlanBody>();
        var plan = await _client.GetFromJsonAsync<TaperPlanBody>(location);
        
        Assert.NotNull(plan);
        Assert.Equal(created!.Id, plan.Id);
        Assert.Equal(patient, plan.PatientName);
        
    }

    /**Exercise 2*/
    [Fact]
    public async Task RejectASaveWithoutAnApiKey()
    {
        //setup 
        var patient = $"No key {Guid.NewGuid()}";
        var body = new { patientName = patient, medicationCode = "STR", startingDailyDoseMg = 40m, weekcount = 4 };
        
        //execute
        var response = await _client.SendAsync(CreateRequest(body, apiKey: null)); 

        //verify the request was rejected
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var plans = await _client.GetFromJsonAsync<List<TaperPlanBody>>("/api/taper-plans");  // get plan list
        Assert.NotNull(plans); 
        Assert.DoesNotContain(plans!, plan => plan.PatientName == patient);
    }

    /**Exercise 3**/
    [Theory]
    [InlineData("wrong-api-key")]
    [InlineData("LAB-DEV-KEY")]
    [InlineData("lab-dev-key-extra")]
    public async Task RejectAPlanWithWrongApiKey(string apiKey)
    {
        //setup
        var patient = $"Wrong key {Guid.NewGuid()}";
        var body = new { patientName = patient, medicationCode = "STR", startingDailyDoseMg = 40m, weekcount = 4 };
        
        //execute 
        var response = await _client.SendAsync(CreateRequest(body, apiKey: apiKey));

        
        //verify
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        
        var plans = await _client.GetFromJsonAsync<List<TaperPlanBody>>("api/taper-plans");
        Assert.NotNull(plans);
        Assert.DoesNotContain(plans, plan => plan.PatientName == patient);
       
    }

    /**Exercise 4**/
    [Theory]
    [InlineData ("NRV", 20.01, "Starting daily dose must not exceed 20 mg for Nervalin.")]
    [InlineData("CLM", 40.01, "Starting daily dose must not exceed 40 mg for Calmafen.")]
    [InlineData ("STR" , 80.01, "Starting daily dose must not exceed 80 mg for Steriva.")]
    public async Task RejectADoseAboveTheMedicationMaximum(string code, decimal dose, string expectedError)
    {
        //setup 
        var patient = $"Ana Pop {Guid.NewGuid()}";
        var body = new { patientName = patient, medicationCode = code, startingDailyDoseMg = dose, weekcount = 4 };
        
        //execute
        var response = await _client.SendAsync(CreateRequest(body, apiKey: "lab-dev-key"));
        
        //verify
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorBody>();
        Assert.NotNull(error);
        Assert.Equal(expectedError, error.Message);
        
        var plans = await _client.GetFromJsonAsync<List<TaperPlanBody>>("api/taper-plans");
        Assert.NotNull(plans);
        Assert.DoesNotContain(plans, plan => plan.PatientName == patient);
        
    }

    /**Exercise 5**/
    [Fact]
    public async Task NotReturnAPlanThatDoesntExist()
    {
        //setup
        var unknownId = Guid.NewGuid();
        
        //execute 
        var response = await _client.GetAsync($"/api/taper-plans/{unknownId}");
        
        //verify
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorBody>();
        Assert.NotNull(error);
        Assert.Equal("Taper plan not found.", error.Message);
        
    }
    
    /**Exercise 6**/
    [Fact]
    public async Task NotSaveAPlanOnPreview()
    {
        //setup
        var body = new { medicationCode = "CLM", startingDailyDoseMg =10.00m, weekCount = 4};
        var before = (await _client.GetFromJsonAsync<List<TaperPlanBody>>("/api/taper-plans"))!.Count;
        
        //execute 
        var response = await _client.PostAsJsonAsync("/api/taper-plans/preview", body);
        
        // verify: the preview really ran
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // verify: nothing was saved
        var after = (await _client.GetFromJsonAsync<List<TaperPlanBody>>("/api/taper-plans"))!.Count;
        Assert.Equal(before, after);
        
    }


    private sealed record ErrorBody(string Message);
    
    private sealed record TaperPlanBody(Guid Id, string ReferenceNumber, string PatientName, string MedicationName);
    
    private static HttpRequestMessage CreateRequest(object body, string? apiKey)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/taper-plans")
        {
            Content = JsonContent.Create(body),
        };
        if (apiKey is not null)
            request.Headers.Add("X-Api-Key", apiKey);
        return request;
    }
}
