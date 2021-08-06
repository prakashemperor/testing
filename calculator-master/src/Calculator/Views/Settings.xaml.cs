using CalculatorApp.Utils;
using CalculatorApp.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CalculatorApp.ViewModel.Common.Automation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CalculatorApp
{
    public sealed partial class Settings : UserControl
    {
        // CSHARP_MIGRATION: TODO:
        // BUILD_YEAR was a C++/CX macro and may update the value from the pipeline
        private const string BUILD_YEAR = "2021";

        public Settings()
        {
            var locService = LocalizationService.GetInstance();
            var resourceLoader = AppResourceProvider.GetInstance();

            InitializeComponent();

            Language = locService.GetLanguage();

            InitializeAboutContentTextBlock();

            var copyrightText =
                LocalizationStringUtil.GetLocalizedString(resourceLoader.GetResourceString("AboutControlCopyright"), BUILD_YEAR);
            AboutControlCopyrightRun.Text = copyrightText;

            InitializeContributeTextBlock();
        }

        private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is RadioButton selectItem)
            {
                ThemeHelper.RootTheme = ThemeHelper.GetEnum<ElementTheme>(selectItem.Tag.ToString());
            }
        }

        public void SetDefaultFocus()
        {
            AppThemeExpander.Focus(FocusState.Programmatic);
        }

        // OnLoaded would be invoked by Popup several times while contructed once
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            AnnouncePageOpened();

            var currentTheme = ThemeHelper.RootTheme.ToString();
            (ThemeRadioButtons.Items.Cast<RadioButton>().FirstOrDefault(c => c?.Tag?.ToString() == currentTheme)).IsChecked = true;

            SetDefaultFocus();
        }

        private void AnnouncePageOpened()
        {
            string announcementText = AppResourceProvider.GetInstance().GetResourceString("SettingsPageOpenedAnnouncement");
            NarratorAnnouncement announcement = CalculatorAnnouncement.GetSettingsPageOpenedAnnouncement(announcementText);
            NarratorNotifier.Announce(announcement);
        }

        // OnUnloaded would be invoked by Popup several times while contructed once
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // back to the default state
            AppThemeExpander.IsExpanded = false;
        }

        private void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            PackageVersion version = Package.Current.Id.Version;
            string versionNumber = "Version ";
            versionNumber = versionNumber + version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision;
            _ = Launcher.LaunchUriAsync(new Uri("windows-feedback:?contextid=130&metadata=%7B%22Metadata%22:[%7B%22AppBuild%22:%22" + versionNumber + "%22%7D]%7D"));
        }

        private void InitializeAboutContentTextBlock()
        {
            SetVersionString();
        }

        private void SetVersionString()
        {
            PackageVersion version = Package.Current.Id.Version;
            string appName = AppResourceProvider.GetInstance().GetResourceString("AppName");
            AboutBuildVersion.Text = appName + " " + version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision;
        }

        private void InitializeContributeTextBlock()
        {
            var resProvider = AppResourceProvider.GetInstance();
            string contributeHyperlinkText = resProvider.GetResourceString("AboutControlContribute");

            // The resource string has the 'GitHub' hyperlink wrapped with '%HL%'.
            // Break the string and assign pieces appropriately.
            string delimiter = "%HL%";
            int delimiterLength = delimiter.Length;

            // Find the delimiters.
            int firstSplitPosition = contributeHyperlinkText.IndexOf(delimiter, 0);
            Debug.Assert(firstSplitPosition != -1);
            int secondSplitPosition = contributeHyperlinkText.IndexOf(delimiter, firstSplitPosition + 1);
            Debug.Assert(secondSplitPosition != -1);
            int hyperlinkTextLength = secondSplitPosition - (firstSplitPosition + delimiterLength);

            // Assign pieces.
            var contributeTextBeforeHyperlink = contributeHyperlinkText.Substring(0, firstSplitPosition);
            var contributeTextLink = contributeHyperlinkText.Substring(firstSplitPosition + delimiterLength, hyperlinkTextLength);
            var contributeTextAfterHyperlink = contributeHyperlinkText.Substring(secondSplitPosition + delimiterLength);

            ContributeRunBeforeLink.Text = contributeTextBeforeHyperlink;
            ContributeRunLink.Text = contributeTextLink;
            ContributeRunAfterLink.Text = contributeTextAfterHyperlink;
        }
    }
}
