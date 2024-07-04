
CREATE TABLE Attachments (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    FileName NVARCHAR(100) NOT NULL UNIQUE,
    FileUrl VARBINARY(MAX) NOT NULL,
    Timestamp DATETIME NOT NULL
);
