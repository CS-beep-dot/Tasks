using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementApi.Domain.Entities;
using TaskManagementApi.Infrastructure.Persistence;

namespace TaskManagementApi.Tests
{
    public class ProjectAuthTests : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;
        private readonly HttpClient _client;
        private readonly AuthenticationHeaderValue _adminAuth;

        public ProjectAuthTests(ApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TestTokens.Create(factory, 1, "Manager"));

            _adminAuth = new AuthenticationHeaderValue("Bearer", TestTokens.Create(factory, 1, "Admin"));
        }
        private async Task<int> CreateProjectInDbAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var project = new Project
            {
                Name = $"Temp {Guid.NewGuid():N}",
                Description = "Created by a test",
                CreatedAt = DateTime.UtcNow
            };

            db.Projects.Add(project);
            await db.SaveChangesAsync();
            return project.Id;
        }


        private Task<HttpResponseMessage> DeleteAsAdminAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Authorization = _adminAuth;
            return _client.SendAsync(request);
        }

        [Fact]
        public async Task DeleteProject_AsAdmin_ReturnsNoContent()
        {
            var projectId = await CreateProjectInDbAsync();

            var response = await DeleteAsAdminAsync($"/api/projects/{projectId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProject_AsManager_ReturnsForbidden()
        {
            var projectId = await CreateProjectInDbAsync();

            var response = await _client.DeleteAsync($"/api/projects/{projectId}");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            
        }

        [Fact]
        public async Task DeleteProject_UnknownId_AsAdmin_ReturnsNotFound()
        {
            var response = await DeleteAsAdminAsync("api/projects/9999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProject_WithoutToken_ReturnsUnauthorized()
        {
            var anonymousClient = _factory.CreateClient();

            var response = await anonymousClient.DeleteAsync("/api/projects/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }
    }
}