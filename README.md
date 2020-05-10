# COVIDSafe Dummy Backend

This is a dummy backend for the COVIDSafe mobile app, for those who want to experiment with it virtually as-is,
but without sending any data back to the Digital Transformation Agency, Department of Health, etc.

## Configuration (Android)

1. In `gradle.properties`, set the following:

```text
TEST_BASE_URL="https://covidsafedummybackend.azurewebsites.net/"
STAGING_BASE_URL="https://covidsafedummybackend.azurewebsites.net/"
PROD_BASE_URL="https://covidsafedummybackend.azurewebsites.net/"


TEST_END_POINT_PREFIX="/dummy"
STAGING_END_POINT_PREFIX="/dummy"
PRODUCTION_END_POINT_PREFIX="/dummy"
```

2. Update `network_security_config.xml` to the following:

```xml
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <domain-config>
        <domain includeSubdomains="true">covidsafedummybackend.azurewebsites.net</domain>
    </domain-config>
</network-security-config>
```

Note that this does not change the local behaviour, so if you build for the `release` configuration, it will still exchange local beacons with production-COVIDSafe users via Bluetooth. To avoid this, use the `debug` or `staging` build configurations, or change the GATT service UUID.

## Configuration (iOS)

TODO: Write this section.

## Behaviour
This is a stateless application, it does not store any of the information you send it.

### Registration
To register, any phone number is valid. The one-time password that would normally be sent to you by SMS is simply the last 6 digits of the phone number.

### TempIDs

Like the production app, TempIDs have a refresh time of one hour and an expiry time of two hours.

These are generated based on a SHA-256 hash of the phone number + system time (to the millisecond).

### Data Upload

The data upload process is not implemented yet.
