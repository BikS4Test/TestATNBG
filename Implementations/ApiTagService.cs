using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

public class ApiTagService : IApiTagService
{
    private readonly IDbConnection _dbConnection;

    public ApiTagService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<string> CreateApiTag(CreateApiTagDto request)
    {
        // Step 1: Validate the request payload
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Create a new ApiTag object
        var apiTag = new ApiTag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Version = 1,
            Created = DateTime.Now
        };

        // Step 3: Insert the newly created ApiTag object to the database
        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                var sql = "INSERT INTO ApiTags (Id, Name, Version, Created) VALUES (@Id, @Name, @Version, @Created)";
                await _dbConnection.ExecuteAsync(sql, apiTag, transaction);
                transaction.Commit();
                return apiTag.Id.ToString();
            }
            catch
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }

    public async Task<ApiTag> GetApiTag(ApiTagRequestDto request)
    {
        // Step 1: Validate the request payload
        if ((request.Id == null || request.Id == Guid.Empty) && string.IsNullOrWhiteSpace(request.Name))
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Initialize an apiTag null object
        ApiTag apiTag = null;

        // Step 3: Fetch ApiTag from database
        if (request.Id != null && request.Id != Guid.Empty && string.IsNullOrWhiteSpace(request.Name))
        {
            var sql = "SELECT * FROM ApiTags WHERE Id = @Id";
            apiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(sql, new { Id = request.Id });
        }
        else if (!string.IsNullOrWhiteSpace(request.Name) && (request.Id == null || request.Id == Guid.Empty))
        {
            var sql = "SELECT * FROM ApiTags WHERE Name = @Name";
            apiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(sql, new { Name = request.Name });
        }
        else
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        return apiTag;
    }

    public async Task<string> UpdateApiTag(UpdateApiTagDto request)
    {
        // Step 1: Validate the request payload
        if (request.Id == null || request.Id == Guid.Empty)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Fetch the ApiTag from the database by Id
        var sqlSelect = "SELECT * FROM ApiTags WHERE Id = @Id";
        var apiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(sqlSelect, new { Id = request.Id });
        if (apiTag == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 3: Update the ApiTag object with the provided changes
        apiTag.Name = request.Name ?? apiTag.Name;
        apiTag.Version += 1;
        apiTag.Changed = DateTime.Now;

        // Step 4: Insert the updated ApiTag object to the database
        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                var sqlUpdate = "UPDATE ApiTags SET Name = @Name, Version = @Version, Changed = @Changed WHERE Id = @Id";
                await _dbConnection.ExecuteAsync(sqlUpdate, apiTag, transaction);
                transaction.Commit();
                return apiTag.Id.ToString();
            }
            catch
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }

    public async Task<bool> DeleteApiTag(DeleteApiTagDto request)
    {
        // Step 1: Validate the request payload
        if (request.Id == null || request.Id == Guid.Empty)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Fetch the ApiTag from the database by Id
        var sqlSelect = "SELECT * FROM ApiTags WHERE Id = @Id";
        var apiTag = await _dbConnection.QuerySingleOrDefaultAsync<ApiTag>(sqlSelect, new { Id = request.Id });
        if (apiTag == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 3: Delete the ApiTag object from the database
        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                var sqlDelete = "DELETE FROM ApiTags WHERE Id = @Id";
                await _dbConnection.ExecuteAsync(sqlDelete, new { Id = request.Id }, transaction);
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }

    public async Task<List<ApiTag>> GetListApiTag(ListApiTagRequestDto request)
    {
        // Step 1: Validate the request payload
        if (request.PageLimit <= 0 || request.PageOffset < 0)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Set default sorting values if not provided
        if (string.IsNullOrWhiteSpace(request.SortField))
        {
            request.SortField = "Id";
        }
        if (string.IsNullOrWhiteSpace(request.SortOrder))
        {
            request.SortOrder = "asc";
        }

        // Step 3: Fetch the list of ApiTags from the database
        var sql = $"SELECT * FROM ApiTags ORDER BY {request.SortField} {request.SortOrder} OFFSET @PageOffset ROWS FETCH NEXT @PageLimit ROWS ONLY";
        var apiTags = await _dbConnection.QueryAsync<ApiTag>(sql, new { PageOffset = request.PageOffset, PageLimit = request.PageLimit });

        return apiTags.ToList();
    }
}
