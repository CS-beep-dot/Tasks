using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManagementApi.Application.DTOs;

namespace TaskManagementApi.Tests
{
    // Manager/Admin can update any task, a plain user only tasks assigned to them.            (Policy "CanUpdateTasks")
    public class TaskUpdateAuthTests : IClassFixture<ApiFactory>
    {
        // Seeded users: 1 = Nino, 2 = Giorgi
        private const int OwnerUserId = 2;
        private const int OtherUserId = 1;

        private readonly HttpClient _managerClient;
        private readonly HttpClient _ownerClient;
        private readonly HttpClient _otherUserClient;
        private readonly HttpClient _anonymousClient;

        public TaskUpdateAuthTests(ApiFactory factory)
        {
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

        // Manager creates a task, assigns to owner. Returns new taskid.
        private async Task<int> CreateTaskAssignedToOwnerAsync()
        {
            var post = await _managerClient.PostAsJsonAsync("/api/tasks", TaskBody("Original title"));
            Assert.Equal(HttpStatusCode.Created, post.StatusCode);

            var created = await post.Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(created);

            var assign = await _managerClient.PutAsJsonAsync(
                $"/api/tasks/{created.Id}/assignee", new { assigneeId = OwnerUserId });
            Assert.Equal(HttpStatusCode.OK, assign.StatusCode);

            return created.Id;
        }

        [Fact]
        public async Task Owner_CanUpdateOwnTask_Returns200()
        {
            var taskId = await CreateTaskAssignedToOwnerAsync();

            var response = await _ownerClient.PutAsJsonAsync($"/api/tasks/{taskId}", TaskBody("Updated by owner"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task OtherUser_CannotUpdateTask_Returns403()
        {
            var taskId = await CreateTaskAssignedToOwnerAsync();

            var response = await _otherUserClient.PutAsJsonAsync($"/api/tasks/{taskId}", TaskBody("Updated by other user"));

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Manager_CanUpdateAnyTask_Returns200()
        {
            var taskId = await CreateTaskAssignedToOwnerAsync();

            var response = await _managerClient.PutAsJsonAsync($"/api/tasks/{taskId}", TaskBody("Updated by manager"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task NoToken_Returns401()
        {
            var taskId = await CreateTaskAssignedToOwnerAsync();

            var response = await _anonymousClient.PutAsJsonAsync($"/api/tasks/{taskId}", TaskBody("Updated anonymously"));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UnknownTask_Returns404()
        {
            var response = await _ownerClient.PutAsJsonAsync("/api/tasks/999999", TaskBody("Does not exist"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
