﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MyCoffeeNote.Domain.Entities;

using System.Collections.ObjectModel;
using MyCoffeeNote.Domain.Contracts;

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
        /// Менеджер информации
        /// </summary>
        [Inject]
        private IDataManager DataManager { get; set; }
        /// <summary>
        /// Таблица с рецептами
        /// </summary>
        private MudDataGrid<Recipe> MudDataGrip { get; set; }

        /// <summary>
        /// Получить все рецепты
        /// </summary>
        /// <returns></returns>
        private void GetAllRecipes() => AllRecipes = DataManager.GetAllRecipes();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            GetAllRecipes();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                MudDataGrip.SetSortAsync(nameof(Recipe.CreationDate), SortDirection.Ascending,
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
    }
}