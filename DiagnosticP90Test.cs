[TestMethod]
public void FullOptimise_P90UI_DiagnosticTest()
{
    var dashboard = this.App.Start(ScenarioNames.FullOptimise1HPass, PlaybackSpeed.Speed100X, TestOptions.UniqueInstanceForTest).LoginResearchSuccessfully();
    dashboard.Shell().WaitForInstrumentToBeConnected();
    var fullOptimiseScreen = dashboard.Shell().ApplicationMenu().SystemSetupsMenu().NavigateToFullOptimise();

    var tuneAndMatch = fullOptimiseScreen.Sample().SampleInserted();
    
    Console.WriteLine("=== Before clicking Continue ===");
    LogAllVisibleElements();
    
    tuneAndMatch.Continue();
    
    Console.WriteLine("=== After clicking Continue ===");
    
    // Wait a bit for the optimization to start
    Thread.Sleep(2000);
    
    // Try to get P90 screen multiple times during the process
    for (int i = 0; i < 30; i++) // Check for 30 seconds
    {
        Console.WriteLine($"=== Check #{i + 1} (Time: {DateTime.Now:HH:mm:ss.fff}) ===");
        
        try
        {
            var p90Screen = fullOptimiseScreen.P90Chart();
            Console.WriteLine($"P90 Screen Debug: {p90Screen.GetDebugInfo()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception getting P90 screen: {ex.Message}");
        }
        
        // Log all elements with "P90" in their automation ID
        LogElementsContaining("P90");
        
        // Log all elements with "Chart" in their automation ID
        LogElementsContaining("Chart");
        
        // Log all elements with "Optimise" or "Optimize" in their automation ID
        LogElementsContaining("Optimi");
        
        Thread.Sleep(1000); // Wait 1 second between checks
    }
    
    Console.WriteLine("=== Final Check ===");
    LogAllVisibleElements();
    
    fullOptimiseScreen.CancelWithConfirmation().Confirm();
}

private void LogAllVisibleElements()
{
    try
    {
        // This method would need to be implemented based on your testing framework
        // Example using Appium WindowsDriver:
        var allElements = this.Driver.FindElementsByXPath("//*[@AutomationId]");
        
        Console.WriteLine($"Found {allElements.Count} elements with AutomationId:");
        
        foreach (var element in allElements.Take(50)) // Limit to first 50 to avoid spam
        {
            try
            {
                var automationId = element.GetAttribute("AutomationId");
                var isDisplayed = element.Displayed;
                var className = element.GetAttribute("ClassName");
                Console.WriteLine($"  - AutomationId: {automationId}, Displayed: {isDisplayed}, Class: {className}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  - Error reading element: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in LogAllVisibleElements: {ex.Message}");
    }
}

private void LogElementsContaining(string searchTerm)
{
    try
    {
        var xpath = $"//*[contains(@AutomationId, '{searchTerm}')]";
        var elements = this.Driver.FindElementsByXPath(xpath);
        
        Console.WriteLine($"Elements containing '{searchTerm}' ({elements.Count} found):");
        
        foreach (var element in elements)
        {
            try
            {
                var automationId = element.GetAttribute("AutomationId");
                var isDisplayed = element.Displayed;
                var className = element.GetAttribute("ClassName");
                Console.WriteLine($"  - AutomationId: {automationId}, Displayed: {isDisplayed}, Class: {className}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  - Error reading element: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error searching for '{searchTerm}': {ex.Message}");
    }
}