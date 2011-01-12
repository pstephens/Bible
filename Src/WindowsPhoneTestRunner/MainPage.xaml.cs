#region Copyright Notice

/* Copyright 2009-2011 Peter Stephens

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

#endregion

using System.Windows;
using Microsoft.Silverlight.Testing;

namespace WindowsPhoneTestRunner
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            UnitTestSystem.RegisterUnitTestProvider(
                new Microsoft.Silverlight.Testing.UnitTesting.Metadata.NUnit.NUnitProvider());

            var settings = UnitTestSystem.CreateDefaultSettings();
            GetTestAssemblies(settings);

            var testPage = UnitTestSystem.CreateTestPage(settings);
            BackKeyPress += (x, xe) => xe.Cancel = ((IMobileTestPage)testPage).NavigateBack();
            Content = testPage;
        }

        private static void GetTestAssemblies(UnitTestSettings settings)
        {
            // Add more assemblies here that need to be unit tested.
            settings.TestAssemblies.Add(BibleLib.UnitTests.Properties.UnitTestAssembly.GetAssembly());
            settings.TestAssemblies.Add(BibleLoader.WP7.UnitTests.Properties.UnitTestAssembly.GetAssembly());
        }
    }
}