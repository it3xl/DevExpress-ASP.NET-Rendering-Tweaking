using System.Reflection;
using DevExpress.Web.ASPxClasses.Internal;

namespace RenderingTweaking 
{
    public static class DevExpressTweaking
    {
        /// <summary>
        /// It forces DevExpress to do not use IE7 and earlier IE's rendering modes.
        /// </summary>
        public static void PreventOldIeRendering()
        {
            if (!RenderUtils.Browser.IsIE)
                return;

            const int minAllowedIeVersion = 8;

            if (minAllowedIeVersion <= RenderUtils.Browser.MajorVersion
                && minAllowedIeVersion <= RenderUtils.Browser.Version)
            {
                return;
            }

            var browserInfo = RenderUtils.Browser;
            var browserInfoType = browserInfo.GetType();

            var stateField = browserInfoType.GetField("state", BindingFlags.Instance | BindingFlags.NonPublic);

            var browserInfoState = stateField?.GetValue(browserInfo);
            var browserInfoStateType = browserInfoState?.GetType();

            if (RenderUtils.Browser.MajorVersion < minAllowedIeVersion)
            {
                var browserMajorVersion = browserInfoStateType?.GetField("BrowserMajorVersion", BindingFlags.Instance | BindingFlags.Public);
                browserMajorVersion?.SetValue(browserInfoState, minAllowedIeVersion);
            }

            if (RenderUtils.Browser.Version < minAllowedIeVersion)
            {
                var browserVersion = browserInfoStateType?.GetField("BrowserVersion", BindingFlags.Instance | BindingFlags.Public);
                browserVersion?.SetValue(browserInfoState, minAllowedIeVersion);
            }
        }
    }
}