namespace Angular.App.Services.Options
{
    public class BrowserCheckServiceOptions
    {
        public List<InvalidBrowser> InvalidBrowsersList { get; set; } = new(); // List of invalid browsers from config
    }

    public class InvalidBrowser
    {
        public required string Vendor { get; set; } // Vendor name (e.g., "Firefox")
        public required List<string> Versions { get; set; } // List of invalid versions
    }
}
