IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Role] (
    [RoleID] int NOT NULL IDENTITY,
    [RoleName] varchar(10) NOT NULL,
    CONSTRAINT [PK__Role__8AFACE3A73EBFAA9] PRIMARY KEY ([RoleID])
);
GO

CREATE TABLE [Student] (
    [StudentID] uniqueidentifier NOT NULL,
    [UserID] uniqueidentifier NULL,
    [StudentCode] varchar(8) NULL,
    [FullName] nvarchar(50) NOT NULL,
    [DayOfBirth] date NULL,
    [Gender] nvarchar(3) NULL,
    [Grade] char(12) NULL,
    [Address] nvarchar(255) NULL,
    [ParentPhoneNumber] varchar(11) NULL,
    [ParentEmailAddress] varchar(70) NULL,
    CONSTRAINT [PK__Student__32C52A7919494F58] PRIMARY KEY ([StudentID])
);
GO

CREATE TABLE [VaccineDetails] (
    [VaccineID] uniqueidentifier NOT NULL,
    [VaccineName] nvarchar(100) NULL,
    [Manufacturer] nvarchar(100) NULL,
    [Disease] nvarchar(100) NULL,
    [VaccineType] nvarchar(50) NULL,
    [AgeRecommendation] nvarchar(50) NULL,
    [BatchNumber] nvarchar(50) NULL,
    [ExpirationDate] date NULL,
    [DoseNumber] int NULL,
    [ContraindicationNotes] nvarchar(255) NULL,
    [Description] nvarchar(255) NULL,
    CONSTRAINT [PK__VaccineD__45DC68E9CAA27428] PRIMARY KEY ([VaccineID])
);
GO

CREATE TABLE [User] (
    [UserID] uniqueidentifier NOT NULL,
    [RoleID] int NULL,
    [FullName] nvarchar(50) NULL,
    [PhoneNumber] varchar(11) NOT NULL,
    [PasswordHash] varchar(255) NOT NULL,
    [EmailAddress] varchar(70) NULL,
    [AvatarUrl] varchar(255) NULL,
    [DayOfBirth] date NULL,
    [Status] bit NULL,
    [Address] nvarchar(255) NULL,
    CONSTRAINT [PK__User__1788CCAC642EA5AB] PRIMARY KEY ([UserID]),
    CONSTRAINT [FK__User__RoleID__4D94879B] FOREIGN KEY ([RoleID]) REFERENCES [Role] ([RoleID])
);
GO

CREATE TABLE [HealthCheckSchedule] (
    [ScheduleID] uniqueidentifier NOT NULL,
    [StudentID] uniqueidentifier NULL,
    [Title] nvarchar(100) NULL,
    [Description] nvarchar(255) NULL,
    [StartDate] date NULL,
    [EndDate] date NULL,
    [TargetGrade] nvarchar(12) NULL,
    [HealthCheckType] nvarchar(30) NULL,
    CONSTRAINT [PK__HealthCh__9C8A5B69CECD15D3] PRIMARY KEY ([ScheduleID]),
    CONSTRAINT [FK__HealthChe__Stude__534D60F1] FOREIGN KEY ([StudentID]) REFERENCES [Student] ([StudentID])
);
GO

CREATE TABLE [HealthProfile] (
    [HealthProfileID] uniqueidentifier NOT NULL,
    [StudentID] uniqueidentifier NULL,
    [CreatedDate] datetime NULL,
    [Notes] nvarchar(255) NULL,
    [ChronicDiseases] nvarchar(255) NULL,
    [DeclarationDate] date NULL,
    [DrugAllergies] nvarchar(255) NULL,
    [FoodAllergies] nvarchar(255) NULL,
    CONSTRAINT [PK__HealthPr__73C2C2B587E14CF3] PRIMARY KEY ([HealthProfileID]),
    CONSTRAINT [FK__HealthPro__Stude__5070F446] FOREIGN KEY ([StudentID]) REFERENCES [Student] ([StudentID])
);
GO

CREATE TABLE [VaccinationSchedule] (
    [ScheduleID] uniqueidentifier NOT NULL,
    [StudentID] uniqueidentifier NULL,
    [VaccineID] uniqueidentifier NULL,
    [Title] nvarchar(100) NULL,
    [Round] nvarchar(10) NULL,
    [Description] nvarchar(255) NULL,
    [StartDate] date NULL,
    [EndDate] date NULL,
    [TargetGrade] nvarchar(12) NULL,
    CONSTRAINT [PK__Vaccinat__9C8A5B69C32EB4D4] PRIMARY KEY ([ScheduleID]),
    CONSTRAINT [FK__Vaccinati__Stude__66603565] FOREIGN KEY ([StudentID]) REFERENCES [Student] ([StudentID]),
    CONSTRAINT [FK__Vaccinati__Vacci__6754599E] FOREIGN KEY ([VaccineID]) REFERENCES [VaccineDetails] ([VaccineID])
);
GO

CREATE TABLE [MedicalEvent] (
    [EventID] uniqueidentifier NOT NULL,
    [StudentID] uniqueidentifier NULL,
    [StaffNurseID] uniqueidentifier NULL,
    [EventType] nvarchar(30) NULL,
    [EventDescription] nvarchar(255) NULL,
    [Location] nvarchar(60) NULL,
    [SeverityLevel] nvarchar(30) NULL,
    [ParentNotified] bit NULL,
    [EventDate] date NULL,
    [Notes] nvarchar(255) NULL,
    CONSTRAINT [PK__MedicalE__7944C870D7609CCF] PRIMARY KEY ([EventID]),
    CONSTRAINT [FK__MedicalEv__Staff__5AEE82B9] FOREIGN KEY ([StaffNurseID]) REFERENCES [User] ([UserID]),
    CONSTRAINT [FK__MedicalEv__Stude__59FA5E80] FOREIGN KEY ([StudentID]) REFERENCES [Student] ([StudentID])
);
GO

CREATE TABLE [MedicalRegistration] (
    [RegistrationID] uniqueidentifier NOT NULL,
    [StudentID] uniqueidentifier NULL,
    [UserID] uniqueidentifier NULL,
    [DateSubmitted] date NULL,
    [MedicationName] nvarchar(100) NULL,
    [TotalDosages] nvarchar(30) NULL,
    [Notes] nvarchar(255) NULL,
    [ParentalConsent] bit NULL,
    [DateApproved] date NULL,
    [StaffNurseID] uniqueidentifier NULL,
    [Status] bit NOT NULL,
    CONSTRAINT [PK__MedicalR__6EF58830AD58F073] PRIMARY KEY ([RegistrationID]),
    CONSTRAINT [FK__MedicalRe__Stude__5DCAEF64] FOREIGN KEY ([StudentID]) REFERENCES [Student] ([StudentID]),
    CONSTRAINT [FK__MedicalRe__UserI__5EBF139D] FOREIGN KEY ([UserID]) REFERENCES [User] ([UserID])
);
GO

CREATE TABLE [Notification] (
    [NotificationID] uniqueidentifier NOT NULL,
    [SendDate] datetime NOT NULL,
    [ConfirmedAt] datetime NULL,
    [Content] nvarchar(max) NULL,
    [IsConfirmed] bit NOT NULL,
    [IsRead] bit NOT NULL,
    [SourceID] uniqueidentifier NOT NULL,
    [Title] nvarchar(max) NULL,
    [Type] int NOT NULL,
    [UserID] uniqueidentifier NULL,
    [SenderID] uniqueidentifier NULL,
    CONSTRAINT [PK__Notifica__20CF2E32D1A83A45] PRIMARY KEY ([NotificationID]),
    CONSTRAINT [FK__Notificat__UserI__619B8048] FOREIGN KEY ([UserID]) REFERENCES [User] ([UserID])
);
GO

CREATE TABLE [HealthCheckResult] (
    [ResultID] uniqueidentifier NOT NULL,
    [ScheduleID] uniqueidentifier NULL,
    [DatePerformed] date NULL,
    [Height] float NULL,
    [Weight] float NULL,
    [VisionLeft] float NULL,
    [VisionRight] float NULL,
    [Hearing] nvarchar(50) NULL,
    [Nose] nvarchar(50) NULL,
    [BloodPressure] nvarchar(50) NULL,
    [Notes] nvarchar(255) NULL,
    [RecordedID] uniqueidentifier NULL,
    [HealthProfileID] uniqueidentifier NOT NULL,
    CONSTRAINT [PK__HealthCh__97690228D7BA8C5C] PRIMARY KEY ([ResultID]),
    CONSTRAINT [FK__HealthChe__Healt__571DF1D5] FOREIGN KEY ([HealthProfileID]) REFERENCES [HealthProfile] ([HealthProfileID]),
    CONSTRAINT [FK__HealthChe__Sched__5629CD9C] FOREIGN KEY ([ScheduleID]) REFERENCES [HealthCheckSchedule] ([ScheduleID])
);
GO

CREATE TABLE [VaccinationResult] (
    [VaccinationResultID] uniqueidentifier NOT NULL,
    [ScheduleID] uniqueidentifier NULL,
    [DoseNumber] int NULL,
    [VaccinationDate] date NULL,
    [InjectionSite] nvarchar(100) NULL,
    [ImmediateReaction] nvarchar(100) NULL,
    [ReactionStartTime] datetime NULL,
    [ReactionType] nvarchar(100) NULL,
    [SeverityLevel] nvarchar(50) NULL,
    [Notes] nvarchar(255) NULL,
    [RecordedID] uniqueidentifier NULL,
    [HealthProfileID] uniqueidentifier NOT NULL,
    CONSTRAINT [PK__Vaccinat__12DE8FD94AFF55DA] PRIMARY KEY ([VaccinationResultID]),
    CONSTRAINT [FK__Vaccinati__Healt__6A30C649] FOREIGN KEY ([HealthProfileID]) REFERENCES [HealthProfile] ([HealthProfileID]),
    CONSTRAINT [FK__Vaccinati__Sched__6B24EA82] FOREIGN KEY ([ScheduleID]) REFERENCES [VaccinationSchedule] ([ScheduleID])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleID', N'RoleName') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] ON;
INSERT INTO [Role] ([RoleID], [RoleName])
VALUES (1, 'admin'),
(2, 'manager'),
(3, 'nurse'),
(4, 'parent');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleID', N'RoleName') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserID', N'Address', N'AvatarUrl', N'DayOfBirth', N'EmailAddress', N'FullName', N'PasswordHash', N'PhoneNumber', N'RoleID', N'Status') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] ON;
INSERT INTO [User] ([UserID], [Address], [AvatarUrl], [DayOfBirth], [EmailAddress], [FullName], [PasswordHash], [PhoneNumber], [RoleID], [Status])
VALUES ('14c6ffeb-7df3-4c6b-a444-3b64683839f0', NULL, NULL, NULL, 'admin@system.local', N'System Admin', 'AQAAAAIAAYagAAAAEKQpu5cJf+JT8hUM0THsrVe5E/G43pp4QFMu3Qp+1O6Th+5iuH9ZHSxfOkyQvpT4mA==', '0900000001', 1, NULL),
('3941ef8f-9141-469a-abf5-42dbb38e6dc8', NULL, NULL, NULL, 'nurse@system.local', N'System Nurse', 'AQAAAAIAAYagAAAAEK1yY/fJJSIoXw2E+hrXt4K1vne5KtuQywNSBA6XagQj8ohqh2lmFUmTM1MxbHb4GQ==', '0900000003', 3, NULL),
('bb3c6cbe-0c50-4ec2-8650-9e5e07967037', NULL, NULL, NULL, 'parent@system.local', N'System Parent', 'AQAAAAIAAYagAAAAEOITkxUKTMZbjct0+sOzYgq65vYUeNpWhSyoy3s0LHqgU/ihiHnd2fLRpl2k5lckHw==', '0900000004', 4, NULL),
('f7135d34-2ec8-4de0-8663-bbee6ae66f18', NULL, NULL, NULL, 'manager@system.local', N'System Manager', 'AQAAAAIAAYagAAAAENYi/7BRP9Vh++yWX3TbbB1qepLlglqoQHYsMjnv7aFd/jlQW1F9V/xB6qmdcY/D5Q==', '0900000002', 2, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserID', N'Address', N'AvatarUrl', N'DayOfBirth', N'EmailAddress', N'FullName', N'PasswordHash', N'PhoneNumber', N'RoleID', N'Status') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] OFF;
GO

CREATE INDEX [IX_HealthCheckResult_HealthProfileID] ON [HealthCheckResult] ([HealthProfileID]);
GO

CREATE INDEX [IX_HealthCheckResult_ScheduleID] ON [HealthCheckResult] ([ScheduleID]);
GO

CREATE INDEX [IX_HealthCheckSchedule_StudentID] ON [HealthCheckSchedule] ([StudentID]);
GO

CREATE INDEX [IX_HealthProfile_StudentID] ON [HealthProfile] ([StudentID]);
GO

CREATE INDEX [IX_MedicalEvent_StaffNurseID] ON [MedicalEvent] ([StaffNurseID]);
GO

CREATE INDEX [IX_MedicalEvent_StudentID] ON [MedicalEvent] ([StudentID]);
GO

CREATE INDEX [IX_MedicalRegistration_StudentID] ON [MedicalRegistration] ([StudentID]);
GO

CREATE INDEX [IX_MedicalRegistration_UserID] ON [MedicalRegistration] ([UserID]);
GO

CREATE INDEX [IX_Notification_UserID] ON [Notification] ([UserID]);
GO

CREATE INDEX [IX_User_RoleID] ON [User] ([RoleID]);
GO

CREATE INDEX [IX_VaccinationResult_HealthProfileID] ON [VaccinationResult] ([HealthProfileID]);
GO

CREATE INDEX [IX_VaccinationResult_ScheduleID] ON [VaccinationResult] ([ScheduleID]);
GO

CREATE INDEX [IX_VaccinationSchedule_StudentID] ON [VaccinationSchedule] ([StudentID]);
GO

CREATE INDEX [IX_VaccinationSchedule_VaccineID] ON [VaccinationSchedule] ([VaccineID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250606022205_SeedRolesAndDefaultUsers', N'8.0.16');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [HealthCheckSchedule] DROP CONSTRAINT [FK__HealthChe__Stude__534D60F1];
GO

ALTER TABLE [VaccinationSchedule] DROP CONSTRAINT [FK__Vaccinati__Stude__66603565];
GO

DROP INDEX [IX_VaccinationSchedule_StudentID] ON [VaccinationSchedule];
GO

DROP INDEX [IX_HealthCheckSchedule_StudentID] ON [HealthCheckSchedule];
GO

DELETE FROM [User]
WHERE [UserID] = '14c6ffeb-7df3-4c6b-a444-3b64683839f0';
SELECT @@ROWCOUNT;

GO

DELETE FROM [User]
WHERE [UserID] = '3941ef8f-9141-469a-abf5-42dbb38e6dc8';
SELECT @@ROWCOUNT;

GO

DELETE FROM [User]
WHERE [UserID] = 'bb3c6cbe-0c50-4ec2-8650-9e5e07967037';
SELECT @@ROWCOUNT;

GO

DELETE FROM [User]
WHERE [UserID] = 'f7135d34-2ec8-4de0-8663-bbee6ae66f18';
SELECT @@ROWCOUNT;

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VaccinationSchedule]') AND [c].[name] = N'StudentID');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [VaccinationSchedule] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [VaccinationSchedule] DROP COLUMN [StudentID];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HealthCheckSchedule]') AND [c].[name] = N'StudentID');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [HealthCheckSchedule] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [HealthCheckSchedule] DROP COLUMN [StudentID];
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserID', N'Address', N'AvatarUrl', N'DayOfBirth', N'EmailAddress', N'FullName', N'PasswordHash', N'PhoneNumber', N'RoleID', N'Status') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] ON;
INSERT INTO [User] ([UserID], [Address], [AvatarUrl], [DayOfBirth], [EmailAddress], [FullName], [PasswordHash], [PhoneNumber], [RoleID], [Status])
VALUES ('307f3796-21ed-4586-9fb5-a96f7cd0560a', NULL, NULL, NULL, 'manager@system.local', N'System Manager', 'AQAAAAIAAYagAAAAEKpjxaK4aLvqIZzQy2+5m2jqvADjBOBDbhROAL2k9N1FtJLKjj5X178TFR4ACUx3jw==', '0900000002', 2, NULL),
('53e6b291-c871-45c9-957c-7b908711f374', NULL, NULL, NULL, 'nurse@system.local', N'System Nurse', 'AQAAAAIAAYagAAAAEABZwqvkPvaU2rMsBdd4DyvKn8bpbAI3vq7NU9a5t75vG7QH8hennmbxDZWg6LetrA==', '0900000003', 3, NULL),
('6a61aaef-72e9-4498-b703-2b425f165675', NULL, NULL, NULL, 'parent@system.local', N'System Parent', 'AQAAAAIAAYagAAAAEAV0NGzjR0EzXScjgcB+qbMi+wQSILDYg1H4zaGj37db6wRrBjFxGt9HjkuapO4pHA==', '0900000004', 4, NULL),
('9443b015-2ac4-4f42-a15a-4053027a1007', NULL, NULL, NULL, 'admin@system.local', N'System Admin', 'AQAAAAIAAYagAAAAEHX0YggcPr2/mztt4lSgmpMh8F9vvfFHnC+eMYEPCJmZzJXdmtD/Qlsh3TDiOwuYfw==', '0900000001', 1, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserID', N'Address', N'AvatarUrl', N'DayOfBirth', N'EmailAddress', N'FullName', N'PasswordHash', N'PhoneNumber', N'RoleID', N'Status') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250624095009_FixMainFlow', N'8.0.16');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DELETE FROM [User]
WHERE [UserID] = '307f3796-21ed-4586-9fb5-a96f7cd0560a';
SELECT @@ROWCOUNT;

GO

DELETE FROM [User]
WHERE [UserID] = '53e6b291-c871-45c9-957c-7b908711f374';
SELECT @@ROWCOUNT;

GO

DELETE FROM [User]
WHERE [UserID] = '6a61aaef-72e9-4498-b703-2b425f165675';
SELECT @@ROWCOUNT;

GO

DELETE FROM [User]
WHERE [UserID] = '9443b015-2ac4-4f42-a15a-4053027a1007';
SELECT @@ROWCOUNT;

GO

ALTER TABLE [VaccinationResult] ADD [Status] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HealthCheckResult] ADD [Status] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserID', N'Address', N'AvatarUrl', N'DayOfBirth', N'EmailAddress', N'FullName', N'PasswordHash', N'PhoneNumber', N'RoleID', N'Status') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] ON;
INSERT INTO [User] ([UserID], [Address], [AvatarUrl], [DayOfBirth], [EmailAddress], [FullName], [PasswordHash], [PhoneNumber], [RoleID], [Status])
VALUES ('2a9411a6-bc8d-4d87-8e80-d0e124726b49', NULL, NULL, NULL, 'nurse@system.local', N'System Nurse', 'AQAAAAIAAYagAAAAEN8BDiWU4MpZs43eQWoGXr5Q+iybxXxmj44DkOY0QOOrT2tqcxU8k1bPoFAOJwMYHw==', '0900000003', 3, NULL),
('4efb2b1f-70b4-4587-9cc4-de8f29793951', NULL, NULL, NULL, 'admin@system.local', N'System Admin', 'AQAAAAIAAYagAAAAEN1BYdqnVPb7zRaKOGjp7qkwg8eVt/R132qDqqi+K6C+beWGfpW8eQxlNhjRq8Qykg==', '0900000001', 1, NULL),
('7222fcfc-d5e3-42e3-bcae-43e819b8ed2f', NULL, NULL, NULL, 'parent@system.local', N'System Parent', 'AQAAAAIAAYagAAAAECK7usNnHQBcnT/L1Mpz1MeUMyjweoh+EB/HubLCJM8xZ1C8Hbm3o7sC1xgewNP/qA==', '0900000004', 4, NULL),
('a7a1d982-e5a0-418b-a98d-981c929eda56', NULL, NULL, NULL, 'manager@system.local', N'System Manager', 'AQAAAAIAAYagAAAAECHnprDihQeUBqRnWCMBtkWvOBZsLqKfNgmqe7GGuEaSPF0fwErcYoxqMgf1rz9j0Q==', '0900000002', 2, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserID', N'Address', N'AvatarUrl', N'DayOfBirth', N'EmailAddress', N'FullName', N'PasswordHash', N'PhoneNumber', N'RoleID', N'Status') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250705065035_UpdateStatusInResults', N'8.0.16');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [Notification];
GO

DELETE FROM [User]
WHERE [UserID] = '2a9411a6-bc8d-4d87-8e80-d0e124726b49';
SELECT @@ROWCOUNT;

GO

DELETE FROM [User]
WHERE [UserID] = '4efb2b1f-70b4-4587-9cc4-de8f29793951';
SELECT @@ROWCOUNT;

GO

DELETE FROM [User]
WHERE [UserID] = '7222fcfc-d5e3-42e3-bcae-43e819b8ed2f';
SELECT @@ROWCOUNT;

GO

DELETE FROM [User]
WHERE [UserID] = 'a7a1d982-e5a0-418b-a98d-981c929eda56';
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserID', N'Address', N'AvatarUrl', N'DayOfBirth', N'EmailAddress', N'FullName', N'PasswordHash', N'PhoneNumber', N'RoleID', N'Status') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] ON;
INSERT INTO [User] ([UserID], [Address], [AvatarUrl], [DayOfBirth], [EmailAddress], [FullName], [PasswordHash], [PhoneNumber], [RoleID], [Status])
VALUES ('06cf676f-fdb0-45ad-ae12-57c76190108b', NULL, NULL, NULL, 'manager@system.local', N'System Manager', 'AQAAAAIAAYagAAAAEIe2uljFDw2/xBSEeVyD+my7rw2JEZviP2Hv9pgNmOv1mExS9JEoUB6JhIUl456raw==', '0900000002', 2, NULL),
('595c4d63-98b8-4fc6-b433-98e6dbda761e', NULL, NULL, NULL, 'nurse@system.local', N'System Nurse', 'AQAAAAIAAYagAAAAEJVDOccSevTPD0SVJ9t2nmT6u1+e3xQzAQPyiQpqEAWhB9VZ98SbOV08x4KrNvdrZA==', '0900000003', 3, NULL),
('7a5efebe-ace2-41fb-acff-e5f855ca4377', NULL, NULL, NULL, 'admin@system.local', N'System Admin', 'AQAAAAIAAYagAAAAEDgvTNhovRzZ3j+Y2eDxhvjLfLhD1Ub4Q4WmgHa2B6wMLP2VL1Z6kdeebpSQQNvwWA==', '0900000001', 1, NULL),
('b0499e5f-7670-4c96-a72c-a60c65fd46e8', NULL, NULL, NULL, 'parent@system.local', N'System Parent', 'AQAAAAIAAYagAAAAEH50Qo+5lnj0iF5eqZNxQCg9sEtmEp9HjPpZyM3Hgl7tbSWYOj4KzA0hlPCcd2mJcg==', '0900000004', 4, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserID', N'Address', N'AvatarUrl', N'DayOfBirth', N'EmailAddress', N'FullName', N'PasswordHash', N'PhoneNumber', N'RoleID', N'Status') AND [object_id] = OBJECT_ID(N'[User]'))
    SET IDENTITY_INSERT [User] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250709102028_removeNotificiation', N'8.0.16');
GO

COMMIT;
GO

