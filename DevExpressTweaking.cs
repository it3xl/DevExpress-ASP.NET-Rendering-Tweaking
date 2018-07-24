using System.Reflection;
using System.Runtime.CompilerServices;
using DevExpress.Web.ASPxClasses.Internal;

namespace RenderingTweaking 
{
    public static class DevExpressTweaking
    {
        /// <summary>
        /// It forces DevExpress to use your desirable rendering modes.
        /// </summary>
        public static void PreventOldIeRendering()
        {
            EnsureNotDefaultStateObject();

            var browserInfo = RenderUtils.Browser;
            var typeBrowserInfo = browserInfo.GetType();

            var fieldState = typeBrowserInfo.GetField("state",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var browserInfoState = fieldState?.GetValue(browserInfo);
            var typeBrowserInfoState = browserInfoState?.GetType();

            if (browserInfo.IsMozilla)
            {
                // Old DevExpress doesn't know new IE's User Agent strings
                // and perceives IE as BrowserType.Mozilla
                var fieldBrowserType = typeBrowserInfoState?.GetField("BrowserType",
                    BindingFlags.Instance | BindingFlags.Public);
                fieldBrowserType?.SetValue(browserInfoState, BrowserType.IE);
            }


            if (!RenderUtils.Browser.IsIE)
                return;

            const int minAllowedIeVersion = 8;

            if (minAllowedIeVersion <= RenderUtils.Browser.MajorVersion
                && minAllowedIeVersion <= RenderUtils.Browser.Version)
            {
                return;
            }

            if (RenderUtils.Browser.MajorVersion < minAllowedIeVersion)
            {
                var fieldBrowserMajorVersion = typeBrowserInfoState?.GetField("BrowserMajorVersion",
                    BindingFlags.Instance | BindingFlags.Public);
                fieldBrowserMajorVersion?.SetValue(browserInfoState, minAllowedIeVersion);
            }

            if (RenderUtils.Browser.Version < minAllowedIeVersion)
            {
                var fieldBrowserVersion = typeBrowserInfoState?.GetField("BrowserVersion",
                    BindingFlags.Instance | BindingFlags.Public);
                fieldBrowserVersion?.SetValue(browserInfoState, minAllowedIeVersion);
            }
        }

        /// <summary>
        /// Ensures that <see cref="RenderUtils.Browser"/> will not substitute its internal state object.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static bool EnsureNotDefaultStateObject()
        {
            // If properties of RenderUtils.Browser was newer read then it contains a disposable default state object.
            // We have to generate a new value for BrowserInfo..state by an implicit call for BrowserInfo..EnsureStateIsActual();
            return RenderUtils.Browser.IsChrome;
        }
    }
}