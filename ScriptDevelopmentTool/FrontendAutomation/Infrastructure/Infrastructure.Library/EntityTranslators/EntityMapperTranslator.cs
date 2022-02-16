/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
//----------------------------------------------------------------------------------------
// patterns & practices - Smart Client Software Factory - Guidance Package
//
// The EntityMapperTranslator class is a base helper implementation of an IEntityTranslator
// that provides placeholders to translate from business entities to service and viceversa
// 
//  
//
//
// Latest version of this Guidance Package: http://go.microsoft.com/fwlink/?LinkId=62182
//----------------------------------------------------------------------------------------

using System;
using IMSWorkBench.Infrastructure.Interface.Services;

namespace IMSWorkBench.Infrastructure.Library.EntityTranslators
{
    public abstract class EntityMapperTranslator<TBusinessEntity, TServiceEntity> : BaseTranslator
    {
        public override bool CanTranslate(Type targetType, Type sourceType)
        {
            return (targetType == typeof(TBusinessEntity) && sourceType == typeof(TServiceEntity)) ||
                (targetType == typeof(TServiceEntity) && sourceType == typeof(TBusinessEntity));
        }

        public override object Translate(IEntityTranslatorService service, Type targetType, object source)
        {
            if (targetType == typeof(TBusinessEntity))
                return ServiceToBusiness(service, (TServiceEntity)source);
            if (targetType == typeof(TServiceEntity))
                return BusinessToService(service, (TBusinessEntity)source);

            throw new EntityTranslatorException();
        }

        protected abstract TServiceEntity BusinessToService(IEntityTranslatorService service, TBusinessEntity value);
        protected abstract TBusinessEntity ServiceToBusiness(IEntityTranslatorService service, TServiceEntity value);
    }
}
