# Clever Ads Solutions Unity Plugin Change Log

## [1.6.0] - 2020-10-07
### Dependencies
- Wraps [Android 1.6.2+ SDK](https://github.com/cleveradssolutions/CAS-Android/releases)
- Wraps [iOS 1.6.1+ SDK](https://github.com/cleveradssolutions/CAS-iOS/releases)
- External Dependency Manager for Unity [1.2.160](https://github.com/googlesamples/unity-jar-resolver/releases/tag/v1.2.160)
### Features
- Added settings to [redirect rewarded video ad impressions](https://github.com/cleveradssolutions/CAS-Unity#allow-interstitial-ads-when-video-cost-are-lower) to interstitial ads at higher cost per impression:
```c#
CAS.MobileAds.settings.allowInterstitialAdsWhenVideoCostAreLower = allow;
```
- Added [CAS Last Page Ad](https://github.com/cleveradssolutions/CAS-Unity#last-page-ad) to your own promotion when there is no paid ad to show or internet availability:
```c#
CAS.MobileAds.manager.lastPageAdContent = new LastPageAdContent(...);
```
- Added methods to get the current banner size in pixels:
```c#
float height = CAS.MobileAds.manager.GetBannerHeightInPixels();
float width = CAS.MobileAds.manager.GetBannerWidthInPixels();
```
- Added more pre-build validation points to easily get a successful build.
### Changes
- Support Unity 2019.3, 2019.4, 2020.1. [New step to include Android](https://github.com/cleveradssolutions/CAS-Unity#support-of-unity-20193) native platform.
- Support Android 11.
- Support iOS 14.
- Update minimum Android API level (minSdkVersion) to 19 (KitKat)
### Bug Fixes
- iOS platform Pause Unity Application while Ad impression

## [1.5.0] - 2020-09-23
### Dependencies
- Wraps Android 1.5.0 SDK
- Wraps iOS 1.5.2 SDK
- External Dependency Manager for Unity 1.2.159