using System.Collections.ObjectModel;
using MyCoffeeNote.Domain.Entities;

namespace MyCoffeeNote.Domain.Contracts
{
    public interface IDataManager
    {
        /// <summary>
        /// Получение всех рецептов
        /// </summary>
        public ObservableCollection<Recipe> GetAllRecipes();
        
        /// <summary>
        /// Запоминание рецепта
        /// </summary>
        /// <returns>успешность запоминания</returns>
        public bool SetRecipe(Recipe newRecipe);
        /// <summary>
        /// Запоминание рецепта
        /// </summary>
        /// <returns>успешность запоминания</returns>
        public void AddEmptyRecipe();
        /// <summary>
        /// Удаление рецепта
        /// </summary>
        /// <param name="recipe"></param>
        public void RemoveRecipe(Recipe recipe);
        /// <summary>
        /// Получение уникальных столбцов
        /// </summary>
        public ObservableCollection<string> GetUniqColumns();

        public delegate void ColumnsUpdate(ObservableCollection<string> newColumnList);
        public event ColumnsUpdate Notify;
    }
}
