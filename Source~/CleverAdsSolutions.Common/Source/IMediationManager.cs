﻿//  Copyright © 2024 CAS.AI. All rights reserved.

using System;

namespace CAS
{
    public delegate void CASTypedEvent(AdType adType);
    public delegate void CASTypedEventWithError(AdType adType, string error);
    public delegate void CASEventWithError(string error);
    public delegate void CASEventWithAdError(AdError error);
    public delegate void CASEventWithMeta(AdMetaData meta);

    public delegate void CASInitCompleteEvent(InitialConfiguration config);
    public delegate void InitCompleteAction(bool success, string error);

    public class WikiPageAttribute : Attribute
    {
        public WikiPageAttribute(string url) { }
    }

    /// <summary>
    /// Interface for managing CAS mediation.
    /// Get instance using the CAS.MobileAds.BuildManager() builder.
    /// </summary>
    [WikiPage("https://github.com/cleveradssolutions/CAS-Unity/wiki/Initialize-SDK")]
    public interface IMediationManager
    {
        #region Interstitial Ads events
        /// <summary>
        /// Called when ad ready to shown.
        /// </summary>
        event Action OnInterstitialAdLoaded;
        /// <summary>
        /// Called when failed to load ad response with error message
        /// </summary>
        event CASEventWithAdError OnInterstitialAdFailedToLoad;
        /// <summary>
        /// Called when the ad is displayed.
        /// </summary>
        event Action OnInterstitialAdShown;
        /// <summary>
        /// The same call as the <see cref="OnInterstitialAdShown"/> but with <see cref="AdMetaData"/> about the impression. 
        /// </summary>
        [Obsolete("Use OnAdImpression event to collect AdMetaData about the ad impression or OnInterstitialAdShown event if AdMetaData is not used.")]
        event CASEventWithMeta OnInterstitialAdOpening;
        /// <summary>
        /// Called when the ad impression detects paid revenue.
        /// </summary>
        event CASEventWithMeta OnInterstitialAdImpression;
        /// <summary>
        /// Called when the ad is failed to display.
        /// </summary>
        event CASEventWithError OnInterstitialAdFailedToShow;
        /// <summary>
        /// Called when the user clicks on the Ad.
        /// </summary>
        event Action OnInterstitialAdClicked;
        /// <summary>
        /// Called when the ad is closed.
        /// </summary>
        event Action OnInterstitialAdClosed;
        #endregion

        #region Rewarded Ads events
        /// <summary>
        /// Called when ad ready to shown.
        /// </summary>
        event Action OnRewardedAdLoaded;
        /// <summary>
        /// Called when failed to load ad response with error message
        /// </summary>
        event CASEventWithAdError OnRewardedAdFailedToLoad;
        /// <summary>
        /// Called when the ad is displayed.
        /// </summary>
        event Action OnRewardedAdShown;
        /// <summary>
        /// The same call as the <see cref="OnRewardedAdShown"/> but with <see cref="AdMetaData"/> about the impression. 
        /// </summary>
        [Obsolete("Use OnAdImpression event to collect AdMetaData about the ad impression or OnRewardedAdShown event if AdMetaData is not used.")]
        event CASEventWithMeta OnRewardedAdOpening;
        /// <summary>
        /// Called when the ad impression detects paid revenue.
        /// </summary>
        event CASEventWithMeta OnRewardedAdImpression;
        /// <summary>
        /// Called when the ad is failed to display.
        /// </summary>
        event CASEventWithError OnRewardedAdFailedToShow;
        /// <summary>
        /// Called when the user clicks on the Ad.
        /// </summary>
        event Action OnRewardedAdClicked;
        /// <summary>
        /// Called when the ad is completed.
        /// </summary>
        event Action OnRewardedAdCompleted;
        /// <summary>
        /// Called when the ad is closed.
        /// </summary>
        event Action OnRewardedAdClosed;
        #endregion

        #region App Open Ads events
        /// <summary>
        /// Called when ad ready to shown.
        /// </summary>
        event Action OnAppOpenAdLoaded;
        /// <summary>
        /// Called when failed to load ad response with error message
        /// </summary>
        event CASEventWithAdError OnAppOpenAdFailedToLoad;
        /// <summary>
        /// Called when the ad is displayed.
        /// </summary>
        event Action OnAppOpenAdShown;
        /// <summary>
        /// Called when the ad impression detects paid revenue.
        /// </summary>
        event CASEventWithMeta OnAppOpenAdImpression;
        /// <summary>
        /// Called when the ad is failed to display.
        /// </summary>
        event CASEventWithError OnAppOpenAdFailedToShow;
        /// <summary>
        /// Called when the user clicks on the Ad.
        /// </summary>
        event Action OnAppOpenAdClicked;
        /// <summary>
        /// Called when the ad is closed.
        /// </summary>
        event Action OnAppOpenAdClosed;
        #endregion

        /// <summary>
        /// The CAS identifier
        /// </summary>
        string managerID { get; }

        /// <summary>
        /// Is Mediation manager use test ads for current session.
        /// </summary>
        bool isTestAdMode { get; }

        /// <summary>
        /// Manual load <see cref="AdType"/> Ad.
        /// <para>Please call load before each show ad whe active load mode is <see cref="LoadingManagerMode.Manual"/>.</para>
        /// <para>You can get a callback for the successful loading of an ad by subscribe OnLoadedAd events</para>
        /// <para>Please for <see cref="AdType.Banner"/> use new ad size api <see cref="GetAdView(AdSize)"/>.Load() instead.</para>
        /// </summary>
        void LoadAd(AdType adType);

        /// <summary>
        /// Check ready selected <see cref="AdType"/> to show.
        /// <para>Please for <see cref="AdType.Banner"/> use new ad size api <see cref="GetAdView(AdSize)"/>.isReady instead.</para>
        /// </summary>
        bool IsReadyAd(AdType adType);

        /// <summary>
        /// Force show ad by selected <see cref="AdType"/>.
        /// <para>Please for <see cref="AdType.Banner"/> use new ad size api <see cref="GetAdView(AdSize)"/>.SetActive(true) instead.</para>
        /// </summary>
        void ShowAd(AdType adType);

        /// <summary>
        /// Get the ad view interface for specific <paramref name="size"/>.
        /// <para>If a view for specific size has already been created then a reference to it
        /// will be returned without creating a new one.</para>
        /// <para>The newly created AdView has an inactive state. When you are ready to show the ad on the screen,
        /// simply call a <see cref="IAdView.SetActive(bool)"/> method.</para>
        /// <para>If you no longer need the AdView with this size, please call <see cref="IDisposable.Dispose()"/> to free memory.</para>
        /// <para>After calling Dispose(), you can use GetAdView() method to create a new view.</para>
        /// </summary>
        /// <param name="size">The ad size you want using.</param>
        IAdView GetAdView(AdSize size);

        #region Return to App Ads eveents
        /// <summary>
        /// Called when the ad is displayed.
        /// </summary>
        event Action OnAppReturnAdShown;
        /// <summary>
        /// The same call as the <see cref="OnAppReturnAdShown"/> but with <see cref="AdMetaData"/> about the impression. 
        /// </summary>
        [Obsolete("Use OnAdImpression event to collect AdMetaData about the ad impression or OnAppReturnAdShown event if AdMetaData is not used.")]
        event CASEventWithMeta OnAppReturnAdOpening;
        /// <summary>
        /// Called when the ad impression detects paid revenue.
        /// </summary>
        event CASEventWithMeta OnAppReturnAdImpression;
        /// <summary>
        /// Called when the ad is failed to display.
        /// </summary>
        event CASEventWithError OnAppReturnAdFailedToShow;
        /// <summary>
        /// Called when the user clicks on the Ad.
        /// </summary>
        event Action OnAppReturnAdClicked;
        /// <summary>
        /// Called when the ad is closed.
        /// </summary>
        event Action OnAppReturnAdClosed;
        #endregion

        /// <summary>
        /// The Return Ad which is displayed once the user returns to your application after a certain period of time.
        /// <para>To minimize the intrusiveness, short time periods are ignored.</para>
        /// <para>Return ads are disabled by default.</para>
        /// </summary>
        void SetAppReturnAdsEnabled(bool enable);

        /// <summary>
        /// Calling this method will indicate to skip one next ad impression when returning to the app.
        /// <para>You can call this method when you intentionally redirect the user to another application (for example Google Play)
        /// and do not want them to see ads when they return to your application.</para>
        /// </summary>
        void SkipNextAppReturnAds();

        /// <summary>
        /// Set enabled <see cref="AdType"/> to processing.
        /// <para>If processing is inactive then all calls to the selected ad type
        /// will fail with error <see cref="AdError.ManagerIsDisabled"/>.</para>
        /// <para>The state will not be saved between sessions.</para>
        /// </summary>
        void SetEnableAd(AdType adType, bool enabled);

        /// <summary>
        /// Check enabled <see cref="AdType"/> is processing.
        /// Read more about <see cref="SetEnableAd"/>.
        /// </summary>
        bool IsEnabledAd(AdType adType);

        /// <summary>
        /// The latest free ad page for your own promotion.
        /// <br>This ad page will be displayed when there is no paid ad to show or internet availability.</br>
        /// <br>By default, this page will not be displayed while the ad content is NULL.</br>
        /// <para>**Attention!** Impressions and clicks on this ad page don't make money.</para>
        /// </summary>
        LastPageAdContent lastPageAdContent { get; set; }
    }
}
