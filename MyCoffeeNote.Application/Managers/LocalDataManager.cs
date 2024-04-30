using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

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
        /// Кеш данных столбцов
        /// </summary>
        private ObservableCollection<string>? ColumnsCache { get; set; }
        public event IDataManager.ColumnsUpdate Notify;
        
        /// <summary>
        /// Сервис работы с локальных хранилищем пользователя
        /// </summary>
        private readonly ISyncLocalStorageService storageService;

        public LocalDataManager(ISyncLocalStorageService localStorageService)
        {
            storageService = localStorageService;
            InitColumnsAutoUpdate();
        }

        #region Действия с столбцами
        
        /// <summary>
        /// Получение уникальных столбцов
        /// </summary>
        public ObservableCollection<string> GetUniqColumns() => new(ColumnsCache.Distinct());


        /// <summary>
        /// Запуск автообновления списка колонок
        /// </summary>
        private void InitColumnsAutoUpdate()
        {
            GetAllRecipes();
            AddAllColumnsToCache();

            DataCache.CollectionChanged += UpdateColumnsCacheOnDataUpdate;
            ColumnsCache.CollectionChanged += (sender, args) => Notify?.Invoke(new(ColumnsCache.Distinct()));
        }
        /// <summary>
        /// Добавляем все столбцы(с повторением) в кеш
        /// </summary>
        private void AddAllColumnsToCache()
        {
            List<string> columns = new();
            foreach (Recipe recipe in DataCache)
            {
                if (recipe.Columns is not null)
                {
                    columns.AddRange(recipe.Columns.Keys);
                }
            }
            ColumnsCache = new(columns);
        }

        private void UpdateColumnsCacheOnDataUpdate(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object newItems in e.NewItems)
                    {
                        if (newItems is not Recipe recipe) continue;
                        if (recipe.Columns is null) continue;
                        foreach (string key in recipe.Columns.Keys)
                        {
                            ColumnsCache.Add(key);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:

                    foreach (object oldItem in e.OldItems)
                    {
                        if (oldItem is not Recipe recipe) continue;
                        if (recipe.Columns is null) continue;
                        foreach (string key in recipe.Columns.Keys)
                        {
                            ColumnsCache.Remove(key);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    AddAllColumnsToCache();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Обновление ячейки
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="columnName"></param>
        /// <param name="context"></param>
        public void UpdateCellValue(string newValue, string columnName, Recipe context)
        {
            ObservableCollection<Recipe> collection = GetAllRecipes();
            Recipe recipeToEdit = collection.Single(recipe => recipe.Id == context.Id);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                recipeToEdit.Columns!.Remove(columnName);
                ColumnsCache.Remove(columnName);
            }
            else
            {
                recipeToEdit.Columns ??= new();
                if (recipeToEdit.Columns!.ContainsKey(columnName))
                {
                    recipeToEdit.Columns[columnName] = newValue;
                }
                else
                {
                    recipeToEdit.Columns.Add(columnName, newValue);
                    ColumnsCache.Add(columnName);
                }
            }
            SetAllRecipes(collection);
        }

        #endregion Действия с столбцами

        #region Действия с рецептами

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
        public void AddEmptyRecipe() => SetRecipe(new() { Id = Guid.NewGuid(), CreationDate = DateTime.Now });

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

        #endregion Действия с рецептами

        public bool IsDarkMode
        {
            get => storageService.GetItem<bool>("isDarkMode");
            set => storageService.SetItem("isDarkMode", value);
        } 
    }
}
