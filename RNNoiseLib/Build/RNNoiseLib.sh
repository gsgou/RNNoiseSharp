echo "========== Build All started =========="
echo ""

echo "Preparing working files and directories"

#Create directories and copy library files to working directory
{
    find . -name "bin" -type d -exec rm -rf "{}" \;
    find . -name "tmp" -type d -exec rm -rf "{}" \;
    mkdir bin
    mkdir bin/iOS
    mkdir bin/Android
    mkdir tmp    
    mkdir tmp/sourceFiles
    mkdir tmp/iOS
    mkdir tmp/Android

    find .. -name "*.c" -exec cp {} tmp/sourceFiles \;
    find .. -name "*.h" -exec cp {} tmp/sourceFiles \;
} &> /dev/null

declare -a iOSArchitectures=("arm64" "arm64e")

LibraryName="rnnoise"
iOS_SDK_Version="15.5"
iOS_SDK_Min_Version="9.0"

echo ""
echo "=== BUILD TARGET (Android) ==="
echo ""

cd Build/jni

arch -x86_64 /Users/$USER/Library/Android/sdk/ndk-bundle/ndk-build NDK_LIBS_OUT=../../tmp/Android/libs NDK_OUT=../../tmp/Android/obj
echo "Copying libs to bin/Android"
{
    cp -a  ../../tmp/Android/libs/. ../../bin/Android/
} &> /dev/null
echo ""

cd ../../

echo "** BUILD SUCCEEDED (Android) **"
echo ""

echo ""
echo "=== BUILD TARGET (iOS) ==="
echo ""

cd tmp

for i in "${iOSArchitectures[@]}"
do
    SdkRootValue="iPhoneOS"
    echo "Build for $i:"
    if [ $i == "x86_64" ]
    then
        SdkRootValue="iPhoneSimulator"
    fi

    export DEVROOT=/Applications/Xcode.app/Contents/Developer/Platforms/$SdkRootValue.platform/Developer
    export IPHONEOS_DEPLOYMENT_TARGET=$iOS_SDK_Version
    export SDKROOT=$DEVROOT/SDKs/$SdkRootValue.sdk
    export CFLAGS="-std=c11 -arch $i -pipe -no-cpp-precomp -fembed-bitcode -isysroot $SDKROOT -miphoneos-version-min=$iOS_SDK_Min_Version -I$SDKROOT/usr/include/"

    echo "Compiling and linking (output as static library)"

    cd sourceFiles
    gcc -c *.c $CFLAGS
    cd ..

    {
        ar ru iOS/${LibraryName}_${i}.a sourceFiles/*.o
    } &> /dev/null    

    cd sourceFiles
    find . -name "*.o" -type f -delete
    cd ..

    echo ""
done

echo "Build universal library:"
lipo iOS/*.a -output iOS/lib$LibraryName.a -create

echo "Copying lib${LibraryName}.a to bin/iOS"
{
    find iOS -name "lib${LibraryName}.a" -exec cp {} ../bin/iOS \;
} &> /dev/null

cd ..

echo "Stripping bitcode from lib${LibraryName}.a"
{
    xcrun bitcode_strip "bin/iOS/lib${LibraryName}.a" -r -o "bin/iOS/lib${LibraryName}.a"
} &> /dev/null

echo ""
echo "** BUILD SUCCEEDED (iOS) **"
echo "" 

echo "========== Build All completed =========="
echo ""

cd ..

#Cleanup working directories
{
    find . -name "tmp" -type d -exec rm -rf "{}" \;
} &> /dev/null

exit 0