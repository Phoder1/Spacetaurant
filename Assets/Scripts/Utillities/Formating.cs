using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Spacetaurant
{
    public static class Formating
    {
        public static CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
        public static string FormatInt(this int value) => value.ToString("##,#", enUS);
    }
}
