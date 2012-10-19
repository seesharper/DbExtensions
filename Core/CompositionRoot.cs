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
            serviceRegistry.Register<IColumnSelector, ColumnSelector>(new PerGraphLifetime());
            serviceRegistry.Decorate(typeof(IColumnSelector), typeof(CachedColumnSelector));
            serviceRegistry.Register<IConstructorSelector, ConstructorSelector>(new SingletonLifetime());
            serviceRegistry.Decorate(typeof(IConstructorSelector), typeof(CachedConstructorSelector));
            serviceRegistry.Register(typeof(IDataReaderMapper<>), typeof(DataReaderMapper<>));
            serviceRegistry.Register(typeof(IDataRecordMapper<>), typeof(DataRecordMapper<>), new PerGraphLifetime());

            serviceRegistry.Register<IKeyDelegateBuilder, KeyDelegateBuilder>(new PerGraphLifetime());
            serviceRegistry.Decorate(typeof(IKeyDelegateBuilder), typeof(CachedKeyDelegateBuilder));

            serviceRegistry.Register(typeof(IInstanceDelegateBuilder<>), typeof(InstanceDelegateBuilder<>), new PerGraphLifetime());
            serviceRegistry.Decorate(typeof(IInstanceDelegateBuilder<>), typeof(CachedInstanceDelegateBuilder<>));

            serviceRegistry.Register<IOrdinalSelector, OrdinalSelector>(new PerGraphLifetime());
            serviceRegistry.Decorate(typeof(IOrdinalSelector), typeof(CachedOrdinalSelector));

            serviceRegistry.Register(typeof(IRelationDelegateBuilder<>), typeof(ManyToOneDelegateBuilder<>), "ManyToOneDelegateBuilder", new PerGraphLifetime());
            serviceRegistry.Register(typeof(IRelationDelegateBuilder<>), typeof(OneToManyDelegateBuilder<>), "OneToManyDelegateBuilder", new PerGraphLifetime());
            serviceRegistry.Decorate(typeof(IRelationDelegateBuilder<>), typeof(CachedRelationDelegateBuilder<>));

            serviceRegistry.Register(typeof(IInstanceExpressionBuilder<>), typeof(InstanceExpressionBuilder<>));

            serviceRegistry.Register(typeof(IMapperDelegateBuilder<>), typeof(ConstructorMapperDelegateBuilder<>), "ConstructorMapperDelegateBuilder", new SingletonLifetime());
            serviceRegistry.Register(typeof(IMapperDelegateBuilder<>), typeof(PropertyMapperDelegateBuilder<>), "PropertyMapperDelegateBuilder", new SingletonLifetime());
            
            serviceRegistry.Register(typeof(IMethodSkeleton<>), typeof(DynamicMethodSkeleton<>));
            
            serviceRegistry.Register<IMethodSelector, MethodSelector>(new SingletonLifetime());
            serviceRegistry.Decorate(typeof(IMethodSelector), typeof(CachedMethodSelector));

            serviceRegistry.Register(typeof(IManyToOneDelegateBuilder<>), typeof(ManyToOneDelegateBuilder<>));

            serviceRegistry.Register<Interfaces.IPropertySelector, Implementation.PropertySelector>(new SingletonLifetime());
            serviceRegistry.Register<Interfaces.IPropertySelector, ComplexPropertySelector>("ComplexPropertySelector", new SingletonLifetime());
            serviceRegistry.Register<Interfaces.IPropertySelector, CollectionPropertySelector>("CollectionPropertySelector", new SingletonLifetime());
            serviceRegistry.Decorate(typeof(Interfaces.IPropertySelector), typeof(CachedPropertySelector));


            serviceRegistry.Register<IPropertyMapper, PropertyMapper>(new PerGraphLifetime());
            serviceRegistry.Decorate(typeof(IPropertyMapper), typeof(CachedPropertyMapper));

            serviceRegistry.Register<IMapperDelegateBuilder<IStructuralEquatable>, ConstructorMapperDelegateBuilder<IStructuralEquatable>>("keyInstanceEmitter");
            serviceRegistry.Register<IOneToManyExpressionBuilder>(
                factory =>
                new OneToManyExpressionBuilder(
                    factory.GetInstance<IPropertyMapper>(), factory.GetInstance<Interfaces.IPropertySelector>("CollectionPropertySelector"), type => factory.GetInstance(type)));
            serviceRegistry.Register<IManyToOneExpressionBuilder>(
                factory =>
                new ManyToOneExpressionBuilder(
                    factory.GetInstance<IPropertyMapper>(), factory.GetInstance<Interfaces.IPropertySelector>("ComplexPropertySelector"), type => factory.GetInstance(type)));                        
            serviceRegistry.Decorate(typeof(IMapperDelegateBuilder<>), typeof(CachedMapperDelegateBuilder<>));                        
        }
    }
}