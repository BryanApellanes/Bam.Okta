namespace Okta.AuthN
{
    public enum Factors
    {
        SecurityQuestion,
        Sms,
        Call,
        VerifyTotp,
        VerifyPush,
        GoogleAuthenticator,
        RsaSecurID,
        SymantecVip,
        YubiKey,
        Email,
        U2F,
        WebAuthn,
        CustomHotp
    }
}