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

using System.Text;
using BibleLib.Reader;
using NUnit.Framework;

namespace BibleLoader.WP7.UnitTests
{
    [TestFixture]
    public class ResourceLoaderTests
    {
        [Test]
        public void ResourceLoader_should_read_data_asynchronously_from_xap()
        {
            var loader = CreateResourceLoaderUnderTest();

            var str = loader.GetResourceStream("Sample.txt");

            var buff = new byte[10000];
            var asyncResult = str.BeginRead(buff, 0, buff.Length, null, null);

            var bytesRead = str.EndRead(asyncResult);
            var data = Encoding.UTF8.GetString(buff, 0, bytesRead);

            Assert.That(data, Is.EqualTo("Some Sample Data. <Use in a unit tests. Do Not Change>"));
        }

        private static IResourceLoader CreateResourceLoaderUnderTest()
        {
            return new ResourceLoader();
        }
    }
}