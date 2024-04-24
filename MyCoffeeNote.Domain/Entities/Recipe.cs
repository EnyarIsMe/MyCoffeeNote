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
        /// Описание
        /// </summary>
        public string? Description { get; set; }
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
