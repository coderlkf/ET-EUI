using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public static class DlgTestSystem
    {

        public static void RegisterUIEvent(this DlgTest self)
        {
            self.View.E_EnterMapButton.AddListener(self.OnEnterMapButtonClickHandler);

        }

        public static void ShowWindow(this DlgTest self, Entity contextData = null)
        {
        }

        public static void OnEnterMapButtonClickHandler(this DlgTest self)
        {
            Log.Debug("Enter map!!!");
        }

    }
}
