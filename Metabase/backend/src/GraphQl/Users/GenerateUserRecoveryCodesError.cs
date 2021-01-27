using System.Collections.Generic;

namespace Metabase.GraphQl.Users
{
    public sealed class GenerateUserTwoFactorRecoveryCodesError
      : GraphQl.UserErrorBase<GenerateUserTwoFactorRecoveryCodesErrorCode>
    {
        public GenerateUserTwoFactorRecoveryCodesError(
            GenerateUserTwoFactorRecoveryCodesErrorCode code,
            string message,
            IReadOnlyList<string> path
            )
          : base(code, message, path)
        {
        }
    }
}