using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MyCoffeeNote.Domain.Entities;

using System.Collections.ObjectModel;
using MyCoffeeNote.Domain.Contracts;
using System.Xml.Linq;

namespace MyCoffeeNote.Pages
{
    /// <summary>
    /// Страница рецептов
    /// </summary>
    public partial class RecipesPage : ComponentBase
    {
        /// <summary>
        /// Все рецепты
        /// </summary>
        private ObservableCollection<Recipe> AllRecipes { get; set; } = new();
        /// <summary>
        /// Все уникальные столбцы
        /// </summary>
        private ObservableCollection<string> AllUniqColumns { get; set; } = new();
        /// <summary>
        /// Менеджер информации
        /// </summary>
        [Inject]
        private IDataManager DataManager { get; set; }
        /// <summary>
        /// Таблица с рецептами
        /// </summary>
        private MudDataGrid<Recipe> MudDataGrid { get; set; }


        /// <summary>
        /// Получить все рецепты
        /// </summary>
        /// <returns></returns>
        private void GetAllRecipes() => AllRecipes = DataManager.GetAllRecipes();
        /// <summary>
        /// Получить все уникальные столбцы
        /// </summary>
        private void GetAllUniqColumns()
        {
            AllUniqColumns = DataManager.GetUniqColumns();
            DataManager.Notify += collection => AllUniqColumns = collection;
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            GetAllRecipes();
            GetAllUniqColumns();
        }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                MudDataGrid.SetSortAsync(nameof(Recipe.CreationDate), SortDirection.Descending,
                    recipe => recipe.CreationDate).GetAwaiter().GetResult();
            }
        }
        /// <summary>
        /// Действие при внесении изменений в ячейку
        /// </summary>
        /// <param name="obj"></param>
        private void CommittedItemChanges(Recipe obj)
        {
            if (!DataManager.SetRecipe(obj))
            {
                //todo ошибка сохранения в локальное
            }
        }
        /// <summary>
        /// Создание новой строки
        /// </summary>
        private void AddNewLine()
        {
            DataManager.AddEmptyRecipe();
        }
        /// <summary>
        /// Создание новой строки
        /// </summary>
        private void DeleteLine(CellContext<Recipe?> obj)
        {
            if (obj.Item is not null)
            {
                DataManager.RemoveRecipe(obj.Item);
            }
        }
        /// <summary>
        /// Получить значение для ячейки
        /// </summary>
        /// <param name="x"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static string GetCellValue(Recipe x, string column)
        {
            return x.Columns.ContainsKey(column) ? x.Columns[column] : String.Empty;
        }

        private void CellValueChanged(string newValue, string columnName, Recipe context)
        {
            DataManager.UpdateCellValue(newValue, columnName, context);
        }
    }
}