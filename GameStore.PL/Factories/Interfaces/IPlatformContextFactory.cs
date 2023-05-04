using GameStore.PL.ViewContexts;
using System.Threading.Tasks;

namespace GameStore.PL.Factories.Interfaces
{
    public interface IPlatformContextFactory
    {
        Task<GamesPlatformDetailsViewContext> BuildGamesPlatformDetailsViewContextAsync(string platform, string localizationCultureCode);
        Task<PlatformTypeEditingViewContext> BuildPlatformTypeEditingViewContextAsync(string type);
    }
}
