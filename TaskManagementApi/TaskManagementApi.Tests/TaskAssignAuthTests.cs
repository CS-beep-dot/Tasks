using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManagementApi.Application.DTOs;

namespace TaskManagementApi.Tests
{
    public class TaskAssignAuthTests : IClassFixture<ApiFactory>
    {

        private const int OwnerUserId = 2;
        private const int OtherUserId = 1;

        private readonly HttpClient _adminClient;
        private readonly HttpClient _managerClient;
        private readonly HttpClient _ownerClient;
        private readonly HttpClient _otherUserClient;
        private readonly HttpClient _anonymousClient;

        public TaskAssignAuthTests(ApiFactory factory)
        {
            _adminClient = factory.CreateClient();
            _adminClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TestTokens.Create(factory, OtherUserId, "Admin"));

            _managerClient = factory.CreateClient();
            _managerClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TestTokens.Create(factory, 1, "Manager"));

            _ownerClient = factory.CreateClient();
            _ownerClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TestTokens.Create(factory, OwnerUserId, "User"));

            _otherUserClient = factory.CreateClient();
            _otherUserClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TestTokens.Create(factory, OtherUserId, "User"));

            _anonymousClient = factory.CreateClient();
        }

        private static object TaskBody(string title) => new
        {
            title,
            description = "Test description",
            status = 0,
            priority = 1,
            dueDate = new DateTime(2030, 1, 1),
            projectId = 1
        };

        // Manager creates an unassigned task. Returns the new task id.
        private async Task<int> CreateTaskAsync()
        {
            var post = await _managerClient.PostAsJsonAsync("/api/tasks", TaskBody("Task to assign"));
            Assert.Equal(HttpStatusCode.Created, post.StatusCode);

            var created = await post.Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(created);

            return created.Id;
        }

        [Fact]
        public async Task Manager_Assigns_Returns200()
        {
            var taskId = await CreateTaskAsync();

            var response = await _managerClient.PutAsJsonAsync(
                $"/api/tasks/{taskId}/assignee", new { assigneeId = OwnerUserId });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var task = await response.Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(task);
            Assert.Equal(OwnerUserId, task.AssigneeId);
        }

        [Fact]
        public async Task Admin_Assigns_Returns200()
        {
            var taskId = await CreateTaskAsync();

            var response = await _adminClient.PutAsJsonAsync(
                $"/api/tasks/{taskId}/assignee", new { assigneeId = OwnerUserId });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var task = await response.Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(task);
            Assert.Equal(OwnerUserId, task.AssigneeId);
        }

        [Fact]
        public async Task User_Assigns_Returns403()
        {
            var taskId = await CreateTaskAsync();

            var response = await _ownerClient.PutAsJsonAsync(
                $"/api/tasks/{taskId}/assignee", new { assigneeId = OwnerUserId });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task NoToken_Assigns_Returns401()
        {
            var taskId = await CreateTaskAsync();

            var response = await _anonymousClient.PutAsJsonAsync(
                $"/api/tasks/{taskId}/assignee", new { assigneeId = OwnerUserId });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UnknownTask_Returns404()
        {
            var response = await _managerClient.PutAsJsonAsync(
                "/api/tasks/999999/assignee", new { assigneeId = OwnerUserId });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UnknownUser_Returns400()
        {
            var taskId = await CreateTaskAsync();

            var response = await _managerClient.PutAsJsonAsync(
                $"/api/tasks/{taskId}/assignee", new { assigneeId = 99999 });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
