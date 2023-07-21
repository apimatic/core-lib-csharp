using APIMatic.Core.Utilities;
using System;
using Newtonsoft.Json;

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
        private class WorkingDaysCase : EnumAnyOfContainer, ICaseValue<WorkingDaysCase, WorkingDays>, ICustomConverter
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

            public JsonConverter GetJsonConverter()
            {
                return new EnumCaseConverter<WorkingDays>();
            }
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<DaysCase, Days>))]
        private class DaysCase : EnumAnyOfContainer, ICaseValue<DaysCase, Days>, ICustomConverter
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

            public JsonConverter GetJsonConverter()
            {
                return new EnumCaseConverter<Days>();
            }
        }
    }
}
