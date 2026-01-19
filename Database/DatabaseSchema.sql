/* =========================================================
   Time Manager Database Schema (Starter / Clean)
   SQL Server
   Creates database + full schema from scratch
   ========================================================= */

IF DB_ID('TimeManagerDB') IS NULL
BEGIN
    CREATE DATABASE TimeManagerDB;
END
GO

USE TimeManagerDB;
GO

/* =========================
   Users
   ========================= */
CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) CONSTRAINT PK_Users PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL CONSTRAINT UQ_Users_UserName UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    Role NVARCHAR(50) NOT NULL CONSTRAINT DF_Users_Role DEFAULT ('User'),
    SleepStart TIME NULL,
    SleepEnd TIME NULL,
    ParentId INT NULL,
    CreatedDate DATETIME NOT NULL CONSTRAINT DF_Users_CreatedDate DEFAULT (GETDATE()),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Kid', 'User', 'Administrator'))
);
GO

ALTER TABLE dbo.Users
ADD CONSTRAINT FK_Users_Parent
FOREIGN KEY (ParentId) REFERENCES dbo.Users(UserId);
GO


/* =========================
   Habits
   ========================= */
CREATE TABLE dbo.HabitCategories (
    HabitCategoryId INT IDENTITY(1,1) CONSTRAINT PK_HabitCategories PRIMARY KEY,
    UserId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL
);
GO

ALTER TABLE dbo.HabitCategories
ADD CONSTRAINT FK_HabitCategories_Users
FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE;
GO

CREATE TABLE dbo.HabitSteps (
    HabitStepId INT IDENTITY(1,1) CONSTRAINT PK_HabitSteps PRIMARY KEY,
    HabitCategoryId INT NOT NULL,
    UserId INT NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    StartDate DATE NULL,
    EndDate DATE NULL,
    RepeatEveryDays INT NULL,
    Description NVARCHAR(500) NULL,
    UseDaytimeSplit BIT NOT NULL CONSTRAINT DF_HabitSteps_UseDaytimeSplit DEFAULT (0),
    EstimatedOccurrences INT NULL,
    RemainingOccurrences INT NULL,
    PointsReward INT NULL
);
GO

ALTER TABLE dbo.HabitSteps
ADD CONSTRAINT FK_HabitSteps_HabitCategories
FOREIGN KEY (HabitCategoryId) REFERENCES dbo.HabitCategories(HabitCategoryId) ON DELETE CASCADE;
GO

ALTER TABLE dbo.HabitSteps
ADD CONSTRAINT FK_HabitSteps_Users
FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);
GO

CREATE TABLE dbo.HabitStepCompletions (
    HabitStepCompletionId INT IDENTITY(1,1) CONSTRAINT PK_HabitStepCompletions PRIMARY KEY,
    HabitStepId INT NOT NULL,
    CompletionDate DATE NOT NULL,
    Part NVARCHAR(10) NULL,
    CONSTRAINT UQ_HabitStepCompletions UNIQUE (HabitStepId, CompletionDate, Part)
);
GO

ALTER TABLE dbo.HabitStepCompletions
ADD CONSTRAINT FK_HabitStepCompletions_HabitSteps
FOREIGN KEY (HabitStepId) REFERENCES dbo.HabitSteps(HabitStepId) ON DELETE CASCADE;
GO


/* =========================
   Core dictionaries
   ========================= */
CREATE TABLE dbo.FoodProducts (
    ProductID INT IDENTITY(1,1) CONSTRAINT PK_FoodProducts PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE dbo.MedicineProducts (
    ProductID INT IDENTITY(1,1) CONSTRAINT PK_MedicineProducts PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);
GO


/* =========================
   Events
   ========================= */
CREATE TABLE dbo.Events (
    EventID INT IDENTITY(1,1) CONSTRAINT PK_Events PRIMARY KEY,
    UserId INT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    StartDateTime DATETIME NOT NULL,
    EndDateTime DATETIME NULL,
    DoInDaytimeUntil BIT NOT NULL CONSTRAINT DF_Events_DoInDaytimeUntil DEFAULT (0),
    DoInDaytimeUntilTime TIME NULL,
    Duration INT NOT NULL CONSTRAINT DF_Events_Duration DEFAULT (60),
    Priority INT NOT NULL CONSTRAINT DF_Events_Priority DEFAULT (1),
    Category INT NOT NULL CONSTRAINT DF_Events_Category DEFAULT (0),
    Deadline DATE NULL,
    IsSetEvent BIT NOT NULL CONSTRAINT DF_Events_IsSetEvent DEFAULT (0),
    CreatedDate DATETIME NOT NULL CONSTRAINT DF_Events_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate DATETIME NOT NULL CONSTRAINT DF_Events_ModifiedDate DEFAULT (GETDATE())
);
GO

ALTER TABLE dbo.Events
ADD CONSTRAINT FK_Events_Users
FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE SET NULL;
GO


/* =========================
   Points / Rewards
   ========================= */
CREATE TABLE dbo.CategoryPoints (
    CategoryPointsID INT IDENTITY(1,1) CONSTRAINT PK_CategoryPoints PRIMARY KEY,
    UserId INT NULL,
    Category NVARCHAR(50) NOT NULL,
    MonthYear NVARCHAR(7) NOT NULL,
    Points INT NOT NULL CONSTRAINT DF_CategoryPoints_Points DEFAULT (0),
    UpdatedDate DATETIME NOT NULL CONSTRAINT DF_CategoryPoints_UpdatedDate DEFAULT (GETDATE())
);
GO

ALTER TABLE dbo.CategoryPoints
ADD CONSTRAINT FK_CategoryPoints_Users
FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE;
GO

CREATE TABLE dbo.UserPoints (
    UserPointsID INT IDENTITY(1,1) CONSTRAINT PK_UserPoints PRIMARY KEY,
    UserId INT NULL,
    TotalPoints INT NOT NULL CONSTRAINT DF_UserPoints_TotalPoints DEFAULT (0),
    LastUpdated DATETIME NOT NULL CONSTRAINT DF_UserPoints_LastUpdated DEFAULT (GETDATE())
);
GO

ALTER TABLE dbo.UserPoints
ADD CONSTRAINT FK_UserPoints_Users
FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE;
GO

CREATE TABLE dbo.Rewards (
    RewardID INT IDENTITY(1,1) CONSTRAINT PK_Rewards PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    PointsCost INT NOT NULL,
    IsActive BIT NULL, 
    UserId INT NULL,
    CreatedDate DATETIME NULL CONSTRAINT DF_Rewards_CreatedDate DEFAULT (GETDATE())
);
GO

ALTER TABLE dbo.Rewards
ADD CONSTRAINT FK_Rewards_Users
FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE;
GO

CREATE TABLE dbo.RewardHistory (
    HistoryID INT IDENTITY(1,1) CONSTRAINT PK_RewardHistory PRIMARY KEY,
    UserId INT NOT NULL,
    RewardName NVARCHAR(255) NOT NULL,
    PointsSpent INT NOT NULL,
    RedeemedDate DATETIME NOT NULL CONSTRAINT DF_RewardHistory_RedeemedDate DEFAULT (GETDATE())
);
GO

ALTER TABLE dbo.RewardHistory
ADD CONSTRAINT FK_RewardHistory_Users
FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE;
GO


/* =========================
   Shopping list
   ========================= */
CREATE TABLE dbo.ShoppingList (
    ShoppingItemID INT IDENTITY(1,1) CONSTRAINT PK_ShoppingList PRIMARY KEY,
    ProductName NVARCHAR(200) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    RecipeID INT NULL,
    CreatedDate DATETIME NOT NULL CONSTRAINT DF_ShoppingList_CreatedDate DEFAULT (GETDATE()),
    AddedDate DATETIME NULL
);
GO


/* =========================
   Fridge / Recipes
   ========================= */
CREATE TABLE dbo.FoodContainers (
    ContainerID INT IDENTITY(1,1) CONSTRAINT PK_FoodContainers PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Type NVARCHAR(20) NOT NULL
);
GO

CREATE TABLE dbo.FridgeItems (
    ItemID INT IDENTITY(1,1) CONSTRAINT PK_FridgeItems PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Quantity DECIMAL(10,2) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    ExpirationDate DATE NULL,
    AddedDate DATETIME NOT NULL CONSTRAINT DF_FridgeItems_AddedDate DEFAULT (GETDATE()),
    LastModified DATETIME NOT NULL CONSTRAINT DF_FridgeItems_LastModified DEFAULT (GETDATE()),
    ContainerID INT NULL,
    ProductID INT NULL
);
GO

ALTER TABLE dbo.FridgeItems
ADD CONSTRAINT FK_FridgeItems_FoodContainers
FOREIGN KEY (ContainerID) REFERENCES dbo.FoodContainers(ContainerID) ON DELETE SET NULL;
GO

ALTER TABLE dbo.FridgeItems
ADD CONSTRAINT FK_FridgeItems_FoodProducts
FOREIGN KEY (ProductID) REFERENCES dbo.FoodProducts(ProductID) ON DELETE SET NULL;
GO

CREATE TABLE dbo.Recipes (
    RecipeID INT IDENTITY(1,1) CONSTRAINT PK_Recipes PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    PreparationTimeMinutes INT NOT NULL,
    NumberOfPortions INT NOT NULL,
    CreatedDate DATETIME NULL CONSTRAINT DF_Recipes_CreatedDate DEFAULT (GETDATE()),
    IsScheduled BIT NOT NULL CONSTRAINT DF_Recipes_IsScheduled DEFAULT (0)
);
GO

CREATE TABLE dbo.RecipeIngredients (
    RecipeIngredientID INT IDENTITY(1,1) CONSTRAINT PK_RecipeIngredients PRIMARY KEY,
    RecipeID INT NOT NULL,
    ProductID INT NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Unit NVARCHAR(20) NOT NULL
);
GO

ALTER TABLE dbo.RecipeIngredients
ADD CONSTRAINT FK_RecipeIngredients_Recipes
FOREIGN KEY (RecipeID) REFERENCES dbo.Recipes(RecipeID) ON DELETE CASCADE;
GO

ALTER TABLE dbo.RecipeIngredients
ADD CONSTRAINT FK_RecipeIngredients_FoodProducts
FOREIGN KEY (ProductID) REFERENCES dbo.FoodProducts(ProductID);
GO


/* =========================
   Medicines
   ========================= */
CREATE TABLE dbo.MedicineContainers (
    ContainerID INT IDENTITY(1,1) CONSTRAINT PK_MedicineContainers PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE dbo.MedicineItems (
    ItemID INT IDENTITY(1,1) CONSTRAINT PK_MedicineItems PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    ProductID INT NULL,
    ContainerID INT NULL,
    Quantity DECIMAL(10,2) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    ExpirationDate DATE NULL,
    AddedDate DATETIME NOT NULL CONSTRAINT DF_MedicineItems_AddedDate DEFAULT (GETDATE()),
    LastModified DATETIME NOT NULL CONSTRAINT DF_MedicineItems_LastModified DEFAULT (GETDATE())
);
GO

ALTER TABLE dbo.MedicineItems
ADD CONSTRAINT FK_MedicineItems_MedicineProducts
FOREIGN KEY (ProductID) REFERENCES dbo.MedicineProducts(ProductID) ON DELETE SET NULL;
GO

ALTER TABLE dbo.MedicineItems
ADD CONSTRAINT FK_MedicineItems_MedicineContainers
FOREIGN KEY (ContainerID) REFERENCES dbo.MedicineContainers(ContainerID) ON DELETE SET NULL;
GO

CREATE TABLE dbo.MedicineSchedules (
    ScheduleID INT IDENTITY(1,1) CONSTRAINT PK_MedicineSchedules PRIMARY KEY,
    MedicineItemID INT NOT NULL,
    MorningDose DECIMAL(10,2) NULL,
    EveningDose DECIMAL(10,2) NULL,
    IntervalDays INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_MedicineSchedules_IsActive DEFAULT (1),
    CreatedDate DATETIME NOT NULL CONSTRAINT DF_MedicineSchedules_CreatedDate DEFAULT (GETDATE())
);
GO

ALTER TABLE dbo.MedicineSchedules
ADD CONSTRAINT FK_MedicineSchedules_MedicineItems
FOREIGN KEY (MedicineItemID) REFERENCES dbo.MedicineItems(ItemID) ON DELETE CASCADE;
GO

CREATE TABLE dbo.MedicineIntakeStatus (
    StatusID INT IDENTITY(1,1) CONSTRAINT PK_MedicineIntakeStatus PRIMARY KEY,
    ScheduleID INT NOT NULL,
    [Date] DATE NOT NULL,
    MorningTaken BIT NOT NULL CONSTRAINT DF_MedicineIntakeStatus_MorningTaken DEFAULT (0),
    EveningTaken BIT NOT NULL CONSTRAINT DF_MedicineIntakeStatus_EveningTaken DEFAULT (0),
    CONSTRAINT UQ_MedicineIntakeStatus UNIQUE (ScheduleID, [Date])
);
GO

ALTER TABLE dbo.MedicineIntakeStatus
ADD CONSTRAINT FK_MedicineIntakeStatus_MedicineSchedules
FOREIGN KEY (ScheduleID) REFERENCES dbo.MedicineSchedules(ScheduleID) ON DELETE CASCADE;
GO


/* =========================
   Plants
   ========================= */
CREATE TABLE dbo.Plants (
    PlantID INT IDENTITY(1,1) CONSTRAINT PK_Plants PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Species NVARCHAR(150) NULL,
    WateringFrequency INT NOT NULL,
    LastWateredDate DATETIME NULL,
    NextWateringDate DATETIME NULL,
    AddedDate DATETIME NOT NULL CONSTRAINT DF_Plants_AddedDate DEFAULT (GETDATE())
);
GO


/* =========================
   Notifications / Focus
   ========================= */
CREATE TABLE dbo.Notifications (
    NotificationID INT IDENTITY(1,1) CONSTRAINT PK_Notifications PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL,
    ReferenceID INT NULL,
    ScheduledDateTime DATETIME NOT NULL,
    IsSent BIT NOT NULL CONSTRAINT DF_Notifications_IsSent DEFAULT (0),
    IsRead BIT NOT NULL CONSTRAINT DF_Notifications_IsRead DEFAULT (0),
    CreatedDate DATETIME NOT NULL CONSTRAINT DF_Notifications_CreatedDate DEFAULT (GETDATE())
);
GO

CREATE TABLE dbo.FocusSessions (
    SessionID INT IDENTITY(1,1) CONSTRAINT PK_FocusSessions PRIMARY KEY,
    EventID INT NULL,
    StartTime DATETIME NOT NULL,
    PlannedDuration INT NOT NULL,
    ActualEndTime DATETIME NULL
);
GO

ALTER TABLE dbo.FocusSessions
ADD CONSTRAINT FK_FocusSessions_Events
FOREIGN KEY (EventID) REFERENCES dbo.Events(EventID);
GO


/* =========================
   Home layout / inventory browser
   ========================= */
CREATE TABLE dbo.HomeLayouts (
    LayoutID INT IDENTITY(1,1) CONSTRAINT PK_HomeLayouts PRIMARY KEY,
    DefaultWidthMeters DECIMAL(9,2) NOT NULL CONSTRAINT DF_HomeLayouts_DefaultWidth DEFAULT (10),
    DefaultHeightMeters DECIMAL(9,2) NOT NULL CONSTRAINT DF_HomeLayouts_DefaultHeight DEFAULT (10),
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_HomeLayouts_CreatedAt DEFAULT (GETDATE())
);
GO

CREATE TABLE dbo.Floors (
    FloorID INT IDENTITY(1,1) CONSTRAINT PK_Floors PRIMARY KEY,
    LayoutID INT NOT NULL,
    FloorNumber INT NOT NULL,
    Title NVARCHAR(50) NULL,
    WidthMeters DECIMAL(9,2) NOT NULL,
    HeightMeters DECIMAL(9,2) NOT NULL
);
GO

ALTER TABLE dbo.Floors
ADD CONSTRAINT FK_Floors_HomeLayouts
FOREIGN KEY (LayoutID) REFERENCES dbo.HomeLayouts(LayoutID) ON DELETE CASCADE;
GO

CREATE TABLE dbo.Walls (
    WallID INT IDENTITY(1,1) CONSTRAINT PK_Walls PRIMARY KEY,
    FloorID INT NOT NULL,
    StartX DECIMAL(9,4) NOT NULL,
    StartY DECIMAL(9,4) NOT NULL,
    EndX DECIMAL(9,4) NOT NULL,
    EndY DECIMAL(9,4) NOT NULL
);
GO

ALTER TABLE dbo.Walls
ADD CONSTRAINT FK_Walls_Floors
FOREIGN KEY (FloorID) REFERENCES dbo.Floors(FloorID) ON DELETE CASCADE;
GO

CREATE TABLE dbo.Containers (
    ContainerID INT IDENTITY(1,1) CONSTRAINT PK_Containers PRIMARY KEY,
    FloorID INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    RoomTag NVARCHAR(50) NULL,
    PointAX DECIMAL(9,4) NOT NULL,
    PointAY DECIMAL(9,4) NOT NULL,
    PointBX DECIMAL(9,4) NOT NULL,
    PointBY DECIMAL(9,4) NOT NULL,
    IsVisible BIT NULL CONSTRAINT DF_Containers_IsVisible DEFAULT (1),
    ParentContainerID INT NULL 
);
GO

ALTER TABLE dbo.Containers
ADD CONSTRAINT FK_Containers_Floors
FOREIGN KEY (FloorID) REFERENCES dbo.Floors(FloorID) ON DELETE CASCADE;
GO

ALTER TABLE dbo.Containers
ADD CONSTRAINT FK_Containers_Parent
FOREIGN KEY (ParentContainerID) REFERENCES dbo.Containers(ContainerID);
GO

CREATE TABLE dbo.Items (
    ItemID INT IDENTITY(1,1) CONSTRAINT PK_Items PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    DefaultUnit NVARCHAR(20) NULL
);
GO

CREATE TABLE dbo.ItemInventory (
    InventoryID INT IDENTITY(1,1) CONSTRAINT PK_ItemInventory PRIMARY KEY,
    ItemID INT NOT NULL,
    ContainerID INT NOT NULL,
    Quantity DECIMAL(12,2) NOT NULL,
    Unit NVARCHAR(20) NOT NULL
);
GO

ALTER TABLE dbo.ItemInventory
ADD CONSTRAINT FK_ItemInventory_Items
FOREIGN KEY (ItemID) REFERENCES dbo.Items(ItemID) ON DELETE CASCADE;
GO

ALTER TABLE dbo.ItemInventory
ADD CONSTRAINT FK_ItemInventory_Containers
FOREIGN KEY (ContainerID) REFERENCES dbo.Containers(ContainerID) ON DELETE CASCADE;
GO


/* =========================
   Seed (optional)
   ========================= */
INSERT INTO dbo.UserPoints (TotalPoints) VALUES (0);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserName = 'admin')
BEGIN
    INSERT INTO dbo.Users (UserName, PasswordHash, Role, SleepStart, SleepEnd)
    VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Administrator', '23:00', '07:00');
END
GO

PRINT 'Database created successfully (clean starter schema).';
GO
