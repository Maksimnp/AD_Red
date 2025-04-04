using ADManager.Services;

namespace ADManager.ViewModels
{
    public class OUViewModel
    {
        private readonly ActiveDirectoryService _adService;
        public List<string> OrganizationalUnits { get; private set; } = new();

        public OUViewModel(ActiveDirectoryService adService)
        {
            _adService = adService;
            LoadOrganizationalUnits();
        }

        private void LoadOrganizationalUnits()
        {
            try
            {
                OrganizationalUnits = _adService.GetOrganizationalUnits();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки подразделений: {ex.Message}", "Ошибка");
            }
        }
    }
}