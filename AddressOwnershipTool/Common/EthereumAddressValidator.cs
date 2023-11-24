using Nethereum.Util;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AddressOwnershipTool.Common;

public static class EthereumAddressValidator
{
    private static readonly Regex AddressRegex = new Regex(@"^0x[a-fA-F0-9]{40}$", RegexOptions.Compiled);

    public static bool IsValidAddress(this string address)
    {
        if (string.IsNullOrWhiteSpace(address) || !AddressRegex.IsMatch(address))
        {
            return false;
        }

        // If the address is all lower or upper case, it is not checksummed or could be valid.
        if (address == address.ToLower() || address == address.ToUpper())
        {
            return true;
        }

        var addressUtil = new AddressUtil();
        return addressUtil.IsChecksumAddress(address);
    }
}
