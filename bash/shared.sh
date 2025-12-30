#!/bin/bash

get_unity_hub() {
  echo "$(mdfind "kMDItemCFBundleIdentifier == 'com.unity3d.unityhub'" | head -n 1)/Contents/MacOS/Unity Hub"
}