CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [Nom] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [ProfilePictureUrl] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Matricule] nvarchar(max) NOT NULL,
    [Poste] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [SoldeConges] int NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [SalaryBase] decimal(18,2) NOT NULL,
    [HireDate] datetime2 NOT NULL,
    [ContractType] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [Department] nvarchar(max) NOT NULL,
    [ManagerId] int NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_Employees_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [Message] nvarchar(max) NOT NULL,
    [Date] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    [EmployeeId] int NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [DocumentRequests] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [DocumentType] nvarchar(max) NOT NULL,
    [RequestDate] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_DocumentRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DocumentRequests_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Leaves] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [RejectionReason] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ApprovedAt] datetime2 NULL,
    [ApprovedById] nvarchar(max) NULL,
    [RejectedAt] datetime2 NULL,
    [RejectedById] nvarchar(max) NULL,
    [CancelledAt] datetime2 NULL,
    [CancelledById] nvarchar(max) NULL,
    CONSTRAINT [PK_Leaves] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Leaves_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Teleworks] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Motif] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [RejectionReason] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ApprovedAt] datetime2 NULL,
    [ApprovedById] nvarchar(max) NULL,
    [RejectedAt] datetime2 NULL,
    [RejectedById] nvarchar(max) NULL,
    [CancelledAt] datetime2 NULL,
    [CancelledById] nvarchar(max) NULL,
    CONSTRAINT [PK_Teleworks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Teleworks_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);
GO


CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO


CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO


CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO


CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO


CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO


CREATE INDEX [IX_DocumentRequests_EmployeeId] ON [DocumentRequests] ([EmployeeId]);
GO


CREATE INDEX [IX_Employees_ManagerId] ON [Employees] ([ManagerId]);
GO


CREATE INDEX [IX_Leaves_EmployeeId] ON [Leaves] ([EmployeeId]);
GO


CREATE INDEX [IX_Teleworks_EmployeeId] ON [Teleworks] ([EmployeeId]);
GO


