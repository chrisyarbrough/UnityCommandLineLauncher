# Security

Commits and releases are cryptographically signed to ensure authenticity.

- **Commits**: Signed with my (Chris Yarbrough's) GPG key `F744D8C299C05EAA`
- **Release Artifacts** (v0.3.0+): Signed with GitHub's build provenance attestations using Sigstore

> If you would like to contribute to this repo, you may optionally also sign your commits, but it's not a requirement, since I will be vetting each pull request anyway.

Continue reading to learn how you can verify the repository and release artifacts.

## Prerequisite

Find public sources of trust for my GPG key ID `F744D8C299C05EAA`, e.g. my website, GitHub, or similar.
Verify the integrity of this guide by comparing the key ID and fingerprint throughout.

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

## Verify Binaries (Recommended: v0.3.0+)

Starting with v0.3.0, releases include cryptographic build provenance attestations that prove the artifacts were built by the official GitHub Actions workflow. This verification method is more secure and easier than GPG verification.

### Prerequisites

Install the GitHub CLI:
```shell
# macOS
brew install gh

# Linux (Debian/Ubuntu)
sudo apt install gh

# Windows
winget install GitHub.cli
```

Authenticate with GitHub:
```shell
gh auth login
```

### Verification Steps

1. Download a release artifact:
   ```shell
   gh release download v0.3.0 --pattern "ucll-osx-arm64.tar.gz" --repo chrisyarbrough/UnityCommandLineLauncher
   ```

2. Verify the attestation:
   ```shell
   gh attestation verify ucll-osx-arm64.tar.gz --owner chrisyarbrough
   ```

3. Successful verification confirms:
   - ✓ The artifact was built by the official GitHub Actions workflow
   - ✓ The artifact matches the exact commit SHA in the repository
   - ✓ The build process is cryptographically signed and logged in Sigstore's transparency log
   - ✓ No tampering occurred after the build

### What the Attestation Proves

The build provenance attestation includes:
- **Repository**: chrisyarbrough/UnityCommandLineLauncher
- **Workflow**: .github/workflows/release.yml
- **Commit SHA**: The exact Git commit that produced this artifact
- **Build Environment**: GitHub-hosted runner details
- **Transparency Log**: Public Sigstore Rekor entry (searchable at https://search.sigstore.dev)

This provides stronger security guarantees than traditional GPG signatures because:
- The signing key is tied to the GitHub repository (not a personal GPG key)
- The entire build process is attested, not just the final artifact
- Signatures are logged in a public, tamper-proof transparency log
- Verification doesn't require trusting or importing external keys

## Verify Binaries (Legacy: GPG Method)

For older releases (pre-v0.3.0) or builds that were created locally:

1. Import my public key:
   ```shell
   gpg --keyserver hkps://keyserver.ubuntu.com --recv-keys F744D8C299C05EAA
   ```
2. Download one of the release files (tarball or zip).
3. Download the SHA256SUMS.asc file which is a signed file containing checksums.
4. In the directory of the downloaded files, verify the integrity:
    ```shell
    gpg --verify SHA256SUMS.asc
    ```
5. Ensure the output shows my name `Chris Yarbrough` and key fingerprint `98A78974F886777AB85CF1D0F744D8C299C05EAA`.
6. Verify the checksum(s):
    ```shell
    sha256sum -c SHA256SUMS.asc
    ```
   The output should indicate, e.g. `ucll-linux-arm64.tar.gz: OK`.
