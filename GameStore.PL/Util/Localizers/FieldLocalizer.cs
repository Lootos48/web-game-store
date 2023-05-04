using AutoMapper.Execution;
using GameStore.PL.Util.Localizers.Interfaces;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace GameStore.PL.Util.Localizers
{
    public static class FieldLocalizer
    {
        public static string GetLocalizedField<T>(Expression<Func<T, string>> field, T entity) where T : class, ILocalizable
        {
            var fieldName = field.GetMember().Name;

            return GetLocalizedField(fieldName, entity);
        }

        public static string GetLocalizedField<T>(string fieldName, T entity) where T : class, ILocalizable
        {
            var localizationsFieldName = typeof(ILocalizable).GetProperties()[0].Name;

            var localizations = entity.GetType().GetProperty(localizationsFieldName).GetValue(entity, null);

            object fieldValue;
            if (localizations is IList localizationsList && localizationsList.Count != 0)
            {
                var localization = localizationsList[0];
                fieldValue = localization
                    .GetType()
                    .GetProperty(fieldName)
                    .GetValue(localization, null);
            }
            else
            {
                fieldValue = entity
                    .GetType()
                    .GetProperty(fieldName)
                    .GetValue(entity, null);
            }

            return fieldValue is null ? "" : fieldValue.ToString();
        }
    }
}
