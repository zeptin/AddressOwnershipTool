# Instructions

## Overview

AddressOwnership tool consists of two components:

1. *Console application* - used to generate swap claims, burn claims and distribution files.
2. *Web application* - for distributing all claimed tokens from burns and manual claims.

## Running from source code

Clone solution from this repository and compile it using dotnet tools.

Run the executable with help options

```
./AddressOwnershipTool.exe --help
```

There will be 3 operations available:

1. *Claim* - this will scan your current wallet for available funds to be swapped to a new token. Once you run it, it will generate a claim file that can be sent to Stratis team for validation and distribution
2. *Validate* - this will validate claim files and will generate a distribution file. This one will be mainly used by Stratis team.
3. *Scan* - this operation will scan Strax and Cirrus network for any burn claims and will also generate a distribution file.

See see parameters required for each command, run `./AddressOwnershipTool.exe <command> --help`

For example:

```
./AddressOwnershipTool.exe claim --help
```

This will show you required and optional parameters that you need to pass.

### Claim

Run a claim, there are number of options available. The most common way to claim your tokens is to have Strax or Cirrus wallet running locally on your machine and then running the follwoing command:

```
./AddressOwnershipTool.exe claim --walletname=your_wallet_name --destination=your_stratisevm_address --walletpassword=your_wallet_password
```

You can also optionally supply destination path `--outputFolder=yourpath` where the file will be generated, otherwise claim file will be created in the same location as AddressOwnershipTool.

### Validate

This step is to be used to validate all claim files and generate a token distribution file.

Validation uses local Strax/Cirrus node to perform balance check. Make sure you have local node running with address indexer and fully synced. For example to run testnet Strax node, use:

```
.\Stratis.StraxD.exe -testnet -addressindex
```

A typical run would look like this:

```
./AddressOwnershipTool.exe validate --sigfolder=path_to_folder_containing_claim_files
```

This command will scan all claim `.csv` files in specified sigfolder location and output distribution file to be used by a web app.

### Scan

Scan command scans Strax or Cirrus snapshot for any token burns and creates a distribution file from it. A typical run would look like:

```
./AddressOwnershipTool.exe scan --start=2062730
```

where `start` is the block number to start scan from. You can optionally supply `end` parameter to specify an end block. Please note, this oeration will take a while to process depending on number of blocks it needs to scan.

Upon completion of this command, a distribution file will be created.

### Distribution

For distribution, please use Web app *AddressOwnershipTool.Web*. It is a web app with ASP.NET backend and Angular frontend.

**Prerequisites**:

1. Chrome browser
2. Metamask
3. Ledger device (optional)

**App usage**:

1. Run web app
2. Connect to the tool using MetaMask, you will be prompted.
3. Once signed in, select a folder containing all your distribution `.csv` files from **Scan** and **Validate** steps above.
4. Select claims to process and validate each transaction with MetaMask.

