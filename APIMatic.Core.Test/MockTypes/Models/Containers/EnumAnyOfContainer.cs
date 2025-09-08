using System;
using Newtonsoft.Json;
using APIMatic.Core.Utilities.Converters;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
       typeof(UnionTypeConverter<EnumAnyOfContainer>),
       new Type[] {
            typeof(WorkingDaysCase),
            typeof(DaysCase),
            typeof(MonthNumberCase)
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

        public static EnumAnyOfContainer FromMonthNumber(MonthNumber value)
        {
            return new MonthNumberCase().Set(value);
        }

        public abstract T Match<T>(Func<WorkingDays, T> workingDays, Func<Days, T> days, Func<MonthNumber, T> monthNumber);

        [JsonConverter(typeof(UnionTypeCaseConverter<WorkingDaysCase, WorkingDays>))]
        private sealed class WorkingDaysCase : EnumAnyOfContainer, ICaseValue<WorkingDaysCase, WorkingDays>
        {
            public WorkingDays value;

            public override T Match<T>(Func<WorkingDays, T> workingDays, Func<Days, T> days, Func<MonthNumber, T> monthNumber)
            {
                return workingDays(value);
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
        private sealed class DaysCase : EnumAnyOfContainer, ICaseValue<DaysCase, Days>
        {
            public Days value;

            public override T Match<T>(Func<WorkingDays, T> workingDays, Func<Days, T> days, Func<MonthNumber, T> monthNumber)
            {
                return days(value);
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

        [JsonConverter(typeof(UnionTypeCaseConverter<MonthNumberCase, MonthNumber>))]
        private sealed class MonthNumberCase : EnumAnyOfContainer, ICaseValue<MonthNumberCase, MonthNumber>
        {
            public MonthNumber value;

            public override T Match<T>(Func<WorkingDays, T> workingDays, Func<Days, T> days, Func<MonthNumber, T> monthNumber)
            {
                return monthNumber(value);
            }

            public MonthNumberCase Set(MonthNumber value)
            {
                this.value = value;
                return this;
            }

            public MonthNumber Get()
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
