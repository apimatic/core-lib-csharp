using System;
using Newtonsoft.Json;
using APIMatic.Core.Utilities.Converters;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
       typeof(UnionTypeConverter<EnumAnyOfContainer>),
       new Type[] {
            typeof(WorkingDaysCase),
            typeof(DaysCase)
       },
       false
   )]
    public abstract class EnumAnyOfContainer
    {
        public static EnumAnyOfContainer FromWorkingDays(WorkingDays value)
        {
            return new WorkingDaysCase().Set(value);
        }

        public static EnumAnyOfContainer FromDays(Days value)
        {
            return new DaysCase().Set(value);
        }


        public abstract T Match<T>(ICases<T> cases);

        public interface ICases<out T>
        {
            T WorkingDays(WorkingDays value);

            T Days(Days value);
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<WorkingDaysCase, WorkingDays>))]
        private class WorkingDaysCase : EnumAnyOfContainer, ICaseValue<WorkingDaysCase, WorkingDays>
        {
            public WorkingDays value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.WorkingDays(value);
            }

            public WorkingDaysCase Set(WorkingDays value)
            {
                this.value = value;
                return this;
            }

            public WorkingDays Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<DaysCase, Days>))]
        private class DaysCase : EnumAnyOfContainer, ICaseValue<DaysCase, Days>
        {
            public Days value;

            public override T Match<T>(ICases<T> cases)
            {
                return cases.Days(value);
            }

            public DaysCase Set(Days value)
            {
                this.value = value;
                return this;
            }

            public Days Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }
    }
}
