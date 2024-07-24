
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class ApiTagService : IApiTagService
    {
        private readonly IDbConnection _dbConnection;

        public ApiTagService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateApiTag(CreateApiTagDto request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var existingTag = await _dbConnection.QueryFirstOrDefaultAsync<ApiTag>(
                "SELECT * FROM APITags WHERE Name = @Name", new { request.Name });

            if (existingTag != null)
            {
                return existingTag.Id.ToString();
            }

            var newApiTag = new ApiTag
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Version = 1,
                Created = DateTime.Now,
                CreatorId = request.CreatorId
            };

            var result = await _dbConnection.ExecuteAsync(
                "INSERT INTO APITags (Id, Name, Version, Created, CreatorId) VALUES (@Id, @Name, @Version, @Created, @CreatorId)",
                newApiTag);

            if (result == 1)
            {
                return newApiTag.Id.ToString();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<ApiTag> GetApiTag(ApiTagRequestDto request)
        {
            if (request.Id == null && string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            ApiTag apiTag = null;

            if (request.Id != null)
            {
                apiTag = await _dbConnection.QueryFirstOrDefaultAsync<ApiTag>(
                    "SELECT * FROM ApiTags WHERE Id = @Id", new { request.Id });
            }
            else if (!string.IsNullOrEmpty(request.Name))
            {
                apiTag = await _dbConnection.QueryFirstOrDefaultAsync<ApiTag>(
                    "SELECT * FROM ApiTags WHERE Name = @Name", new { request.Name });
            }

            return apiTag;
        }

        public async Task<string> UpdateApiTag(UpdateApiTagDto request)
        {
            if (request.Id == null || string.IsNullOrEmpty(request.Name) || request.ChangedUser == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var apiTag = await _dbConnection.QueryFirstOrDefaultAsync<ApiTag>(
                "SELECT * FROM ApiTags WHERE Id = @Id", new { request.Id });

            if (apiTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            apiTag.Name = request.Name;
            apiTag.Version += 1;
            apiTag.Changed = DateTime.Now;
            apiTag.ChangedUser = request.ChangedUser;

            var result = await _dbConnection.ExecuteAsync(
                "UPDATE ApiTags SET Name = @Name, Version = @Version, Changed = @Changed, ChangedUser = @ChangedUser WHERE Id = @Id",
                apiTag);

            if (result == 1)
            {
                return apiTag.Id.ToString();
            }
            else
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }

        public async Task<bool> DeleteApiTag(DeleteApiTagDto request)
        {
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var apiTag = await _dbConnection.QueryFirstOrDefaultAsync<ApiTag>(
                "SELECT * FROM ApiTags WHERE Id = @Id", new { request.Id });

            if (apiTag == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            var result = await _dbConnection.ExecuteAsync(
                "DELETE FROM ApiTags WHERE Id = @Id", new { request.Id });

            return result == 1;
        }

        public async Task<List<ApiTag>> GetListApiTag(ListApiTagRequestDto request)
        {
            if (request.PageLimit <= 0 || request.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            if (string.IsNullOrEmpty(request.SortField) || string.IsNullOrEmpty(request.SortOrder))
            {
                request.SortField = "Id";
                request.SortOrder = "asc";
            }

            var apiTags = await _dbConnection.QueryAsync<ApiTag>(
                $"SELECT * FROM ApiTags ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY",
                new { request.PageOffset, request.PageLimit });

            return apiTags.AsList();
        }
    }
}
