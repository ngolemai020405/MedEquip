namespace MedEquipMobile;

public static class Constants
{
    // Use 10.0.2.2 to access localhost from Android emulator
    public static string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android 
        ? "https://10.0.2.2:7001" 
        : "https://localhost:7001";
        
    public static string ApiUrl = $"{BaseUrl}/api";
}
