
CREATE TABLE APIEndpoints (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    ApiContext nvarchar(200),
    ApiReferenceId nvarchar(200),
    ApiResource nvarchar(max),
    ApiScope nvarchar(200),
    ApiScopeProduction nvarchar(200),
    ApiSecurity nvarchar(max),
    ApiTags uniqueidentifier,
    Deprecated bit NOT NULL,
    Description nvarchar(max),
    Documentation nvarchar(max),
    EndpointUrls nvarchar(200),
    EnvironmentId nvarchar(200),
    ProviderId nvarchar(200),
    Swagger nvarchar(max),
    Tour nvarchar(max),
    Updated timestamp,
    Version nvarchar(200)
);
