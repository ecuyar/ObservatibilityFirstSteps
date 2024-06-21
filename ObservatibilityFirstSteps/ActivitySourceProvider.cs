namespace ObservatibilityFirstSteps
{
	//ActivitySourceProvider must be created once. So make it static or Singleton
	//Any activity that is created with ActivitySourceProvider will be watched from OpenTelemetry. 
	internal static class ActivitySourceProvider
	{
		public static ActivitySource Source = new(OpenTelemetryConstants.ActivitySourceName);
		public static ActivitySource CustomSource = new(OpenTelemetryConstants.CustomActivitySourceName);
	}
}
