namespace DemoAPI;

public class MyEndpoints
{
	public void ConfigureMyEndpoints(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/");

		group.MapGet("weatherForecast", GetWeatherForecast)
			.WithTags("Forecast")
			.WithSummary("Get Weather Forecast")
			.WithDescription("Next 5 days weather forecast.")
			.Produces<WeatherForecast>()
			.Produces(400)
			.Produces<ProblemDetails>(500);

		group.MapGet("devInfo", GetDevInfo)
			.WithTags("Developer")
			.WithSummary("Developer Info")
			.WithDescription("This endpoint shows the name of the developer of this application.")
			.Produces<DeveloperInfo>()
			.Produces(400);

		group.MapGet("helloWorld", GetHelloWorld)
			.Produces<string>()
			.ProducesProblem(400);

		group.MapGet("someData", GetSomeData)
			.Produces<SomeData>()
			.Produces(400);
	}

	private Results<Ok<WeatherForecast[]>, BadRequest, ValidationProblem> GetWeatherForecast()
	{
		string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

		try
		{
			var forecast = Enumerable.Range(1, 5).Select(index =>
					new WeatherForecast
					(
						DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
						Random.Shared.Next(-20, 55),
						summaries[Random.Shared.Next(summaries.Length)]
					))
				.ToArray();

			var random = Random.Shared.Next(2);

			if (random == 0)
				throw new Exception("A custom exception occurred!");

			return forecast.Length != 0
				? TypedResults.Ok(forecast)
				: TypedResults.BadRequest();
		}
		catch (Exception ex)
		{
			ProblemDetails problemDetails = new()
			{
				Title = "An error occured.",
				Detail = ex.Message,
				Status = (int)HttpStatusCode.BadRequest,
				Instance = "GET /weatherforecast",
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			};

			return TypedResults.ValidationProblem([], problemDetails.Detail, problemDetails.Instance, problemDetails.Title, problemDetails.Type);
		}
	}

	private Results<Ok<DeveloperInfo>, BadRequest, ValidationProblem> GetDevInfo(HttpContext context, [FromHeader(Name = "X-Test-Header")] string requestHeader)
	{
		// var requestHeader = context.Request.Headers["X-Test-Header"].ToString();

		if (string.IsNullOrEmpty(requestHeader))
		{
			ProblemDetails problemDetails = new()
			{
				Title = "Bad Request.",
				Detail = "Expected a request header X-Test-Header value.",
				Status = (int)HttpStatusCode.BadRequest,
				Instance = "GET /devinfo",
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			};

			return TypedResults.ValidationProblem([], problemDetails.Detail, problemDetails.Instance, problemDetails.Title, problemDetails.Type);
		}

		DeveloperInfo response = new("Jiten", "Shahani", requestHeader);

		context.Response.Headers.Append("X-Sample-Header", response.ToString());
		context.Response.Headers["X-Another-Header"] = "Hello, World!";

		return
			!string.IsNullOrEmpty(response.FirstName) &&
			!string.IsNullOrEmpty(response.LastName) &&
			!string.IsNullOrEmpty(response.RequestHeaderValue)
				? TypedResults.Ok(response)
				: TypedResults.BadRequest();
	}

	private Results<Ok<string>, ValidationProblem> GetHelloWorld([FromQuery] string? request)
	{
		if (string.IsNullOrEmpty(request))
		{
			ProblemDetails problemDetails = new()
			{
				Title = "Invalid request",
				Detail = "The request parameter cannot be null or empty.",
				Status = (int)HttpStatusCode.BadRequest,
				Instance = $"GET /helloWorld?request={request}",
				Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
			};

			return TypedResults.ValidationProblem([], problemDetails.Detail, problemDetails.Instance, problemDetails.Title, problemDetails.Type);
		}

		return TypedResults.Ok($"Hello, {request}");
	}

	private Results<Ok<SomeData>, BadRequest> GetSomeData()
	{
		SomeData someData = new("John", "Doe");

		return
			string.IsNullOrEmpty(someData.FirstName) &&
			string.IsNullOrEmpty(someData.LastName)
				? TypedResults.BadRequest()
				: TypedResults.Ok(someData);
	}
}

record SomeData(string FirstName, string LastName);