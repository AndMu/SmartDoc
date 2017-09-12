"%WIX%bin\candle.exe" -arch x86 -dPlatform="x86" -dSourceFilesDir=..\SmartDoc\bin\Decoded -dProductVersion=1.0.0.1 Components.wxs product.wxs -ext WixUIExtension
"%WIX%bin\light.exe" -out bin\Release\SmartDoc.msi -loc product.wxl components.wixobj product.wixobj -ext WixUIExtension
"%WIX%bin\candle.exe" -arch x86 -dPlatform="x86" -dSmartDoc.TargetPath=bin\Release\SmartDoc.msi -dProductVersion=1.0.0.1 bundle.wxs -ext WixUIExtension -ext WixNetFxExtension -ext WixBalExtension
"%WIX%bin\light.exe" -out bin\Release\SmartDoc.exe -loc product.wxl bundle.wixobj -ext WixUIExtension -ext WixNetFxExtension -ext WixBalExtension
