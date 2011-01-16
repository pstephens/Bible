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

using System.IO;
using System.Text;
using BibleLib.Loader;
using BibleLib.Reader;
using NUnit.Framework;

namespace BibleLib.UnitTests.Loader
{
    [TestFixture]
    public class ResourceLoaderTests
    {
        private const string sampleTxtData =
            @"Some Sample Data. UTF8 with no byte order mark. <Used in ResourceLoaderTests unit tests. Do Not Change>";
        private const string sampleSubdirectoryTxtData =
            @"Some Sample Data. UTF8 with no byte order mark. <Used in ResourceLoaderTests ""Subdirectory"" unit tests. Do Not Change>";
        private const string sampleBaseTxtData =
            @"Some Sample Data. UTF8 with no byte order mark. <Used in ResourceLoaderTests ""Base"" unit tests. Do Not Change>";

        [TestCase(null, "ResourceLoaderTestData.txt", sampleTxtData)]
        [TestCase(null, "ResourceLoaderTestData/ResourceLoaderTestData.txt", sampleSubdirectoryTxtData, 
            Description = "With Subdirectory")]
        [TestCase("", "ResourceLoaderTestData.txt", sampleTxtData,
            Description = "With empty string for BaseUri")]
        [TestCase("ResourceLoaderTestData/", "BaseResourceLoaderTestData.txt", sampleBaseTxtData,
            Description = "With BaseUri")]
        public void ResourceLoader_should_read_data_asynchronously_from_xap(string baseUri, string path, string expectedValue)
        {
            var loader = (IResourceLoader) new ResourceLoader(baseUri);

            var str = loader.GetResourceStream(path);

            var data = ReadDataFromStream(str);

            Assert.That(data, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ResourceLoader_should_throw_if_resource_not_found()
        {
            var loader = (IResourceLoader) new ResourceLoader("ResourceLoaderTestData/");
            
            Assert.Throws<FileNotFoundException>(() => loader.GetResourceStream("FileNotFound.txt"));
        }

        private static string ReadDataFromStream(Stream str)
        {
            var buff = new byte[10000];
            var asyncResult = str.BeginRead(buff, 0, buff.Length, null, null);

            var bytesRead = str.EndRead(asyncResult);
            return Encoding.UTF8.GetString(buff, 0, bytesRead);
        }
    }
}