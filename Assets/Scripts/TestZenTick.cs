using System.Collections.Generic;
using UnityEngine;
using Zenject;
using IInitializable = Zenject.IInitializable;

public class TestZenTick : MonoBehaviour
{
    private DiContainer _container;
    private TestTickable.Factory _factory;
    
    [Inject]
    private void Construct(DiContainer container, TestTickable.Factory factory)
    {
        _container = container;
        _factory = factory;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_factory == null || _container == null) return;

        //var t = new  TestTickable();
        //_container.BindInterfacesAndSelfTo<TestTickable>().FromInstance(t).AsSingle().NonLazy();
        var t = _factory.Create();
        t.name = "TestZenTick";
        //var t = _container.Instantiate<TestTickable>();
        //_container.BindInterfacesTo<TestZenTick.TestTickable>().AsTransient();
        //t.name = "1";
        //t = _container.Instantiate<TestTickable>();
        //t.name = "2";
        //_container.BindInterfacesAndSelfTo<TestTickable>().FromFactory<TestTickable,TestTickable.Factory>().NonLazy();
        //_container.BindInterfacesAndSelfTo<TestTickable>().FromNew().NonLazy();
    }

    public class TestTickable : ITickable, IInitializable
    {
        public string name;
        public void Tick()
        {
            Debug.Log("Tick " + name);
        }
        
        public class Factory : PlaceholderFactory<TestTickable> { }

        public void Initialize()
        {
            Debug.Log("Initialize " + name);
        }
    }

    public class TestTickableFactory : IFactory<TestTickable>
    {
        
        private readonly List<TestTickable> _tickables = new ();
        
        private DiContainer _container;
        
        [Inject]
        private void Construct(DiContainer container)
        {
            _container = container;
        }
        
        public TestTickable Create()
        {
            var t = new TestTickable();
            _container.Resolve<TickableManager>().Add(t);
            return t;
        }
    }
}
