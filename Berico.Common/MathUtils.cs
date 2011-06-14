//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.

//-------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Berico.Common
{
    /// <summary>
    /// Provides mathematical utility functions
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Calculates the normal distribution for the given value.  This version
        /// of the function assumes that the mean and standard deviation have 
        /// already been calculated in relation to the value.
        /// http://mathworld.wolfram.com/NormalDistribution.html
        /// </summary>
        /// <param name="z">The value to compute the normal distribution for</param>
        /// <returns>the normal distribution for the provided value</returns>
        public static double NormalCdf(double z)
        {
            return 0.5 * (1.0 + Erf(z / (Math.Sqrt(2.0))));
        }

        /// <summary>
        /// Calculates the normal distribution for the given value
        /// using the provided mean and standard deviation.
        /// http://mathworld.wolfram.com/NormalDistribution.html
        /// </summary>
        /// <param name="z">A single value (variate) in the data set</param>
        /// <param name="mean">The mean of the data set</param>
        /// <param name="sd">The standard deviation of the data set</param>
        /// <returns>the normal distribution for the provided value</returns>
        public static double NormalCdf(double z, double mean, double sd)
        {
            return NormalCdf((z - mean) / sd);
        }

        /// <summary>
        /// Used by the NormalCdf in the calculation of the normal distribution
        /// http://mathworld.wolfram.com/Erf.html
        /// </summary>
        /// <param name="val">The value to compute the error function against</param>
        /// <returns>the probability of error in the given value</returns>
        public static double Erf(double z)
        {
            double t = 1.0 / (1.0 + 0.47047 * Math.Abs(z));
            double poly = t * (0.3480242 + t * (-0.0958798 + t * (0.7478556)));
            double ans = 1.0 - poly * Math.Exp(-z * z);

            if (z >= 0)
                return ans;
            else
                return -ans;

        }

        /// <summary>
        /// Calculates the standard deviation of the given collection of doubles using
        /// the provided mean value
        /// </summary>
        /// <param name="values">The collection of double values to computer the 
        /// standard deviation for</param>
        /// <param name="mean">The pre-calculated mean value</param>
        /// <returns>the standard deviation for the provided population</returns>
        public static double StandardDeviation(this IEnumerable<double> values, double mean)
        {
            // Calculate the sum of the deviation (value - mean).
            // This constitutes the second loop over the values.
            double sum = values.Sum(value => Math.Pow(value - mean, 2));

            // Get the average deviation (allowing for bias correction)
            sum = sum / (values.Count() - 1);

            // Take the square root of the variance for the standard deviation
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Calculates the standard deviation using a value and the frequency (the amount of
        /// times that values occurs) of that value
        /// </summary>
        /// <param name="frequencyPlusValues">A collection of values and the frequency
        /// of those values</param>
        /// <param name="mean">The pre-calculated mean value</param>
        /// <returns>the standard deviation for the provided values</returns>
        public static double StandardDeviation(this IEnumerable<Tuple<double, int>> frequencyPlusValues, double mean)
        {
            // One of the main calculations performed by the standard deviation
            // formula is determining the deviation.  The deviation is typically
            // computed by adding together the square of each value minus the
            // mean.  This version of the formula is different in that the frequency
            // is multiplied by the square of each value mins the mean, before it
            // is summed.
            //
            // The first value in each tuple is the value while the second value
            // is the frequency of that value.

            double totalCount = 0;
            double sum = 0;

            // Loop over each of the value and frequency tuples
            foreach (Tuple<double, int> frequencyPlusValue in frequencyPlusValues)
            {
                int frequency = frequencyPlusValue.Item2;
                double value = frequencyPlusValue.Item1;

                // Determine the deviation
                double deviation = Math.Pow(value - mean, 2);

                // Multiple the deviation by the frequency and increment the sum
                sum += frequency * deviation;

                // Keep track of the total count
                totalCount += frequency;
            }

            // Complete the calculation
            return Math.Sqrt(sum / totalCount);

        }

        /// <summary>
        /// Calculates the standard deviation using a value and the frequency (the amount of
        /// times that values occurs) of that value
        /// </summary>
        /// <param name="frequencyPlusValues">A collection of values and the frequency
        /// of those values</param>
        /// <returns>the standard deviation for the provided values</returns>
        public static double StandardDeviation(this IEnumerable<Tuple<double, int>> frequencyPlusValues)
        {
            // Compute the mean
            double mean = frequencyPlusValues.Mean();

            return frequencyPlusValues.StandardDeviation(mean);
        }

        /// <summary>
        /// Calculates the standard deviation of the given collection of doubles
        /// </summary>
        /// <param name="values">The collection of double values to computer the 
        /// standard deviation for</param>
        /// <returns>the standard deviation for the provided population</returns>
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            // Compute the mean (first loop over values)
            double mean = values.Average();

            return StandardDeviation(values, mean);
        }

        /// <summary>
        /// Calculates the mean using a value and the frequency (the amount of
        /// times that values occurs) of that value
        /// </summary>
        /// <param name="frequencyPlusValues">A collection of values and the frequency
        /// of those values</param>
        /// <returns>the mean (average) for the provided values</returns>
        public static double Mean(this IEnumerable<Tuple<double, int>> frequencyPlusValues)
        {
            // Typically, the mean is calculated by adding up all the values and 
            // dividing that sum by the total number of values.  Here, we are 
            // handling the situation that we only have the unique values (no
            // duplicates) and the frequency that those values occur.  Also, Adding
            // up the frequencies gives you the total count.
            //
            // The first value in each tuple is the value while the second value
            // is the frequency of that value.

            double totalCount = 0;
            double sum = 0;

            // Loop over each of the value and frequency tuples
            foreach (Tuple<double, int> frequencyPlusValue in frequencyPlusValues)
            {
                int frequency = frequencyPlusValue.Item2;
                double value = frequencyPlusValue.Item1;

                // Multiple the value by the frequency and increment the sum
                sum += frequency * value;
                
                // Keep track of the total count
                totalCount += frequency;
            }

            // Complete the calculation
            return sum / totalCount;
        }

        /// <summary>
        /// Calculates the log factorial for the given value
        /// </summary>
        /// <param name="n">The value to compute log n! for</param>
        /// <returns>log n!</returns>
        public static double LogFactorial(int n)
        {
            double ans = 0.0;

            for (int i = 1; i <= n; i++)
                ans += Math.Log(i);

            return ans;
        }

    }
}