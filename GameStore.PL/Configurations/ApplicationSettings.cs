using GameStore.BLL.Settings;
using GameStore.DAL.Settings;

namespace GameStore.PL.Configurations
{
    public class ApplicationSettings
    {
        public DownloadingFileSettings DownloadingFileSettings { get; set; } = new DownloadingFileSettings();
        
        public BanSettings BanSettings { get; set; } = new BanSettings();
        
        public DateOfPublishingFilterSettings DateOfPublishingFilterSettings { get; set; } = new DateOfPublishingFilterSettings();
        
        public GamesPerPageSettings GamesPerPageSettings { get; set; } = new GamesPerPageSettings();
        
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();

        public MongoDbSettings MongoDbSettings { get; set; } = new MongoDbSettings();

        public MongoIntegrationSettings MongoIntegrationSettings { get; set; } = new MongoIntegrationSettings();

        public GuestCookieSettings GuestCookieSettings { get; set; } = new GuestCookieSettings();
    }
}
