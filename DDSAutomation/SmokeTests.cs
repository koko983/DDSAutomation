using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Appium;
using System.Collections.Generic;
using System;
using System.Linq;
using OpenQA.Selenium;

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
            this.desktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), desktopCapabilities, TimeSpan.FromMinutes(2));
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

        public void Log(params string[] things)
        {
            foreach (var something in things)
            {
                Console.WriteLine(something);
            }
        }

        void LogAttributes(AppiumWebElement element, params string[] attributeNames)
        {
            foreach (var item in attributeNames)
            {
                Log(item, element.GetAttribute(item));
            }
        }

        public void Log<T>(T element) where T : AppiumWebElement
        {
            Log("Text", element.Text, "Name", element.GetAttribute("Name"), "TagName", element.TagName, "ClassName", element.GetAttribute("ClassName"), "Value", element.GetAttribute("Value"), element.GetAttribute("value"), "IsReadOnly", element.GetAttribute("IsReadOnly"), "AutoId", element.GetAttribute("AutomationId"));
            LogAttributes(element, "HelpText", "FrameworkId", "ControlType", "BoundingRectangle", "AccessKey", "FullDescription", "HasKeyboardFocus");
        }

        public void LoopAndLog<T>(IReadOnlyCollection<T> elements) where T : AppiumWebElement
        {
            Log($"Count is {elements.Count}");
            foreach (var element in elements)
            {
                Log(element);
            }
        }

        [TestMethod]
        public void CaseBox_GetFirstDeleteBtn()
        {
            var btn1 = ddsSession.FindElementByClassName("button");
            var btn2 = ddsSession.FindElementByTagName("ControlType.Button");
            var delBtn = ddsSession.FindElementByAccessibilityId("DeleteMode");
            LoopAndLog(new[] { btn1, btn2, delBtn });
        }

        public void WaysToFindElement()
        {
            //var btn1 = ddsSession.FindElementByClassName("button");
            //var btn2 = ddsSession.FindElementByTagName("ControlType.Button");
            //var btn3 = ddsSession.FindElementByAccessibilityId("DeleteMode");
            //session.FindElementByXPath("//Button[@AutomationId=\"equalButton\"]").Click();
            //session.FindElementByName("One").Click();
        }

        [TestMethod]
        public void LocatingStuff()
        {
            try
            {

                var kpss = ddsSession.FindElementByAccessibilityId("btnAvailableRefresh");
                Log(kpss);
            }
            catch (Exception)
            {
                {
                    var kpss = ddsSession.FindElementByWindowsUIAutomation("btnAvailableRefresh");
                    Log(kpss);
                }
            };
            return;
            var edits = ddsSession.FindElementsByClassName("TextBox");
            //var edits = ddsSession.FindElementsByTagName("ControlType.Text");
            Assert.AreNotEqual(0, edits.Count);
            LoopAndLog(edits);
            var firstEdit = ddsSession.FindElementByClassName("TextBox");
            firstEdit.Click();
            //ddsSession.FindElementByAccessibilityId("btnSearch").Click();
            //var children = firstEdit.FindElementsByClassName("TextBlock");
            //LoopAndLog(children);
            //edits[0].SendKeys("h");
        }

        [TestMethod]
        public void MyTestMethod()
        {
            //var topLevel = ddsSession.FindElementsByXPath("/*[1]/*");
            //Console.WriteLine("topLevel.Count {0}", topLevel.Count);
            //foreach (var item in topLevel.Where(i => i.Displayed))
            //{
            //	Console.WriteLine("Tag: {0}, Text: {1}", item.TagName, item.Text);
            //}
            var elm3 = ddsSession.FindElementsByTagName("edit");
            var elm4 = ddsSession.FindElementsByXPath("//Button");
            var elm2 = ddsSession.FindElementsByTagName("ControlType.Edit");
            var kk = ddsSession.FindElementsByXPath("//Button[text = 'reset']");
            Assert.IsTrue(kk.Count > 0);
            var k = ddsSession.FindElementsByXPath("//Button").Where(e => !string.IsNullOrEmpty(e.Text));
            foreach (var item in k)
            {
                Console.WriteLine(item.Text);
            }
            //elm2.SendKeys("hi");
            //var elm = ddsSession.FindElementByLinkText("Search");
            //var ieServer = ddsSession.FindElementByClassName("Internet Explorer_Server");
            //var userName = ieServer.FindElementByAccessibilityId("userName");
            //userName.Clear();
            //userName.SendKeys("koko");
            //var password = ieServer.fin
        }

        [TestMethod]
        public void Login()
        {
            WindowsElement ieServer;
            try
            {
                ieServer = ddsSession.FindElementByClassName("Internet Explorer_Server");
            }
            catch (NotFoundException ex)
            {

                throw;
            }
            var userName = ieServer.FindElementByAccessibilityId("Username");
            userName.Clear();
            userName.SendKeys("superadmin");
            var password = ieServer.FindElementByAccessibilityId("Password");
            password.Clear();
            password.SendKeys("Password123!");
            var signInBtn = ieServer.FindElementByName("Sign In");
            signInBtn.Click();
        }

        [TestMethod]
        public void Logout()
        {
            WindowsElement ieServer;
            try
            {
                ieServer = ddsSession.FindElementByClassName("Internet Explorer_Server");
            }
            catch (NotFoundException ex)
            {

                throw;
            }
            var userName = ieServer.FindElementByAccessibilityId("Username");
            userName.Clear();
            userName.SendKeys("superadmin");
            var password = ieServer.FindElementByAccessibilityId("Password");
            password.Clear();
            password.SendKeys("Password123!");
            var signInBtn = ieServer.FindElementByName("Sign In");
            signInBtn.Click();
        }
    }
}
