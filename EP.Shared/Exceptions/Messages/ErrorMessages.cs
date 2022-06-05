namespace EP.Shared.Exceptions.Messages
{
    public static class ErrorMessages
    {
        public static class Generic
        {
            public static string ObjectNotFound = "Object not found Request";
            public static string InvalidRequest = "Invalid Request";
            public static string SomethingWentWrong = "Something went wrong, please try again later";
            public static string UnableToProcess = "Unable to process request";
            public static string InvalidPayload = "Invalid Payload";
            public static string TypeBadRequest = "Bad Request";
            public static string AlreadyExists = "Already exists";
        }

        public static class Jwt
        {
            public static string JwtNotExpired = "Jwt token has not expired";
            public static string JwtExpired = "Jwt token has expired";
            public static string RefreshTokenExpired = "Refresh Token has expired";
            public static string InvalidRefreshToken = "Invalid refresh token or Does not exist";
            public static string TokenAlreadyUsed = "Refresh has been used, it cannot be reused";
            public static string IsRevoked = "Refresh has been revoked, it cannot be used";
            public static string InvalidTokenId = "Refresh Token reference does not match the jwt token";

        }

        public static class Profile
        {
            public static string UserNotFound = "User not found";
        }

        public static class Users
        {
            public static string EmailAlreadyUsed = "Email already in use";
            public static string UserNotFound = "User not found";
            public static string UsersNotFound = "Users not found";
            public static string RoleDoesNotExist = "Role Does not Exists";
        }
    }
}
