
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ProjectName.ControllersExceptions;
using ProjectName.Interfaces;
using ProjectName.Types;

namespace ProjectName.Services
{
    public class APIEndpointService : IAPIEndpointService
    {
        private readonly IDbConnection _dbConnection;

        public APIEndpointService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateAPIEndpoint(CreateAPIEndpointDto request)
        {
            // Step 1: Validate Fields
            if (string.IsNullOrEmpty(request.Name) || request.Deprecated == null)
            {
                throw new BusinessException("DP-422", "Mandatory fields are missing.");
            }

            // Step 2: Fetch Tags
            var tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Name IN @Names", new { Names = request.ApiTags });
            if (tags.Count() != request.ApiTags.Count)
            {
                throw new BusinessException("DP-404", "One or more tags do not exist.");
            }

            // Step 3: Create APIEndpoint Object
            var apiEndpoint = new APIEndpoint
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ApiContext = request.ApiContext,
                ApiReferenceId = request.ApiReferenceId,
                ApiResource = request.ApiResource,
                ApiScope = request.ApiScope,
                ApiScopeProduction = request.ApiScopeProduction,
                ApiSecurity = request.ApiSecurity,
                Deprecated = request.Deprecated,
                Description = request.Description,
                Documentation = request.Documentation,
                EndpointUrls = request.EndpointUrls,
                EnvironmentId = request.EnvironmentId,
                ProviderId = request.ProviderId,
                Swagger = request.Swagger,
                Tour = request.Tour,
                Updated = DateTime.UtcNow,
                Version = request.Version
            };

            // Step 4: Create APIEndpointTags List
            var apiEndpointTags = tags.Select(tag => new APIEndpointTag
            {
                Id = Guid.NewGuid(),
                APIEndpointId = apiEndpoint.Id,
                APITagId = tag.Id
            }).ToList();

            // Step 5: Database Transaction
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert the apiEndpoint object into the APIEndpoints database table
                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpoints (Id, Name, ApiContext, ApiReferenceId, ApiResource, ApiScope, ApiScopeProduction, ApiSecurity, Deprecated, Description, Documentation, EndpointUrls, EnvironmentId, ProviderId, Swagger, Tour, Updated, Version) VALUES (@Id, @Name, @ApiContext, @ApiReferenceId, @ApiResource, @ApiScope, @ApiScopeProduction, @ApiSecurity, @Deprecated, @Description, @Documentation, @EndpointUrls, @EnvironmentId, @ProviderId, @Swagger, @Tour, @Updated, @Version)", apiEndpoint, transaction);

                    // Insert the related APIEndpointTags into the respective database table
                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (Id, APIEndpointId, APITagId) VALUES (@Id, @APIEndpointId, @APITagId)", apiEndpointTags, transaction);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");
                }
            }

            // Step 7: Return APIEndpointId
            return apiEndpoint.Id.ToString();
        }

        public async Task<APIEndpoint> GetAPIEndpoint(APIEndpointRequestDto request)
        {
            // Step 1: Validate Input
            if (request.Id == null && string.IsNullOrEmpty(request.Name))
            {
                throw new BusinessException("DP-422", "Either Id or Name must be provided.");
            }

            APIEndpoint apiEndpoint;

            // Step 3: Fetch API Endpoint
            if (request.Id != null)
            {
                apiEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            }
            else
            {
                apiEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Name = @Name", new { Name = request.Name });
            }

            // Step 4: Fetch Associated Tags
            if (apiEndpoint != null)
            {
                var tagIds = await _dbConnection.QueryAsync<Guid>("SELECT APITagId FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = apiEndpoint.Id });
                var tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Id IN @Ids", new { Ids = tagIds });

                if (tags.Count() != tagIds.Count())
                {
                    throw new BusinessException("DP-404", "One or more tags do not exist.");
                }

                apiEndpoint.ApiTags = tags.ToList();
            }
            else
            {
                throw new BusinessException("DP-404", "API endpoint not found.");
            }

            return apiEndpoint;
        }

        public async Task<string> UpdateAPIEndpoint(UpdateAPIEndpointDto request)
        {
            // Step 1: Validate Necessary Parameters
            if (request.Id == null || string.IsNullOrEmpty(request.Name) || request.Deprecated == null || string.IsNullOrEmpty(request.Version))
            {
                throw new BusinessException("DP-422", "Mandatory fields are missing.");
            }

            // Step 2: Fetch Existing API Endpoint
            var existingEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            if (existingEndpoint == null)
            {
                throw new BusinessException("DP-404", "API endpoint not found.");
            }

            // Step 3: Update the APIEndpoint object with the provided changes
            existingEndpoint.Name = request.Name;
            existingEndpoint.ApiContext = request.ApiContext;
            existingEndpoint.ApiReferenceId = request.ApiReferenceId;
            existingEndpoint.ApiResource = request.ApiResource;
            existingEndpoint.ApiScope = request.ApiScope;
            existingEndpoint.ApiScopeProduction = request.ApiScopeProduction;
            existingEndpoint.ApiSecurity = request.ApiSecurity;
            existingEndpoint.Deprecated = request.Deprecated;
            existingEndpoint.Description = request.Description;
            existingEndpoint.Documentation = request.Documentation;
            existingEndpoint.EndpointUrls = request.EndpointUrls;
            existingEndpoint.EnvironmentId = request.EnvironmentId;
            existingEndpoint.ProviderId = request.ProviderId;
            existingEndpoint.Swagger = request.Swagger;
            existingEndpoint.Tour = request.Tour;
            existingEndpoint.Updated = DateTime.UtcNow;
            existingEndpoint.Version = request.Version;

            // Step 4: Update APIEndpointTags
            var tags = await _dbConnection.QueryAsync<ApiTag>("SELECT * FROM ApiTags WHERE Name IN @Names", new { Names = request.ApiTags });
            if (tags.Count() != request.ApiTags.Count)
            {
                throw new BusinessException("DP-422", "One or more tags do not exist.");
            }

            var apiEndpointTags = tags.Select(tag => new APIEndpointTag
            {
                Id = Guid.NewGuid(),
                APIEndpointId = existingEndpoint.Id,
                APITagId = tag.Id
            }).ToList();

            // Step 5: Save Changes to Database
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try:
                    # Update APIEndpoints Table
                    await _dbConnection.ExecuteAsync("UPDATE APIEndpoints SET Name = @Name, ApiContext = @ApiContext, ApiReferenceId = @ApiReferenceId, ApiResource = @ApiResource, ApiScope = @ApiScope, ApiScopeProduction = @ApiScopeProduction, ApiSecurity = @ApiSecurity, Deprecated = @Deprecated, Description = @Description, Documentation = @Documentation, EndpointUrls = @EndpointUrls, EnvironmentId = @EnvironmentId, ProviderId = @ProviderId, Swagger = @Swagger, Tour = @Tour, Updated = @Updated, Version = @Version WHERE Id = @Id", existingEndpoint, transaction);

                    # Remove all the old tags associated with the API endpoint from the APIEndpointTags table
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingEndpoint.Id }, transaction);

                    # Insert the new tags into the APIEndpointTags table
                    await _dbConnection.ExecuteAsync("INSERT INTO APIEndpointTags (Id, APIEndpointId, APITagId) VALUES (@Id, @APIEndpointId, @APITagId)", apiEndpointTags, transaction);

                    transaction.Commit();
                except Exception:
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");

            return existingEndpoint.Id.ToString();
        }

        public async Task<bool> DeleteAPIEndpoint(DeleteAPIEndpointDto request)
        {
            # Step 1: Validate Input
            if (request.Id == null)
            {
                throw new BusinessException("DP-422", "Id is missing.");
            }

            # Step 2: Fetch Existing API Endpoint
            var existingEndpoint = await _dbConnection.QuerySingleOrDefaultAsync<APIEndpoint>("SELECT * FROM APIEndpoints WHERE Id = @Id", new { Id = request.Id });
            if (existingEndpoint == null)
            {
                throw new BusinessException("DP-404", "API endpoint not found.");
            }

            # Step 3: Delete API Endpoint
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try:
                    # Delete from APIEndpointTags table
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpointTags WHERE APIEndpointId = @APIEndpointId", new { APIEndpointId = existingEndpoint.Id }, transaction);

                    # Delete from APIEndpoints table
                    await _dbConnection.ExecuteAsync("DELETE FROM APIEndpoints WHERE Id = @Id", new { Id = existingEndpoint.Id }, transaction);

                    transaction.Commit();
                except Exception:
                    transaction.Rollback();
                    throw new TechnicalException("DP-500", "A technical exception has occurred, please contact your system administrator.");

            return True;
        }
    }
}
