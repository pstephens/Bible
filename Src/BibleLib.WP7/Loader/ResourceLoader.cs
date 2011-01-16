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

using System;
using System.IO;
using System.Windows;
using BibleLib.Reader;

namespace BibleLib.Loader
{
    public class ResourceLoader : IResourceLoader
    {
        private readonly string baseUri;

        public ResourceLoader(string baseUri)
        {
            this.baseUri = baseUri ?? "";
        }

        public Stream GetResourceStream(string name)
        {
            var resourceUri = new Uri(baseUri + name, UriKind.Relative);
            var resourceInfo = Application.GetResourceStream(resourceUri);
            if(resourceInfo == null)
                throw new FileNotFoundException(
                    string.Format("Resource {0} was not found.", resourceUri));
            return resourceInfo.Stream;
        }
    }
}