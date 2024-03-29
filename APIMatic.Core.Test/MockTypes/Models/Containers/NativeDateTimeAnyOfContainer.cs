﻿using APIMatic.Core.Utilities.Converters;
using APIMatic.Core.Utilities.Date;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace APIMatic.Core.Test.MockTypes.Models.Containers
{
    [JsonConverter(
        typeof(UnionTypeConverter<NativeDateTimeAnyOfContainer>),
        new Type[] {
            typeof(DateTimeCase),
            typeof(DateTimeCase2)
        },
        false
    )]
    public abstract class NativeDateTimeAnyOfContainer
    {
        public static NativeDateTimeAnyOfContainer FromRFC1123DateTime(DateTime value)
        {
            return new DateTimeCase().Set(value);
        }

        public static NativeDateTimeAnyOfContainer FromRFC3339DateTime(DateTime value)
        {
            return new DateTimeCase2().Set(value);
        }

        public abstract T Match<T>(Func<DateTime?, T> rfc1123DateTime, Func<DateTime?, T> rfc3339DateTime);

        [JsonConverter(typeof(UnionTypeCaseConverter<DateTimeCase, DateTime?>), JTokenType.Date)]
        private class DateTimeCase : NativeDateTimeAnyOfContainer, ICaseValue<DateTimeCase, DateTime?>, ICustomConverter
        {
            public DateTime? value;

            public override T Match<T>(Func<DateTime?, T> rfc1123DateTime, Func<DateTime?, T> rfc3339DateTime)
            {
                return rfc1123DateTime(value);
            }

            public DateTimeCase Set(DateTime? value)
            {
                this.value = value;
                return this;
            }

            public DateTime? Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }

            public JsonConverter GetJsonConverter()
            {
                return new CoreCustomDateTimeConverter("r");
            }
        }

        [JsonConverter(typeof(UnionTypeCaseConverter<DateTimeCase2, DateTime?>), JTokenType.Date)]
        private class DateTimeCase2 : NativeDateTimeAnyOfContainer, ICaseValue<DateTimeCase2, DateTime?>, ICustomConverter
        {
            public DateTime? value;

            public override T Match<T>(Func<DateTime?, T> rfc1123DateTime, Func<DateTime?, T> rfc3339DateTime)
            {
                return rfc3339DateTime(value);
            }

            public DateTimeCase2 Set(DateTime? value)
            {
                this.value = value;
                return this;
            }

            public DateTime? Get()
            {
                return value;
            }

            public override string ToString()
            {
                return value.ToString();
            }

            public JsonConverter GetJsonConverter()
            {
                return new CoreCustomDateTimeConverter("yyyy-MM-ddTHH:mm:ssZ");
            }
        }
    }
}
