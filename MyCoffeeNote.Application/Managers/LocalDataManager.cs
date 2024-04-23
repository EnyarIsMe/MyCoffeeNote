using System.Collections.ObjectModel;
using Blazored.LocalStorage;
using MyCoffeeNote.Domain.Contracts;
using MyCoffeeNote.Domain.Entities;

namespace MyCoffeeNote.Application.Managers
{
    /// <summary>
    /// Менеджер рецептов
    /// </summary>
    public class LocalDataManager : IDataManager
    {
        /// <summary>
        /// Имя для словаря локального хранилища
        /// </summary>
        private const string storageKeyName = "DataList";
        /// <summary>
        /// Кеш данных
        /// </summary>
        private ObservableCollection<Recipe>? DataCache { get; set; }
        /// <summary>
        /// Сервис работы с локальных хранилищем пользователя
        /// </summary>
        private readonly ISyncLocalStorageService storageService;

        public LocalDataManager(ISyncLocalStorageService localStorageService)
        {
            storageService = localStorageService;
        }

        /// <summary>
        /// Получение списка рецептов
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Recipe> GetAllRecipes()
        {
            return DataCache ??= storageService.GetItem<ObservableCollection<Recipe>>(storageKeyName) ?? new();
        }

        /// <summary>
        /// Запоминание списка рецептов в локальном хранилище
        /// </summary>
        /// <returns></returns>
        private void SetAllRecipes(ObservableCollection<Recipe> dataList) => storageService.SetItem(storageKeyName, dataList);

        /// <summary>
        /// Запоминание рецепта
        /// </summary>
        /// <returns>успешность запоминания</returns>
        public bool SetRecipe(Recipe newRecipe)
        {
            ObservableCollection<Recipe> allRecipes = GetAllRecipes();
            Recipe? foundRecipe;
            try
            {
                foundRecipe = allRecipes.SingleOrDefault(recipe => recipe.Id == newRecipe.Id);
            }
            catch (InvalidOperationException)
            {
                //получили одинаковые id
                foreach (Recipe sameIdRecipe in allRecipes.Where(recipe => recipe.Id == newRecipe.Id))
                {
                    sameIdRecipe.Id = new Guid();
                }

                return false;
            }

            if (foundRecipe == null)
            {
                allRecipes.Add(newRecipe);
            }
            else
            {
                foundRecipe.Description = newRecipe.Description;
            }

            try
            {
                SetAllRecipes(allRecipes);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Создание нового рецепта
        /// </summary>
        /// <returns>успешность запоминания</returns>
        public void AddEmptyRecipe() => SetRecipe(new() { Id = Guid.NewGuid(), CreationDate = DateTime.Now});
        /// <summary>
        /// Удаление рецепта
        /// </summary>
        /// <returns>успешность запоминания</returns>
        public void RemoveRecipe(Recipe recipe)
        {
            ObservableCollection<Recipe> allRecipes = GetAllRecipes();
            bool remove = allRecipes.Remove(recipe);
            if (remove)
            {
                SetAllRecipes(allRecipes);
            }
        }
    }
}
