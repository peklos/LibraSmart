#!/bin/bash

echo "========================================"
echo "LibraSmart - Linux Build Script"
echo "========================================"
echo ""

# Check Node.js
if ! command -v node &> /dev/null; then
    echo "ERROR: Node.js not found!"
    echo "Install: sudo apt install nodejs npm"
    exit 1
fi

# Check Rust
if ! command -v rustc &> /dev/null; then
    echo "ERROR: Rust not found!"
    echo "Install: curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh"
    exit 1
fi

# Install system dependencies
echo "Installing system dependencies..."
echo "You may need to enter your password (sudo)"
sudo apt-get update
sudo apt-get install -y \
    libwebkit2gtk-4.0-dev \
    build-essential \
    curl \
    wget \
    file \
    libssl-dev \
    libgtk-3-dev \
    libayatana-appindicator3-dev \
    librsvg2-dev

if [ $? -ne 0 ]; then
    echo "ERROR: Failed to install system dependencies"
    exit 1
fi

echo ""
echo "Installing npm dependencies..."
npm install

if [ $? -ne 0 ]; then
    echo "ERROR: Failed to install npm dependencies"
    exit 1
fi

echo ""
echo "========================================"
echo "Building Linux app..."
echo "This may take 10-30 minutes..."
echo "========================================"
echo ""

npm run tauri:build:linux

if [ $? -ne 0 ]; then
    echo "ERROR: Build failed"
    exit 1
fi

echo ""
echo "========================================"
echo "BUILD SUCCESSFUL!"
echo "========================================"
echo ""
echo "Your app is ready:"
echo ""
echo "DEB Package (for Ubuntu/Debian/RedOS):"
echo "src-tauri/target/release/bundle/deb/librasmart_1.0.0_amd64.deb"
echo ""
echo "AppImage (universal):"
echo "src-tauri/target/release/bundle/appimage/librasmart_1.0.0_amd64.AppImage"
echo ""
echo "Executable:"
echo "src-tauri/target/release/librasmart"
echo ""
echo "To install DEB:"
echo "sudo dpkg -i src-tauri/target/release/bundle/deb/librasmart_1.0.0_amd64.deb"
echo ""
echo "To run AppImage:"
echo "chmod +x src-tauri/target/release/bundle/appimage/librasmart_1.0.0_amd64.AppImage"
echo "./src-tauri/target/release/bundle/appimage/librasmart_1.0.0_amd64.AppImage"
echo ""
