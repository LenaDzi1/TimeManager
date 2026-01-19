/* =========================================================
   Time Manager Sample Data
   Populates the database with test data for Users, Habits, Events, 
   Inventory, Recipes, and Home Layout.
   ========================================================= */

USE TimeManagerDB;
GO

/* =========================================================
   Safety Cleanup: Ensure IDENTITY_INSERT is OFF for all tables
   This prevents errors if a previous run failed mid-execution.
   ========================================================= */
BEGIN TRY
	SET IDENTITY_INSERT dbo.Users OFF;
	SET IDENTITY_INSERT dbo.HomeLayouts OFF;
	SET IDENTITY_INSERT dbo.Floors OFF;
	SET IDENTITY_INSERT dbo.Walls OFF;
	SET IDENTITY_INSERT dbo.Containers OFF;
	SET IDENTITY_INSERT dbo.Items OFF;
	SET IDENTITY_INSERT dbo.ItemInventory OFF;
	SET IDENTITY_INSERT dbo.FoodContainers OFF;
	SET IDENTITY_INSERT dbo.FoodProducts OFF;
	SET IDENTITY_INSERT dbo.FridgeItems OFF;
	SET IDENTITY_INSERT dbo.HabitCategories OFF;
	SET IDENTITY_INSERT dbo.HabitSteps OFF;
	SET IDENTITY_INSERT dbo.MedicineContainers OFF;
	SET IDENTITY_INSERT dbo.MedicineProducts OFF;
	SET IDENTITY_INSERT dbo.MedicineItems OFF;
	SET IDENTITY_INSERT dbo.MedicineSchedules OFF;
	SET IDENTITY_INSERT dbo.Plants OFF;
	SET IDENTITY_INSERT dbo.Recipes OFF;
	SET IDENTITY_INSERT dbo.RecipeIngredients OFF;
	SET IDENTITY_INSERT dbo.Rewards OFF;
	SET IDENTITY_INSERT dbo.UserPoints OFF;
	SET IDENTITY_INSERT dbo.CategoryPoints OFF;
	SET IDENTITY_INSERT dbo.Events OFF;
END TRY
BEGIN CATCH
    -- Ignore errors here (e.g. if table doesn't exist yet or verify check fails)
	-- We just want to ensure session is clean if possible.
END CATCH
GO


BEGIN TRY
    BEGIN TRAN;

    /* =========================================================
       0) Users (force IDs: admin=1, Kid=2, Parent=3)
       ========================================================= */
    PRINT 'Seeding Users...';

    -- Ensure Parent and Kid exist with expected IDs
    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserId IN (2,3))
    BEGIN
        SET IDENTITY_INSERT dbo.Users ON;

        -- Parent must exist before Kid (Kid.ParentId = 3)
        IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserId = 3)
        BEGIN
            INSERT INTO dbo.Users (UserId, UserName, PasswordHash, Role, SleepStart, SleepEnd, ParentId, CreatedDate)
            VALUES (3, 'Parent', 'admin_hash_placeholder', 'User', '23:00', '07:00', NULL, GETDATE());
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserId = 2)
        BEGIN
            INSERT INTO dbo.Users (UserId, UserName, PasswordHash, Role, SleepStart, SleepEnd, ParentId, CreatedDate)
            VALUES (2, 'Kid', 'user_hash_placeholder', 'Kid', '22:00', '07:30', 3, GETDATE());
        END

        SET IDENTITY_INSERT dbo.Users OFF;
    END

    /* =========================================================
       1) HomeLayouts + Floors + Walls
       ========================================================= */
    PRINT 'Seeding HomeLayouts...';

    IF NOT EXISTS (SELECT 1 FROM dbo.HomeLayouts WHERE LayoutID = 1)
    BEGIN
        SET IDENTITY_INSERT dbo.HomeLayouts ON;
        INSERT INTO dbo.HomeLayouts (LayoutID, DefaultWidthMeters, DefaultHeightMeters, CreatedAt)
        VALUES (1, 10.00, 10.00, '2026-01-15 21:39:05.150');
        SET IDENTITY_INSERT dbo.HomeLayouts OFF;
    END

    PRINT 'Seeding Floors...';
    IF NOT EXISTS (SELECT 1 FROM dbo.Floors WHERE FloorID IN (1,2))
    BEGIN
        SET IDENTITY_INSERT dbo.Floors ON;

        INSERT INTO dbo.Floors (FloorID, LayoutID, FloorNumber, Title, WidthMeters, HeightMeters)
        VALUES
        (1, 1, 0, 'Ground', 10.00, 10.00),
        (2, 1, 1, 'Attic', 10.00, 10.00);

        SET IDENTITY_INSERT dbo.Floors OFF;
    END

    PRINT 'Seeding Walls...';
    IF NOT EXISTS (SELECT 1 FROM dbo.Walls WHERE WallID IN (10,11,12,13,14,15,16,17,18))
    BEGIN
        SET IDENTITY_INSERT dbo.Walls ON;

        INSERT INTO dbo.Walls (WallID, FloorID, StartX, StartY, EndX, EndY)
        VALUES
        (10, 1, 0.0000, 0.6250, 0.2304, 0.6250),
        (11, 1, 0.2304, 0.7500, 0.2304, 1.0000),
        (12, 1, 0.2304, 0.4583, 0.5200, 0.4583),
        (13, 1, 0.5172, 0.0049, 0.5172, 0.4583),
        (14, 1, 0.0000, 0.1667, 0.1900, 0.1667),
        (15, 1, 0.5123, 0.1667, 0.2990, 0.1667),
        (16, 1, 1.0000, 0.2623, 0.6700, 0.2623),
        (17, 1, 0.6716, 0.9926, 0.6716, 0.6520),
        (18, 1, 0.6716, 0.2583, 0.6716, 0.5245); -- missing row you added

        SET IDENTITY_INSERT dbo.Walls OFF;
    END

    /* =========================================================
       2) Containers (home storage)
       ========================================================= */
    PRINT 'Seeding Containers...';

    IF NOT EXISTS (SELECT 1 FROM dbo.Containers WHERE ContainerID IN (23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49))
    BEGIN
        SET IDENTITY_INSERT dbo.Containers ON;

        INSERT INTO dbo.Containers
        (ContainerID, FloorID, Name, RoomTag, PointAX, PointAY, PointBX, PointBY, IsVisible, ParentContainerID)
        VALUES
        (23, 1, 'Chest of drawers', 'Kitchen',         0.1250, 0.0000, 0.0000, 0.1446, 1, NULL),
        (24, 1, 'Chest of drawers', 'Storage',         0.6299, 0.0000, 1.0000, 0.1642, 1, NULL),
        (25, 1, 'Wardrobe',         'Bedroom',         0.9000, 1.0000, 1.0000, 0.5809, 1, NULL),
        (26, 1, 'Nightstand',       'Bedroom',         0.8750, 0.3848, 0.6789, 0.2672, 1, NULL),
        (27, 1, 'Cabinet',          'Living room',     0.3799, 0.4534, 0.5098, 0.1740, 1, NULL),
        (28, 1, 'Closet',           'Walk-in Closet',  0.0000, 0.6348, 0.1078, 0.9926, 1, NULL),

        (29, 1, 'Drawer 1',         'Kitchen',         0.0000, 0.0000, 0.1250, 0.1446, 1, 23),
        (30, 1, 'Dwarer 2',         'Kitchen',         0.0000, 0.0000, 0.1250, 0.1446, 1, 23),
        (31, 1, 'Drawer 3',         'Kitchen',         0.0000, 0.0000, 0.1250, 0.1446, 1, 23),

        (32, 1, 'Drawer 1',         'Storage',         0.6299, 0.0000, 1.0000, 0.1642, 1, 24),
        (33, 1, 'Drawer 2',         'Storage',         0.6299, 0.0000, 1.0000, 0.1642, 1, 24),
        (34, 1, 'Drawer 3',         'Storage',         0.6299, 0.0000, 1.0000, 0.1642, 1, 24),
        (35, 1, 'Drawer 4',         'Storage',         0.6299, 0.0000, 1.0000, 0.1642, 1, 24),

        (36, 1, 'Left cabinet',     'Living room',     0.3799, 0.1740, 0.5098, 0.3400, 1, 27),
        (37, 1, 'Right cabinet',    'Living room',     0.3799, 0.3400, 0.5098, 0.4534, 1, 27),

        (38, 1, 'Shelf 1',          'Walk-in Closet',  0.0000, 0.6348, 0.1078, 0.9926, 1, 28),
        (39, 1, 'Shelf 2',          'Walk-in Closet',  0.0000, 0.6348, 0.1078, 0.9926, 1, 28),
        (40, 1, 'Shelf 3',          'Walk-in Closet',  0.0000, 0.6348, 0.1078, 0.9926, 1, 28),

        (41, 1, 'Box',              'Kitchen',         0.0125, 0.0145, 0.1125, 0.1301, 1, 30),
        (42, 1, 'Box',              'Storage',         0.6669, 0.0164, 0.9630, 0.1478, 1, 33),
        (43, 1, 'Box',              'Storage',         0.6669, 0.0164, 0.9630, 0.1478, 1, 34),
        (44, 1, 'Box',              'Storage',         0.6669, 0.0164, 0.9630, 0.1478, 1, 35),

        (45, 2, 'Attic storage',    'attic',           0.0079, 0.0079, 0.7421, 0.2146, 1, NULL),
        (46, 2, 'Box 1',            'attic',           0.0079, 0.0079, 0.3500, 0.1000, 1, 45),
        (47, 2, 'Box 2',            'attic',           0.3500, 0.0079, 0.7421, 0.1000, 1, 45),
        (48, 2, 'Box 3',            'attic',           0.0079, 0.1000, 0.3500, 0.2146, 1, 45),
        (49, 2, 'Box 4',            'attic',           0.3500, 0.1000, 0.7421, 0.2146, 1, 45);

        SET IDENTITY_INSERT dbo.Containers OFF;
    END

    /* =========================================================
       3) Items dictionary + ItemInventory
       ========================================================= */
    PRINT 'Seeding Items...';

    IF NOT EXISTS (SELECT 1 FROM dbo.Items WHERE ItemID = 61)
    BEGIN
        SET IDENTITY_INSERT dbo.Items ON;

        INSERT INTO dbo.Items (ItemID, Name, DefaultUnit)
        VALUES
        (1,'Jacket','pcs'),
        (2,'Blue coat','pcs'),
        (3,'Grey coat','pcs'),
        (4,'Formal shirts','pcs'),
        (5,'Hodies','pcs'),
        (6,'Book','pcs'),
        (7,'Phone charger','pcs'),
        (8,'Glasses','pcs'),
        (9,'Hand cream','l'),
        (10,'Cutlery set','pcs'),
        (11,'Kitchen towels','pcs'),
        (12,'Measuring spoons','pcs'),
        (13,'Cooking utensils','pcs'),
        (14,'Food containers','pcs'),
        (15,'Aluminum foil','pcs'),
        (16,'Baking paper','pcs'),
        (17,'Plastic wrap','pcs'),
        (18,'Extension cable','pcs'),
        (19,'Batteries AA','pcs'),
        (20,'Screwdrivers','pcs'),
        (21,'Allen keys set','pcs'),
        (22,'Light bulbs','pcs'),
        (23,'Flashlight','pcs'),
        (24,'Documents folder','pcs'),
        (25,'Spare keys','pcs'),
        (26,'Board games','pcs'),
        (27,'Photo albums','pcs'),
        (28,'Candles','pcs'),
        (29,'Remote controls','pcs'),
        (30,'Batteries AAA','pcs'),
        (31,'Cleaning wipes','pcs'),
        (32,'Shoes (pairs)','pcs'),
        (33,'Backpacks','pcs'),
        (34,'Hats','pcs'),
        (35,'Caps','pcs'),
        (36,'Scarves','pcs'),
        (37,'Suitcases','pcs'),
        (38,'Storage boxes','pcs'),
        (39,'Spice jars','pcs'),
        (40,'Tea bags','pcs'),
        (41,'Screws','kg'),
        (42,'Electrical tape','pcs'),
        (43,'USB cables','pcs'),
        (44,'Memory cards','pcs'),
        (45,'Old contracts','pcs'),
        (46,'Warranty documents','pcs'),
        (47,'Instruction manuals','pcs'),
        (48,'Tax documents','pcs'),
        (49,'Certificates','pcs'),
        (50,'Loose photographs','pcs'),
        (51,'Greeting cards','pcs'),
        (52,'Letters','pcs'),
        (53,'Souvenirs','pcs'),
        (54,'Christmas decorations','pcs'),
        (55,'Easter decorations','pcs'),
        (56,'Holiday lights','pcs'),
        (57,'Gift wrapping paper rolls','pcs'),
        (58,'Old phone','pcs'),
        (59,'Laptop charger','pcs'),
        (60,'HDMI cables','pcs'),
        (61,'External hard drive','pcs');

        SET IDENTITY_INSERT dbo.Items OFF;
    END

    PRINT 'Seeding ItemInventory...';
    IF NOT EXISTS (SELECT 1 FROM dbo.ItemInventory WHERE InventoryID = 63)
    BEGIN
        SET IDENTITY_INSERT dbo.ItemInventory ON;

        INSERT INTO dbo.ItemInventory (InventoryID, ItemID, ContainerID, Quantity, Unit)
        VALUES
        (1, 1, 25, 1.00, 'pcs'),
        (2, 2, 25, 1.00, 'pcs'),
        (3, 3, 25, 1.00, 'pcs'),
        (4, 4, 25, 6.00, 'pcs'),
        (5, 5, 25, 4.00, 'pcs'),
        (6, 6, 26, 2.00, 'pcs'),
        (7, 7, 26, 1.00, 'pcs'),
        (8, 8, 26, 1.00, 'pcs'),
        (9, 9, 26, 0.10, 'l'),
        (10, 10, 29, 1.00, 'pcs'),
        (11, 11, 29, 4.00, 'pcs'),
        (12, 12, 29, 1.00, 'pcs'),
        (13, 13, 30, 5.00, 'pcs'),
        (14, 14, 30, 6.00, 'pcs'),
        (15, 15, 31, 1.00, 'pcs'),
        (16, 16, 31, 1.00, 'pcs'),
        (17, 17, 31, 1.00, 'pcs'),
        (18, 18, 32, 2.00, 'pcs'),
        (19, 19, 32, 8.00, 'pcs'),
        (20, 20, 33, 3.00, 'pcs'),
        (21, 21, 33, 1.00, 'pcs'),
        (22, 22, 34, 6.00, 'pcs'),
        (23, 23, 34, 1.00, 'pcs'),
        (24, 24, 35, 2.00, 'pcs'),
        (25, 25, 35, 3.00, 'pcs'),
        (26, 26, 36, 3.00, 'pcs'),
        (27, 27, 36, 2.00, 'pcs'),
        (28, 28, 36, 4.00, 'pcs'),
        (29, 29, 37, 3.00, 'pcs'),
        (30, 30, 37, 6.00, 'pcs'),
        (31, 31, 37, 2.00, 'pcs'),
        (32, 32, 38, 6.00, 'pcs'),
        (33, 33, 38, 2.00, 'pcs'),
        (34, 34, 39, 4.00, 'pcs'),
        (35, 35, 39, 3.00, 'pcs'),
        (36, 36, 39, 5.00, 'pcs'),
        (37, 37, 40, 2.00, 'pcs'),
        (38, 38, 40, 3.00, 'pcs'),
        (39, 39, 41, 6.00, 'pcs'),
        (40, 40, 41, 40.00, 'pcs'),
        (41, 41, 42, 0.50, 'kg'),
        (42, 42, 43, 2.00, 'pcs'),
        (43, 43, 44, 4.00, 'pcs'),
        (44, 44, 44, 2.00, 'pcs'),
        (45, 45, 46, 12.00, 'pcs'),
        (46, 46, 46, 8.00, 'pcs'),
        (47, 47, 46, 15.00, 'pcs'),
        (48, 48, 46, 5.00, 'pcs'),
        (49, 49, 46, 3.00, 'pcs'),
        (50, 27, 47, 4.00, 'pcs'),
        (51, 50, 47, 120.00, 'pcs'),
        (52, 51, 47, 25.00, 'pcs'),
        (53, 52, 47, 30.00, 'pcs'),
        (54, 53, 47, 10.00, 'pcs'),
        (55, 54, 48, 1.00, 'pcs'),
        (56, 55, 48, 1.00, 'pcs'),
        (57, 56, 48, 3.00, 'pcs'),
        (58, 57, 48, 6.00, 'pcs'),
        (59, 58, 49, 2.00, 'pcs'),
        (60, 59, 49, 3.00, 'pcs'),
        (61, 60, 49, 4.00, 'pcs'),
        (62, 43, 49, 6.00, 'pcs'),
        (63, 61, 49, 1.00, 'pcs');

        SET IDENTITY_INSERT dbo.ItemInventory OFF;
    END

    /* =========================================================
       4) FoodContainers + FoodProducts + FridgeItems
       ========================================================= */
    PRINT 'Seeding FoodContainers...';
    IF NOT EXISTS (SELECT 1 FROM dbo.FoodContainers WHERE ContainerID = 3)
    BEGIN
        SET IDENTITY_INSERT dbo.FoodContainers ON;

        INSERT INTO dbo.FoodContainers (ContainerID, Name, Type)
        VALUES
        (1, 'Fridge 1',  'Fridge'),
        (2, 'Freezer 1', 'Freezer'),
        (3, 'Pantry 1',  'Pantry');

        SET IDENTITY_INSERT dbo.FoodContainers OFF;
    END

    PRINT 'Seeding FoodProducts...';
    IF NOT EXISTS (SELECT 1 FROM dbo.FoodProducts WHERE ProductID = 24)
    BEGIN
        SET IDENTITY_INSERT dbo.FoodProducts ON;

        INSERT INTO dbo.FoodProducts (ProductID, Name)
        VALUES
        (1,'Milk'),
        (2,'Butter'),
        (3,'Yellow cheese'),
        (4,'Natural yogurt'),
        (5,'Cream'),
        (6,'Eggs'),
        (7,'Chicken breast'),
        (8,'Ham'),
        (9,'Bell pepper'),
        (10,'Tomato'),
        (11,'Cucumber'),
        (12,'Lettuce'),
        (13,'Ketchup'),
        (14,'Mustard'),
        (15,'Fish fillet'),
        (16,'Vegetables mix'),
        (17,'Spinach'),
        (18,'Rice'),
        (19,'Pasta'),
        (20,'Flour'),
        (21,'Sugar'),
        (22,'Salt'),
        (23,'Honey'),
        (24,'Oil');

        SET IDENTITY_INSERT dbo.FoodProducts OFF;
    END

    PRINT 'Seeding FridgeItems...';
    IF NOT EXISTS (SELECT 1 FROM dbo.FridgeItems WHERE ItemID = 23)
    BEGIN
        SET IDENTITY_INSERT dbo.FridgeItems ON;

        INSERT INTO dbo.FridgeItems
        (ItemID, Name, Quantity, Unit, ExpirationDate, AddedDate, LastModified, ContainerID, ProductID)
        VALUES
        (1,'Milk',1.00,'l','2026-01-22','2026-01-15 22:00:04.137','2026-01-15 22:00:04.137',1,1),
        (2,'Butter',200.00,'g','2026-03-22','2026-01-15 22:00:20.260','2026-01-15 22:00:20.260',1,2),
        (3,'Yellow cheese',300.00,'g','2026-01-22','2026-01-15 22:00:36.013','2026-01-15 22:00:36.013',1,3),
        (4,'Natural yogurt',400.00,'g','2026-01-22','2026-01-15 22:00:51.727','2026-01-15 22:00:51.727',1,4),
        (5,'Cream',200.00,'ml','2026-01-22','2026-01-15 22:01:05.147','2026-01-15 22:01:05.147',1,5),
        (6,'Eggs',2.00,'pcs','2026-01-22','2026-01-15 22:01:18.147','2026-01-16 01:32:09.080',1,6),
        (7,'Ham',150.00,'g','2026-01-22','2026-01-15 22:01:34.493','2026-01-15 22:01:34.493',1,8),
        (8,'Bell pepper',2.00,'pcs','2026-01-30','2026-01-15 22:01:44.817','2026-01-15 22:01:44.817',1,9),
        (9,'Tomato',4.00,'pcs','2026-01-30','2026-01-15 22:01:58.190','2026-01-15 22:01:58.190',1,10),
        (10,'Cucumber',1.00,'pcs','2026-01-30','2026-01-15 22:02:12.160','2026-01-15 22:02:12.160',1,11),
        (11,'Lettuce',1.00,'pcs','2026-01-30','2026-01-15 22:02:34.770','2026-01-15 22:02:34.770',1,12),
        (12,'Ketchup',300.00,'ml','2026-06-22','2026-01-15 22:02:59.663','2026-01-15 22:02:59.663',1,13),
        (13,'Mustard',200.00,'ml','2026-06-22','2026-01-15 22:03:18.260','2026-01-15 22:03:18.260',1,14),
        (14,'Chicken breast',1.00,'kg','2026-03-22','2026-01-15 22:03:49.490','2026-01-15 22:03:49.490',2,7),
        (15,'Fish fillet',600.00,'g','2026-03-22','2026-01-15 22:04:20.490','2026-01-15 22:04:20.490',2,15),
        (16,'Vegetables mix',750.00,'g','2026-03-22','2026-01-15 22:04:33.983','2026-01-15 22:04:33.983',2,16),
        (17,'Spinach',400.00,'g','2026-03-22','2026-01-15 22:04:52.063','2026-01-15 22:04:52.063',2,17),
        (18,'Rice',1.00,'kg',NULL,'2026-01-15 22:05:12.283','2026-01-15 22:05:12.283',3,18),
        (19,'Pasta',500.00,'g','2027-01-22','2026-01-15 22:05:37.840','2026-01-15 22:05:37.840',3,19),
        (20,'Flour',1.00,'kg',NULL,'2026-01-15 22:05:58.083','2026-01-15 22:05:58.083',3,20),
        (21,'Sugar',1.00,'kg',NULL,'2026-01-15 22:06:15.230','2026-01-15 22:06:15.230',3,21),
        (22,'Salt',500.00,'g',NULL,'2026-01-15 22:06:43.283','2026-01-15 22:06:43.283',3,22),
        (23,'Honey',250.00,'g','2027-01-22','2026-01-15 22:06:59.860','2026-01-15 22:06:59.860',3,23);

        SET IDENTITY_INSERT dbo.FridgeItems OFF;
    END

    /* =========================================================
       5) Habits (categories + steps)
       ========================================================= */
    PRINT 'Seeding HabitCategories...';
    IF NOT EXISTS (SELECT 1 FROM dbo.HabitCategories WHERE HabitCategoryId = 4)
    BEGIN
        SET IDENTITY_INSERT dbo.HabitCategories ON;

        INSERT INTO dbo.HabitCategories (HabitCategoryId, UserId, Name)
        VALUES
        (1, 2, 'My morning habits'),
        (2, 2, 'My learning habits'),
        (3, 3, 'Home Care'),
        (4, 3, 'Self growth');

        SET IDENTITY_INSERT dbo.HabitCategories OFF;
    END

    PRINT 'Seeding HabitSteps...';
    IF NOT EXISTS (SELECT 1 FROM dbo.HabitSteps WHERE HabitStepId = 9)
    BEGIN
        SET IDENTITY_INSERT dbo.HabitSteps ON;

        INSERT INTO dbo.HabitSteps
        (HabitStepId, HabitCategoryId, UserId, Name, StartDate, EndDate, RepeatEveryDays, Description,
         UseDaytimeSplit, EstimatedOccurrences, RemainingOccurrences, PointsReward)
        VALUES
        (1, 1, 2, 'Brush teeth', '2026-01-15', '2026-01-30', NULL, 'Brush teeth twice a day for 2 min', 1, 16, 16, 20),
        (2, 1, 2, 'Take a shower', '2026-01-15', '2026-01-30', NULL, 'Take a shower at least once a day', 0, 16, 16, 20),
        (3, 1, 2, 'Do a workout', '2026-01-15', '2026-01-30', 2, 'Do morning workout each 2 days', 0, 8, 8, 30),
        (4, 2, 2, 'Read a chapter of a book', '2026-01-15', '2026-01-30', NULL, 'read 15 min daily', 0, 16, 16, 30),
        (5, 2, 2, 'Repeat for an exam', '2026-01-17', '2026-01-31', 7, 'Repeat the materials once a week', 1, 3, 3, 40),
        (6, 3, 3, 'Take out trash or recycle', '2026-01-15', '2026-01-30', 7, 'Take out the trash at least once a week', 0, 3, 3, 10),
        (7, 3, 3, 'Tidy one small area', '2026-01-15', '2026-01-30', 2, 'Tidy up a little each 2 days', 1, 8, 8, 15),
        (8, 4, 3, 'Read 5 pages of a book', '2026-01-15', '2026-01-30', 1, 'Read a book', 0, 16, 16, 15),
        (9, 3, 3, 'Clean the dishes', '2026-01-15', '2026-01-30', NULL, 'Clean the dishes everyday', 0, 16, 15, 20);

        SET IDENTITY_INSERT dbo.HabitSteps OFF;
    END

    /* =========================================================
       6) Medicines (containers + products + items + schedules)
       ========================================================= */
    PRINT 'Seeding MedicineContainers...';
    IF NOT EXISTS (SELECT 1 FROM dbo.MedicineContainers WHERE ContainerID = 2)
    BEGIN
        SET IDENTITY_INSERT dbo.MedicineContainers ON;

        INSERT INTO dbo.MedicineContainers (ContainerID, Name)
        VALUES
        (1, 'First aid'),
        (2, 'Toilet cabinet');

        SET IDENTITY_INSERT dbo.MedicineContainers OFF;
    END

    PRINT 'Seeding MedicineProducts...';
    IF NOT EXISTS (SELECT 1 FROM dbo.MedicineProducts WHERE ProductID = 20)
    BEGIN
        SET IDENTITY_INSERT dbo.MedicineProducts ON;

        INSERT INTO dbo.MedicineProducts (ProductID, Name)
        VALUES
        (1,'Paracetamol'),
        (2,'Ibuprofen'),
        (3,'Aspirin'),
        (4,'Cough syrup'),
        (5,'Antihistamine tablets (allergy)'),
        (6,'Activated charcoal'),
        (7,'Adhesive bandages'),
        (8,'Sterile gauze pads'),
        (9,'Medical tape'),
        (10,'Elastic bandage'),
        (11,'Antiseptic spray'),
        (12,'Cotton pads'),
        (13,'Cotton swabs'),
        (14,'Disposable gloves'),
        (15,'Nasal spray'),
        (16,'Eye drops'),
        (18,'Multivitamin'),
        (19,'Vitamin D3'),
        (20,'cos');

        SET IDENTITY_INSERT dbo.MedicineProducts OFF;
    END

    PRINT 'Seeding MedicineItems...';
    IF NOT EXISTS (SELECT 1 FROM dbo.MedicineItems WHERE ItemID = 16)
    BEGIN
        SET IDENTITY_INSERT dbo.MedicineItems ON;

        INSERT INTO dbo.MedicineItems
        (ItemID, Name, ProductID, ContainerID, Quantity, Unit, ExpirationDate, AddedDate, LastModified)
        VALUES
        (1,'Paracetamol',1,1,20.00,'pcs','2027-01-15','2026-01-15 23:05:13.770','2026-01-15 23:05:13.770'),
        (2,'Ibuprofen',2,1,20.00,'pcs','2027-01-15','2026-01-15 23:05:19.747','2026-01-15 23:05:19.747'),
        (3,'Activated charcoal',6,1,10.00,'pcs','2027-01-15','2026-01-15 23:05:25.090','2026-01-15 23:05:25.090'),
        (4,'Antihistamine tablets (allergy)',5,1,10.00,'pcs','2027-01-15','2026-01-15 23:05:36.473','2026-01-15 23:05:36.473'),
        (5,'Adhesive bandages',7,1,30.00,'pcs',NULL,'2026-01-15 23:05:46.193','2026-01-15 23:05:46.193'),
        (6,'Sterile gauze pads',8,1,10.00,'pcs',NULL,'2026-01-15 23:05:54.617','2026-01-15 23:05:54.617'),
        (7,'Medical tape',9,1,1.00,'pcs',NULL,'2026-01-15 23:06:07.350','2026-01-15 23:06:07.350'),
        (8,'Elastic bandage',10,1,2.00,'pcs',NULL,'2026-01-15 23:06:17.137','2026-01-15 23:06:17.137'),
        (9,'Antiseptic spray',11,1,100.00,'ml',NULL,'2026-01-15 23:06:32.667','2026-01-15 23:06:32.667'),
        (10,'Disposable gloves',14,2,20.00,'pcs',NULL,'2026-01-15 23:06:57.750','2026-01-15 23:06:57.750'),
        (11,'Cotton pads',12,2,100.00,'pcs',NULL,'2026-01-15 23:07:15.827','2026-01-15 23:07:15.827'),
        (12,'Cotton swabs',13,2,200.00,'pcs',NULL,'2026-01-15 23:07:24.920','2026-01-15 23:07:24.920'),
        (13,'Nasal spray',15,2,20.00,'ml','2027-01-15','2026-01-15 23:07:38.293','2026-01-15 23:07:38.293'),
        (14,'Eye drops',16,2,10.00,'ml','2027-01-15','2026-01-15 23:07:49.477','2026-01-15 23:07:49.477'),
        (15,'Multivitamin',18,2,20.00,'pcs','2027-01-15','2026-01-15 23:08:51.153','2026-01-16 02:07:56.230'),
        (16,'Vitamin D3',19,1,30.00,'pcs','2027-01-15','2026-01-15 23:09:14.240','2026-01-15 23:09:14.240');

        SET IDENTITY_INSERT dbo.MedicineItems OFF;
    END

    PRINT 'Seeding MedicineSchedules...';
    IF NOT EXISTS (SELECT 1 FROM dbo.MedicineSchedules WHERE ScheduleID = 2)
    BEGIN
        SET IDENTITY_INSERT dbo.MedicineSchedules ON;

        INSERT INTO dbo.MedicineSchedules
        (ScheduleID, MedicineItemID, MorningDose, EveningDose, IntervalDays, StartDate, EndDate, IsActive, CreatedDate)
        VALUES
        (1, 15, 1.00, NULL, 1, '2026-01-15', '2026-01-30', 1, '2026-01-15 23:12:19.293'),
        (2, 16, NULL, 1.00, 2, '2026-01-15', '2026-01-30', 1, '2026-01-15 23:12:44.080');

        SET IDENTITY_INSERT dbo.MedicineSchedules OFF;
    END

    /* =========================================================
       7) Plants
       ========================================================= */
    PRINT 'Seeding Plants...';
    IF NOT EXISTS (SELECT 1 FROM dbo.Plants WHERE PlantID = 2)
    BEGIN
        SET IDENTITY_INSERT dbo.Plants ON;

        INSERT INTO dbo.Plants
        (PlantID, Name, Species, WateringFrequency, LastWateredDate, NextWateringDate, AddedDate)
        VALUES
        (1,'Monstera','Monstera deliciosa',7,'2026-01-15 22:14:46.630','2026-01-22 22:14:46.630','2026-01-15 22:14:43.793'),
        (2,'Aloe','Aloe Vera',14,'2026-01-15 22:15:12.860','2026-01-29 22:15:12.860','2026-01-15 22:15:11.823');

        SET IDENTITY_INSERT dbo.Plants OFF;
    END

    /* =========================================================
       8) Recipes + RecipeIngredients
       ========================================================= */
    PRINT 'Seeding Recipes...';
    IF NOT EXISTS (SELECT 1 FROM dbo.Recipes WHERE RecipeID = 2)
    BEGIN
        SET IDENTITY_INSERT dbo.Recipes ON;

        INSERT INTO dbo.Recipes
        (RecipeID, Name, Description, PreparationTimeMinutes, NumberOfPortions, CreatedDate, IsScheduled)
        VALUES
        (1,
         'Cheese & Vegetable Omelette',
         'Instructions:   Crack the eggs into a bowl and season with salt and pepper.   Chop the bell pepper and tomato into small pieces.   Melt butter in a pan over medium heat.   Add vegetables and sauté for 1–2 minutes.   Pour in the eggs and sprinkle cheese on top.   Cook on low heat until set.',
         10, 1, '2026-01-15 22:10:15.137', 0),
        (2,
         'Chicken in Yogurt Mustard Sauce with Fresh Salad',
         'Instructions:   Cut the chicken into strips and season with salt and pepper.   Heat butter or oil in a pan and fry the chicken until golden.   In a bowl, mix yogurt with mustard.   Remove pan from heat and stir in the yogurt sauce.   Chop lettuce, cucumber, and tomato to prepare a fresh salad.   Serve the chicken with the salad on the side.',
         25, 2, '2026-01-15 22:12:42.710', 0);

        SET IDENTITY_INSERT dbo.Recipes OFF;
    END

    PRINT 'Seeding RecipeIngredients...';
    IF NOT EXISTS (SELECT 1 FROM dbo.RecipeIngredients WHERE RecipeIngredientID IN (6,7,8,9,10,11,12,18,19,20,21,22))
    BEGIN
        SET IDENTITY_INSERT dbo.RecipeIngredients ON;

        INSERT INTO dbo.RecipeIngredients
        (RecipeIngredientID, RecipeID, ProductID, ProductName, Amount, Unit)
        VALUES
        (6, 2, 7,  'Chicken breast', 300.00, 'g'),
        (7, 2, 4,  'Natural yogurt', 150.00, 'g'),
        (8, 2, 14, 'Mustard',        15.00,  'g'),
        (9, 2, 24, 'Oil',            10.00,  'ml'),
        (10,2, 12, 'Lettuce',        0.50,   'pcs'),
        (11,2, 11, 'Cucumber',       0.50,   'pcs'),
        (12,2, 10, 'Tomato',         1.00,   'pcs'),

        (18,1, 6,  'Eggs',           3.00,   'pcs'),
        (19,1, 3,  'Yellow cheese',  60.00,  'g'),
        (20,1, 9,  'Bell pepper',    0.50,   'pcs'),
        (21,1, 2,  'Butter',         10.00,  'g'),
        (22,1, 10, 'Tomato',         1.00,   'pcs');

        SET IDENTITY_INSERT dbo.RecipeIngredients OFF;
    END

    /* =========================================================
       9) Rewards
       ========================================================= */
    PRINT 'Seeding Rewards...';
    IF NOT EXISTS (SELECT 1 FROM dbo.Rewards WHERE RewardID = 5)
    BEGIN
        SET IDENTITY_INSERT dbo.Rewards ON;

        INSERT INTO dbo.Rewards (RewardID, Name, Description, PointsCost, IsActive, UserId, CreatedDate)
        VALUES
        (1,'Coffee Out','A coffee or tea break outside the house.',100,NULL,3,'2026-01-15 23:48:48.930'),
        (2,'Extra Screen Time','30 minutes of extra tablet / TV / game time',80,NULL,2,'2026-01-15 23:49:23.500'),
        (3,'Small Toy or Treat','A small toy, sticker set, or favorite sweet',150,NULL,2,'2026-01-15 23:49:53.250'),
        (4,'Cinema','Going out to the cinema',300,NULL,2,'2026-01-15 23:50:14.797'),
        (5,'Favorite Snack or Drink','Buy yourself a favorite snack, dessert, or drink',70,NULL,3,'2026-01-15 23:53:39.020');

        SET IDENTITY_INSERT dbo.Rewards OFF;
    END

    /* =========================================================
       10) UserPoints
       ========================================================= */
    PRINT 'Seeding UserPoints...';
    IF NOT EXISTS (SELECT 1 FROM dbo.UserPoints WHERE UserPointsID = 3)
    BEGIN
        SET IDENTITY_INSERT dbo.UserPoints ON;

        INSERT INTO dbo.UserPoints (UserPointsID, UserId, TotalPoints, LastUpdated)
        VALUES
        (2, 2,    670, '2026-01-16 01:26:44.980'),
        (3, 3,    360, '2026-01-16 01:26:26.260');

        SET IDENTITY_INSERT dbo.UserPoints OFF;
    END

    /* =========================================================
       11) CategoryPoints
       ========================================================= */
    PRINT 'Seeding CategoryPoints...';
    IF NOT EXISTS (SELECT 1 FROM dbo.CategoryPoints WHERE CategoryPointsID = 19)
    BEGIN
        SET IDENTITY_INSERT dbo.CategoryPoints ON;

        INSERT INTO dbo.CategoryPoints
        (CategoryPointsID, UserId, Category, MonthYear, Points, UpdatedDate)
        VALUES
        (4,  3, 'health',                         '2026-01', 60,  '2026-01-15 23:32:10.147'),
        (5,  3, 'family',                         '2026-01', 50,  '2026-01-15 23:32:10.147'),
        (6,  3, 'mentality',                      '2026-01', 70,  '2026-01-15 23:32:10.147'),
        (7,  3, 'finance',                        '2026-01', 30,  '2026-01-15 23:32:10.147'),
        (8,  3, 'work and career',                '2026-01', 80,  '2026-01-15 23:32:10.147'),
        (9,  3, 'relax',                          '2026-01', 10,  '2026-01-15 23:32:10.147'),
        (10, 3, 'self development and education', '2026-01', 50,  '2026-01-15 23:32:10.147'),
        (11, 3, 'friends and people',             '2026-01', 20,  '2026-01-15 23:32:10.147'),
        (12, 2, 'health',                         '2026-01', 10,  '2026-01-15 23:32:43.367'),
        (13, 2, 'family',                         '2026-01', 70,  '2026-01-15 23:32:43.367'),
        (14, 2, 'mentality',                      '2026-01', 90,  '2026-01-15 23:32:43.367'),
        (15, 2, 'finance',                        '2026-01', 40,  '2026-01-15 23:32:43.367'),
        (16, 2, 'work and career',                '2026-01', 40,  '2026-01-15 23:32:43.367'),
        (17, 2, 'relax',                          '2026-01', 100, '2026-01-15 23:32:43.367'),
        (18, 2, 'self development and education', '2026-01', 30,  '2026-01-15 23:32:43.367'),
        (19, 2, 'friends and people',             '2026-01', 60,  '2026-01-15 23:32:43.367');

        SET IDENTITY_INSERT dbo.CategoryPoints OFF;
    END

    /* =========================================================
       12) Events
       ========================================================= */
    PRINT 'Seeding Events...';
    IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventID = 60)
    BEGIN
        SET IDENTITY_INSERT dbo.Events ON;

        INSERT INTO dbo.Events
        (EventID, UserId, Title, Description, StartDateTime, EndDateTime,
         DoInDaytimeUntil, DoInDaytimeUntilTime, Duration, Priority, Category, Deadline, IsSetEvent, CreatedDate, ModifiedDate)
        VALUES
        (23,3,'Parent_event_p4.1',NULL,'2026-01-16 08:30:00.000','2026-01-16 09:30:00.000',0,NULL,60,4,'3',NULL,0,GETDATE(),GETDATE()),
        (24,3,'Parent_event_p4.2',NULL,'2026-01-16 09:35:00.000','2026-01-16 10:35:00.000',0,NULL,60,4,'3',NULL,0,GETDATE(),GETDATE()),
        (25,3,'Parent_event_p4.3',NULL,'2026-01-16 10:40:00.000','2026-01-16 11:40:00.000',0,NULL,60,4,'3',NULL,0,GETDATE(),GETDATE()),
        (26,3,'Parent_event_p4.4','Do In Daytime Untill 10AM','2026-01-17 08:30:00.000','2026-01-17 09:30:00.000',1,'10:00:00',60,4,'3',NULL,0,GETDATE(),GETDATE()),

        (28,3,'Parent_event_p3.2',NULL,'2026-01-16 17:05:00.000','2026-01-16 18:05:00.000',0,NULL,60,3,'2',NULL,0,GETDATE(),GETDATE()),
        (29,3,'Parent_event_p3.3',NULL,'2026-01-16 18:10:00.000','2026-01-16 19:10:00.000',0,NULL,60,3,'2',NULL,0,GETDATE(),GETDATE()),
        (30,3,'Parent_event_p3.1',NULL,'2026-01-16 16:00:00.000','2026-01-16 17:00:00.000',0,NULL,60,3,'2',NULL,0,GETDATE(),GETDATE()),
        (31,3,'Parent_event_p3.4',NULL,'2026-01-16 12:30:00.000','2026-01-16 13:30:00.000',0,NULL,60,3,'2',NULL,0,GETDATE(),GETDATE()),

        (32,3,'Parent_event_p2.1',NULL,'2026-01-17 14:40:00.000','2026-01-17 15:40:00.000',0,NULL,60,2,'1',NULL,0,GETDATE(),GETDATE()),
        (33,3,'Parent_event_p2.2',NULL,'2026-01-16 14:40:00.000','2026-01-16 15:40:00.000',0,NULL,60,2,'1',NULL,0,GETDATE(),GETDATE()),
        (34,3,'Parent_event_p2.3',NULL,'2026-01-17 12:30:00.000','2026-01-17 13:30:00.000',0,NULL,60,2,'1',NULL,0,GETDATE(),GETDATE()),
        (35,3,'Parent_event_p2.4',NULL,'2026-01-17 13:35:00.000','2026-01-17 14:35:00.000',0,NULL,60,2,'1',NULL,0,GETDATE(),GETDATE()),

        (36,3,'Parent_event_p1.1',NULL,'2026-01-16 19:30:00.000','2026-01-16 20:30:00.000',0,NULL,60,1,'0',NULL,0,GETDATE(),GETDATE()),
        (37,3,'Parent_event_p1.2',NULL,'2026-01-16 20:35:00.000','2026-01-16 21:35:00.000',0,NULL,60,1,'0',NULL,0,GETDATE(),GETDATE()),
        (38,3,'Parent_event_p1.3',NULL,'2026-01-16 21:40:00.000','2026-01-16 22:40:00.000',0,NULL,60,1,'0',NULL,0,GETDATE(),GETDATE()),
        (39,3,'Parent_event_p1.4',NULL,'2026-01-17 09:35:00.000','2026-01-17 10:35:00.000',0,NULL,60,1,'0',NULL,0,GETDATE(),GETDATE()),

        (41,3,'Parent_event_With_Deadline_1','Deadline 30.01, priority: 3','2026-01-16 13:35:00.000','2026-01-16 14:35:00.000',0,NULL,60,3,'4','2026-01-30',0,GETDATE(),GETDATE()),
        (42,3,'Parent_event_With_Deadline_2','Deadline: 30.01, priority: 2','2026-01-18 12:30:00.000','2026-01-18 13:30:00.000',0,NULL,60,2,'4','2026-01-30',0,GETDATE(),GETDATE()),

        (43,2,'Kid_event_p4.1',NULL,'2026-01-16 09:10:00.000','2026-01-16 09:40:00.000',0,NULL,30,4,'7',NULL,0,GETDATE(),GETDATE()),
        (44,2,'Kid_event_p4.2',NULL,'2026-01-16 09:40:00.000','2026-01-16 10:10:00.000',0,NULL,30,4,'7',NULL,0,GETDATE(),GETDATE()),
        (45,2,'Kid_event_p4.3',NULL,'2026-01-16 10:15:00.000','2026-01-16 10:45:00.000',0,NULL,30,4,'7',NULL,0,GETDATE(),GETDATE()),
        (46,2,'Kid_event_p4.4','Do In Daytime Until 10 AM','2026-01-17 09:10:00.000','2026-01-17 09:40:00.000',1,'10:00:00',30,4,'7',NULL,0,GETDATE(),GETDATE()),

        (47,2,'Kid_event_p3.1',NULL,'2026-01-16 16:40:00.000','2026-01-16 17:10:00.000',0,NULL,30,3,'6',NULL,0,GETDATE(),GETDATE()),
        (48,2,'Kid_event_p3.2',NULL,'2026-01-16 17:10:00.000','2026-01-16 17:40:00.000',0,NULL,30,3,'6',NULL,0,GETDATE(),GETDATE()),
        (49,2,'Kid_event_p3.3',NULL,'2026-01-16 17:45:00.000','2026-01-16 18:15:00.000',0,NULL,30,3,'6',NULL,0,GETDATE(),GETDATE()),
        (50,2,'Kid_event_p3.4',NULL,'2026-01-16 18:15:00.000','2026-01-16 18:45:00.000',0,NULL,30,3,'6',NULL,0,GETDATE(),GETDATE()),

        (51,2,'Kid_event_p2.1',NULL,'2026-01-16 13:10:00.000','2026-01-16 13:40:00.000',0,NULL,30,2,'5',NULL,0,GETDATE(),GETDATE()),
        (52,2,'Kid_event_p2.2',NULL,'2026-01-16 13:40:00.000','2026-01-16 14:10:00.000',0,NULL,30,2,'5',NULL,0,GETDATE(),GETDATE()),
        (53,2,'Kid_event_p2.3',NULL,'2026-01-16 14:15:00.000','2026-01-16 14:45:00.000',0,NULL,30,2,'5',NULL,0,GETDATE(),GETDATE()),
        (54,2,'Kid_event_p2.4',NULL,'2026-01-16 14:45:00.000','2026-01-16 15:15:00.000',0,NULL,30,2,'5',NULL,0,GETDATE(),GETDATE()),

        (55,2,'Kid_event_p1.1',NULL,'2026-01-16 10:45:00.000','2026-01-16 11:15:00.000',0,NULL,30,1,'0',NULL,0,GETDATE(),GETDATE()),
        (56,2,'Kid_event_p1.2',NULL,'2026-01-16 11:20:00.000','2026-01-16 11:50:00.000',0,NULL,30,1,'0',NULL,0,GETDATE(),GETDATE()),
        (57,2,'Kid_event_p1.3',NULL,'2026-01-16 11:50:00.000','2026-01-16 12:20:00.000',0,NULL,30,1,'0',NULL,0,GETDATE(),GETDATE()),
        (58,2,'Kid_event_p1.4',NULL,'2026-01-16 12:25:00.000','2026-01-16 12:55:00.000',0,NULL,30,1,'0',NULL,0,GETDATE(),GETDATE()),

        (59,2,'Kid_event_With_Deadline_1','priority p3, deadline: 30.01','2026-01-16 18:50:00.000','2026-01-16 19:20:00.000',0,NULL,30,3,'8','2026-01-30',0,GETDATE(),GETDATE()),
        (60,2,'Kid_event_With_Deadline_2','priority: p2, deadline: 30.01','2026-01-16 15:20:00.000','2026-01-16 15:50:00.000',0,NULL,30,2,'8','2026-01-30',0,GETDATE(),GETDATE());

        SET IDENTITY_INSERT dbo.Events OFF;
    END

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;
END CATCH;
GO
