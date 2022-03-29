namespace Homework.Settings
{
    public class AppSettings
    {
        public AppSettings_FilePaths FilePaths { get; set; }

        public AppSettings()
        {
            FilePaths = new AppSettings_FilePaths();
        }
    }
}
