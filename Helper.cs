﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Win11ClockToggler
{
    internal class Helper
    {
        //Enum to decide if you want to hide the clock only or the full notification area
        internal enum TaskbarElement
        {
            Clock = 0,
            FullNotificationArea = 1
        }

        //Enum to inform about the operation taking place
        internal enum SWOperation
        {
            None = 0,   //No operation performed (element not found)
            Show = 1,   //Shows element(s)
            Hide = 2    //Hides element(s)
        }

        /// <summary>
        /// Shows or hides the indicated element
        /// </summary>
        /// <param name="tbeToShowOrHide">The element to show or hide (full notificatipn area or just the clock and system icons)</param>
        /// <param name="shown">This reference parameter is used to indicate inf the element has been shown (true) or hidden (false) 
        /// to make further decissions later in the code based on this.</param>
        /// <remarks>It will detect the current element state (shown or hidden) and toggle it</remarks>
        internal static bool ShowOrHideTaskbarElementWindow(TaskbarElement tbeToShowOrHide, out SWOperation operationPerformed)
        {
            //Find Taskbar handler
            IntPtr hWndTaskbar = Win32APIs.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
            if (hWndTaskbar != IntPtr.Zero)
            {
                //Windows 11: Find the notification area handler inside the taskbar (includes the system icons, which include the clock)
                IntPtr hWndNotifArea = Win32APIs.FindWindowEx(hWndTaskbar, IntPtr.Zero, "TrayNotifyWnd", null);
                if (hWndNotifArea != IntPtr.Zero)
                {
                    //First thing: get the current visible status of the TrayNotifyWnd element, that defines the whole set visibility status
                    bool isCurrentlyVisible = Win32APIs.IsWindowVisible(hWndNotifArea);
                    operationPerformed = isCurrentlyVisible ? SWOperation.Hide : SWOperation.Show;  //To notify the operation to the caller
                    int operation = isCurrentlyVisible ? Win32APIs.SW_HIDE : Win32APIs.SW_SHOW;

                    //If we want to actuate over the full notification area, then we need to show/hide all the children
                    if (tbeToShowOrHide == TaskbarElement.FullNotificationArea)
                    {
                        List<int> hiddenHwnds = new List<int>();
                        //If its a show operation, get the already hidden hwnds, if any
                        if (operationPerformed == SWOperation.Show)
                        {
                            hiddenHwnds = Helper.GetHiddenHwnds();
                        }
                        //Enumerate notification area children to find the system icons (and therefore the clock)
                        List<IntPtr> children = Win32APIs.GetChildWindows(hWndNotifArea);
                        foreach (IntPtr child in children)
                        {
                            if (operationPerformed == SWOperation.Hide)
                            {
                                if (!Win32APIs.IsWindowVisible(child))
                                {
                                    //Add the element to the already hidden list, so that we don't show them in the future
                                    hiddenHwnds.Add(child.ToInt32());
                                    continue;
                                }
                            }
                            else
                            {
                                //If it was initially hidden then skip it
                                if (hiddenHwnds.Contains(child.ToInt32()))
                                    continue;
                            }
                            Win32APIs.ShowWindow(child, operation);
                            
                        }

                        //When hiding, save the hwnds of elements already hidden in origin (to prevent showing then later)
                        if (operationPerformed == SWOperation.Hide)
                            SaveHiddenHwnds(hiddenHwnds);
                        else
                            SaveHiddenHwnds(new List<int>() { });   //Clear the list
                    }
                    //The clock (and system icons) area must be the last in being hidden,
                    //or it would interfere in the state for the elements in the previous loop...
                    Win32APIs.ShowWindow(hWndNotifArea, operation);
                    return true;    //Success
                }
            }
            //If we reach this point, nothing has been discovered
            operationPerformed = SWOperation.None;
            return false;   //Failure
        }

        //Registry location for this app settings
        static string REG_KEY = @"SOFTWARE\Win11ClockToggler";
        static string REG_VALUE = "HiddenHwnds";
        //Read the list of Hwnds already hidden from Windows registry
        internal static List<int> GetHiddenHwnds()
        {
            List<int> res = new List<int>();
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(REG_KEY);
                string strHwnds = (string)registryKey.GetValue(REG_VALUE);
                res = strHwnds.Split('|').Select(int.Parse).ToList<int>();
            }
            catch {}
            return res;
        }

        //Saves to te registry the list of already hiden Hwnds before hiding the elements
        internal static void SaveHiddenHwnds(List<int> hiddenHwnds)
        {
            string value = string.Join("|", hiddenHwnds.ToArray());
            if (!HiddenHwndsKeyExists())
                Registry.CurrentUser.CreateSubKey(REG_KEY, true);

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(REG_KEY, true);
            registryKey.SetValue(REG_VALUE, value);
        }

        //Check if the key exists or not
        internal static bool HiddenHwndsKeyExists()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(REG_KEY);
                return (key != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
