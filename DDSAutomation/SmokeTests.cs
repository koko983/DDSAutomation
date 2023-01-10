using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Appium;
using System;
using System.Linq;
using OpenQA.Selenium;
using System.Threading;

namespace DDSAutomation
{
    [TestClass]
    public class SmokeTests
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private WindowsDriver<WindowsElement> ddsSession;
        private WindowsDriver<WindowsElement> desktopSession;

        [TestInitialize()]
        public void Setup()
        {
            var desktopCapabilities = new AppiumOptions();
            desktopCapabilities.AddAdditionalCapability("app", "Root");
            desktopCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
            this.desktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), desktopCapabilities, TimeSpan.FromMinutes(1.5));
            var windows = desktopSession.FindElementsByXPath("//Window");
            Console.WriteLine("Windows " + windows.Count);
            var window = windows.FirstOrDefault(w => w.Text.Contains("Digital Design"));
            if (window != null)
            {
                var windowHandle = window.GetAttribute("NativeWindowHandle");
                windowHandle = (int.Parse(windowHandle)).ToString("x"); // Convert to Hex
                Console.WriteLine("windowHandle {0}", windowHandle);

                var appCapabilities = new AppiumOptions();
                appCapabilities.AddAdditionalCapability("appTopLevelWindow", windowHandle);
                appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
                this.ddsSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
            }
        }

        [TestCleanup()]
        public void Teardown()
        {
            desktopSession.Quit();
        }


        [TestMethod]
        public void CaseBox_GetFirstDeleteBtn()
        {
            var btn1 = ddsSession.FindElementByClassName("button");
            var btn2 = ddsSession.FindElementByTagName("ControlType.Button");
            var delBtn = ddsSession.FindElementByAccessibilityId("DeleteMode");
            Logger.LoopAndLog(new[] { btn1, btn2, delBtn });
        }

        public void WaysToFindElement()
        {
            //var btn1 = ddsSession.FindElementByClassName("button");
            //var btn2 = ddsSession.FindElementByTagName("ControlType.Button");
            //var kk = ddsSession.FindElementsByXPath("//Button[text = 'reset']");
            //var btn3 = ddsSession.FindElementByAccessibilityId("DeleteMode");
            //session.FindElementByXPath("//Button[@AutomationId=\"equalButton\"]").Click();
            //session.FindElementByName("One").Click();

            //var edits = ddsSession.FindElementsByClassName("TextBox");
            //var edits = ddsSession.FindElementsByTagName("ControlType.Edit");
            //var texts = ddsSession.FindElementsByClassName("TextBlock");
            //var texts = ddsSession.FindElementsByTagName("ControlType.Text");

            //var topLevel = ddsSession.FindElementsByXPath("/*[1]/*");
        }

        [TestMethod]
        public void CheckoutFirstCase()
        {
            CheckAndSelectTabItem(TabNames.Available);
            var btnCheckout = ddsSession.FindElementByAccessibilityId("btnCheckout");
            btnCheckout.Click();
            Wait(40);
            Assert.IsTrue(GetTabItemIsSelected(TabNames.Model));
        }

        [TestMethod]
        public void OpenFirstCaseModel()
        {
            CheckAndSelectTabItem(TabNames.CheckedOut);
            var btnOpen = ddsSession.FindElementByAccessibilityId("btnOpen");
            btnOpen.Click();
            Wait(40);
            Assert.IsTrue(GetTabItemIsSelected(TabNames.Model));
        }

        [TestMethod]
        public void Login()
        {
            WindowsElement ieServer;
            try
            {
                ieServer = GetIeServer();
            }
            catch (NotFoundException ex)
            {
                if (DigitalDesignIntroHeaderPresent())
                {
                    SelectShitFromRightMenu("Login");
                    ieServer = GetIeServer();
                }
                else
                {
                    SelectShitFromRightMenu("Logout");
                    if (DigitalDesignIntroHeaderPresent())
                    {
                        SelectShitFromRightMenu("Login");
                        ieServer = GetIeServer();
                    }
                    throw new InvalidOperationException();
                }
            }
            var userName = ieServer.FindElementByAccessibilityId("Username");
            userName.Clear();
            userName.SendKeys("SuperAdmin");
            var password = ieServer.FindElementByAccessibilityId("Password");
            password.Clear();
            password.SendKeys("Password123!");
            var signInBtn = ieServer.FindElementByName("Sign In");
            signInBtn.Click();
        }

        [TestMethod]
        public void Logout()
        {
            SelectShitFromRightMenu("Logout");
            Assert.IsTrue(DigitalDesignIntroHeaderPresent());
        }

        private void CheckAndSelectTabItem(string tabItemName, int waitTimeInSeconds = 5)
        {
            var isSelected = GetTabItemIsSelected(tabItemName);
            if (!isSelected)
            {
                ClickTabItem(tabItemName, waitTimeInSeconds);
            }
        }

        private void ClickTabItem(string tabItemName, int waitSeconds = 2)
        {
            var tabItem = ddsSession.FindElementByName(tabItemName);
            tabItem.Click();
            Wait(waitSeconds);
        }

        private bool GetTabItemIsSelected(string tabItemName)
        {
            var tabItem = ddsSession.FindElementByName(tabItemName);
            var isSelected = bool.Parse(tabItem.GetAttribute("SelectionItem.IsSelected"));
            return isSelected;
        }

        private WindowsElement GetIeServer()
        {
            return ddsSession.FindElementByClassName("Internet Explorer_Server");
        }

        private void SelectShitFromRightMenu(string elementName)
        {
            var rightWindowCommands = ddsSession.FindElementByClassName("RightWindowCommands");
            var menuItem = rightWindowCommands.FindElementByClassName("MenuItem");
            menuItem.Click();
            Wait();
            var popUp = ddsSession.FindElementByClassName("Popup");
            var logoutMenuItem = popUp.FindElementByName(elementName);
            AssertElementIsInvokable(logoutMenuItem);
            logoutMenuItem.Click();
            Wait();
        }

        private bool DigitalDesignIntroHeaderPresent()
        {
            //var element = ddsSession.FindElementByClassName("TextBlock");
            var elements = ddsSession.FindElementsByName("Digital Design Suite");
            return elements.Count > 0;
        }

        private void AssertElementIsInvokable(AppiumWebElement element)
        {
            var isIt = bool.Parse(element.GetAttribute("IsInvokePatternAvailable"));
            Assert.IsTrue(isIt, $"Element {element} is not invokable");
        }

        private void Wait(int seconds = 2)
        {
            Thread.Sleep(seconds * 1000);
        }
    }
}
