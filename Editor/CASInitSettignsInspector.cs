﻿//#define UserDataPrivacySettings
using System;
using UnityEngine;
using UnityEditor;

namespace CAS.UEditor
{
    [CustomEditor( typeof( CASInitSettings ) )]
    internal class CASInitSettignsInspector : Editor
    {
        private SerializedProperty testAdModeProp;
        private SerializedProperty managerIdsProp;
        private SerializedProperty allowedAdFlagsProp;
        private SerializedProperty audienceTaggedProp;
        private SerializedProperty debugModeProp;
        private SerializedProperty bannerRefreshProp;
        private SerializedProperty interstitialIntervalProp;
        private SerializedProperty loadingModeProp;
        private SerializedProperty bannerSizeProp;
        private SerializedProperty locationUsageDescriptionProp;

        private BuildTarget platform;
        private bool promoDependencyExist;
        private bool teenDependencyExist;
        private bool generalDependencyExist;

        private int editorRuntimeActiveAdFlags;
        private Vector2 mediationNetworkScroll;

        private readonly PartnerNetwork[] mediationPartners = new PartnerNetwork[]
        {
            new PartnerNetwork("SuperAwesome", "https://www.superawesome.com", Audience.Children, Audience.Mixed),
            new PartnerNetwork("FacebookAds", "https://www.facebook.com/business/marketing/audience-network", Audience.NotChildren, Audience.Mixed),
            new PartnerNetwork("YandexAds", "https://yandex.ru/dev/mobile-ads/", Audience.NotChildren, Audience.Mixed),
            new PartnerNetwork("GoogleAds", "https://admob.google.com/home/", Audience.Mixed, Audience.Mixed),
            new PartnerNetwork("AppLovin", "https://www.applovin.com", Audience.Mixed, Audience.Mixed),
            new PartnerNetwork("UnityAds", "https://unity.com/solutions/unity-ads", Audience.Mixed, Audience.Mixed),
            new PartnerNetwork("Vungle", "https://vungle.com", Audience.Mixed, Audience.Mixed),
            new PartnerNetwork("AdColony", "https://www.adcolony.com", Audience.Mixed, Audience.Mixed),
            new PartnerNetwork("IronSource", "https://www.ironsrc.com", Audience.Mixed, Audience.Mixed),
            new PartnerNetwork("Kidoz", "https://kidoz.net", Audience.Mixed, Audience.Mixed),
            new PartnerNetwork("InMobi", "https://www.inmobi.com", Audience.Mixed, Audience.Mixed),
            new PartnerNetwork("Chartboost", "https://www.chartboost.com", Audience.Mixed, Audience.Mixed),
            new PartnerNetwork("StartApp", "https://www.startapp.com", Audience.Mixed, Audience.Mixed)
        };

        private void OnEnable()
        {
            var props = serializedObject;
            testAdModeProp = props.FindProperty( "testAdMode" );
            managerIdsProp = props.FindProperty( "managerIds" );
            allowedAdFlagsProp = props.FindProperty( "allowedAdFlags" );
            audienceTaggedProp = props.FindProperty( "audienceTagged" );
            debugModeProp = props.FindProperty( "debugMode" );
            bannerRefreshProp = props.FindProperty( "bannerRefresh" );
            interstitialIntervalProp = props.FindProperty( "interstitialInterval" );
            loadingModeProp = props.FindProperty( "loadingMode" );
            bannerSizeProp = props.FindProperty( "bannerSize" );
            locationUsageDescriptionProp = props.FindProperty( "locationUsageDescription" );

            editorRuntimeActiveAdFlags = PlayerPrefs.GetInt( CASEditorUtils.editorRuntomeActiveAdPrefs, -1 );

            string assetName = target.name;
            if (assetName.EndsWith( BuildTarget.Android.ToString() ))
                platform = BuildTarget.Android;
            else if (assetName.EndsWith( BuildTarget.iOS.ToString() ))
                platform = BuildTarget.iOS;
            else
                platform = BuildTarget.NoTarget;

            generalDependencyExist = CASEditorUtils.IsDependencyFileExists( CASEditorUtils.generalTemplateDependency, platform );
            teenDependencyExist = CASEditorUtils.IsDependencyFileExists( CASEditorUtils.teenTemplateDependency, platform );
            promoDependencyExist = CASEditorUtils.IsDependencyFileExists( CASEditorUtils.promoTemplateDependency, platform );
        }

        public override void OnInspectorGUI()
        {
            var obj = serializedObject;
            obj.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField( testAdModeProp );
            EditorGUI.BeginDisabledGroup( testAdModeProp.boolValue );
            EditorGUILayout.PropertyField( managerIdsProp, true );
            OnManagerIDVerificationGUI();
            EditorGUI.EndDisabledGroup();

            allowedAdFlagsProp.intValue = Convert.ToInt32(
               EditorGUILayout.EnumFlagsField( "Allowed ads in app", ( AdFlags )allowedAdFlagsProp.intValue ) );

            DrawSeparator();
            bool isChildrenAudience = OnAudienceGUIActiveChildren();
            EditorGUI.BeginDisabledGroup( isChildrenAudience );
#if UserDataPrivacySettings
            OnUserConsentGUI();
            OnCCPAStatusGUI();
#endif
            EditorGUI.EndDisabledGroup();
            OnIOSLocationUsageDescriptionGUI();

            DrawSeparator();
            OnBannerSizeGUI();
            bannerRefreshProp.intValue = Mathf.Clamp(
                 EditorGUILayout.IntField( "Banner refresh rate(sec):", bannerRefreshProp.intValue ), 10, short.MaxValue );

            DrawSeparator();
            interstitialIntervalProp.intValue = Math.Max( 0,
                EditorGUILayout.IntField( "Interstitial impression interval(sec):", interstitialIntervalProp.intValue ) );
            
            DrawSeparator();
            OnLoadingModeGUI();
            EditorGUILayout.PropertyField( debugModeProp );
            OnEditroRuntimeActiveAdGUI();

            DrawSeparator();
            OnPromoDependenciesGUI();

            DrawSeparator();
            OnAllowedAdNetworksGUI();

            DrawSeparator();
            OnCASAboutGUI();
            obj.ApplyModifiedProperties();
        }

        private void DrawSeparator()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void OnManagerIDVerificationGUI()
        {
            if (testAdModeProp.boolValue)
                return;

            if (managerIdsProp.arraySize == 0)
                EditorGUILayout.HelpBox( "Build is not supported without a manager ID.", MessageType.Error );
            else if (string.IsNullOrEmpty( managerIdsProp.GetArrayElementAtIndex( 0 ).stringValue ))
                EditorGUILayout.HelpBox( "Manager ID cannot be empty.", MessageType.Error );
            else
                return;
            EditorGUILayout.HelpBox( "If you haven't created an CAS account and registered an manager yet, " +
                "now's a great time to do so at cleveradssolutions.com. " +
                "If you're just looking to experiment with the SDK, though, you can use the Test Ad Mode above.", MessageType.Info );
        }

#if UserDataPrivacySettings
        private void OnUserConsentGUI()
        {
            var consentStatus = ( ConsentStatus )EditorGUILayout.EnumPopup( "User Consent",
                    ( ConsentStatus )consentStatusProp.enumValueIndex );
            consentStatusProp.enumValueIndex = ( int )consentStatus;
            EditorGUI.indentLevel++;
            switch (consentStatus)
            {
                case ConsentStatus.Undefined:
                    EditorGUILayout.HelpBox( "Mediation ads network behavior.", MessageType.None );
                    break;
                case ConsentStatus.Accepted:
                    EditorGUILayout.HelpBox( "User consents to behavioral targeting in compliance with GDPR.", MessageType.None );
                    break;
                case ConsentStatus.Denied:
                    EditorGUILayout.HelpBox( "User does not consent to behavioral targeting in compliance with GDPR.", MessageType.None );
                    break;
            }
            EditorGUI.indentLevel--;
        }
#endif

        private bool OnAudienceGUIActiveChildren()
        {
            var targetAudience = ( Audience )EditorGUILayout.EnumPopup( "Audience Tagged",
                ( Audience )audienceTaggedProp.enumValueIndex );
            audienceTaggedProp.enumValueIndex = ( int )targetAudience;
            if (targetAudience == Audience.Children)
            {
#if UserDataPrivacySettings
                consentStatusProp.enumValueIndex = ( int )ConsentStatus.Denied;
                ccpaStatusProp.enumValueIndex = ( int )CCPAStatus.OptOutSale;
#endif
                EditorGUI.indentLevel++;
                if (!generalDependencyExist)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox( "Apps tagged for kids require General CAS Dependencies to be activated. " +
                        "Please use Android Resolver after the change.",
                        MessageType.Error );
                    if (GUILayout.Button( "Activate", GUILayout.Height( 38 ) ))
                        if (ActivateDependencies( CASEditorUtils.generalTemplateDependency ))
                            generalDependencyExist = true;
                    EditorGUILayout.EndHorizontal();
                }
                if (teenDependencyExist)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox( "Teen CAS Dependencies not supported and should be deactivated. " +
                        "Please use Android Resolver after the change.",
                        MessageType.Error );
                    if (GUILayout.Button( "Deactivate", GUILayout.Height( 38 ) ))
                        if (DeactivateDependencies( CASEditorUtils.teenTemplateDependency ))
                            teenDependencyExist = false;
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.HelpBox( "The children's audience tagged does not allow (GDPR, CCPA) requests " +
                    "for consent to the use of personal data. Also Families Ads Program allows using only certified ad networks.", MessageType.Info );
                EditorGUI.indentLevel--;
                return true;
            }
            else
            {
                if (generalDependencyExist)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox( "General CAS Dependencies not supported and should be deactivated. " +
                        "Please use Android Resolver after the change.",
                        MessageType.Error );
                    if (GUILayout.Button( "Deactivate", GUILayout.Height( 38 ) ))
                        if (DeactivateDependencies( CASEditorUtils.generalTemplateDependency ))
                            generalDependencyExist = false;
                    EditorGUILayout.EndHorizontal();
                }
                if (!teenDependencyExist)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox( "Apps tagged for " + targetAudience.ToString() +
                        " require Teen CAS Dependencies to be activated. " +
                        "Please use Android Resolver after the change.", MessageType.Error );
                    if (GUILayout.Button( "Activate", GUILayout.Height( 38 ) ))
                        if (ActivateDependencies( CASEditorUtils.teenTemplateDependency ))
                            teenDependencyExist = true;
                    EditorGUILayout.EndHorizontal();
                }
            }
            return false;
        }

#if UserDataPrivacySettings
        private void OnCCPAStatusGUI()
        {
            var ccpaStatus = ( CCPAStatus )EditorGUILayout.EnumPopup( "CCPA Status",
                    ( CCPAStatus )ccpaStatusProp.enumValueIndex );
            ccpaStatusProp.enumValueIndex = ( int )ccpaStatus;
            EditorGUI.indentLevel++;
            switch (ccpaStatus)
            {
                case CCPAStatus.Undefined:
                    EditorGUILayout.HelpBox( "Mediation ads network behavior.", MessageType.None );
                    break;
                case CCPAStatus.OptInSale:
                    EditorGUILayout.HelpBox( "User consents to the sale of his or her personal information in compliance with CCPA.", MessageType.None );
                    break;
                case CCPAStatus.OptOutSale:
                    EditorGUILayout.HelpBox( "User does not consent to the sale of his or her personal information in compliance with CCPA.", MessageType.None );
                    break;
            }
            EditorGUI.indentLevel--;
        }
#endif

        private void OnIOSLocationUsageDescriptionGUI()
        {
            if (platform != BuildTarget.iOS)
                return;
            EditorGUILayout.PropertyField( locationUsageDescriptionProp );
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox( "NSUserTrackingUsageDescription key with a custom message describing your usage. Can be empty.", MessageType.None );
            if (GUILayout.Button( "Info", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
                Application.OpenURL( CASEditorUtils.configuringPrivacyURL );
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        private void OnBannerSizeGUI()
        {
            EditorGUILayout.PropertyField( bannerSizeProp );
            EditorGUI.indentLevel++;
            switch (( AdSize )bannerSizeProp.intValue)
            {
                case AdSize.Banner:
                    EditorGUILayout.HelpBox( "Current size: 320:50", MessageType.None );
                    break;
                case AdSize.AdaptiveBanner:
                    EditorGUILayout.HelpBox( "Pick the best ad size in full width screen for improved performance.", MessageType.None );
                    break;
                case AdSize.SmartBanner:
                    EditorGUILayout.HelpBox( "Typically, Smart Banners on phones have a Banner size. " +
                        "Or on tablets a Leaderboard size", MessageType.None );
                    break;
                case AdSize.Leaderboard:
                    EditorGUILayout.HelpBox( "Current size: 728:90", MessageType.None );
                    break;
                case AdSize.MediumRectangle:
                    EditorGUILayout.HelpBox( "Current size: 300:250", MessageType.None );
                    break;
            }
            EditorGUI.indentLevel--;
        }

        private void OnLoadingModeGUI()
        {
            var loadingMode = ( LoadingManagerMode )EditorGUILayout.EnumPopup( "Loading mode",
                    ( LoadingManagerMode )loadingModeProp.enumValueIndex );
            loadingModeProp.enumValueIndex = ( int )loadingMode;

            float requestsVal = 0.5f;
            float performVal = 0.5f;
            switch (loadingMode)
            {
                case LoadingManagerMode.FastestRequests:
                    requestsVal = 1.0f;
                    performVal = 0.1f;
                    break;
                case LoadingManagerMode.FastRequests:
                    requestsVal = 0.75f;
                    performVal = 0.4f;
                    break;
                case LoadingManagerMode.HighePerformance:
                    requestsVal = 0.25f;
                    performVal = 0.7f;
                    break;
                case LoadingManagerMode.HighestPerformance:
                case LoadingManagerMode.Manual:
                    requestsVal = 0.0f;
                    performVal = 1.0f;
                    break;
            }
            if (debugModeProp.boolValue)
                performVal -= 0.1f;

            EditorGUI.BeginDisabledGroup( true );
            DrawEffectiveSlider( "Requests:", requestsVal );
            DrawEffectiveSlider( "Performance:", performVal );
            EditorGUI.EndDisabledGroup();
        }

        private static void DrawEffectiveSlider( string label, float performVal )
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space( 15.0f );
            GUILayout.Label( label, GUILayout.Width( 80.0f ) );
            GUILayout.Label( "slow", EditorStyles.miniLabel, GUILayout.ExpandWidth( false ) );
            GUILayout.HorizontalSlider( performVal, 0, 1 );
            GUILayout.Label( "fast", EditorStyles.miniLabel, GUILayout.ExpandWidth( false ) );
            EditorGUILayout.EndHorizontal();
        }

        private void OnEditroRuntimeActiveAdGUI()
        {
            if (editorRuntimeActiveAdFlags > -1)
            {
                DrawSeparator();
                EditorGUI.BeginChangeCheck();
                editorRuntimeActiveAdFlags = Convert.ToInt32(
                    EditorGUILayout.EnumFlagsField( "Editor runtime Active ad", ( AdFlags )editorRuntimeActiveAdFlags ) );
                if (EditorGUI.EndChangeCheck())
                    PlayerPrefs.SetInt( CASEditorUtils.editorRuntomeActiveAdPrefs, editorRuntimeActiveAdFlags );
            }
        }

        private static void OnCASAboutGUI()
        {
            EditorGUILayout.LabelField( "CAS Unity wrapper version: " + MobileAds.wrapperVersion );
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button( "GitHub", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
                Application.OpenURL( CASEditorUtils.githubURL );
            if (GUILayout.Button( "Support", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
                Application.OpenURL( CASEditorUtils.supportURL );
            if (GUILayout.Button( "cleveradssolutions.com", EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
                Application.OpenURL( CASEditorUtils.websiteURL );
            EditorGUILayout.EndHorizontal();
        }

        private void OnPromoDependenciesGUI()
        {
            if (promoDependencyExist != EditorGUILayout.Toggle( "Cross Promotion enabled", promoDependencyExist ))
            {
                if (promoDependencyExist)
                {
                    if (DeactivateDependencies( CASEditorUtils.promoTemplateDependency ))
                        promoDependencyExist = false;
                }
                else
                {
                    if (ActivateDependencies( CASEditorUtils.promoTemplateDependency ))
                        promoDependencyExist = true;
                }
            }
            if(platform == BuildTarget.Android)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox( "Changing this flag will change the project dependencies. " +
                    "Please use Android Resolver after the change.", MessageType.None );
                EditorGUI.indentLevel--;
            }    
        }

        private void OnAllowedAdNetworksGUI()
        {
            var audience = ( Audience )audienceTaggedProp.enumValueIndex;
            mediationNetworkScroll = EditorGUILayout.BeginScrollView( mediationNetworkScroll,
                GUILayout.ExpandHeight( false ), GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 5.5f) );

            EditorGUILayout.LabelField( "Allowed partners networks for " + audience.ToString() + " audience" );
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < mediationPartners.Length; i++)
            {
                var partner = mediationPartners[i];
                var partnerTagged = partner.GetAudience( platform );
                if (platform == BuildTarget.iOS || ( audience == Audience.Mixed && partnerTagged == Audience.NotChildren )
                    || partnerTagged == audience || partnerTagged == Audience.Mixed)
                {
                    if (GUILayout.Button( partner.name, EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
                        Application.OpenURL( partner.url );
                }
            }
            EditorGUILayout.EndHorizontal();
            if (platform == BuildTarget.Android)
            {
                EditorGUILayout.LabelField( "Not allowed partners networks for " + audience.ToString() + " audience" );
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < mediationPartners.Length; i++)
                {
                    var partner = mediationPartners[i];
                    var partnerTagged = partner.GetAudience( platform );
                    if (partnerTagged != audience && partnerTagged != Audience.Mixed
                        && !( audience == Audience.Mixed && partnerTagged == Audience.NotChildren ))
                    {
                        if (GUILayout.Button( partner.name, EditorStyles.miniButton, GUILayout.ExpandWidth( false ) ))
                            Application.OpenURL( partner.url );
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }



        private bool ActivateDependencies( string template )
        {
            CASEditorUtils.CreateFolderInAssets( "Editor" );

            string fromPath = CASEditorUtils.GetTemplatePath( template + platform.ToString() + ".xml" );
            if (string.IsNullOrEmpty( fromPath ))
                return false;

            string dest = CASEditorUtils.editorFolderPath + "/"
                + template + platform.ToString() + CASEditorUtils.dependenciesExtension;
            return CASEditorUtils.TryCopyFile( fromPath, dest );
        }

        private bool DeactivateDependencies( string template )
        {
            return AssetDatabase.DeleteAsset( CASEditorUtils.editorFolderPath + "/"
                + template + platform.ToString() + CASEditorUtils.dependenciesExtension );
        }


        private struct PartnerNetwork
        {
            public readonly string name;
            public readonly string url;
            public readonly Audience androidAudience;
            public readonly Audience iOSAudience;
            public PartnerNetwork( string name, string url, Audience androidAudience, Audience iOSAudience )
            {
                this.name = name;
                this.url = url;
                this.androidAudience = androidAudience;
                this.iOSAudience = iOSAudience;
            }
            public Audience GetAudience( BuildTarget platform )
            {
                if (platform == BuildTarget.Android)
                    return androidAudience;
                return iOSAudience;
            }
        }
    }
}