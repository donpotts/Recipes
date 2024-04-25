using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorRecipes.Shared.Blazor.Services
{
    public class TagService
    {
        public string SelectedTag { get; private set; }

        public event Action OnChange;

        public void SelectTag(string tag)
        {
            SelectedTag = tag;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}