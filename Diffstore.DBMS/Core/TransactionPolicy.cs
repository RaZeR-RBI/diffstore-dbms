using System;
using System.Collections.Generic;
using System.Linq;

namespace Diffstore.DBMS.Core
{
    /// <summary>
    /// Defines options related to transactions.
    /// </summary>
    public class TransactionPolicyInfo
    {
        /// <summary>
        /// Time to wait between transaction attempts.
        /// </summary>
        public IEnumerable<TimeSpan> RetryTimeouts { get; set; } =
            Enumerable.Repeat(TimeSpan.FromSeconds(1), 3);
    }

    /// <summary>
    /// Helper class for creating <see cref="TransactionPolicyInfo" />.
    /// </summary>
    public static class TransactionPolicy
    {
        /// <summary>
        /// Creates a <see cref="TransactionPolicyInfo" /> with the specified
        ///  retry count and wait time.
        /// </summary>
        public static TransactionPolicyInfo FixedRetries(this TransactionPolicyInfo policy,
            int retries, TimeSpan timeBetweenRetries)
        {
            policy.RetryTimeouts = Enumerable.Repeat(timeBetweenRetries, retries);
            return policy;
        }

        /// <summary>
        /// Creates a <see cref="TransactionPolicyInfo" /> with the specified
        ///  retry count and wait time.
        /// </summary>
        public static TransactionPolicyInfo FixedRetries(int retries, TimeSpan timeBetweenRetries) =>
            new TransactionPolicyInfo().FixedRetries(retries, timeBetweenRetries);

        /// <summary>
        /// Creates a <see cref="TransactionPolicyInfo" /> with single retry 
        /// attempt and the specified wait time
        /// </summary>
        public static TransactionPolicyInfo SingleRetry(this TransactionPolicyInfo policy,
            TimeSpan timeBeforeRetry) => policy.FixedRetries(1, timeBeforeRetry);

        /// <summary>
        /// Creates a <see cref="TransactionPolicyInfo" /> with single retry 
        /// attempt and the specified wait time.
        /// </summary>
        public static TransactionPolicyInfo SingleRetry(TimeSpan timeBeforeRetry) =>
            new TransactionPolicyInfo().SingleRetry(timeBeforeRetry);

        /// <summary>
        /// Creates a <see cref="TransactionPolicyInfo" /> with the specified
        ///  retry times.
        /// </summary>
        public static TransactionPolicyInfo WithRetries(this TransactionPolicyInfo policy,
            params TimeSpan[] waitTimes)
        {
            policy.RetryTimeouts = waitTimes;
            return policy;
        }

        /// <summary>
        /// Creates a <see cref="TransactionPolicyInfo" /> with the specified
        ///  retry times.
        /// </summary>
        public static TransactionPolicyInfo WithRetries(params TimeSpan[] waitTimes) =>
            new TransactionPolicyInfo().WithRetries(waitTimes);
    }
}