# Security

Commits and releases are cryptographically signed with my GPG key `F744D8C299C05EAA` to ensure authenticity.

## Prerequisite

Find public sources of trust for my GPG key ID `F744D8C299C05EAA`, e.g. my website, GitHub, or similar.
Verify the integrity of this guide by comparing the key ID and fingerpint throughout.

## Verify Source

1. Clone the repository.
2. Checkout a release tag or commit and take note of the signature, e.g:
   ```shell
   git tag -v v1.0.0
   ```
   ```shell
   git log --show-signature
   ```
3. Import my public key:
   ```shell
   gpg --keyserver hkps://keyserver.ubuntu.com --recv-keys F744D8C299C05EAA
   ```
4. Show the fingerprint:
   ```shell
   gpg --fingerprint F744D8C299C05EAA
   ```
5. Verify that the fingerprint from the keyserver matches:
	- The git tag signature.
	- This guide.
	- The public source of trust.

## Verify Binaries

1. Import my public key:
   ```shell
   gpg --keyserver hkps://keyserver.ubuntu.com --recv-keys F744D8C299C05EAA
   ```
2. Download one of the release files (tarball or zip).
3. Download the SHA256SUMS.asc file which is a signed file containing checksums.
3. In the directory of the downloaded files, verify the integrity:
    ```shell
    gpg --verify SHA256SUMS.asc
    ```
4. Ensure the output shows my name `Chris Yarbrough` and key fingerprint `98A78974F886777AB85CF1D0F744D8C299C05EAA`.
5. Verify the checksum(s):
    ```shell
    sha256sum -c SHA256SUMS.asc
    ```
   The output should indicate, e.g. `ucll-linux-arm64.tar.gz: OK`.