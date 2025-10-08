// <copyright file="MachineDashboard.razor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Components.Pages
{
    using IndTrace.Domain.Entities;
    using Microsoft.AspNetCore.Components;
    using MudBlazor;

    /// <summary>
    /// Dashboard component for displaying machine OEE (Overall Equipment Effectiveness) data and management.
    /// </summary>
    public partial class MachineDashboard
    {
        private MudTheme theme = new();
        private bool isDarkMode;

        /// <summary>
        /// Gets or sets the event callback for theme changes.
        /// </summary>
        [Parameter]
        public EventCallback<string> OnThemeChanged { get; set; }

        /// <summary>
        /// Handles theme changes and updates the MudBlazor theme configuration.
        /// </summary>
        /// <param name="theme">The theme name to apply.</param>
        private void HandleThemeChange(string theme)
        {
            this.isDarkMode = theme == "dark";

            this.theme = new MudTheme
            {
                PaletteLight = new PaletteLight
                {
                    Background = "#f4f4f4",
                    Primary = "#003366",
                    AppbarBackground = "#e0e0e0",
                },
                PaletteDark = new PaletteDark
                {
                    Background = "#2a2a35",
                    Primary = "#00ff99",
                    AppbarBackground = "#1f1f1f",
                },
            };

            // Optionally customize the blue variant via dark/light toggle
            if (theme == "blue")
            {
                this.theme.PaletteDark.Primary = "#00cfff";
                this.theme.PaletteDark.Background = "#001f3f";
            }
        }

        /// <summary>
        /// Gets or sets the currently selected theme.
        /// </summary>
        public string SelectedTheme { get; set; } = "dark";

        /// <summary>
        /// Gets or sets the collection of machine data to display on the dashboard.
        /// </summary>
        public List<MachineWidgetData> Machines { get; set; } =
        [
            new MachineWidgetData
            {
                MachineId = "MC-101",
                Status = "Running",
                OEE = 82.4,
                Availability = 90.0,
                Performance = 85.0,
                Quality = 95.0,
                OeeTrend = [78, 79, 81, 82, 84, 83, 82],
            },

            new MachineWidgetData
            {
                MachineId = "MC-202",
                Status = "Stopped",
                OEE = 45.0,
                Availability = 50.0,
                Performance = 48.0,
                Quality = 90.0,
                OeeTrend = [50, 48, 47, 46, 45, 44, 43],
            },

            new MachineWidgetData
            {
                MachineId = "MC-303",
                Status = "Running",
                OEE = 68.3,
                Availability = 70.0,
                Performance = 65.0,
                Quality = 90.0,
                OeeTrend = [65, 66, 67, 68, 69, 68, 68],
            }
        ];
    }
}
