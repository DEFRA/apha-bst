namespace Apha.BST.Web.PresentationService
{
    public interface IUserDataService
    {
        string? GetUsername();
        Task<bool> CanEditPage(string action);
    }
}
