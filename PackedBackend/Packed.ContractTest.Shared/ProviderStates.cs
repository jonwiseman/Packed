// Date Created: 2022/12/27
// Created by: JSW

namespace Packed.ContractTest.Shared
{
    /// <summary>
    /// Class containing all possible provider states
    /// </summary>
    public static class ProviderStates
    {
        /// <summary>
        /// State where one list exists
        /// </summary>
        public const string ListExists = "there is at least one list";

        /// <summary>
        /// State where a list with the same description as one which is going to be added already exists
        /// </summary>
        public const string DuplicateList = "there is a list with the same description";
    }
}