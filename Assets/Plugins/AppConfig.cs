/// <summary>
/// Contain configuration of a specific App.
/// </summary>
public static class AppConfig
{
    /*
    * Enter your application Client ID and Client Secret below.
    * You can obtain these credentials after registering your App ID with the Cortex SDK for development.
    * For instructions, visit: https://emotiv.gitbook.io/cortex-api#create-a-cortex-app
    */
    public static string ClientId = "sb1Tspn1606VwJ2M9B9C7K6OrqXti6uz0f7kBkrw";
    public static string ClientSecret = "3s5EK43vKXZwZ7XVAyGldW4qW7MGvv3OT7k5B27GvkZISuiqflgkK1LVA7TnPElAXm7WQisC2QBT0JP0Iq6bFq7WDp8BYtccd2n9XTdox9bU2EvTqAfQTTYtOPHqa5SH";

    public static string AppVersion = "1.1.0";
    public static string AppName = "UnityApp";
    public static bool IsDataBufferUsing = false; // Set false if you want to display data directly to MessageLog without storing in Data Buffer
    public static bool AllowSaveLogToFile = false; // Set true to save log to file and cortex token to local file for next time use

    #if !USE_EMBEDDED_LIB && !UNITY_ANDROID && !UNITY_IOS
    // only for desktop without embedded cortex
    public static string AppUrl = "wss://localhost:6868"; // for desktop without embedded cortex
    #else
    public static string AppUrl = ""; // Don't need AppUrl for mobile and embedded cortex
    #endif

}