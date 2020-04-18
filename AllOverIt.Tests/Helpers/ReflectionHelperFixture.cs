using AllOverIt.Reflection;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
  public class ReflectionHelperFixture : AllOverItFixtureBase
  {
    private class DummyBaseClass
    {
      public int Prop1 { get; set; }
      public string Prop2 { get; set; }
      public virtual double Prop3 { get; set; }

      public void Method1()
      {
      }

      private void Method2()
      {
      }
    }

    private class DummySuperClass : DummyBaseClass
    {
      public override double Prop3 { get; set; }
      private long Prop4 { get; set; }

      public void Method3()
      {
      }

      private void Method4()
      {
      }
    }

    private class DummyComposite<T1, T2>
    {
    }

    public enum DummyEnum { One, Two, Three }

    public class GetPropertyInfo_Property : ReflectionHelperFixture
    {
      [Fact]
      public void Should_Get_Property_In_Super()
      {
        var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("Prop3");

        actual.Should().BeEquivalentTo(
          new
          {
            Name = "Prop3",
            PropertyType = typeof(double)
          }
        );
      }

      [Fact]
      public void Should_Get_Property_In_Base()
      {
        var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("Prop1");

        actual.Should().BeEquivalentTo(
          new
          {
            Name = "Prop1",
            PropertyType = typeof(int)
          }
        );
      }

      [Fact]
      public void Should_Not_Find_Property()
      {
        var actual = (object)ReflectionHelper.GetPropertyInfo<DummySuperClass>("PropXYZ");

        actual.Should().BeNull();
      }
    }

    public class GetPropertyInfo_Bindings : ReflectionHelperFixture
    {
      [Fact]
      public void Should_Use_Default_Binding_Not_Declared_Only()
      {
        var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>();

        actual.Should().BeEquivalentTo(
          new
          {
            Name = "Prop1",
            PropertyType = typeof(int)
          },
          new
          {
            Name = "Prop2",
            PropertyType = typeof(string)
          },
          new
          {
            Name = "Prop3",
            PropertyType = typeof(double)
          }
        );
      }

      [Fact]
      public void Should_Use_Default_Binding_Declared_Only()
      {
        var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>(BindingOptions.Default, true);

        actual.Should().BeEquivalentTo(
          new
          {
            Name = "Prop3",
            PropertyType = typeof(double)
          }
        );
      }

      [Fact]
      public void Should_Include_Private_Property()
      {
        var binding = BindingOptions.DefaultScope | BindingOptions.Private | BindingOptions.DefaultAccessor | BindingOptions.DefaultVisibility;

        var actual = ReflectionHelper.GetPropertyInfo<DummySuperClass>(binding, false);

        actual.Single(item => item.Name == "Prop4").Should().NotBeNull();
      }
    }

    public class GetMethodInfo : ReflectionHelperFixture
    {
      private readonly string[] _knownMethods = new[] { "Method1", "Method2", "Method3", "Method4" };

      // GetMethod() returns methods of object as well as property get/set methods, so these tests filter down to expected (non-property) methods in the dummy classes

      [Fact]
      public void Should_Use_Default_Binding_Not_Declared_Only()
      {
        var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>()
          .Where(item => _knownMethods.Contains(item.Name))
          .Select(item => new
          {
            item.Name,
            item.DeclaringType
          });

        actual.Should().BeEquivalentTo(
          new
          {
            Name = "Method1",
            DeclaringType = typeof(DummyBaseClass)
          },
          new
          {
            Name = "Method3",
            DeclaringType = typeof(DummySuperClass)
          }
        );
      }

      [Fact]
      public void Should_Use_Default_Binding_Declared_Only()
      {
        var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(BindingOptions.Default, true)
          .Where(item => _knownMethods.Contains(item.Name))
          .Select(item => new
          {
            item.Name,
            item.DeclaringType
          });

        actual.Should().BeEquivalentTo(
          new
          {
            Name = "Method3",
            DeclaringType = typeof(DummySuperClass)
          }
        );
      }

      [Fact]
      public void Should_Get_All_Base_Methods_Only()
      {
        var actual = ReflectionHelper.GetMethodInfo<DummyBaseClass>(BindingOptions.All, true)
          .Where(item => _knownMethods.Contains(item.Name))
          .Select(item => new
          {
            item.Name,
            item.DeclaringType
          });

        actual.Should().BeEquivalentTo(
          new
          {
            Name = "Method1",
            DeclaringType = typeof(DummyBaseClass)
          },
          new
          {
            Name = "Method2",
            DeclaringType = typeof(DummyBaseClass)
          }
        );
      }

      [Fact]
      public void Should_Get_All_Super_Methods_Only()
      {
        var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(BindingOptions.All, true)
          .Where(item => _knownMethods.Contains(item.Name))
          .Select(item => new
          {
            item.Name,
            item.DeclaringType
          });

        actual.Should().BeEquivalentTo(
          new
          {
            Name = "Method3",
            DeclaringType = typeof(DummySuperClass)
          },
          new
          {
            Name = "Method4",
            DeclaringType = typeof(DummySuperClass)
          }
        );
      }

      [Fact]
      public void Should_Get_Private_Methods_Only()
      {
        var actual = ReflectionHelper.GetMethodInfo<DummySuperClass>(BindingOptions.Private, false)   // default scope and visibility is implied
          .Where(item => _knownMethods.Contains(item.Name))
          .Select(item => new
          {
            item.Name,
            item.DeclaringType
          });

        actual.Should().BeEquivalentTo(
          new
          {
            Name = "Method2",
            DeclaringType = typeof(DummyBaseClass)
          },
          new
          {
            Name = "Method4",
            DeclaringType = typeof(DummySuperClass)
          }
        );
      }
    }
  }
}