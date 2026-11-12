using System.Net;
using System.Net.Http.Json;
using TaskManagementApi.DTOs;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Tests
{
    public class TaskApiTests : IClassFixture<ApiFactory>
    {

        private readonly HttpClient _client;

        public TaskApiTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        //helper static
        private static object ValidBody(string title = "Test task", int projectId = 1, int status = 0) => new
        {
            title,
            description = "Test description",
            status,
            priority = 1,
            dueDate = new DateTime(2030, 1, 1),
            projectId
        };

        private async Task<TaskResponseDto> CreateTaskAsync(string title = "Test task", int status = 0)
        {
            var post = await _client.PostAsJsonAsync("/api/tasks", ValidBody(title, status: status));

            Assert.True(post.StatusCode == HttpStatusCode.Created,
                $"POST returned {(int)post.StatusCode}: {await post.Content.ReadAsStringAsync()}");

            var created = await post.Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(created);
            return created;
        }


        // Testes

        [Fact]
        public async Task GetTasks_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/tasks");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostTask_ValidBody_ReturnsCreatedAndTaskCanBeFetched()
        {
            var post = await _client.PostAsJsonAsync("/api/tasks", ValidBody());

            Assert.Equal(HttpStatusCode.Created, post.StatusCode);
            Assert.NotNull(post.Headers.Location);

            var created = await post.Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(created);

            var get = await _client.GetAsync(post.Headers.Location);
            Assert.Equal(HttpStatusCode.OK, get.StatusCode);

            var fetched = await get.Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);
        }


        [Theory]
        [InlineData("", 1)]
        [InlineData("   ", 1)]
        [InlineData("Valid", 999)]
        public async Task PostTask_InvalidBody_ReturnsBadRequest(string title, int projectId)
        {
            var response = await _client.PostAsJsonAsync("/api/tasks", ValidBody(title, projectId));

            Assert.True(response.StatusCode == HttpStatusCode.BadRequest,
                $"Expected 400 but got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        public async Task UnknownId_ReturnsNotFound(string method)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/tasks/999999");

            if (method == "PUT")
                request.Content = JsonContent.Create(ValidBody());

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTask_Existing_ReturnsNoContentAndTaskIsGone()
        {
            var created = await CreateTaskAsync();

            var delete = await _client.DeleteAsync($"/api/tasks/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

            var get = await _client.GetAsync($"/api/tasks/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        }

        [Fact]
        public async Task PutTask_Existing_ReturnsOkAndChangesAreSaved()
        {
            var created = await CreateTaskAsync();

            var put = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}", ValidBody("New title"));
            Assert.Equal(HttpStatusCode.OK, put.StatusCode);

            var get = await _client.GetAsync($"/api/tasks/{created.Id}");
            Assert.Equal (HttpStatusCode.OK, get.StatusCode);

            var fetched = await get.Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(fetched);
            Assert.Equal("New title", fetched.Title);
        }

        [Fact]
        public async Task PostTask_MissingDueDate_ReturnsBadRequest()
        {
            var body = new
            {
                title = "No due date",
                description = "Test",
                status = 0,
                priority = 1,
                projectId = 1
                //due date not sent
            };

            var response = await _client.PostAsJsonAsync("/api/tasks", body);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetTasks_PageSizeOver100_IsCappedAt100()
        {
            var response = await _client.GetAsync("/api/tasks?pageSize=1000");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var page = await response.Content.ReadFromJsonAsync<PagedResult<TaskResponseDto>>();
            Assert.NotNull(page);
            Assert.Equal(100, page.PageSize);
        }

        [Fact]
        public async Task GetTasks_StatusFilter_ReturnsOnlyMatchingTasks()
        {
            var created = await CreateTaskAsync("Completed one", status: 2);

            var response = await _client.GetAsync("/api/tasks?status=Completed&pageSize=100");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var page = await response.Content.ReadFromJsonAsync<PagedResult<TaskResponseDto>>();
            Assert.NotNull(page);
            Assert.All(page.Items, item => Assert.Equal(TaskItemStatus.Completed, item.Status));
            Assert.Contains(page.Items, item => item.Id == created.Id);
              
        }
    }
}
