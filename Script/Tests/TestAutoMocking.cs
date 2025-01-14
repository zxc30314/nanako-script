using NUnit.Framework;
using UnityEngine;
using NUnitAssert = NUnit.Framework.Assert;

namespace Zenject.Tests
{
    public class TestAutoMocking : ZenjectUnitTestFixture
    {
        private BarMo _addComponent;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            Container.Bind<IFoo>().FromMock();
            _addComponent = Container.InstantiateComponent<BarMo>(new GameObject());
        }

        [Test]
        public  void Test1()
        {
            _addComponent.Run();
        }

        public class BarMo : MonoBehaviour
        {
            [Inject] IFoo _foo;

            public void Run()
            {
                _foo.DoSomething();

                var result = _foo.GetTest();

                NUnitAssert.IsNull(result);
            }
        }

        public interface IFoo
        {
            string GetTest();
            void DoSomething();
        }
    }
}