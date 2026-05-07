#import <Foundation/Foundation.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>

extern "C" void UnitySendMessage(const char *obj, const char *method, const char *msg);

extern "C"
{
    int MIGetTrackingAuthorizationStatus()
    {
        if (@available(iOS 14.0, *))
        {
            return (int)[ATTrackingManager trackingAuthorizationStatus];
        }

        return 3;
    }

    void MIRequestTrackingAuthorization(const char *gameObjectName, const char *callbackMethod)
    {
        if (@available(iOS 14.0, *))
        {
            [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status)
            {
                NSString *statusString = [NSString stringWithFormat:@"%ld", (long)status];
                UnitySendMessage(gameObjectName, callbackMethod, [statusString UTF8String]);
            }];
            return;
        }

        UnitySendMessage(gameObjectName, callbackMethod, "3");
    }
}
