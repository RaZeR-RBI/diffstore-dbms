using System;
using System.Collections.Generic;
using System.Linq;

namespace Diffstore.DBMS.Core
{
    public class TransactionPolicyInfo
    {
        public IEnumerable<TimeSpan> RetryTimeouts { get; set; } =
            Enumerable.Repeat(TimeSpan.FromSeconds(1), 3);
    }

    public static class TransactionPolicy
    {
        public static TransactionPolicyInfo FixedRetries(this TransactionPolicyInfo policy,
            int retries, TimeSpan timeBetweenRetries)
        {
            policy.RetryTimeouts = Enumerable.Repeat(timeBetweenRetries, retries);
            return policy;
        }

        public static TransactionPolicyInfo FixedRetries(int retries, TimeSpan timeBetweenRetries) =>
            new TransactionPolicyInfo().FixedRetries(retries, timeBetweenRetries);

        public static TransactionPolicyInfo SingleRetry(this TransactionPolicyInfo policy,
            TimeSpan timeBeforeRetry) => policy.FixedRetries(1, timeBeforeRetry);

        public static TransactionPolicyInfo SingleRetry(TimeSpan timeBeforeRetry) =>
            new TransactionPolicyInfo().SingleRetry(timeBeforeRetry);

        public static TransactionPolicyInfo WithRetries(this TransactionPolicyInfo policy,
            params TimeSpan[] waitTimes)
        {
            policy.RetryTimeouts = waitTimes;
            return policy;
        }

        public static TransactionPolicyInfo WithRetries(params TimeSpan[] waitTimes) =>
            new TransactionPolicyInfo().WithRetries(waitTimes);
    }
}