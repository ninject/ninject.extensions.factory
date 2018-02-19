﻿//-------------------------------------------------------------------------------
// <copyright file="FuncTests.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Extensions.Factory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Ninject.Extensions.Factory.Fakes;

    using Xunit;
    using Xunit.Extensions;

    public class FuncTests : IDisposable
    {
        private readonly StandardKernel kernel;

        public FuncTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void FuncInjection()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();

            var service = this.kernel.Get<ClassWithFunc>();
            var weapon1 = service.CreateWeapon();
            var weapon2 = service.CreateWeapon();

            weapon1.Should().BeOfType<Sword>();
            weapon2.Should().BeOfType<Sword>();
            weapon1.Should().NotBeSameAs(weapon2);
        }

        [Fact]
        public void FuncInjectionWithParameters()
        {
            const string Name = "Excalibur";
            const int Width = 34;
            const int Length = 123;

            this.kernel.Bind<ICustomizableWeapon>().To<CustomizableSword>();

            var service = this.kernel.Get<ClassWithParameterizedFunc>();
            var weapon = service.CreateWeapon(Name, Width, Length);

            weapon.Should().BeOfType<CustomizableSword>();
            weapon.Name.Should().Be(Name);
            weapon.Width.Should().Be(Width);
            weapon.Length.Should().Be(Length);
        }

        [Fact]
        public void FuncInjectionWithParametersAndConfiguredValues()
        {
            const string Name = "Excalibur";
            const int Width = 34;
            const int Length = 123;

            this.kernel.Bind<ICustomizableWeapon>()
                .To<CustomizableSword>()
                .WithConstructorArgument("width", Width);

            var service = this.kernel.Get<ClassWithParameterizedFunc>();
            var weapon = service.CreateWeapon(Name, Length);

            weapon.Should().BeOfType<CustomizableSword>();
            weapon.Name.Should().Be(Name);
            weapon.Width.Should().Be(Width);
            weapon.Length.Should().Be(Length);
        }

        [Fact]
        public void FuncInjectionWithParametersAndInjectedValues()
        {
            const string Name = "Excalibur";
            const int Width = 34;
            const int Length = 123;

            this.kernel.Bind<ICustomizableWeapon>().To<CustomizableSword>();
            this.kernel.Bind<int>().ToConstant(Length);

            var service = this.kernel.Get<ClassWithParameterizedFunc>();
            var weapon = service.CreateWeapon(Name, Width);

            weapon.Should().BeOfType<CustomizableSword>();
            weapon.Name.Should().Be(Name);
            weapon.Width.Should().Be(Width);
            weapon.Length.Should().Be(Length);
        }

        [Fact]
        public void Eumerable()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var service = this.kernel.Get<ClassWithCollectionFunc>();
            var weapons = service.CreateWeaponsEnumerable();

            weapons.Should().HaveCount(2);
            weapons.Any(w => w.GetType() == typeof(Sword)).Should().BeTrue();
            weapons.Any(w => w.GetType() == typeof(Dagger)).Should().BeTrue();
        }

        [Fact]
        public void Collection()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var service = this.kernel.Get<ClassWithCollectionFunc>();
            var weapons = service.CreateWeaponsCollection();

            weapons.Should().HaveCount(2);
            weapons.Any(w => w.GetType() == typeof(Sword)).Should().BeTrue();
            weapons.Any(w => w.GetType() == typeof(Dagger)).Should().BeTrue();
        }

        [Fact]
        public void ListInterface()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var service = this.kernel.Get<ClassWithCollectionFunc>();
            var weapons = service.CreateWeaponsListInterface();

            weapons.Should().HaveCount(2);
            weapons.Any(w => w.GetType() == typeof(Sword)).Should().BeTrue();
            weapons.Any(w => w.GetType() == typeof(Dagger)).Should().BeTrue();
        }

        [Fact]
        public void List()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var service = this.kernel.Get<ClassWithCollectionFunc>();
            var weapons = service.CreateWeaponsList();

            weapons.Should().HaveCount(2);
            weapons.Any(w => w.GetType() == typeof(Sword)).Should().BeTrue();
            weapons.Any(w => w.GetType() == typeof(Dagger)).Should().BeTrue();
        }

        [Fact]
        public void Array()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var service = this.kernel.Get<ClassWithCollectionFunc>();
            var weapons = service.CreateWeaponsArray();

            weapons.Should().HaveCount(2);
            weapons.Any(w => w.GetType() == typeof(Sword)).Should().BeTrue();
            weapons.Any(w => w.GetType() == typeof(Dagger)).Should().BeTrue();
        }

#if !NET_35 && !SILVERLIGHT_30 && !SILVERLIGHT_20 && !WINDOWS_PHONE && !NETCF_35
        [Fact]
        public void LazyInjection()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();

            var service = this.kernel.Get<ClassWithLazy>();
            var weapon1 = service.GetWeapon();
            var weapon2 = service.GetWeapon();

            weapon1.Should().BeOfType<Sword>();
            weapon2.Should().BeOfType<Sword>();
            weapon1.Should().BeSameAs(weapon2);
        }

        [Fact]
        public void GenericLazyInjection()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();

            var service = this.kernel.Get<ClassWithGenericLazy>();
            var weapon1 = service.GetWeapon();
            var weapon2 = service.GetWeapon();

            weapon1.Should().BeOfType<Sword>();
            weapon2.Should().BeOfType<Sword>();
            weapon1.Should().BeSameAs(weapon2);
        }
#endif

        [Theory]
        [InlineData(new[] { 1 }, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2 }, new[] { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3 }, new[] { 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
#if !NET_35 && !SILVERLIGHT_30 && !SILVERLIGHT_20 && !WINDOWS_PHONE && !NETCF_35
        [InlineData(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6 }, new[] { 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7 }, new[] { 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 0, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0 })]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })]
#endif
        public void FuncOverloads(int[] input, int[] expectedValues)
        {
            var methodInfo = typeof(FuncOverloadTestFake).GetMethod("Create", input.Select(i => i.GetType()).ToArray());

            var factory = this.kernel.Get<FuncOverloadTestFake>();
            var instance = (FuncOverloadTestImplementationFake)methodInfo.Invoke(factory, input.Cast<object>().ToArray());

            instance.Arg1.Should().Be(expectedValues[0]);
            instance.Arg2.Should().Be(expectedValues[1]);
            instance.Arg3.Should().Be(expectedValues[2]);
            instance.Arg4.Should().Be(expectedValues[3]);
#if !NET_35 && !SILVERLIGHT_30 && !SILVERLIGHT_20 && !WINDOWS_PHONE && !NETCF_35
            instance.Arg5.Should().Be(expectedValues[4]);
            instance.Arg6.Should().Be(expectedValues[5]);
            instance.Arg7.Should().Be(expectedValues[6]);
            instance.Arg8.Should().Be(expectedValues[7]);
            instance.Arg9.Should().Be(expectedValues[8]);
            instance.Arg10.Should().Be(expectedValues[9]);
            instance.Arg11.Should().Be(expectedValues[10]);
            instance.Arg12.Should().Be(expectedValues[11]);
            instance.Arg13.Should().Be(expectedValues[12]);
            instance.Arg14.Should().Be(expectedValues[13]);
            instance.Arg15.Should().Be(expectedValues[14]);
            instance.Arg16.Should().Be(expectedValues[15]);
#endif
        }
    }
}