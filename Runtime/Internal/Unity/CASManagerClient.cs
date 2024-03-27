﻿//  Copyright © 2024 CAS.AI. All rights reserved.

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CAS.Unity
{
    [AddComponentMenu("")]
    internal class CASManagerClient : MonoBehaviour, IInternalManager
    {
        private List<Action> _eventsQueue = new List<Action>();
        private GUIStyle _btnStyle = null;

        [SerializeField]
        private string _casId;

        private bool[] _enabledTypes;

        [SerializeField]
        private List<CASViewClient> _adViews = new List<CASViewClient>();
        [SerializeField]
        private CASFullscreenView _interstitial;
        [SerializeField]
        private CASFullscreenView _rewarded;
        [SerializeField]
        private CASFullscreenView _appOpen;

        private LastPageAdContent _lastPageAdContent;

        internal CASSettingsClient _settings;

        public string managerID { get { return _casId; } }
        public bool isTestAdMode { get { return true; } set { } }

        public CASInitCompleteEvent initCompleteEvent { get; set; }
        public InitCompleteAction initCompleteAction { get; set; }
        public InitialConfiguration initialConfig { get; set; }

        public LastPageAdContent lastPageAdContent
        {
            get { return _lastPageAdContent; }
            set
            {
                _lastPageAdContent = value;
                if (value == null)
                    CASFactory.UnityLog("CAS Last Page Ad content cleared");
                else
                    CASFactory.UnityLog(new StringBuilder("CAS Last Page Ad apply content:")
                        .Append("\n- Headline:").Append(value.Headline)
                        .Append("\n- DestinationURL:").Append(value.DestinationURL)
                        .Append("\n- ImageURL:").Append(value.ImageURL)
                        .Append("\n- IconURL:").Append(value.IconURL)
                        .Append("\n- AdText:").Append(value.AdText)
                        .ToString());
            }
        }

#pragma warning disable 67

        #region Interstitial ad Events
        public event Action OnInterstitialAdLoaded
        {
            add { _interstitial.OnAdLoaded += value; }
            remove { _interstitial.OnAdLoaded -= value; }
        }
        public event CASEventWithAdError OnInterstitialAdFailedToLoad
        {
            add { _interstitial.OnAdFailedToLoad += value; }
            remove { _interstitial.OnAdFailedToLoad -= value; }
        }
        public event Action OnInterstitialAdShown
        {
            add { _interstitial.OnAdShown += value; }
            remove { _interstitial.OnAdShown -= value; }
        }
        public event CASEventWithMeta OnInterstitialAdOpening
        {
            add { _interstitial.OnAdOpening += value; }
            remove { _interstitial.OnAdOpening -= value; }
        }
        public event CASEventWithMeta OnInterstitialAdImpression
        {
            add { _interstitial.OnAdImpression += value; }
            remove { _interstitial.OnAdImpression -= value; }
        }
        public event CASEventWithError OnInterstitialAdFailedToShow
        {
            add { _interstitial.OnAdFailedToShow += value; }
            remove { _interstitial.OnAdFailedToShow -= value; }
        }
        public event Action OnInterstitialAdClicked
        {
            add { _interstitial.OnAdClicked += value; }
            remove { _interstitial.OnAdClicked -= value; }
        }
        public event Action OnInterstitialAdClosed
        {
            add { _interstitial.OnAdClosed += value; }
            remove { _interstitial.OnAdClosed -= value; }
        }
        #endregion

        #region Rewarded Ad Events
        public event Action OnRewardedAdLoaded
        {
            add { _rewarded.OnAdLoaded += value; }
            remove { _rewarded.OnAdLoaded -= value; }
        }
        public event CASEventWithAdError OnRewardedAdFailedToLoad
        {
            add { _rewarded.OnAdFailedToLoad += value; }
            remove { _rewarded.OnAdFailedToLoad -= value; }
        }
        public event Action OnRewardedAdShown
        {
            add { _rewarded.OnAdShown += value; }
            remove { _rewarded.OnAdShown -= value; }
        }
        public event CASEventWithMeta OnRewardedAdOpening
        {
            add { _rewarded.OnAdOpening += value; }
            remove { _rewarded.OnAdOpening -= value; }
        }
        public event CASEventWithMeta OnRewardedAdImpression
        {
            add { _rewarded.OnAdImpression += value; }
            remove { _rewarded.OnAdImpression -= value; }
        }
        public event CASEventWithError OnRewardedAdFailedToShow
        {
            add { _rewarded.OnAdFailedToShow += value; }
            remove { _rewarded.OnAdFailedToShow -= value; }
        }
        public event Action OnRewardedAdClicked
        {
            add { _rewarded.OnAdClicked += value; }
            remove { _rewarded.OnAdClicked -= value; }
        }
        public event Action OnRewardedAdCompleted
        {
            add { _rewarded.OnAdCompleted += value; }
            remove { _rewarded.OnAdCompleted -= value; }
        }
        public event Action OnRewardedAdClosed
        {
            add { _rewarded.OnAdClosed += value; }
            remove { _rewarded.OnAdClosed -= value; }
        }
        #endregion

        #region Return to app not supported for Editor
        public event Action OnAppReturnAdShown;
        public event CASEventWithMeta OnAppReturnAdOpening;
        public event CASEventWithMeta OnAppReturnAdImpression;
        public event CASEventWithError OnAppReturnAdFailedToShow;
        public event Action OnAppReturnAdClicked;
        public event Action OnAppReturnAdClosed;
        #endregion

        #region AppOpen Ad Events
        public event Action OnAppOpenAdLoaded
        {
            add { _appOpen.OnAdLoaded += value; }
            remove { _appOpen.OnAdLoaded -= value; }
        }
        public event CASEventWithAdError OnAppOpenAdFailedToLoad
        {
            add { _appOpen.OnAdFailedToLoad += value; }
            remove { _appOpen.OnAdFailedToLoad -= value; }
        }
        public event Action OnAppOpenAdShown
        {
            add { _appOpen.OnAdShown += value; }
            remove { _appOpen.OnAdShown -= value; }
        }
        public event CASEventWithMeta OnAppOpenAdImpression
        {
            add { _appOpen.OnAdImpression += value; }
            remove { _appOpen.OnAdImpression -= value; }
        }
        public event CASEventWithError OnAppOpenAdFailedToShow
        {
            add { _appOpen.OnAdFailedToShow += value; }
            remove { _appOpen.OnAdFailedToShow -= value; }
        }
        public event Action OnAppOpenAdClicked
        {
            add { _appOpen.OnAdClicked += value; }
            remove { _appOpen.OnAdClicked -= value; }
        }
        public event Action OnAppOpenAdClosed
        {
            add { _appOpen.OnAdClosed += value; }
            remove { _appOpen.OnAdClosed -= value; }
        }
        #endregion
#pragma warning restore 67

        internal static IInternalManager Create(CASInitSettings initSettings)
        {
            var obj = new GameObject("[CAS] Mediation Manager");
            //obj.hideFlags = HideFlags.HideInHierarchy;
            MonoBehaviour.DontDestroyOnLoad(obj);
            var behaviour = obj.AddComponent<CASManagerClient>();
            behaviour.Init(initSettings);
            return behaviour;
        }

        private void Init(CASInitSettings initSettings)
        {
            // Set Settings before any other calls.
            _settings = CAS.MobileAds.settings as CASSettingsClient;

            _casId = initSettings.targetId;
            _enabledTypes = new bool[(int)AdType.None];
            for (int i = 0; i < _enabledTypes.Length; i++)
                _enabledTypes[i] = ((int)initSettings.defaultAllowedFormats & (1 << i)) != 0;

            _interstitial = new CASFullscreenView(this, AdType.Interstitial);
            _rewarded = new CASFullscreenView(this, AdType.Rewarded);
            _appOpen = new CASFullscreenView(this, AdType.AppOpen);

            CASFactory.HandleConsentFlow(initSettings.consentFlow, ConsentFlowStatus.Unavailable);
        }

        #region IMediationManager implementation

        public bool IsEnabledAd(AdType adType)
        {
            // App Open disabled processing not supported
            if (adType == AdType.AppOpen)
                return true;
            return _enabledTypes[(int)adType];
        }

        public void SetEnableAd(AdType adType, bool enabled)
        {
            _enabledTypes[(int)adType] = enabled;
            if (enabled && IsAutoload(adType))
            {
                if (adType == AdType.Banner)
                {
                    for (int i = 0; i < _adViews.Count; i++)
                        _adViews[i].Load();
                }
                else if (adType != AdType.AppOpen)
                {
                    LoadAd(adType);
                }
            }
        }

        public bool IsReadyAd(AdType adType)
        {
            switch (adType)
            {
                case AdType.Interstitial:
                    return _interstitial.GetReadyError().HasValue == false;
                case AdType.Rewarded:
                    return _rewarded.GetReadyError().HasValue == false;
                case AdType.AppOpen:
                    return _appOpen.GetReadyError().HasValue == false;
            }
            return false;
        }

        public void LoadAd(AdType adType)
        {
            switch (adType)
            {
                case AdType.Banner:
                    throw new NotSupportedException("Use GetAdView(AdSize).Load() instead");
                case AdType.Interstitial:
                    _interstitial.Load();
                    break;
                case AdType.Rewarded:
                    _rewarded.Load();
                    break;
                case AdType.AppOpen:
                    _appOpen.Load();
                    break;
                default:
                    throw new NotSupportedException("Load ad function not support for AdType: " + adType.ToString());
            }
        }

        public void ShowAd(AdType adType)
        {
            switch (adType)
            {
                case AdType.Banner:
                    throw new NotSupportedException("Use GetAdView(AdSize).SetActive(true) instead");
                case AdType.Interstitial:
                    Post(_interstitial.Show);
                    break;
                case AdType.Rewarded:
                    Post(_rewarded.Show);
                    break;
                case AdType.AppOpen:
                    Post(_appOpen.Show);
                    break;
                default:
                    throw new NotSupportedException("Show ad function not support for AdType: " + adType.ToString());
            }
        }

        public bool TryOpenDebugger()
        {
            CASFactory.UnityLog("Ad Debugger in Editor not supported.");
            return false;
        }

        public void SetAppReturnAdsEnabled(bool enable)
        {
            CASFactory.UnityLog("Set auto show ad on App Return enabled: " + enable +
                ". But Auto Show Ad in Editor not supported");
        }

        public void SkipNextAppReturnAds()
        {
            CASFactory.UnityLog("The next time user return to the app, no ads will appear.");
        }

        public IAdView GetAdView(AdSize size)
        {
            if (size < AdSize.Banner)
                throw new ArgumentException("Invalid AdSize " + size.ToString());
            for (int i = 0; i < _adViews.Count; i++)
            {
                if (_adViews[i].size == size)
                    return _adViews[i];
            }
            var view = new CASViewClient(this, size);
            _adViews.Add(view);
            return view;
        }

        public void RemoveAdViewFromFactory(CASViewClient view)
        {
            _adViews.Remove(view);
        }
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            if (IsAutoload(AdType.Interstitial))
                LoadAd(AdType.Interstitial);
            if (IsAutoload(AdType.Rewarded))
                LoadAd(AdType.Rewarded);
            if (IsAutoload(AdType.AppOpen))
                LoadAd(AdType.AppOpen);

            Post(CallInitComplete);
        }

        private void CallInitComplete()
        {
            CASFactory.OnManagerInitialized(this, null, "US", true, true);
        }

        public void Update()
        {
            if (_eventsQueue.Count == 0)
                return;
            for (int i = 0; i < _eventsQueue.Count; i++)
            {
                try
                {
                    var action = _eventsQueue[i];
                    if (action != null)
                        action.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            _eventsQueue.Clear();
        }

        public void OnGUI()
        {
            if (_btnStyle == null)
                _btnStyle = new GUIStyle("Button");
            _btnStyle.fontSize = (int)(Math.Min(Screen.width, Screen.height) * 0.035f);

            for (int i = 0; i < _adViews.Count; i++)
            {
                _adViews[i].OnGUIAd(_btnStyle);
            }
            _interstitial.OnGUIAd(_btnStyle);
            _rewarded.OnGUIAd(_btnStyle);
            _appOpen.OnGUIAd(_btnStyle);
        }
        #endregion

        #region Utils
        public void Post(Action action)
        {
            if (action != null)
                _eventsQueue.Add(action);
        }

        public void Post(Action action, float delay)
        {
            if (action != null)
                StartCoroutine(DelayAction(action, delay));
        }

        public bool isFullscreenAdVisible
        {
            get { return _rewarded.active || _interstitial.active; }
        }

        public bool IsAutoload(AdType type)
        {
            // AppOpen format autoload not supported
            return type != AdType.AppOpen
                && _settings.loadingMode != LoadingManagerMode.Manual;
        }

        private IEnumerator DelayAction(Action action, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            action();
        }
        #endregion
    }
}
#endif