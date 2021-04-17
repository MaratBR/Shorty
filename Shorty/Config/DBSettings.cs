using System.ComponentModel.DataAnnotations;

namespace Shorty.Config
{
    public class DBSettings
    {
        public enum DBType
        {
            Mssql,
            Sqlite,
            InMemory
        }
        
        [Required]
        public DBType Type { get; set; }
    }
}