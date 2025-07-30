[TestMethod]
public void FullOptimise_P90UIShouldBeAsExpected()
{
    var dashboard = this.App.Start(ScenarioNames.FullOptimise1HPass, PlaybackSpeed.Speed100X, TestOptions.UniqueInstanceForTest).LoginResearchSuccessfully();
    dashboard.Shell().WaitForInstrumentToBeConnected();
    var fullOptimiseScreen = dashboard.Shell().ApplicationMenu().SystemSetupsMenu().NavigateToFullOptimise();

    var tuneAndMatch = fullOptimiseScreen.Sample().SampleInserted();
    tuneAndMatch.Continue();

    // Get the P90 screen reference but don't immediately check for it
    var p90Screen = fullOptimiseScreen.P90Chart();
    
    Console.WriteLine($"Debug info before waiting: {p90Screen.GetDebugInfo()}");

    // Wait for the P90 chart to appear during the optimisation process
    var chartAppeared = p90Screen.WaitForP90ChartToAppear(60000); // 60 second timeout
    
    Console.WriteLine($"Chart appeared: {chartAppeared}");
    Console.WriteLine($"Debug info after waiting: {p90Screen.GetDebugInfo()}");

    if (chartAppeared)
    {
        // Chart appeared, now verify it's visible
        p90Screen.P90ChartIsVisible().Should().BeTrue("P90 chart should be visible when optimization is running");
        
        // Optionally check header as well
        // p90Screen.P90HeaderIsVisible().Should().BeTrue("P90 header should be visible when optimization is running");
        
        Console.WriteLine("P90 chart verification passed");
    }
    else
    {
        // Chart didn't appear, this might indicate a problem with timing or the wrong UI element
        Console.WriteLine("P90 chart did not appear within timeout");
        Console.WriteLine($"Final debug info: {p90Screen.GetDebugInfo()}");
        
        // You might want to fail the test here or investigate further
        Assert.Fail("P90 chart did not appear during the full optimisation process");
    }

    fullOptimiseScreen.CancelWithConfirmation().Confirm();
}