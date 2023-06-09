﻿using CommunityToolkit.Maui;

namespace FleetCoreMAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Quicksand-VariableFont_wght.ttf", "Quicksand");
                fonts.AddFont("SegoeWP.ttf", "Segoe");
            })
            .UseMauiCommunityToolkit();

        return builder.Build();
		
	}
}
