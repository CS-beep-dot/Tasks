using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagementApi.Application.DTOs;
using TaskManagementApi.Application.Services;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Tests
{
    public class ErrorResponseTests : IClassFixture<ApiFactory>
    {
        private const string ProblemJson = "application/problem+json";

        private readonly ApiFactory _factory;
        private readonly HttpClient _client;

        public ErrorResponseTests(ApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TestTokens.Create(factory, userId: 1, role: "Manager"));
        }

        private static object Body(
            string title = "Test task",
            int projectId = 1,
            int status = 0,
            int priority = 1,
            DateTime? dueDate = null) => new
            {
                title,
                description = "Test description",
                status,
                priority,
                dueDate = dueDate ?? new DateTime(2030, 1, 1),
                projectId
            };

        // Reads the body as raw JSON and asserts it is ProblemDetails with the expected status.
        private static async Task<JsonElement> AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expected)
        {
            var raw = await response.Content.ReadAsStringAsync();

            Assert.True(response.StatusCode == expected,
                $"Expected {(int)expected} but got {(int)response.StatusCode}: {raw}");
            Assert.Equal(ProblemJson, response.Content.Headers.ContentType?.MediaType);

            var json = JsonDocument.Parse(raw).RootElement;
            Assert.Equal((int)expected, json.GetProperty("status").GetInt32());
            Assert.True(json.TryGetProperty("title", out _), "ProblemDetails must have a title.");
            Assert.True(json.TryGetProperty("traceId", out _), "ProblemDetails must have a traceId.");
            return json;
        }

        // ---------- 400: validation ----------

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task PostTask_EmptyTitle_Returns400ProblemWithTitleError(string title)
        {
            var response = await _client.PostAsJsonAsync("/api/tasks", Body(title: title));

            var json = await AssertProblemAsync(response, HttpStatusCode.BadRequest);
            Assert.True(json.GetProperty("errors").TryGetProperty("Title", out _),
                $"Expected an error for Title: {json}");
        }

        [Fact]
        public async Task PostTask_TitleOf150Chars_IsAccepted()
        {
            var response = await _client.PostAsJsonAsync("/api/tasks", Body(title: new string('a', 150)));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PostTask_TitleOf151Chars_Returns400()
        {
            var response = await _client.PostAsJsonAsync("/api/tasks", Body(title: new string('a', 151)));

            var json = await AssertProblemAsync(response, HttpStatusCode.BadRequest);
            Assert.True(json.GetProperty("errors").TryGetProperty("Title", out _));
        }

        [Theory]
        [InlineData(99, 1, "Status")]
        [InlineData(0, 99, "Priority")]
        public async Task PostTask_EnumOutOfRange_Returns400(int status, int priority, string field)
        {
            var response = await _client.PostAsJsonAsync("/api/tasks", Body(status: status, priority: priority));

            var json = await AssertProblemAsync(response, HttpStatusCode.BadRequest);
            Assert.True(json.GetProperty("errors").TryGetProperty(field, out _),
                $"Expected an error for {field}: {json}");
        }

        [Fact]
        public async Task PostTask_LowestEnumValues_AreAccepted()
        {
            // Pending = 0 and Low = 0 must not be rejected (NotEmpty on enums would do that).
            var response = await _client.PostAsJsonAsync("/api/tasks",
                Body(status: (int)TaskItemStatus.Pending, priority: (int)TaskPriority.Low));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        // ---------- 400: malformed request ----------

        [Fact]
        public async Task PostTask_MalformedJson_Returns400Problem()
        {
            var content = new StringContent("{ \"title\": \"oops\", ", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/tasks", content);

            await AssertProblemAsync(response, HttpStatusCode.BadRequest);
        }

        // ---------- 400: invalid reference (thrown by service, mapped by GlobalExceptionHandler) ----------

        [Fact]
        public async Task PostTask_UnknownProject_Returns400ProblemWithDetail()
        {
            var response = await _client.PostAsJsonAsync("/api/tasks", Body(projectId: 999));

            var json = await AssertProblemAsync(response, HttpStatusCode.BadRequest);
            Assert.Contains("Project 999", json.GetProperty("detail").GetString());
        }

        [Fact]
        public async Task PutTask_UnknownProject_Returns400Problem()
        {
            var created = await (await _client.PostAsJsonAsync("/api/tasks", Body()))
                .Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(created);

            var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}", Body(projectId: 999));

            var json = await AssertProblemAsync(response, HttpStatusCode.BadRequest);
            Assert.Contains("Project 999", json.GetProperty("detail").GetString());
        }

        // ---------- 404 ----------

        [Theory]
        [InlineData("GET")]
        [InlineData("PUT")]
        public async Task UnknownTask_Returns404Problem(string method)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/tasks/999999");
            if (method == "PUT")
                request.Content = JsonContent.Create(Body());

            var response = await _client.SendAsync(request);

            await AssertProblemAsync(response, HttpStatusCode.NotFound);
        }

        // ---------- 500: no leaking in Production ----------

        [Fact]
        public async Task UnhandledException_InProduction_Returns500WithoutLeakingDetails()
        {
            var factory = _factory.WithWebHostBuilder(b =>
            {
                b.UseEnvironment("Production");
                b.ConfigureTestServices(s =>
                {
                    s.RemoveAll<ITaskService>();
                    s.AddScoped<ITaskService, ThrowingTaskService>();
                });
            });

            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TestTokens.Create(_factory, userId: 1, role: "Manager"));

            var response = await client.GetAsync("/api/tasks/1");
            var raw = await response.Content.ReadAsStringAsync();

            var json = await AssertProblemAsync(response, HttpStatusCode.InternalServerError);
            Assert.DoesNotContain(ThrowingTaskService.SecretMessage, raw);
            Assert.DoesNotContain("   at ", raw);                     // stack trace line
            Assert.DoesNotContain(nameof(InvalidOperationException), raw); // exception type name
            Assert.False(json.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String,
                $"Production 500 must not include detail: {raw}");
        }

        private sealed class ThrowingTaskService : ITaskService
        {
            public const string SecretMessage = "secret db failure: Server=prod-sql;Password=hunter2";

            private static Exception Boom() => new InvalidOperationException(SecretMessage);

            public Task<PagedResult<TaskResponseDto>> GetTasks(
                TaskItemStatus? status = null,
                int? assigneeId = null,
                TaskPriority? priority = null,
                int? projectId = null,
                string? search = null,
                string? sort = null,
                int page = 1,
                int pageSize = 20) => throw Boom();
            public Task<TaskResponseDto?> GetTaskById(int id) => throw Boom();
            public Task<TaskResponseDto?> UpdateTask(int id, UpdateTaskDto taskItem) => throw Boom();
            public Task<TaskResponseDto> CreateTask(CreateTaskDto createTask) => throw Boom();
            public Task<TaskResponseDto?> AssignTask(int id, int? assigneeId) => throw Boom();
            public Task<bool> DeleteTask(int id) => throw Boom();
        }
    }
}
