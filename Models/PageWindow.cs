using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ScrollBase.Models
{
    public class PageWindow : ContentPage
    {
        public string? PageName { get; set; }
        public string? PageLink { get; set; }

        // Added to satisfy bindings in MainPage (ItemsSource expects `Url` and `HeightRequest`)
        public string? Url
        {
            get => PageLink;
            set => PageLink = value;
        }

        public double HeightRequest { get; set; } = 700;

        public PageWindow()
        {
            Title = PageName ?? "Post";
            Content = new StackLayout
            {
                Padding = 20,
                Children =
                {
                    new Label { Text = PageName ?? "Post", FontAttributes = FontAttributes.Bold, FontSize = 18 },
                    new Label { Text = PageLink ?? string.Empty, LineBreakMode = LineBreakMode.WordWrap },
                    new Button
                    {
                        Text = "Close",
                        Command = new Command(async () =>
                        {
                            if (Navigation != null)
                                await Navigation.PopModalAsync();
                        })
                    }
                }
            };
        }
    }
}
