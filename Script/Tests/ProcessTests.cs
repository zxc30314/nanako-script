using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;
[Category("Script")]
public class ProcessTests : ZenjectUnitTestFixture
{
    private Process _process;
    private MainProcess _customData;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _customData = ScriptableObject.CreateInstance<MainProcess>();
        Container.Bind<IUIControl>().FromMock();
        Container.Bind<MainProcess>().FromInstance(_customData);
        _process = Container.InstantiateComponent<Process>(new GameObject());
     
    }

    [TearDown]
    public override void Teardown()
    {
        base.Teardown();
        Object.Destroy(_process.gameObject);
    }

    [Test]
    public void Init_WhenCalled_ShouldInitializeProcessData()
    {
        
        // Arrange
        var processData = new List<ProcessData>
        {
            new MockProcessData(),
            new MockProcessData(),
            new MockProcessData(),
            new MockProcessData(),
        };
        _customData.processData = processData;

        // Act
        _process.Init();

        // Assert
        Assert.AreEqual(processData[0], _process.CurrentProcessData);
        Assert.AreEqual(processData[1], _process.NextProcessData);
        foreach (var data in _customData.processData)
        {
            Assert.AreEqual(true, (data as MockProcessData).IsInitEnd);
        }
    }

    [Test]
    public void NextProcess_WhenCalled_ShouldChangeStateAndProcessData()
    {
        // Arrange
        var processData = new List<ProcessData>
        {
            new MockProcessData(),
            new MockProcessData(),
            new MockProcessData(),
            new MockProcessData(),
        };
        _customData.processData = processData;
        _process.Init();

        // Act
        _process.NextProcess();

        // Assert
        Assert.AreEqual(processData[1], _process.CurrentProcessData);
        Assert.AreEqual(processData[2], _process.NextProcessData);
    }

    [Test]
    public void NextProcess_WhenNoMoreProcessData_ShouldChangeToFirstProcessData()
    {
        // Arrange
        var processData = new List<ProcessData>
        {
            new MockProcessData(),
            new MockProcessData(),
            new MockProcessData(),
            new MockProcessData(),
        };
        _customData.processData = processData;
        _process.Init();
        _process.NextProcess();
        _process.NextProcess();
        _process.NextProcess();

        // Act
        _process.NextProcess();

        // Assert
        Assert.AreEqual(processData[0], _process.CurrentProcessData);
        Assert.AreEqual(processData[1], _process.NextProcessData);
    }
}