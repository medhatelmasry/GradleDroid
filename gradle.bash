# bash /Users/medhatelmasry/OneDrive/Utils/androidOnMac/gradle.bash

#
# update local.properties:
# sdk.dir=/Users/medhatelmasry/Library/Android/sdk
#
# brew install dos2unix # Installs dos2unix Mac

chmod +x ./gradlew
dos2unix ./gradlew
./gradlew assembleDebug
