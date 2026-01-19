// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET

namespace TimeManager.Models
{
    /// <summary>
    /// Model użytkownika systemu.
    /// 
    /// Role: Kid (dziecko), User (rodzic/użytkownik), Administrator
    /// </summary>
    public class User
    {
        // ID użytkownika w bazie
        public int UserId { get; set; }
        // Nazwa użytkownika (login)
        public string UserName { get; set; }
        // Rola: "Kid", "User", "Administrator"
        public string Role { get; set; }
        // Hash hasła (SHA256)
        public string PasswordHash { get; set; }
        // Godzina rozpoczęcia snu (dla algorytmu planowania)
        public TimeSpan? SleepStart { get; set; }
        // Godzina zakończenia snu
        public TimeSpan? SleepEnd { get; set; }
        // ID rodzica (dla kont dzieci)
        public int? ParentId { get; set; }
    }
}
