using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace MyCoffeeNote.Domain.Entities
{
    /// <summary>
    /// Единица рецепта
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Список столбцов с названиями
        /// </summary>
        public Dictionary<string, string>? Columns { get; set; }
    }
}