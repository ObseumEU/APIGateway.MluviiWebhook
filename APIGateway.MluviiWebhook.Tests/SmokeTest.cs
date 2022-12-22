namespace APIGateway.MluviiWebhook.Tests;

public class SmokeTest : IClassFixture<ApplicationFixture> {
    public SmokeTest(ApplicationFixture fixture)
    {
        this.fixture = fixture;
    }

    readonly ApplicationFixture fixture;
    private TestServer Server => fixture.Server;

    [Fact]
    public async Task Ensure_that_application_can_start_and_answer_on_HTTP_request()
    {
        var req = Server.CreateRequest("/test");
        var res = await req.GetAsync();

        res.StatusCode.Should().Be(HttpStatusCode.OK);
        (await res.Content.ReadAsStringAsync()).Should().Be("Ok");
    }
}
