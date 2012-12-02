/*
 * Convert one specific string to a Resource
 */

//#define DEBUG_ON

using System;
using System.Windows;
using System.Windows.Data;

namespace CIS681.Fall2012.VDS.UI.Converters {
    class ResouceConverter<T> : IMultiValueConverter {
        /// <summary>
        /// Main convert part
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            FrameworkElement targetElement = values[0] as FrameworkElement;
            string resName = values[1] as string;

            if (resName == null)
                return null;
            T res = (T)targetElement.TryFindResource(resName);
#if DEBUG_ON
            System.Console.WriteLine("{0} ResouceConverter T {1} resource {2}", System.DateTime.Now.Millisecond, typeof(T), resName);
#endif
            return res;
        }

        /// <summary>
        /// Convert back.  No need to implement this actually
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
