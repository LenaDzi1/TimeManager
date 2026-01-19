// ============================================================================
// HomeLayoutService.cs
// Serwis zarządzania układem domu: piętra, kontenery, przedmioty.
// ============================================================================

#nullable enable

#region Importy
using System;                   // Podstawowe typy (DateTime, Decimal)
using System.Collections.Generic; // Kolekcje (List, Dictionary)
using System.Data;              // Typy bazodanowe (DataRow, DataTable)
using System.Data.SqlClient;    // Klient SQL Server
using System.Linq;              // LINQ (Select, Where, Cast)
using TimeManager.Database;     // DatabaseHelper
using TimeManager.Models;       // LayoutModel, FloorModel, ContainerModel, etc.
#endregion

namespace TimeManager.Services
{
    /// <summary>
    /// Serwis do zarządzania układem domu - piętra, pokoje, kontenery, przedmioty.
    /// 
    /// STRUKTURA DANYCH:
    /// - LayoutModel: główny layout domu (jeden na użytkownika)
    /// - FloorModel: piętra w domu (np. parter, piętro)
    /// - WallModel: ściany na piętrze (linie rysowane na mapie)
    /// - ContainerModel: kontenery (szafki, półki) - mogą być zagnieżdżone
    /// - ItemInventoryModel: przedmioty w kontenerach
    /// 
    /// FUNKCJE:
    /// - Tworzenie i zapisywanie layoutu domu
    /// - Zarządzanie piętrami, ścianami i kontenerami
    /// - Zagnieżdżone kontenery (ParentContainerId)
    /// - Wyszukiwanie przedmiotów po nazwie
    /// - Automatyczne tworzenie tabel w bazie danych
    /// </summary>
    public class HomeLayoutService
    {
        #region Pola

        /// <summary>
        /// Flaga sprawdzająca czy tabele już zostały utworzone (cache).
        /// Thread-safe dzięki lock w EnsureTablesExist().
        /// </summary>
        private static bool _tablesChecked = false;

        #endregion

        #region Konstruktor

        /// <summary>
        /// Konstruktor - zapewnia że wymagane tabele istnieją.
        /// </summary>
        public HomeLayoutService()
        {
            EnsureTablesExist();
        }

        #endregion

        #region Inicjalizacja bazy danych

        /// <summary>
        /// Tworzy wymagane tabele w bazie danych jeśli nie istnieją.
        /// 
        /// Tabele: HomeLayouts, Floors, Walls, Containers, Items, ItemInventory
        /// Obsługuje też migracje (np. dodanie kolumny ParentContainerID).
        /// 
        /// UWAGA: Thread-safe dzięki lock.
        /// </summary>
        private void EnsureTablesExist()
        {
            // Skip jeśli już sprawdzono
            if (_tablesChecked) return;

            // Thread-safe inicjalizacja
            lock (typeof(HomeLayoutService))
            {
                if (_tablesChecked) return;

                using var connection = DatabaseHelper.GetConnection();
                connection.Open();

                try
                {
                   


                    
                  
                   
                }
                catch (SqlException)
                {
                    // Tabele mogą już istnieć - ignoruj błąd
                }
                catch (Exception)
                {
                    // Nieoczekiwany błąd - próbuj kontynuować
                }
            }
        }

        #endregion

        #region Operacje na layoutach i piętrach

        /// <summary>
        /// Ładuje lub tworzy layout domu.
        /// Jeśli nie ma żadnych pięter - tworzy domyślne piętro parterowe.
        /// </summary>
        public LayoutModel LoadOrCreateLayout()
        {
            // Upewnij się że tabele istnieją przed operacjami
            EnsureTablesExist();
            
            var layoutId = EnsureLayoutRow();
            var layout = GetLayout(layoutId);
            layout.Floors = LoadFloors(layout.LayoutId);

            if (!layout.Floors.Any())
            {
                var defaultFloor = CreateFloor(layout.LayoutId, 0, "Ground", layout.DefaultWidthMeters, layout.DefaultHeightMeters);
                layout.Floors.Add(defaultFloor);
            }

            return layout;
        }

        public FloorModel AddFloor(LayoutModel layout, string? title = null)
        {
            var nextNumber = layout.Floors.Any() ? layout.Floors.Max(f => f.FloorNumber) + 1 : 0;
            var floor = CreateFloor(layout.LayoutId, nextNumber, title ?? "New floor", layout.DefaultWidthMeters, layout.DefaultHeightMeters);
            layout.Floors.Add(floor);
            return floor;
        }

        public void SaveLayout(LayoutModel layout)
        {
            // Upewnij się że tabele istnieją przed zapisem
            EnsureTablesExist();
            
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var tx = connection.BeginTransaction();

            var updateLayout = new SqlCommand(
                "UPDATE HomeLayouts SET DefaultWidthMeters=@w, DefaultHeightMeters=@h WHERE LayoutID=@id",
                connection, tx);
            updateLayout.Parameters.AddWithValue("@w", layout.DefaultWidthMeters);
            updateLayout.Parameters.AddWithValue("@h", layout.DefaultHeightMeters);
            updateLayout.Parameters.AddWithValue("@id", layout.LayoutId);
            updateLayout.ExecuteNonQuery();

            foreach (var floor in layout.Floors)
            {
                SaveFloor(connection, tx, floor, layout.LayoutId);
            }

            tx.Commit();
        }

        public List<ItemInventoryModel> SearchItems(string term)
        {
            // Upewnij się że tabele istnieją przed wyszukiwaniem
            EnsureTablesExist();
            
            var results = new List<ItemInventoryModel>();
            term ??= string.Empty;
            
            if (string.IsNullOrWhiteSpace(term))
            {
                return results;
            }

            const string query = @"
SELECT inv.InventoryID, inv.ItemID, inv.Quantity, inv.Unit,
       i.Name AS ItemName,
       c.Name AS ContainerName, c.ContainerID,
       f.FloorNumber, c.RoomTag
FROM ItemInventory inv
JOIN Items i ON i.ItemID = inv.ItemID
JOIN Containers c ON c.ContainerID = inv.ContainerID
JOIN Floors f ON f.FloorID = c.FloorID
WHERE i.Name LIKE @term
ORDER BY i.Name";

            var parameter = new SqlParameter("@term", SqlDbType.NVarChar, 120) { Value = $"%{term}%" };
            var table = DatabaseHelper.ExecuteQuery(query, parameter);
            foreach (DataRow row in table.Rows)
            {
                results.Add(new ItemInventoryModel
                {
                    InventoryId = Convert.ToInt32(row["InventoryID"]),
                    ItemId = Convert.ToInt32(row["ItemID"]),
                    ItemName = row["ItemName"].ToString() ?? string.Empty,
                    Quantity = Convert.ToDecimal(row["Quantity"]),
                    Unit = row["Unit"].ToString() ?? "pcs",
                    ContainerName = row["ContainerName"].ToString(),
                    FloorNumber = Convert.ToInt32(row["FloorNumber"]),
                    RoomTag = row["RoomTag"] as string,
                    ContainerId = Convert.ToInt32(row["ContainerID"])
                });
            }

            return results;
        }

        #endregion

        #region Zapisywanie danych (SaveFloor, ExecuteDelete)

        /// <summary>
        /// Zapisuje piętro do bazy danych.
        /// Obsługuje INSERT (nowe) lub UPDATE (istniejące) + synchronizację ścian i kontenerów.
        /// </summary>
        private static void SaveFloor(SqlConnection connection, SqlTransaction tx, FloorModel floor, int layoutId)
        {
            if (floor.FloorId == 0)
            {
                var insertFloor = new SqlCommand(
                    @"INSERT INTO Floors(LayoutID, FloorNumber, Title, WidthMeters, HeightMeters)
                      OUTPUT INSERTED.FloorID
                      VALUES(@layoutId, @number, @title, @w, @h)",
                    connection, tx);

                insertFloor.Parameters.AddWithValue("@layoutId", layoutId);
                insertFloor.Parameters.AddWithValue("@number", floor.FloorNumber);
                insertFloor.Parameters.AddWithValue("@title", (object?)floor.Title ?? DBNull.Value);
                insertFloor.Parameters.AddWithValue("@w", floor.WidthMeters);
                insertFloor.Parameters.AddWithValue("@h", floor.HeightMeters);
                floor.FloorId = Convert.ToInt32(insertFloor.ExecuteScalar());
            }
            else
            {
                var updateFloor = new SqlCommand(
                    @"UPDATE Floors
                      SET FloorNumber=@number, Title=@title, WidthMeters=@w, HeightMeters=@h
                      WHERE FloorID=@id",
                    connection, tx);

                updateFloor.Parameters.AddWithValue("@number", floor.FloorNumber);
                updateFloor.Parameters.AddWithValue("@title", (object?)floor.Title ?? DBNull.Value);
                updateFloor.Parameters.AddWithValue("@w", floor.WidthMeters);
                updateFloor.Parameters.AddWithValue("@h", floor.HeightMeters);
                updateFloor.Parameters.AddWithValue("@id", floor.FloorId);
                updateFloor.ExecuteNonQuery();

                ExecuteDelete(connection, tx, "DELETE FROM Walls WHERE FloorID=@id", floor.FloorId);
                ExecuteDelete(connection, tx, "DELETE FROM ItemInventory WHERE ContainerID IN (SELECT ContainerID FROM Containers WHERE FloorID=@id)", floor.FloorId);
                ExecuteDelete(connection, tx, "DELETE FROM Containers WHERE FloorID=@id", floor.FloorId);
            }

            foreach (var wall in floor.Walls)
            {
                var insertWall = new SqlCommand(
                    @"INSERT INTO Walls(FloorID, StartX, StartY, EndX, EndY)
                      OUTPUT INSERTED.WallID
                      VALUES(@floor, @sx, @sy, @ex, @ey)",
                    connection, tx);
                insertWall.Parameters.AddWithValue("@floor", floor.FloorId);
                insertWall.Parameters.AddWithValue("@sx", wall.StartX);
                insertWall.Parameters.AddWithValue("@sy", wall.StartY);
                insertWall.Parameters.AddWithValue("@ex", wall.EndX);
                insertWall.Parameters.AddWithValue("@ey", wall.EndY);
                wall.WallId = Convert.ToInt32(insertWall.ExecuteScalar());
            }

            // Upewnij się że kolumna ParentContainerID istnieje przed zapisem
            EnsureParentContainerIdColumn(connection, tx);
            
            // Pierwszy przebieg: Zapisz wszystkie kontenery i pobierz ich nowe ID
            var containerIdMap = new Dictionary<int, int>();
            var containerParentMap = new Dictionary<int, int?>();
            
            foreach (var container in floor.Containers)
            {
                var oldId = container.ContainerId;
                
                var insertContainer = new SqlCommand(
                    @"INSERT INTO Containers(FloorID, Name, RoomTag, PointAX, PointAY, PointBX, PointBY, IsVisible, ParentContainerID)
                      OUTPUT INSERTED.ContainerID
                      VALUES(@floor, @name, @room, @ax, @ay, @bx, @by, @visible, NULL)",
                    connection, tx);
                insertContainer.Parameters.AddWithValue("@floor", floor.FloorId);
                insertContainer.Parameters.AddWithValue("@name", container.Name);
                insertContainer.Parameters.AddWithValue("@room", (object?)container.RoomTag ?? DBNull.Value);
                insertContainer.Parameters.AddWithValue("@ax", container.PointAX);
                insertContainer.Parameters.AddWithValue("@ay", container.PointAY);
                insertContainer.Parameters.AddWithValue("@bx", container.PointBX);
                insertContainer.Parameters.AddWithValue("@by", container.PointBY);
                insertContainer.Parameters.AddWithValue("@visible", container.IsVisible);
                var newId = Convert.ToInt32(insertContainer.ExecuteScalar());
                
                container.ContainerId = newId;
                // Mapuj zarówno zapisane ID (dodatnie) jak i tymczasowe ID w pamięci (ujemne),
                // żeby mapowanie ParentContainerId działało nawet dla nowo utworzonych drzew kontenerów.
                if (oldId != 0)
                {
                    containerIdMap[oldId] = newId;
                }
                
                // Zapisz stare ID rodzica do późniejszego mapowania
                containerParentMap[newId] = container.ParentContainerId;

                // Zapisz przedmioty dla tego kontenera
                foreach (var inventory in container.Items)
                {
                    var itemId = EnsureItem(connection, tx, inventory.ItemName, inventory.Unit);
                    var insertInventory = new SqlCommand(
                        @"INSERT INTO ItemInventory(ItemID, ContainerID, Quantity, Unit)
                          VALUES(@item, @container, @qty, @unit)",
                        connection, tx);
                    insertInventory.Parameters.AddWithValue("@item", itemId);
                    insertInventory.Parameters.AddWithValue("@container", container.ContainerId);
                    insertInventory.Parameters.AddWithValue("@qty", inventory.Quantity);
                    insertInventory.Parameters.AddWithValue("@unit", inventory.Unit);
                    insertInventory.ExecuteNonQuery();
                }
            }
            
            // Drugi przebieg: Zaktualizuj ParentContainerID z nowymi zmapowanymi ID
            foreach (var kvp in containerParentMap)
            {
                var newContainerId = kvp.Key;
                var oldParentId = kvp.Value;
                
                if (oldParentId.HasValue && containerIdMap.ContainsKey(oldParentId.Value))
                {
                    var newParentId = containerIdMap[oldParentId.Value];
                    var updateParent = new SqlCommand(
                        @"UPDATE Containers SET ParentContainerID = @parent WHERE ContainerID = @child",
                        connection, tx);
                    updateParent.Parameters.AddWithValue("@parent", newParentId);
                    updateParent.Parameters.AddWithValue("@child", newContainerId);
                    updateParent.ExecuteNonQuery();
                    
                    // Zaktualizuj też model
                    var container = floor.Containers.FirstOrDefault(c => c.ContainerId == newContainerId);
                    if (container != null)
                    {
                        container.ParentContainerId = newParentId;
                    }
                }
            }
        }

        private static void EnsureParentContainerIdColumn(SqlConnection connection, SqlTransaction tx)
        {
            // Sprawdź czy kolumna istnieje
            var checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('Containers') AND name = 'ParentContainerID'",
                connection, tx);
            var columnExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            
            if (!columnExists)
            {
                // Dodaj kolumnę
                var addColumnCmd = new SqlCommand(
                    "ALTER TABLE Containers ADD ParentContainerID INT NULL",
                    connection, tx);
                addColumnCmd.ExecuteNonQuery();
                
                // Dodaj constraint klucza obcego
                try
                {
                    var addFkCmd = new SqlCommand(
                        "ALTER TABLE Containers ADD CONSTRAINT FK_Containers_ParentContainer FOREIGN KEY (ParentContainerID) REFERENCES Containers(ContainerID) ON DELETE SET NULL",
                        connection, tx);
                    addFkCmd.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    // Constraint może już istnieć lub mogą być problemy z danymi, kontynuuj
                }
            }
        }

        private static void ExecuteDelete(SqlConnection connection, SqlTransaction tx, string sql, int floorId)
        {
            var cmd = new SqlCommand(sql, connection, tx);
            cmd.Parameters.AddWithValue("@id", floorId);
            cmd.ExecuteNonQuery();
        }

        private static int EnsureItem(SqlConnection connection, SqlTransaction tx, string name, string unit)
        {
            var select = new SqlCommand("SELECT ItemID FROM Items WHERE Name=@name", connection, tx);
            select.Parameters.AddWithValue("@name", name);
            var existing = select.ExecuteScalar();
            if (existing != null && existing != DBNull.Value)
            {
                return Convert.ToInt32(existing);
            }

            var insert = new SqlCommand(
                "INSERT INTO Items(Name, DefaultUnit) OUTPUT INSERTED.ItemID VALUES(@name, @unit)",
                connection, tx);
            insert.Parameters.AddWithValue("@name", name);
            insert.Parameters.AddWithValue("@unit", (object?)unit ?? DBNull.Value);
            return Convert.ToInt32(insert.ExecuteScalar());
        }

        #endregion

        #region Ładowanie danych (Load methods)

        /// <summary>
        /// Pobiera layout po ID.
        /// </summary>
        private static LayoutModel GetLayout(int layoutId)
        {
            const string sql = "SELECT LayoutID, DefaultWidthMeters, DefaultHeightMeters FROM HomeLayouts WHERE LayoutID=@id";
            var table = DatabaseHelper.ExecuteQuery(sql, new SqlParameter("@id", layoutId));
            if (table.Rows.Count == 0)
            {
                throw new InvalidOperationException("Layout row missing.");
            }

            var row = table.Rows[0];
            return new LayoutModel
            {
                LayoutId = Convert.ToInt32(row["LayoutID"]),
                DefaultWidthMeters = Convert.ToDecimal(row["DefaultWidthMeters"]),
                DefaultHeightMeters = Convert.ToDecimal(row["DefaultHeightMeters"])
            };
        }

        private List<FloorModel> LoadFloors(int layoutId)
        {
            // Upewnij się że tabele istnieją przed ładowaniem
            EnsureTablesExist();
            
            const string sql = @"SELECT FloorID, FloorNumber, Title, WidthMeters, HeightMeters
                                 FROM Floors WHERE LayoutID=@id ORDER BY FloorNumber";
            var table = DatabaseHelper.ExecuteQuery(sql, new SqlParameter("@id", layoutId));
            var floors = new List<FloorModel>();
            foreach (DataRow row in table.Rows)
            {
                var floor = new FloorModel
                {
                    FloorId = Convert.ToInt32(row["FloorID"]),
                    FloorNumber = Convert.ToInt32(row["FloorNumber"]),
                    Title = row["Title"] as string,
                    WidthMeters = Convert.ToDecimal(row["WidthMeters"]),
                    HeightMeters = Convert.ToDecimal(row["HeightMeters"])
                };
                floor.Walls = LoadWalls(floor.FloorId);
                floor.Containers = LoadContainers(floor.FloorId);
                floors.Add(floor);
            }
            return floors;
        }

        private List<WallModel> LoadWalls(int floorId)
        {
            const string sql = @"SELECT WallID, StartX, StartY, EndX, EndY FROM Walls WHERE FloorID=@id";
            var table = DatabaseHelper.ExecuteQuery(sql, new SqlParameter("@id", floorId));
            return table.Rows.Cast<DataRow>()
                .Select(r => new WallModel
                {
                    WallId = Convert.ToInt32(r["WallID"]),
                    StartX = Convert.ToSingle(r["StartX"]),
                    StartY = Convert.ToSingle(r["StartY"]),
                    EndX = Convert.ToSingle(r["EndX"]),
                    EndY = Convert.ToSingle(r["EndY"])
                })
                .ToList();
        }

        private List<ContainerModel> LoadContainers(int floorId)
        {
            // Upewnij się że tabele istnieją przed zapytaniem
            EnsureTablesExist();
            
            // Sprawdź czy kolumna ParentContainerID istnieje
            bool hasParentColumn = false;
            try
            {
                using var checkConn = DatabaseHelper.GetConnection();
                checkConn.Open();
                using var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('Containers') AND name = 'ParentContainerID'", 
                    checkConn);
                hasParentColumn = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            }
            catch { }
            
            string sql;
            if (hasParentColumn)
            {
                sql = @"
SELECT c.ContainerID, c.Name, c.RoomTag, c.PointAX, c.PointAY, c.PointBX, c.PointBY, c.IsVisible,
       c.ParentContainerID
FROM Containers c
WHERE c.FloorID=@id";
            }
            else
            {
                sql = @"
SELECT c.ContainerID, c.Name, c.RoomTag, c.PointAX, c.PointAY, c.PointBX, c.PointBY, c.IsVisible,
       NULL AS ParentContainerID
FROM Containers c
WHERE c.FloorID=@id";
            }

            var table = DatabaseHelper.ExecuteQuery(sql, new SqlParameter("@id", floorId));
            var containers = new List<ContainerModel>();

            foreach (DataRow row in table.Rows)
            {
                var container = new ContainerModel
                {
                    ContainerId = Convert.ToInt32(row["ContainerID"]),
                    Name = row["Name"].ToString() ?? string.Empty,
                    RoomTag = row["RoomTag"] as string,
                    PointAX = Convert.ToSingle(row["PointAX"]),
                    PointAY = Convert.ToSingle(row["PointAY"]),
                    PointBX = Convert.ToSingle(row["PointBX"]),
                    PointBY = Convert.ToSingle(row["PointBY"]),
                    IsVisible = Convert.ToBoolean(row["IsVisible"]),
                    ParentContainerId = row["ParentContainerID"] == DBNull.Value || !hasParentColumn
                        ? null
                        : (int?)Convert.ToInt32(row["ParentContainerID"])
                };
                container.Items = LoadItems(container.ContainerId);
                containers.Add(container);
            }

            return containers;
        }

        private List<ItemInventoryModel> LoadItems(int containerId)
        {
            const string sql = @"
SELECT inv.InventoryID, inv.ItemID, inv.Quantity, inv.Unit, i.Name
FROM ItemInventory inv
JOIN Items i ON i.ItemID = inv.ItemID
WHERE inv.ContainerID=@id";

            var table = DatabaseHelper.ExecuteQuery(sql, new SqlParameter("@id", containerId));
            return table.Rows.Cast<DataRow>()
                .Select(r => new ItemInventoryModel
                {
                    InventoryId = Convert.ToInt32(r["InventoryID"]),
                    ItemId = Convert.ToInt32(r["ItemID"]),
                    ItemName = r["Name"].ToString() ?? string.Empty,
                    Quantity = Convert.ToDecimal(r["Quantity"]),
                    Unit = r["Unit"].ToString() ?? "pcs",
                    ContainerId = containerId
                })
                .ToList();
        }

        #endregion

        #region Metody pomocnicze (EnsureLayoutRow, CreateFloor)

        /// <summary>
        /// Zapewnia że istnieje wiersz layoutu w bazie.
        /// Zwraca ID istniejącego lub nowo utworzonego layoutu.
        /// </summary>
        private static int EnsureLayoutRow()
        {
            const string sql = "SELECT TOP 1 LayoutID FROM HomeLayouts ORDER BY LayoutID";
            var table = DatabaseHelper.ExecuteQuery(sql);
            if (table.Rows.Count > 0)
            {
                return Convert.ToInt32(table.Rows[0]["LayoutID"]);
            }

            const string insert = @"INSERT INTO HomeLayouts(DefaultWidthMeters, DefaultHeightMeters)
                                    OUTPUT INSERTED.LayoutID VALUES(10, 10)";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(insert));
        }

        private static FloorModel CreateFloor(int layoutId, int number, string title, decimal width, decimal height)
        {
            const string insert = @"
INSERT INTO Floors(LayoutID, FloorNumber, Title, WidthMeters, HeightMeters)
OUTPUT INSERTED.FloorID
VALUES(@layoutId, @number, @title, @w, @h)";

            var id = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insert,
                new SqlParameter("@layoutId", layoutId),
                new SqlParameter("@number", number),
                new SqlParameter("@title", (object?)title ?? DBNull.Value),
                new SqlParameter("@w", width),
                new SqlParameter("@h", height)));

            return new FloorModel
            {
                FloorId = id,
                FloorNumber = number,
                Title = title,
                WidthMeters = width,
                HeightMeters = height
            };
        }

        #endregion
    }
}

