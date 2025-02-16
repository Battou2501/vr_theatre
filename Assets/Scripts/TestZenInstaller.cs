using UnityEngine;
using Zenject;

public class TestZenInstaller : MonoInstaller
{
    public TestZenTick tickMono;
    
    public override void InstallBindings()
    {
        Container.Bind<TestZenTick>().FromInstance(tickMono).AsSingle();
        //Container.BindInterfacesTo<TestZenTick.TestTickable>().AsTransient();
        Container.BindFactory<TestZenTick.TestTickable, TestZenTick.TestTickable.Factory>().FromFactory<TestZenTick.TestTickableFactory>();
        //Container.BindInterfacesTo<TestZenTick.TestTickableFactory>();
    }
}
