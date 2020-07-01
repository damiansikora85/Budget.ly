
#!/usr/bin/env bash -e

echo "Found Unit test projects:"
find $APPCENTER_SOURCE_DIRECTORY -regex '.*UnitTests.*\.csproj' -exec echo {} \;
echo