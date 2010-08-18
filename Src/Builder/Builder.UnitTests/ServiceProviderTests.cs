#region Copyright Notice

/* Copyright 2009-2010 Peter Stephens

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
using NUnit.Framework;

namespace Builder.UnitTests
{
    [TestFixture]
    public class ServiceProviderTests
    {
        [Test]
        public void GetService_should_create_instance_of_FooService()
        {
            var related = new Foo();

            var foo = related.GetService<FooService>();

            Assert.That(foo, Is.InstanceOf(typeof (FooService)));
        }
    }

    public class Foo : ServiceProvider<IFoo>, IFoo 
    {
        public string SomeData { get; set; }
    }

    public interface IFoo
    {
        string SomeData { get; set; }
    }

    public class FooService : IService<IFoo>
    {
        public IFoo Related { get; set; }
    }
}