using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;

namespace OI.NMR.UITests.Common.Screens
{
    /// <summary>
    /// Screen for P90 Chart with improved error handling and timing
    /// </summary>
    public class P90ChartScreen : BaseScreen<NMRApplicationScreen>
    {
        private const int DEFAULT_TIMEOUT_MS = 30000; // 30 seconds
        private const int POLL_INTERVAL_MS = 500; // 0.5 seconds

        public P90ChartScreen(NMRApplicationScreen app) : base(app, "P90ChartView")
        {
        }

        /// <inheritdoc cref="BaseScreen{TBaseApplication}.WaitUntilLoaded"/>
        protected override void WaitUntilLoaded()
        {
            base.WaitUntilLoaded();
            
            // Wait for either the header or chart to be present
            this.Driver.WaitUntil((d) => 
            {
                try
                {
                    var header = this.GetP90PulseWidthHeader();
                    var chart = this.GetP90Chart();
                    return (header?.Enabled == true) || (chart?.Displayed == true);
                }
                catch
                {
                    return false;
                }
            }, TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT_MS));
        }

        /// <summary>
        /// Waits for the P90 chart to appear with custom timeout
        /// </summary>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        /// <returns>True if chart becomes visible within timeout</returns>
        public bool WaitForP90ChartToAppear(int timeoutMs = DEFAULT_TIMEOUT_MS)
        {
            var endTime = DateTime.Now.AddMilliseconds(timeoutMs);
            
            while (DateTime.Now < endTime)
            {
                try
                {
                    if (IsP90ChartPresent() && P90ChartIsVisible())
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // Log exception if needed
                    Console.WriteLine($"Exception while waiting for P90 chart: {ex.Message}");
                }
                
                Thread.Sleep(POLL_INTERVAL_MS);
            }
            
            return false;
        }

        /// <summary>
        /// Checks if P90 chart element exists in the UI tree
        /// </summary>
        public bool IsP90ChartPresent()
        {
            try
            {
                var element = this.GetP90Chart();
                return element != null;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the P90Chart is visible with retry logic
        /// </summary>
        public bool P90ChartIsVisible()
        {
            try
            {
                var chart = this.GetP90Chart();
                return chart?.Displayed == true;
            }
            catch (StaleElementReferenceException)
            {
                // Element reference is stale, try to find it again
                try
                {
                    var chart = this.GetP90Chart();
                    return chart?.Displayed == true;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the P90 header is visible with retry logic
        /// </summary>
        public bool P90HeaderIsVisible()
        {
            try
            {
                var header = this.GetP90PulseWidthHeader();
                return header?.Displayed == true;
            }
            catch (StaleElementReferenceException)
            {
                // Element reference is stale, try to find it again
                try
                {
                    var header = this.GetP90PulseWidthHeader();
                    return header?.Displayed == true;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets debug information about the current state
        /// </summary>
        public string GetDebugInfo()
        {
            try
            {
                var headerPresent = false;
                var headerVisible = false;
                var chartPresent = false;
                var chartVisible = false;

                try
                {
                    var header = this.GetP90PulseWidthHeader();
                    headerPresent = header != null;
                    headerVisible = header?.Displayed == true;
                }
                catch { }

                try
                {
                    var chart = this.GetP90Chart();
                    chartPresent = chart != null;
                    chartVisible = chart?.Displayed == true;
                }
                catch { }

                return $"P90 Debug - Header: Present={headerPresent}, Visible={headerVisible} | Chart: Present={chartPresent}, Visible={chartVisible}";
            }
            catch (Exception ex)
            {
                return $"P90 Debug - Error getting info: {ex.Message}";
            }
        }

        private WindowsElement GetP90PulseWidthHeader()
        {
            return this.Driver.FindElementByAccessibilityId("P90ChartViewHeader");
        }

        private WindowsElement GetP90Chart()
        {
            return this.Driver.FindElementByAccessibilityId("P90ChartViewChart");
        }
    }
}