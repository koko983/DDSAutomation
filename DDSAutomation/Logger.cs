using OpenQA.Selenium.Appium;
using System.Collections.Generic;
using System;

namespace DDSAutomation
{
    public static class Logger
    {
        public static void Log(params string[] things)
        {
            foreach (var something in things)
            {
                Console.WriteLine(something);
            }
        }

        public static void LogAttributes(AppiumWebElement element, params string[] attributeNames)
        {
            foreach (var item in attributeNames)
            {
                Log(item, element.GetAttribute(item));
            }
        }

        public static void Log(AppiumWebElement element)
        {
            Log("Text", element.Text, "Name", element.GetAttribute("Name"), "TagName", element.TagName, "ClassName", element.GetAttribute("ClassName"), "Value", element.GetAttribute("Value"), element.GetAttribute("value"));
            LogAttributes(element, "IsReadOnly", "AutomationId", "HelpText", "FrameworkId", "ControlType", "BoundingRectangle", "AccessKey", "FullDescription", "HasKeyboardFocus");
        }

        public static void LoopAndLog<T>(IReadOnlyCollection<T> elements) where T : AppiumWebElement
        {
            Log($"Count is {elements.Count}");
            foreach (var element in elements)
            {
                Log(element);
            }
        }
    }
}
