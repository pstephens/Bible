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
using Builder.Model;
using NUnit.Framework;

namespace Builder.UnitTests.Model
{
    [TestFixture]
    public class ServiceProviderTests
    {
        [Test]
        public void GetService_should_create_instance_of_FooService()
        {
            var related = new Foo();

            var fooService = related.GetService<FooService>();

            Assert.That(fooService, Is.InstanceOf(typeof (FooService)));
        }

        [Test]
        public void GetService_should_return_same_instance_of_FooService_when_called_twice()
        {
            var related = new Foo();

            var fooService1 = related.GetService<FooService>();
            var fooService2 = related.GetService<FooService>();

            Assert.That(fooService1, Is.SameAs(fooService2));
        }

        [Test]
        public void GetService_should_set_Related_property_to_Foo_on_service()
        {
            var related = new Foo();

            var fooService = related.GetService<FooService>();

            Assert.That(fooService.Related, Is.EqualTo(related));
        }

        [Test]
        public void GetService_when_related_doesnt_implement_proper_interface_should_throw()
        {
            var related = new Foo2();

            Assert.Throws<InvalidOperationException>(() => related.GetService<FooService>());
        }
    }

    public class Foo : ServiceProvider<IFoo>, IFoo 
    {
        public string SomeData { get; set; }
    }

    public class Foo2 : ServiceProvider<IFoo>
    {
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