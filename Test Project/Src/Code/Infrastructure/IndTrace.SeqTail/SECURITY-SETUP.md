# IndTrace.SeqTail - Security Configuration Setup

## ⚠️ SECURITY NOTICE

This application previously contained a **hardcoded API key** which has been removed for security reasons.

## 🔐 Secure Configuration Options

### Option 1: User Secrets (Recommended for Development)

```bash
# Navigate to the project directory
cd Src/Code/Infrastructure/IndTrace.SeqTail

# Set the API key via user secrets
dotnet user-secrets set "SeqApi:ApiKey" "YOUR_ACTUAL_SEQ_API_KEY"
```

### Option 2: Environment Variables (Recommended for Production)

```bash
# Set environment variable
set SeqApi__ApiKey=YOUR_ACTUAL_SEQ_API_KEY

# Or in Linux/macOS:
export SeqApi__ApiKey=YOUR_ACTUAL_SEQ_API_KEY
```

### Option 3: appsettings.Development.json (Local Development Only)

Edit `appsettings.Development.json` and replace `YOUR_SEQ_API_KEY_HERE` with your actual key:

```json
{
  "SeqApi": {
    "Server": "http://localhost:5341",
    "ApiKey": "your-actual-api-key-here",
    "Filter": "@Level = 'Error'"
  }
}
```

**⚠️ NEVER commit this file with real API keys to source control!**

## 🚫 SECURITY FIXES APPLIED

1. **Removed hardcoded API key** from `SeqApiOptions.cs`
2. **Added configuration injection** in `Program.cs`
3. **Updated Worker class** to use injected configuration
4. **Created secure configuration files** with placeholders
5. **Added User Secrets support** via project file

## 🔍 Previous Security Issue

The hardcoded API key `yM2x4D7gilOEE3RV3BBM` has been removed from the source code.

**If this key was used in production, it should be rotated immediately.**

## ✅ Configuration Validation

The application will now fail gracefully if no API key is configured, rather than using a hardcoded value.

To verify your configuration is working:

1. Set the API key using one of the methods above
2. Run the application
3. Check that it connects to your Seq server successfully

## 📋 Configuration Schema

```json
{
  "SeqApi": {
    "Server": "http://localhost:5341",    // Seq server URL
    "ApiKey": "",                         // Your API key (configure securely)
    "Filter": "@Level = 'Error'"          // Default log filter
  }
}
```
