using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace MercivKit
{
    public class MercivXRManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _xrOrigin;

        [SerializeField]
        private GameObject _desktopCamera;

        [SerializeField]
        private Button _button;

        private void Awake()
        {
            Log.Info($"MercivXRManager: Startup - XR Initialized: {IsXRInitialized()}, XR Active: {IsXRActive()}");
            _button.onClick.AddListener(ToggleXR);
        }

        private bool IsXRInitialized()
        {
            return XRGeneralSettings.Instance.Manager.isInitializationComplete;
        }

        private bool IsXRActive()
        {
            return XRSettings.isDeviceActive;
        }

        private void ToggleXR()
        {
            if (IsXRActive())
            {
                StopXR();
            }
            else
            {
                StartXR();
            }
        }

        private void StartXR()
        {
            Log.Info($"MercivXRManager: Toggle XR - Initialized: {IsXRInitialized()}, Active: {IsXRActive()}");
            if (!IsXRInitialized())
            {
                Log.Info("MercivXRManager: Initializing XR...");
                XRGeneralSettings.Instance.Manager.InitializeLoaderSync();

                if (IsXRInitialized())
                {
                    Log.Info("MercivXRManager: XR Initialized!");
                }
                else
                {
                    Log.Info("MercivXRManager: XR failed to initialize!");
                    return;
                }
            }

            if (!IsXRActive())
            {
                Log.Info("MercivXRManager: Starting XR...");
                XRGeneralSettings.Instance.Manager.StartSubsystems();
                _desktopCamera.SetActive(false);
                _xrOrigin.SetActive(true);
            }
        }

        private void StopXR()
        {
            if (IsXRActive())
            {
                Log.Info("MercivXRManager: Stopping XR...");
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                _desktopCamera.SetActive(true);
                _xrOrigin.SetActive(false);
            }
        }

        private void OnDisable()
        {
            StopXR();

            if (IsXRInitialized())
            {
                Log.Info("MercivXRManager: Deinitializing XR...");
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
        }
    }
}