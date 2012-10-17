namespace DbExtensions.Core
{
    using System.Collections;

    using DbExtensions.Decorators;
    using DbExtensions.Implementation;
    using DbExtensions.Interfaces;

    /// <summary>
    /// A <see cref="ICompositionRoot"/>implementation that registers the required services for this library. 
    /// </summary>
    internal class CompositionRoot : ICompositionRoot
    {
        /// <summary>
        /// Composes services by adding services to the <paramref name="serviceRegistry"/>.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        public void Compose(IServiceRegistry serviceRegistry)
        {
                        
            serviceRegistry.RegisterAssembly(typeof(CompositionRoot).Assembly);
            serviceRegistry.Register<IColumnSelector, ColumnSelector>(new PerGraphLifetime());
            serviceRegistry.Decorate(typeof(IColumnSelector), typeof(CachedColumnSelector));
            serviceRegistry.Register<IConstructorSelector, ConstructorSelector>(new SingletonLifetime());
            serviceRegistry.Decorate(typeof(IConstructorSelector), typeof(CachedConstructorSelector));
            serviceRegistry.Register(typeof(IDataReaderMapper<>), typeof(DataReaderMapper<>));
            serviceRegistry.Register(typeof(IDataRecordMapper<>), typeof(DataRecordMapper<>), new PerGraphLifetime());

            serviceRegistry.Register<IKeyDelegateBuilder, KeyDelegateBuilder>(new PerGraphLifetime());
            serviceRegistry.Decorate(typeof(IKeyDelegateBuilder), typeof(CachedKeyDelegateBuilder));

            serviceRegistry.Register(typeof(IInstanceDelegateBuilder<>), typeof(InstanceDelegateBuilder<>), new PerGraphLifetime());
            serviceRegistry.Decorate(typeof(IInstanceDelegateBuilder<>),typeof(CachedInstanceDelegateBuilder<>));

            serviceRegistry.Register<IMapper<IStructuralEquatable>, ConstructorEmitter<IStructuralEquatable>>("keyInstanceEmitter");
            serviceRegistry.Register<IOneToManyExpressionBuilder>(
                factory =>
                new OneToManyExpressionBuilder(
                    factory.GetInstance<IPropertyMapper>(), factory.GetInstance<Interfaces.IPropertySelector>("CollectionPropertySelector"), type => factory.GetInstance(type)));
            serviceRegistry.Register<IManyToOneExpressionBuilder>(
                factory =>
                new ManyToOneExpressionBuilder(
                    factory.GetInstance<IPropertyMapper>(), factory.GetInstance<Interfaces.IPropertySelector>("ComplexPropertySelector"), type => factory.GetInstance(type)));                        
            serviceRegistry.Decorate(typeof(IMapper<>), typeof(CachedMapper<>));
            
            
        }
    }
}