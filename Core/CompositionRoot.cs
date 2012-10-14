namespace DbExtensions.Core
{
    using System.Collections;

    using DbExtensions.Decorators;
    using DbExtensions.Implementation;
    using DbExtensions.Interfaces;

    internal class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.RegisterAssembly(typeof(CompositionRoot).Assembly);
            serviceRegistry.Register<IKeyDelegateBuilder, KeyDelegateBuilder>();
            serviceRegistry.Register<IMethodEmitter<IStructuralEquatable>, ConstructorEmitter<IStructuralEquatable>>("keyInstanceEmitter");
            serviceRegistry.Register<IOneToManyExpressionBuilder>(
                factory =>
                new OneToManyExpressionBuilder(
                    factory.GetInstance<IPropertyMapper>(), factory.GetInstance<Interfaces.IPropertySelector>("CollectionPropertySelector"), type => factory.GetInstance(type)));
            serviceRegistry.Register<IManyToOneExpressionBuilder>(
                factory =>
                new ManyToOneExpressionBuilder(
                    factory.GetInstance<IPropertyMapper>(), factory.GetInstance<Interfaces.IPropertySelector>("ComplexPropertySelector"), type => factory.GetInstance(type)));                        
            serviceRegistry.Decorate(typeof(IMethodEmitter<>), typeof(CachedMethodEmitter<>));
            serviceRegistry.Decorate(typeof(IKeyDelegateBuilder), typeof(CachedKeyDelegateBuilder));
        }
    }
}